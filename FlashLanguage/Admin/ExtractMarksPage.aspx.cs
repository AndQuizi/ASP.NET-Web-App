using FlashLanguage2.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2.Admin
{
    public partial class ExtractMarksPage : System.Web.UI.Page
    {
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                           System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
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

                int langID = Convert.ToInt32(SelectLanguage.Value);
                insertSQL = "SELECT testID, testName FROM Test WHERE testLanguageID = " + langID + "";
                DataTable subjects2 = new DataTable();

                using (OleDbConnection myConnection = new OleDbConnection(conString))
                {
                    myConnection.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(insertSQL, myConnection);

                  

                    adapter.Fill(subjects2);

                    SelectTest.DataSource = subjects2;
                    SelectTest.DataTextField = "testName";
                    SelectTest.DataValueField = "testID";
                    SelectTest.DataBind();
                }
            
            }
        }

        protected void updateTests(object sender, EventArgs e)
        {
            int langID = Convert.ToInt32(SelectLanguage.Value);
           string insertSQL = "SELECT testID, testName FROM Test WHERE testLanguageID = " + langID + "";
            DataTable subjects2 = new DataTable();

            using (OleDbConnection myConnection = new OleDbConnection(conString))
            {
                myConnection.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter(insertSQL, myConnection);



                adapter.Fill(subjects2);

                SelectTest.DataSource = subjects2;
                SelectTest.DataTextField = "testName";
                SelectTest.DataValueField = "testID";
                SelectTest.DataBind();

            }
        }
        protected void ExtractMarksBtn(object sender, EventArgs e)
        {
            
            int testID = Convert.ToInt32(SelectTest.Value);
            List<StudentScore> scores = new List<StudentScore>();
            try
            {
                string insertSQL = "SELECT studentID, Score FROM  StudentTests WHERE testID= ?";

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


                            scores.Add(new StudentScore(Convert.ToString(reader[0]), Convert.ToInt32(reader[1])));
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
            string testName = "";
            try
            {
                string insertSQL = "SELECT testName FROM Test WHERE testID= ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@testID", testID);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        testName = Convert.ToString(reader[0]);
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
            foreach (StudentScore student in scores)
            {
                string id = student.studentID;
                try
                {
                    string insertSQL = "SELECT FirstName, LastName FROM aspnet_Membership WHERE UserID= ?";

                    using (OleDbConnection conn = new OleDbConnection(conString))
                    {
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                        cmd.Parameters.AddWithValue("@UserID", id);
                        OleDbDataReader reader = cmd.ExecuteReader();

                        if (reader != null && reader.HasRows)
                        {
                            reader.Read();

                            student.firstName = Convert.ToString(reader[0]);
                            student.lastName = Convert.ToString(reader[1]);

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



            int total = getTotalQuestions(testID);


            FileDownload dwnload = new FileDownload();
            dwnload.ProcessRequest(HttpContext.Current, testName, scores, total);

           
        }

      

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
    }
}