using System;
using System.Configuration;
using System.Collections.Generic;

namespace SecurityQuestion
{
    /*
     * Applet prompts a user for a name.  Searches local file for that name.  If found requires user to answer a correct question to pass security.
     * If user is not found they may enter answers for a minimum number of questions asked from a list after which all data is persisted.
     */
    class Program
    {
        static void Main(string[] args)
        {
            #region properties
            string _file = ConfigurationManager.AppSettings["FilePath"] + ConfigurationManager.AppSettings["FileName"];
            bool _continue = true;
            bool _correct = false;      //Answered Correctly
            bool _answerQuestion = false;           //If user wants to answer security questions
            User _user;  // = new User();
            string _answer = "";
            SecurityQ _secQ = new SecurityQ();
            #endregion properties

            #region Main Process Loop
            //Repeat until user Enters matching security question, creates security questions or exits
            while (_continue)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Security Questions");
                Console.WriteLine("***Please note all input is case sensative");
                Console.WriteLine("-------------------------------------------------------");
                
                Console.WriteLine("Hi, what is your name?");
                _user = new User();                                 //Start with fresh user
                _user.Name = Console.ReadLine();

                //If user exists offer to allow answering security question.  If no require them to provide answers to questions again.
                if (_user.Load())
                {
                    Console.WriteLine("Do you want to answer a secruity question? [y/n]");
                    ConsoleKeyInfo _resp = Console.ReadKey(true);     //If anything by y/Y entered assume no
                    if (_resp.KeyChar == 'Y' || _resp.KeyChar == 'y')
                        _answerQuestion = true;
                    else
                    {   //User choosing not to answer questions.  Delete them so that they can provide new answers
                        Console.WriteLine("Account removed.  New security answers must be provided.");
                        _user.Delete();
                        DataStore _ds = new DataStore(_file);    //Security File
                        _ds.FileCheck();
                        _answerQuestion = false;
                    }
                }

                //If user exists and wants to answer a security question.  
                if (_user.Exists() && _answerQuestion)
                {
                    //Go through all answered questions till user enters correct one or all attempts possible.
                    foreach (QnAItem qa in _user)
                    {
                        Console.WriteLine(_secQ.GetQuestion(qa.QId));
                        _answer = Console.ReadLine();
                        if (_answer == qa.Answer)
                        {
                            _correct = true;
                            Console.WriteLine("Correct answer.  Congratulations!");
                            Console.WriteLine("Press any key to continue or ESC to exit");
                            ConsoleKeyInfo _resp = Console.ReadKey(true);    
                            if (_resp.Key == ConsoleKey.Escape)
                                _continue = false;
                            break;
                        }
                    }
                    if (!_correct)
                    {
                        Console.WriteLine("Sorry, none of your answers matched.  Press any key to continue.");
                        Console.ReadKey(true);
                        _correct = false;           //reset
                    }
                }
                else    //user not found or chose not to answer security question.  Add new user if name provided
                {
                    if (_user.Name == string.Empty)
                        continue;

                    Console.WriteLine("Name not found.");
                    Console.WriteLine("Creating new account for: " + _user.Name);
                    Console.WriteLine("Would you like to store answers to security questions? [y/n/esc]");
                    ConsoleKeyInfo _resp = Console.ReadKey(true);     //If anything by y/Y entered assume no
                    if (_resp.KeyChar == 'Y' || _resp.KeyChar == 'y')
                    {
                        Console.WriteLine("****************************************");
                        Console.WriteLine("Preview of questions that will be asked:");
                        Console.WriteLine("Answers to " + _secQ.ReqQuestions.ToString() + " security questions must be provided or process will be repeated.");
                        Console.WriteLine("Press enter to skip question.");
                        Console.WriteLine("****************************************");
                        foreach (KeyValuePair<int, string> item in _secQ)           //Show preview of all possible questions
                        {
                            Console.WriteLine(item.Key + ") " + item.Value);
                        }
                        Console.WriteLine();
                        foreach (KeyValuePair<int, string> item in _secQ)           //Prompt questions until minimum are answered if not restart.
                        {
                            Console.WriteLine(item.Value);          //As Question
                            _answer = Console.ReadLine();
                            if (_answer != string.Empty)
                                _user.AddAnswer(item.Key, _answer);
                            if (_user.QnACount() == _secQ.ReqQuestions)
                                break;
                        }

                        //Persist questions and answers if required number answers provided
                        if (_user.QnACount() == _secQ.ReqQuestions)
                        {
                            _user.SaveQnA();
                            Console.WriteLine("User " + _user.Name + " persisted.  Press any key to continue.");
                        }
                        else
                        {
                            Console.WriteLine("You didn't provide answers to " + _secQ.ReqQuestions + " questions.  Press any key to continue.");
                            _user.ClearQnA();               //Clear all answers from user
                        }
                        Console.ReadKey(true);
                    }
                    else if (_resp.Key == ConsoleKey.Escape)
                        break;
                }
                Console.Clear();
            }
            #endregion Main Process Loop
        }
    }
}
