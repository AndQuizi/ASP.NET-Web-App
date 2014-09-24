using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashLanguage2.Helpers
{
    [Serializable]
    public class TestAttempts
    {
        public string _testName = "";
        public int _testAttempts = 0;
        public int _userAttempts = 0;

        public TestAttempts(String testName, int testAttempts, int userAttempts)
        {
            this._testName = testName;
            this._testAttempts = testAttempts;
            this._userAttempts = userAttempts;

        }

        public string testName
        {
            get
            {
                return _testName;
            }
            set
            {
                _testName = value;
            }
        }


        public int testAttempts
        {
            get
            {
                return _testAttempts;
            }
            set
            {
                _testAttempts = value;
            }
        }

        public int userAttempts
        {
            get
            {
                return _userAttempts;
            }
            set
            {
                _userAttempts = value;
            }
        }
    }

}