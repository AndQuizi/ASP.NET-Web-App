using FlashLanguage2.Helpers;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2.Evaluations
{
    public partial class TestPage : System.Web.UI.Page
    {
        private bool _refreshState;
        private bool _isRefresh;
        public static string strScoreValue;
        public static string strMaxScore;
        public static string totalQuestions;
        public static string attempt;
        public static string totalAttempts;
        public static List<Word> originalQueue;
        public static List<UserResults> userResults;
        public String currWordString;
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["currTestID"] == null || Session["currAttempt"] == null || Session["currQuestion"] == null)
            {
                Response.Redirect("../Default.aspx");
            }
            //get attempt and question number 
            totalQuestions = Convert.ToString(Session["totalQuestions"]);
            int tmp = (int)Session["currAttempt"];
            attempt = Convert.ToString(tmp);

            totalAttempts = Convert.ToString((int)Session["maxAttempts"]);
            btn1.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(btn1, null) + ";");
            btn2.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(btn2, null) + ";");
            btn3.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(btn3, null) + ";");
            btn4.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(btn4, null) + ";");
        }

        protected override void LoadViewState(object savedState)
        {
            object[] AllStates = (object[])savedState;
            base.LoadViewState(AllStates[0]);
            _refreshState = bool.Parse(AllStates[1].ToString());
            _isRefresh = _refreshState == bool.Parse(Session["__ISREFRESH"].ToString());
        }

        protected override object SaveViewState()
        {
            Session["__ISREFRESH"] = _refreshState;
            object[] AllStates = new object[2];
            AllStates[0] = base.SaveViewState();
            AllStates[1] = !(_refreshState);
            return AllStates;
        }

        //gets word from db
        public Word GetWord(int wordID)
        {
            Word word = new Word();
            try
            {
                string insertSQL = "SELECT EnglishWord, ImagePath FROM  Words WHERE WordID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@WordID", wordID);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();


                        word = new Word(wordID, Convert.ToString(reader[0]), Convert.ToString(reader[1]));

                        reader.Close();
                    }


                    cmd.Dispose();
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;

            }

            return word;
        }

        //get word translations
        public WordLanguage GetWordLanguage(int wordID)
        {
            int languageId = (int)Session["currLanguage"];
            WordLanguage word = new WordLanguage();
            try
            {
                string insertSQL = "SELECT WLWord FROM WordLanguages WHERE WLWordID = ? AND WLLanguageID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@WordID", wordID);
                    cmd.Parameters.AddWithValue("@WLLanguageID", languageId);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();


                        word = new WordLanguage(wordID, Convert.ToString(reader[0]));

                        reader.Close();
                    }


                    cmd.Dispose();
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;

            }
            return word;
        }

        //put the current word back into the question queue
        protected void Skip(object sender, EventArgs e)
        {

            Queue<Word> temp = (Queue<Word>)Session["wordQueue"];


            if (temp.Count != 0)
            {
                Word currWord = (Word)ViewState["currentWord"];
                temp.Enqueue(currWord);
                Session["wordQueue"] = temp;

                Response.Redirect(Request.RawUrl);
            }

        }
        //called on the first question
        //gets all test questions and words from DB
        public List<Word> GetWords(int languageId)
        {
            //get test questions
            int testID = (int)Session["currTestID"];
            List<int> testQuestions = new List<int>();
            try
            {
                string insertSQL = "SELECT wordID FROM TestQuestions WHERE testID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@testID", testID);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {

                        while (reader.Read())
                        {
                            testQuestions.Add(Convert.ToInt32(reader[0]));
                        }




                        reader.Close();
                    }


                    cmd.Dispose();
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;

            }


            //get words 
            List<Word> words = new List<Word>();
            strMaxScore = Convert.ToString(testQuestions.Count);
            for (int i = testQuestions.Count - 1; i >= 0; i--)
            {
                int currWord = testQuestions.ElementAt(i);
                testQuestions.RemoveAt(i);

                words.Add(GetWord(currWord));

            }
            return words;
        }

        //called when displaying the image
        //initalizes/gets the word queue, dequeues a word and sets it as current
        //returns the current word
        public Word WordSequence()
        {
            int languageId = (int)Session["currLanguage"];
            int qID = (int)Session["currQuestion"];

            List<Word> wordListTemp;
            List<Word> wordList;
            Queue<Word> wordQueue;
            Word currentWord = null;

            //if first question initiate question queue
            if (qID == 1)
            {
                userResults = new List<UserResults>();
                wordListTemp = GetWords(languageId);
                Random rnd = new Random();
                wordList = wordListTemp.OrderBy(x => rnd.Next()).ToList();
                wordQueue = new Queue<Word>(wordList);

                originalQueue = wordQueue.ToList();
                if (!_isRefresh)
                {
                    currentWord = wordQueue.Dequeue();
                }
                Session["wordQueue"] = wordQueue;

                strScoreValue = "0";
            }

            //if not first question dequeue next word
            else if (qID > 1)
            {
                Queue<Word> temp = (Queue<Word>)Session["wordQueue"];
                if (!_isRefresh)
                {
                    if (temp.Count != 0)
                    {
                        if (!_isRefresh)
                        {
                            currentWord = temp.Dequeue();
                        }
                    }
                    else
                    {
                        Response.Redirect("TestIntro.aspx");
                    }
                }
                Session["wordQueue"] = temp;
            }


            ViewState["currentWord"] = currentWord;
            GetAnswers();
            return currentWord;

        }

        //called when displaying the answer buttons
        //gets the answer word plus 3 other random ones
        //displays in random order
        public IQueryable<WordLanguage> GetAnswers()
        {
            Word word = (Word)ViewState["currentWord"];
            int wordID = word.WordID;
            int langID = (int)Session["currLanguage"];
            List<WordLanguage> answers = new List<WordLanguage>();
            try
            {
                string insertSQL = "SELECT WLWord FROM WordLanguages WHERE WLWordID = ? AND WLLanguageID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@WLWordID", wordID);
                    cmd.Parameters.AddWithValue("@WLLanguageID", langID);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();


                        answers.Add(new WordLanguage(wordID, Convert.ToString(reader[0])));

                        reader.Close();
                    }


                    cmd.Dispose();
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;

            }

            //get 3 extra answers, none of which can repeat

            Random random = new Random();
            for (int i = 0; i < 3; i++)
            {

                int randomNumber = random.Next(1, 9);

                int index = answers.FindIndex(f => f.WordID == randomNumber);

                while (randomNumber == wordID || index >= 0)
                {
                    randomNumber = random.Next(1, 9);
                    index = answers.FindIndex(f => f.WordID == randomNumber);
                }

                try
                {
                    string insertSQL = "SELECT WLWord FROM WordLanguages WHERE WLWordID = ? AND WLLanguageID = ?";

                    using (OleDbConnection conn = new OleDbConnection(conString))
                    {
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                        cmd.Parameters.AddWithValue("@WLWordID", randomNumber);
                        cmd.Parameters.AddWithValue("@WLLanguageID", langID);
                        OleDbDataReader reader = cmd.ExecuteReader();

                        if (reader != null && reader.HasRows)
                        {
                            reader.Read();


                            answers.Add(new WordLanguage(wordID, Convert.ToString(reader[0])));

                            reader.Close();
                        }


                        cmd.Dispose();
                    }
                }
                catch (OleDbException ex)
                {
                    string msg = "Select Error:";
                    msg += ex.Message;

                }

            }


            Random rnd = new Random();
            List<WordLanguage> shuffledList = answers.OrderBy(x => rnd.Next()).ToList();
            btn1.Text = shuffledList[0].Word;
            btn2.Text = shuffledList[1].Word;
            btn3.Text = shuffledList[2].Word;
            btn4.Text = shuffledList[3].Word;
            IQueryable<WordLanguage> finalAnswers = shuffledList.AsQueryable();
            return finalAnswers;

            // return query;
        }

        //called when user clicks an answer
        //increments score if correct
        //saves result as UserResults and move on to next question
        protected void AnswerBtn(object sender, EventArgs e)
        {

            Button clickedButton = (Button)sender;
            String choice = clickedButton.Text;
            int langID = (int)Session["currLanguage"];
            int id = -1;
            try
            {
                string insertSQL = "SELECT WLWordID FROM WordLanguages WHERE WLWord = ? AND WLLanguageID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@WLWord", choice);
                    cmd.Parameters.AddWithValue("@WLLanguageID", langID);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();

                        id = Convert.ToInt32(reader[0]);

                        reader.Close();
                    }


                    cmd.Dispose();
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;

            }

            Word word = (Word)ViewState["currentWord"];
            int wordID = word.WordID;
            //increase score
            if (id == wordID)
            {
                int tempID = Convert.ToInt32(strScoreValue);
                tempID++;
                strScoreValue = Convert.ToString(tempID);
            }

            Queue<Word> temp = (Queue<Word>)Session["wordQueue"];

            //move on to next question
            if (temp.Count != 0)
            {
                Word nextWord = temp.Peek();
                int nextWordID = nextWord.WordID;

                int currQ = (int)Session["currQuestion"];
                currQ++;
                Session["currQuestion"] = currQ;


                UserResults result = new UserResults(currQ - 1, word, choice, GetWordLanguage(wordID).Word);
                userResults.Add(result);
                Response.Redirect(Request.RawUrl);
            }
            //end of practise
            else
            {
                Session["finalScore"] = strScoreValue;
                Session["ogQueue"] = originalQueue as List<Word>;



                UserResults result = new UserResults((int)Session["currQuestion"], word, choice, GetWordLanguage(wordID).Word);
                userResults.Add(result);
                Session["userResults"] = userResults as List<UserResults>;

                Response.Redirect("TestResults.aspx");
            }

        }
    }
}