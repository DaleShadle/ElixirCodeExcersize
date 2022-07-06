using System.Collections.Generic;
using System.Collections;
using System.Xml.Linq;
using System.Linq;

namespace SecurityQuestion
{
    #region structs
    public struct QnAItem
    {   public QnAItem(int QId, string Answer)
        {
            _qId = QId;
            _answer = Answer;
        }
        #region properties
        private int _qId;
        private string _answer;

        public int QId { get { return _qId; } set { _qId = value; } }
        public string Answer { get { return _answer; } set { _answer = value; } }
        #endregion properties

    }
    #endregion structs

    class User : IEnumerable
    {
        #region properties
        private string _name;
        public string Name { get { return _name; } set { _name = value; } }

        private List<QnAItem> _QnAList = new List<QnAItem>();
        public IEnumerator GetEnumerator()
        {
            return _QnAList.GetEnumerator();
        }
        #endregion properties

        #region methods
        //Search for and load the user along with security question/answers provided
        public bool Load(DataStore ds)
        {
            if (!ds.Exists())           //Security File doesn't exist yet so nor does user.
                return false;
            
            var _secFile = XDocument.Load(ds.FileName);

            var _users = from _user in _secFile.Descendants("User")
                         where _user.Attribute("name").Value == Name
                         let _data = new { _qId = _user.Elements("QId").ToArray(), _answer = _user.Elements("Answer").ToArray() }
                         select _data;

            //if we didn't find a user return
            if (_users.Count() == 0)
                return false;

            //Load all questions and answers user previously provided
            foreach (var q in _users)
            {
                for (int idx = 0; idx < q._qId.Count(); idx++)
                {
                    _QnAList.Add(new QnAItem(int.Parse(q._qId[idx].Value), q._answer[idx].Value));
                }
            }
            return true;    //User exists and has been loaded
        }

        //Remove user from file
        public void Delete(DataStore ds)
        {
            this.ClearQnA();                //Remove QnA
            ds.DeleteUser(this);
        }
        //Save user and Question/answers to file
        public bool SaveQnA(DataStore ds)
        {
            ds.CreateSecFile(this);

            return true;
        }

        public void AddAnswer(int idx, string answer)
        {
            _QnAList.Add(new QnAItem(idx, answer));
        }

        //Remove all question and answers
        public void ClearQnA()
        {
            _QnAList.Clear();
        }

        public int QnACount()
        {
            return _QnAList.Count();
        }

        public bool Exists()
        {
            return _QnAList.Count() > 0;        //User exists if answers provided   
        }
        #endregion methods
    }
}
