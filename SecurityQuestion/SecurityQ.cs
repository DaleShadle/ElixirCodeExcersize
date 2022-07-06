using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;

namespace SecurityQuestion
{
    /* 
     * SecurityQ Is enumerated list of possible security questions
     * Ultimately these questions are system data and should be stored in config (file or DB)
     */

    class SecurityQ : IEnumerable<KeyValuePair<int, string>>
    {
        public readonly int ReqQuestions = int.Parse(ConfigurationManager.AppSettings["ReqQuestions"].ToString());

        private readonly Dictionary<int, string> _questions = new Dictionary<int, string>()
        {
            {1, "In what city were you born?" },
            {2, "What was the name of your first pet?" },
            {3, "What is your mother's maiden name?" },
            {4, "What high school did you attend?" },
            {5, "What was your high school mascot?" },
            {6, "What was the make of your first car?" },
            {7, "What was your favorite toy as a child?" },
            {8, "Who is your favorite actor/actress?" },
            {9, "What is your favorite Album?" },
            {10, "What is your father's middle name?" }
        };

        public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
        {
            return _questions.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public string GetQuestion(int i)
        {
            return _questions[i];
        }
    }   
}
