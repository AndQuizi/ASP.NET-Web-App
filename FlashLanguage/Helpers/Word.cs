using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlashLanguage2;

namespace FlashLanguage2.Helpers
{
    [Serializable]
    public class Word
    {
        public int _wordID;
        public string _englishWord;
        public string _imagePath;

        public Word(int id, string word, string path)
        {
            this._wordID = id;
            this._englishWord = word;
            this._imagePath = path;
        }

        public Word()
        {
            this._wordID = -1;
            this._englishWord = "";
            this._imagePath = "";
        }

        public int WordID
        {
            get { return _wordID; }
            set { _wordID = value; }
        }
        public string EnglishWord
        {
            get { return _englishWord; }
            set { _englishWord = value; }
        }
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }
    }
}