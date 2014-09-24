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
    public partial class TestResults : System.Web.UI.Page
    {
        public String strFinalScore;
        public String totalQuestions;

        public List<Word> questionQueue;
        public List<UserResults> userResults;
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                    System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (Session["currTestID"] == null || Session["currAttempt"] == null || Session["totalQuestions"] == null)
            {
                Response.Redirect("../Default.aspx");
            }

            //get results and test questions
            strFinalScore = (String)Session["finalScore"];
            questionQueue = Session["ogQueue"] as List<Word>;
            userResults = Session["userResults"] as List<UserResults>;

            totalQuestions = Convert.ToString(Session["totalQuestions"]);



            //record score in DB
            if (getHighScore() < Convert.ToInt32((String)Session["finalScore"]))
            {
                recordResults();
            }
            //had to move these because they were called before recordResults(). Why is that?
            /*
            Session.Remove("finalScore");
            Session.Remove("currentWord");
            Session.Remove("wordQueue");
            Session.Remove("ogQueue");
            Session.Remove("userResults");
            Session.Remove("currTestID");
            Session.Remove("currQuestion");
            Session.Remove("currAttempt");
            Session.Remove("totalQuestions");
           */
        }

        //returns to their current language
        protected void Finish(object sender, EventArgs e)
        {
            int id = (int)Session["currLanguage"];
            Session.Remove("currLanguage");
            Session.Remove("finalScore");
            Session.Remove("currentWord");
            Session.Remove("wordQueue");
            Session.Remove("ogQueue");
            Session.Remove("userResults");
            Session.Remove("currTestID");
            Session.Remove("currQuestion");
            Session.Remove("currAttempt");
            Session.Remove("totalQuestions");

            Session.Remove("currUserID");
            Session.Remove("maxAttempts");
            if (id == 1)
            {

                Response.Redirect("../Italian.aspx?");
            }
            else
            {
                Response.Redirect("../French.aspx?");
            }
        }
        //returns to test intro
        protected void TryAgain(object sender, EventArgs e)
        {
            Session.Remove("finalScore");
            Session.Remove("currentWord");
            Session.Remove("wordQueue");
            Session.Remove("ogQueue");
            Session.Remove("userResults");
            Session.Remove("currTestID");
            Session.Remove("currQuestion");
            Session.Remove("currAttempt");
            Session.Remove("totalQuestions");

            Session.Remove("currUserID");
            Session.Remove("maxAttempts");

            Response.Redirect("/Evaluations/TestIntro.aspx?");
        }

       

        //Get user results to display
        public IQueryable<UserResults> GetResults()
        {

            IQueryable<UserResults> query = Queryable.AsQueryable(userResults);
            return query;
        }

        //Update DB on students test. Always replaces old score.
        public void recordResults()
        {

            String id = (String)Session["currUserID"];
            int score = Convert.ToInt32((String)Session["finalScore"]);
            int testID = (int)Session["currTestID"];

      
            try
            {
                string insertSQL = "UPDATE StudentTests SET Score = ? WHERE testID = ? AND studentID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@testID", testID);
                    cmd.Parameters.AddWithValue("@studentID", id);
                       
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Insert Error:";
                msg += ex.Message;

            }
        }
        public int getHighScore()
        {
            String id = (String)Session["currUserID"];

            int testID = (int)Session["currTestID"];
            int score = -1;
            try
            {
                string insertSQL = "SELECT Score FROM StudentTests WHERE studentID = ? AND testID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@studentID", id);
                    cmd.Parameters.AddWithValue("@testID", testID);

                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        
                            score = (int)reader[0];
                            reader.Close();

                    }
                    cmd.Dispose();
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Insert Error:";
                msg += ex.Message;

            }
            return score;
        }
    }
}