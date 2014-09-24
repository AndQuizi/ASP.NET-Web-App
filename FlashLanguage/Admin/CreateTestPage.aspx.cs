using FlashLanguage2.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2.Admin
{
    public partial class CreateTestPage : System.Web.UI.Page
    {
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                           System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";

        public static List<Word> testWords = new List<Word>();
  
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string insertSQL = "SELECT LanguageID, LanguageName FROM Languages";
                DataTable subjects = new DataTable();

                using (OleDbConnection myConnection = new OleDbConnection(conString))
                {
                    myConnection.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(insertSQL, myConnection);

                    adapter.Fill(subjects);

                    SelectLanguage.DataSource = subjects;
                    SelectLanguage.DataTextField = "LanguageName";
                    SelectLanguage.DataValueField = "LanguageID";
                    SelectLanguage.DataBind();
                }

                Session["words"] = new List<WordList>();
               BindList();
             
            }
            StartDate.Visible = false;
            EndDate.Visible = false;
        }

        private void BindList()
        {
            ListView1.DataSource = (List<WordList>)Session["words"];
            ListView1.DataBind();
        }
        protected void AddWordBtn(object sender, EventArgs e)
        {
            string englishWord = EnglishWord.Text;
             int wordID = GetWord(englishWord);
             int langID = Convert.ToInt32(SelectLanguage.Value);
             if (wordID != -1)
             {

                 WordLanguage word = GetWordLanguage(wordID, langID);

                 if (Session["words"] == null)
                 {
                     Session["words"] = new List<WordList>();
                 }
                 //show list of words for test
                 List<WordList> words = (List<WordList>)Session["words"];
                 words.Add(new WordList(englishWord, word.Word));
                 Session["words"] = words;
                 BindList();

                 testWords.Add(new Word(wordID, englishWord, ""));


             }
             //english word does not exist in Word table
             else
             {
                 notification.InnerText = "Word does not exist in database.";

             }
        }
        protected void CreateTestBtn(object sender, EventArgs e)
        {
            int langID = Convert.ToInt32(SelectLanguage.Value);
            string testName = TestName.Text;
            int attempts = Convert.ToInt32(Attempts.Text);
            DateTime startDate = StartDate.SelectedDate;
            DateTime endDate = EndDate.SelectedDate;
            if (checkTest(langID, testName)==-1)
            {
                int testID = AddTest(langID, testName, startDate, endDate, attempts);
                if (testID != -1)
                {
                    foreach (Word word in testWords)
                    {
                        AddTestWord(word, testID);
                    }
                    Response.Redirect("AdminPage.aspx");
                }
                else
                {
                    notification.InnerText = "Could not create Test in database.";
                }

            }
            else
            {
                notification.InnerText = "A Test with the same name for that language already exists.";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (StartDate.Visible)
            {
                StartDate.Visible = false;
            }
            else
            {
                StartDate.Visible = true;
            }
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            if (EndDate.Visible)
            {
                EndDate.Visible = false;
            }
            else
            {
                EndDate.Visible = true;
            }
        }

        public int AddTest(int langID, string name, DateTime start, DateTime end, int attempts)
        {
            
            try
            {
                string insertSQL = "INSERT INTO Test (testLanguageID, testName, startDate, endDate, attempts) values (?,?, #"+start+"#, #"+end+"#,?)";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@testLanguageID", langID);
                    cmd.Parameters.AddWithValue("@testName", name);
                    cmd.Parameters.AddWithValue("@attempts", attempts);
                  
                    cmd.ExecuteNonQuery();


                    cmd.Dispose();
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;
                return -1;
            }
            int testID = -1;
            try
            {
                string insertSQL = "SELECT testID FROM Test WHERE testLanguageID= ? AND testName = ? AND attempts = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                   cmd.Parameters.AddWithValue("@testLanguageID", langID);
                    cmd.Parameters.AddWithValue("@testName", name);
                    cmd.Parameters.AddWithValue("@attempts", attempts);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();

                        testID = (int)reader[0];

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



            return testID;

        }
        public int checkTest(int langID, string name)
        {
            int testID = -1;
            try
            {
                string insertSQL = "SELECT testID FROM Test WHERE testLanguageID= ? AND testName = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@testLanguageID", langID);
                    cmd.Parameters.AddWithValue("@testName", name);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();

                        testID = (int)reader[0];

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
            return testID;
        }

        public bool AddTestWord(Word word, int testID)
        {
            
            try
            {
                string insertSQL = "INSERT INTO TestQuestions (testID, wordID) values (?,?)";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@testID", testID);
                    cmd.Parameters.AddWithValue("@wordID", word.WordID);

                    cmd.ExecuteNonQuery();


                    cmd.Dispose();
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;
                return false;
            }
            return true;

        }
        //gets word from db
        public int GetWord(string word)
        {
            int id = -1;
            try
            {
                string insertSQL = "SELECT WordID FROM Words WHERE EnglishWord = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@EnglishWord", word);
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

            return id;
        }

        private class WordList
        {
            public String EnglishWord { get; set; }
            public String Translation { get; set; }

            public WordList(string word, string t)
            {
                EnglishWord = word;
                Translation = t;

            }
        }
        //get word translations
        public WordLanguage GetWordLanguage(int wordID, int langID)
        {
       
            WordLanguage word = new WordLanguage();
            try
            {
                string insertSQL = "SELECT WLWord FROM WordLanguages WHERE WLWordID = ? AND WLLanguageID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@WordID", wordID);
                    cmd.Parameters.AddWithValue("@WLLanguageID", langID);
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
    }
}