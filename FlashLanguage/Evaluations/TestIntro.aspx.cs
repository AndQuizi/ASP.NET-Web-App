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
    public partial class TestInto : System.Web.UI.Page
    {
        //max attempts doesnt check DB
        //set to 3 for now
        
        public static List<Test> currTests;
        public static List<TestAttempts> currAttempts;
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                    System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["regToken"] != null)
            {
                Session.Remove("regToken");
            }
           if (!InCourse())
            {
                Response.Redirect("CourseRegister.aspx");
            }


         
        }
        public Boolean InCourse()
        {
            String course = (String)Session["CurrentCourse"];
            if (course == null)
            {
                Response.Redirect("../Default.aspx");
            }
            String id = getUserID();
            Session["currUserID"] = id;
            Boolean signedUp = false;
            try
            {
                string insertSQL = "SELECT  " + course + "  FROM StudentCourses WHERE studentID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);
                    cmd.Parameters.AddWithValue("@studentID", id);

                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();

                        signedUp = Convert.ToBoolean(reader[0]);

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
            return signedUp;


        }

        //get users id from DB
        public String getUserID()
        {
            String currentName = HttpContext.Current.User.Identity.Name;
            String id = "";
            try
            {
                string insertSQL = "SELECT UserId FROM aspnet_Membership WHERE Username = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@Username", currentName);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();

                        id = Convert.ToString(reader[0]);

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

        //get tests with current language and date
        public IQueryable<TestAttempts> GetTests()
        {
            int languageId = (int)Session["currLanguage"];
            currTests = new List<Test>();
            DateTime currTime = DateTime.Now;
            try
            {
                string insertSQL = "SELECT testID, testName, startDate, endDate, attempts FROM Test WHERE testLanguageID= ? AND startDate < #" + currTime + "# AND endDate > #"+currTime+"#";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@testLanguageID", languageId);
                    //cmd.Parameters.AddWithValue("@startDate", currTime);
                    //cmd.Parameters.AddWithValue("@endDate", currTime);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            currTests.Add(new Test((int)reader[0], languageId, (string)reader[1], (DateTime)reader[2], (DateTime)reader[3], (int)reader[4]));
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
            int c = currTests.Count;
            currAttempts = new List<TestAttempts>();

            foreach (Test test in currTests)
            {
                int testID = test.TestID;
                String testName = test.TestName;
                int Tattempts = test.Attempts;
                String id = getUserID();
                int attempts = 0;

                try
                {
                    string insertSQL = "SELECT Attempts FROM StudentTests WHERE studentID = ? AND testID = ?";

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
                            attempts = (int)reader[0];
                        }


                        cmd.Dispose();
                    }
                }
                catch (OleDbException ex)
                {
                    string msg = "Select Error:";
                    msg += ex.Message;

                }

                currAttempts.Add(new TestAttempts(testName, Tattempts, attempts));
            }
            IQueryable<TestAttempts> query2 = currAttempts.AsQueryable();
            return query2;
        }


        //called when user clicks test to start
        //checks attempts and starts test
        protected void StartTest(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            String choice = clickedButton.Text;
            int languageId = (int)Session["currLanguage"];
            List<Test> tempList = new List<Test>();
            try
            {
                string insertSQL = "SELECT testID, testName, startDate, endDate, attempts FROM Test WHERE testLanguageID= ? AND testName = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@testLanguageID", languageId);
                    cmd.Parameters.AddWithValue("@testName", choice);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tempList.Add(new Test((int)reader[0], languageId, (string)reader[1], (DateTime)reader[2], (DateTime)reader[3], (int)reader[4]));
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
            
            int id = tempList.First().TestID;

            Session["currTestID"] = id;
            Session["currQuestion"] = 1;
            Session["totalQuestions"] = getTotalQuestions(id);

            //check if studentTest has this student and test. if not create a row. if so, increase attempt count
            int attempts = checkAttempts(id);

            //attempts = -1 , means student never taken this test before
            if (attempts < 0)
            {
                CreateStudentTest(id);
                Session["currAttempt"] = 1;
                Session["maxAttempts"] = checkTestAttempts(id);
                Response.Redirect("TestPage.aspx");
            }
            //student can take test again
            else if (attempts < checkTestAttempts(id))
            {
                attempts++;
                IncrementAttempts(id, attempts);
                Session["currAttempt"] = attempts;
                Session["maxAttempts"] = checkTestAttempts(id);
                Response.Redirect("TestPage.aspx");
            }
            //student maxed out their attempts
            else
            {
                notification.InnerText = "Max Attempts has been reached for this test.";
            }



           
        }

        //get number of questions in test chosen
        public int getTotalQuestions(int id)
        {
            int questionCount = 0;
            try
            {
                string insertSQL = "SELECT count(*) FROM TestQuestions WHERE testID= ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);
                    cmd.Parameters.AddWithValue("@testID", id);

                    questionCount = (int)cmd.ExecuteScalar();


                    cmd.Dispose();
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;

            }

            return questionCount;

        }
        //check if user can attempt test
        //returns student last attempt number
        public int checkAttempts(int testID)
        {
            String id = (String)Session["currUserID"];
            int attempts = -1;
            try
            {
                string insertSQL = "SELECT Attempts FROM StudentTests WHERE studentID = ? AND testID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);
                    cmd.Parameters.AddWithValue("@studentID", id);
                    cmd.Parameters.AddWithValue("@testID", testID);

                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            attempts = (int)reader[0];
                        }
                    }

                    cmd.Dispose();
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Select Error:";
                msg += ex.Message;

            }
            return attempts;

        }

        public int checkTestAttempts(int testID)
        {
 
            int languageId = (int)Session["currLanguage"];
            currTests = new List<Test>();
            DateTime currTime = DateTime.Now;
            int attempts = -1;
            try
            {
                string insertSQL = "SELECT attempts FROM Test WHERE testID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@testID", testID);
       
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        attempts = (int)reader[0];
                           


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

            return attempts;
        }
        //called when student takes test for the first time
        //adds row to studentTest table with attempt 1 and score 0
        public void CreateStudentTest(int testID)
        {
            String id = (String)Session["currUserID"];
            try
            {
                string insertSQL = "INSERT INTO StudentTests (studentID, testID, Attempts, Score) VALUES ( ?, ?, ?, ?) ";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                   OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                   cmd.Parameters.AddWithValue("@studentID", id);
                   cmd.Parameters.AddWithValue("@testID", testID);
                   cmd.Parameters.AddWithValue("@Attempts", 1);
                   cmd.Parameters.AddWithValue("@Score", 0);

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

        //called if student took test before
        //increments test attempt in DB
        public void IncrementAttempts(int testID, int attempts)
        {
            String id = (String)Session["currUserID"];
            try
            {
                string insertSQL = "UPDATE StudentTests SET Attempts = ? WHERE testID = ? AND studentID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@Attempts", attempts);
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

    }
}