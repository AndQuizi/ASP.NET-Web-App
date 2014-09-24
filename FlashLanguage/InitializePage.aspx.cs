using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2
{
    //THIS PAGE IS A WORKAROUND TO ASSIGN A NEWLY CREATED ACCOUNT THEIR CURRENT COURSES
    //PUTING THIS CODE IN THE "REGISTER" PAGE DOES NOT WORK (EVEN AFTER SIGNIN FUNCTION CALL) BECAUSE HttpContext.Current.User.Identity.Name; RETURNS NOTHING
    public partial class InitializePage : System.Web.UI.Page
    {
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                     System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = getUserID();

            try
            {
                string insertSQL = "INSERT INTO StudentCourses (studentID, French, Italian) VALUES (?,?,?)";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);
                    
                        
                     
                        cmd.Parameters.AddWithValue("@studentID", id);
                        cmd.Parameters.AddWithValue("@French", false);
                        cmd.Parameters.AddWithValue("@Italian", false);

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();


                    
                }
            }
            catch (OleDbException ex)
            {
                string msg = "Insert Error:";
                msg += ex.Message;

            }
  
            Response.Redirect("Default.aspx");
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