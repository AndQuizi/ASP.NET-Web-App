using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2
{
    public partial class French : System.Web.UI.Page
    {
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                     System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Practise(object sender, EventArgs e)
        {
            Session["currLanguage"] = "French";
            Response.Redirect("~/Practise/PractiseIntro.aspx");
        }

        //called when user clicks test button
        //checks if the user is registered for course
        //if not redirect them to course register page
        protected void Test(object sender, EventArgs e)
        {
            if (InCourse())
            {
                Session["currLanguage"] = 2;
                Session["CurrentCourse"] = "French";
                Response.Redirect("/Evaluations/TestIntro.aspx");
            }
            else{
                Session["currLanguage"] = 2;
                Session["CurrentCourse"] = "French";
                Session["regToken"] = 1;
                Response.Redirect("/Evaluations/CourseRegister.aspx");
            }
        }

        //checks DB if student is registered for course
        public Boolean InCourse()
        {
            String id = getUserID();
            Session["currUserID"] = id;
            Boolean signedUp = false;
            try
            {
                string insertSQL = "SELECT French FROM StudentCourses WHERE studentID = ?";

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

    }
}