using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashLanguage2.Helpers
{
    [Serializable]
    public class WordLanguage
    {
        public int _wordID;
        public string _word;

        public WordLanguage(int id, string word)
        {
            this._wordID = id;
            this._word = word;
        }

        public WordLanguage()
        {
            this._wordID = -1;
            this._word = "";
        }

        public int WordID
        {
            get { return _wordID; }
            set { _wordID = value; }
        }
        public string Word
        {
            get { return _word; }
            set { _word = value; }
        }
      
    }
}