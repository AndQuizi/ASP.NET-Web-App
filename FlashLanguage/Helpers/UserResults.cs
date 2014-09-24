using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashLanguage2.Helpers
{
    public class UserResults
    {
        public int _qNumber;
        public Word _word;
        public String _userAnswer = "";
        public String _correctAnswer = "";
        public UserResults(int questionNumber, Word word, String userAnswer, String answer)
        {
            this._qNumber = questionNumber;
            this._word = word;
            this._userAnswer = userAnswer;
            this._correctAnswer = answer;
        }


        public int qNumber
        {
            get
            {
                return _qNumber;
            }
            set
            {
                _qNumber = value;
            }
        }


        public Word word
        {
            get
            {
                return _word;
            }
            set
            {
                _word = value;
            }
        }


        public String userAnswer
        {
            get
            {
                return _userAnswer;
            }
            set
            {
                _userAnswer = value;
            }
        }
        public String correctAnswer
        {
            get
            {
                return _correctAnswer;
            }
            set
            {
                _correctAnswer = value;
            }
        }

    }
}