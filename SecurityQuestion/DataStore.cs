using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace SecurityQuestion
{
    /*
     * Read/Write of data store (local xml file)
     */
    class DataStore : XmlDocument
    {
        public DataStore(string filename)
        {
            _filename = filename;
        }

        #region Properties
        private string _filename;
        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }
        #endregion Properties

        #region Methods

        public bool Exists()
        {
            return File.Exists(FileName);
        }
        public void CreateSecFile(User user)
        {
            try
            {
                //If the XML security file doesn't exist then create it 
                if (!Exists())
                {
                    XmlTextWriter writer = new XmlTextWriter(FileName, System.Text.Encoding.UTF8);
                    writer.WriteStartDocument(true);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartElement("Users");
                    WriteFirstUser(writer, user);              //Persist first user
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                    Console.WriteLine("Persisting File: {0}", FileName);
                }
                else
                {
                    AddUser(user);      //Add a new user and answers to file
                }
            }
            catch (IOException ioe)
            {
                Console.WriteLine("Create File - File error: {0}", ioe.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("General Error: {0}", e.Message);
            }
        }

        void WriteFirstUser(XmlTextWriter writer, User user)
        {
            writer.WriteStartElement("User");
            writer.WriteAttributeString("name", user.Name);
            foreach (QnAItem qa in user)
            {
                CreateXMLQnANode(qa.QId.ToString(), qa.Answer, writer);
            }
            writer.WriteEndElement();
        }

        void AddUser(User user)
        {
            try
            {
                //Write User and all question/answers
                XDocument xDocument = XDocument.Load(FileName);
                XElement root = xDocument.Element("Users");
                IEnumerable<XElement> rows = root.Descendants("User");
                XElement firstRow = rows.First();
                firstRow.AddBeforeSelf(
                    new XElement("User",
                    new XAttribute("name", user.Name)));
                firstRow = rows.First();
                foreach (QnAItem qa in user)
                {
                    firstRow.Add(
                    new XElement("QId", qa.QId.ToString()),
                    new XElement("Answer", qa.Answer));
                }
                xDocument.Save(FileName);
            }
            catch (IOException ioe)
            {
                Console.WriteLine("AddUser - File error: {0}", ioe.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("General Error: {0}", e.Message);

            }
        }

        public void DeleteUser(User user)
        {
            try
            { 
                //Find user node and remove it along with all child nodes
                XDocument xDocument = XDocument.Load(FileName);
                xDocument.Root.Descendants("User")
                       .Where(el => (string)el.Attribute("name") == user.Name)
                       .Remove();
                xDocument.Save(FileName);
            }
            catch (IOException ioe)
            {
                Console.WriteLine("Delete user - File error: {0}", ioe.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("General Error: {0}", e.Message);

            }
        }

        /// <summary>
        /// Check if at least 1 user exists.  If not delete the file for fresh start.
        /// </summary>
        public void FileCheck()
        {
            try 
            { 
                //Since a user can replace their answers it's possible for the file to be reduce to no users.
                //Check the file for existance of at least 1 user.  If not just delete the file so that we don't get improper XML structure on re-adding first user
                XDocument xDocument = XDocument.Load(FileName);
                XElement root = xDocument.Element("Users");
                if (!root.HasElements)
                    File.Delete(FileName);
            }
            catch (IOException ioe)
            {
                Console.WriteLine("File Check - File error: {0}", ioe.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("General Error: {0}", e.Message);

            }

        }
        private void CreateXMLQnANode(string qId, string answer, XmlTextWriter writer)
        {
            writer.WriteStartElement("QId");
            writer.WriteString(qId);
            writer.WriteEndElement();
            writer.WriteStartElement("Answer");
            writer.WriteString(answer);
            writer.WriteEndElement();
        }

        #endregion Methods

    }
}
