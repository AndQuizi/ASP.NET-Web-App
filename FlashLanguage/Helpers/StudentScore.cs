using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashLanguage2.Helpers
{
    public class StudentScore
    {
        public string _studentID;
        public int _score;
        public string _firstName;
        public string _lastName;

        public StudentScore(string id, int s)
        {
            this._studentID = id;
            this._score = s;
            this._firstName = "";
            this._lastName = "";
        }

        public string studentID
        {
            get
            {
                return _studentID;
            }
            set
            {
                _studentID = value;
            }
        }
        public int score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
            }
        }
        public string firstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
            }
        }
        public string lastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
            }
        }
    }
}