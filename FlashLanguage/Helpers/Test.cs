using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashLanguage2.Helpers
{
    [Serializable]
    public class Test
    {
        public int _testID;
        public int _langaugeID;
        public string _testName;
        public DateTime _startDate;
        public DateTime _endDate;
        public int _attempts;

        public Test(int testID, int id, string name, DateTime start, DateTime end, int attempts)
        {

            this._testID = testID;
            this._testName = name;
            this._startDate = start;
            this._endDate = end;
            this._attempts = attempts;
        }
        public int TestID
        {
            get { return _testID; }
            set { _testID = value; }
        }
        public int LangaugeID
        {
            get { return _langaugeID; }
            set { _langaugeID = value; }
        }
        public string TestName
        {
            get { return _testName; }
            set { _testName = value; }
        }
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
        public int Attempts
        {
            get { return _attempts; }
            set { _attempts = value; }
        }

    }
}