using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FlashLangauge2;
using System.Data.OleDb;
using FlashLanguage2.Helpers;


namespace FlashLanguage2.Practise
{
    public partial class PractisePage : System.Web.UI.Page
    {
        private bool _refreshState;
        private bool _isRefresh;
        public static List<UserResults> userResults;
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                     System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
        public static List<Word> originalQueue;
        public static string strScoreValue;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["currLanguage"] == null || Session["difficulty"] == null)
            {
                Response.Redirect("../Default.aspx");
            }
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

        //end current practise
        //goes to result screen
        protected void End(object sender, EventArgs e)
        {
            
            Session["finalScore"] = strScoreValue;
            Session["ogQueue"] = originalQueue as List<Word>;

            Session["userResults"] = userResults as List<UserResults>;
            Session["currQuestion"] = (int)Session["currQuestion"] - 1;
            Response.Redirect("PractiseResults.aspx");
               
        }

        //skips current question and adds it to queue
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

        //gets words from db
        //called only on first question
        //gets words with associated practise difficulty and language
        public List<Word> GetWords(int languageId)
        {
            List<Word> words = new List<Word>();
            List<int> wordIDList = new List<int>();
            int diff = (int)Session["difficulty"];
            try
            {
                string sql = "Select [WLWordID] from [WordLanguages] where WLLanguageID = ? and Difficulty = ?";
                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@WLLanguage", languageId);
                    cmd.Parameters.AddWithValue("@Difficulty", diff);

                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            wordIDList.Add(Convert.ToInt32(reader[0]));
                        }

                        reader.Close();
                    }

                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;
                int k = 0;
            }

            while (wordIDList.Count > 0)
            {
                int currID = wordIDList.ElementAt(0);
                wordIDList.RemoveAt(0);
                try
                {
                    string sql = "Select EnglishWord, ImagePath from Words where WordID= ?";
                    using (OleDbConnection conn = new OleDbConnection(conString))
                    {
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@WordID", currID);


                        OleDbDataReader reader = cmd.ExecuteReader();

                        if (reader != null && reader.HasRows)
                        {
                            reader.Read();
                            string engWord = Convert.ToString(reader[0]);
                            string path = Convert.ToString(reader[1]);

                            words.Add(new Word(currID, engWord, path));

                            reader.Close();
                        }

                    }
                }
                catch (OleDbException ex)
                {
                    string msg = "Select Error:";
                    msg += ex.Message;
                }
            }
            int c = words.Count;
            return words;
        }


        //called when displaying words image
        //generates question queue if first turn
        //dequeus question queue if not first turn
        public Word WordSequence()
        {
            int languageId = (int)Session["currLanguageID"];
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
                //empty here
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
                if (temp.Count == 0)
                {
                    Response.Redirect("PractiseIntro.aspx");
                }
                else if (!_isRefresh)
                {
                    currentWord = temp.Dequeue();
                }
                Session["wordQueue"] = temp;
            }
            if (currentWord != null)
            {
                ViewState["currentWord"] = currentWord;
            }
            else
            {
                Response.Redirect("../Default.aspx");
            }

            GetAnswers();
            return currentWord;

        }

        //called when displaying the answer buttons
        //gets the answer word plus 3 other random ones
        //displays in random order
        public IQueryable<WordLanguage> GetAnswers()
        {
            //get the answer and add it to list
            Word w = (Word)ViewState["currentWord"];
            int wordID = w.WordID;
            int langID = (int)Session["currLanguageID"];
            List<WordLanguage> answers = new List<WordLanguage>();

            string sql = "Select WLWord from [WordLanguages] where WLLanguageID = ? and WLWordID = ?";
            try
            {
                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@WLLanguageID", langID);
                    cmd.Parameters.AddWithValue("@WLWordID", wordID);

                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        string wString = Convert.ToString(reader[0]);
                        answers.Add(new WordLanguage(wordID, wString));
                        reader.Close();
                    }

                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;
            }

            int diff = (int)Session["difficulty"];
            List<int> allWords = new List<int>();
            sql = "Select WLWordID from [WordLanguages] where WLLanguageID = ? and Difficulty = ?";
            try
            {
                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@WLLanguageID", langID);
                    cmd.Parameters.AddWithValue("@Difficulty", diff);

                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader[0]);
                            allWords.Add(id);
                        }
                        reader.Close();
                    }

                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;
            }



            Random random = new Random();
            for (int i = 0; i < 3; i++)
            {
              

                int randomNumber = random.Next(0, allWords.Count);
                int currID = allWords[randomNumber];

                int index = answers.FindIndex(f => f.WordID == currID);
                while (currID == wordID || index >= 0)
                {
                    randomNumber = random.Next(0, allWords.Count);
                    currID = allWords[randomNumber];
                    index = answers.FindIndex(f => f.WordID == currID);
                }

                sql = "Select WLWord from [WordLanguages] where WLLanguageID = ? and WLWordID = ?";
                try
                {
                    using (OleDbConnection conn = new OleDbConnection(conString))
                    {
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@WLLanguageID", langID);
                        cmd.Parameters.AddWithValue("@WLWordID", currID);

                        OleDbDataReader reader = cmd.ExecuteReader();

                        if (reader != null && reader.HasRows)
                        {
                            reader.Read();
                            string wString = Convert.ToString(reader[0]);
                            answers.Add(new WordLanguage(currID, wString));
                            reader.Close();
                        }

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
        }

        //called when user clicks an answer
        //increments score if correct
        //saves result as UserResults and move on to next question
        protected void AnswerBtn(object sender, EventArgs e)
        {

            Button clickedButton = (Button)sender;
            String choice = clickedButton.Text;
            int languageId = (int)Session["currLanguageID"];
            Word w = (Word)ViewState["currentWord"];
            int wordID = w.WordID;
            int id=-1;
            string sql = "Select WLWordID from [WordLanguages] where WLLanguageID = ? and WLWord = ?";
            try
            {
                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@WLLanguageID", languageId);
                    cmd.Parameters.AddWithValue("@WLWord", choice);

                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                            id = Convert.ToInt32(reader[0]);
                        reader.Close();
                    }

                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;
            }
             sql = "Select WLWord from [WordLanguages] where WLLanguageID = ? and WLWordID = ?";
             string answerWord = "";
            try
            {
                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@WLLanguageID", languageId);
                    cmd.Parameters.AddWithValue("@WLWordID", wordID);

                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        answerWord = Convert.ToString(reader[0]);
                        reader.Close();
                    }

                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;
            }
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

                UserResults result = new UserResults(currQ - 1, w, choice, answerWord);
                userResults.Add(result);
                Response.Redirect(Request.RawUrl);
            }
            //end of practise
            else
            {
                Session["finalScore"] = strScoreValue;
                Session["ogQueue"] = originalQueue as List<Word>;


                Word currWord = (Word)ViewState["currentWord"];

                UserResults result = new UserResults((int)Session["currQuestion"], w, choice, answerWord);
                userResults.Add(result);
                Session["userResults"] = userResults as List<UserResults>;

                Response.Redirect("PractiseResults.aspx");
            }

        }

    }

}
