using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2.Evaluations
{
    public partial class CourseRegister : System.Web.UI.Page
    {
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                     System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["currLanguage"] == null || Session["regToken"] ==null)
            {
                Response.Redirect("../Default.aspx");
            }
           
        }

        //called when user clicks submit code button
        //calls method to check if the entered string matches the course code
        protected void SubmitCode(object sender, EventArgs e)
        {
            //get user input
            String userCode = CourseCode.Text;
            //get actual course code
            String courseCode = getCourseCode();

            if (userCode == courseCode)
            {
                //register user for course
                activiateAccount();
                // String course = (String)Session["CurrentCourse"];

                
                Response.Redirect("/Evaluations/TestIntro.aspx");


            }
            else
            {
                notification.InnerText = "Incorrect Code";
            }



        }

        //checks DB for the course code of the current language
        public String getCourseCode()
        {
            int courseID = (int)Session["currLanguage"];
        
            String code = "";
            try
            {
                string insertSQL = "SELECT CourseCode FROM Languages WHERE LanguageID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@LanguageID", courseID);

                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();

                        code = Convert.ToString(reader[0]);

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
            return code;
        }

        //set language DB value to true
        public void activiateAccount()
        {
            int courseID = (int)Session["currLanguage"]; 


           // String id = getUserID();
            string id = (string)Session["currUserID"];
            if (courseID == 1)
            {
                try
                {
                    string insertSQL = "UPDATE StudentCourses SET Italian = ? WHERE studentID  = ?";

                    using (OleDbConnection conn = new OleDbConnection(conString))
                    {
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(insertSQL, conn);


                        cmd.Parameters.AddWithValue("@Italian", true);
                        cmd.Parameters.AddWithValue("@studentID", id);

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();



                    }
                }
                catch (OleDbException ex)
                {
                    string msg = "Update Error:";
                    msg += ex.Message;

                }
            }

            else
            {
                try
                {
                    string insertSQL = "UPDATE StudentCourses SET French = ? WHERE studentID  = ?";

                    using (OleDbConnection conn = new OleDbConnection(conString))
                    {
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(insertSQL, conn);


                        cmd.Parameters.AddWithValue("@French", true);
                        cmd.Parameters.AddWithValue("@studentID", id);

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();



                    }
                }
                catch (OleDbException ex)
                {
                    string msg = "Update Error:";
                    msg += ex.Message;

                }
            }

        }

       

    }
}