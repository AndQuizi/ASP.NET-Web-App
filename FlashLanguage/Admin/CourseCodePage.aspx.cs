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
    public partial class CourseCodePage : System.Web.UI.Page
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
            }
        }

        protected void UpdateCourseCode(object sender, EventArgs e)
        {
            int langID = Convert.ToInt32(SelectLanguage.Value);
            string newCode = CourseCode.Text;

            try
            {
                string insertSQL = "UPDATE Languages SET CourseCode = ? WHERE LanguageID= ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@CourseCode", newCode);
                    cmd.Parameters.AddWithValue("@LanguageID", langID);
 
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                }
               
            }
            catch (OleDbException ex)
            {
                string msg = "Update Error:";
                msg += ex.Message;
                notification.InnerText = "Could not update.";
            }
            if (langID == 1)
            {
                try
                {
                    string insertSQL = "UPDATE StudentCourses SET Italian= ?";

                    using (OleDbConnection conn = new OleDbConnection(conString))
                    {
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                        cmd.Parameters.AddWithValue("@Italian", false);
   

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }
                    notification.InnerText = "Update Successful.";
                }
                catch (OleDbException ex)
                {
                    string msg = "Update Error:";
                    msg += ex.Message;
                    notification.InnerText = "Could not update.";
                }

            }
            if (langID == 2)
            {
                try
                {
                    string insertSQL = "UPDATE StudentCourses SET French= ?";

                    using (OleDbConnection conn = new OleDbConnection(conString))
                    {
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                        cmd.Parameters.AddWithValue("@Italian", false);


                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }
                    notification.InnerText = "Update Successful.";
                }
                catch (OleDbException ex)
                {
                    string msg = "Update Error:";
                    msg += ex.Message;
                    notification.InnerText = "Could not update.";
                }

            }
        }
    }
}