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
    public partial class DeleteTestPage : System.Web.UI.Page
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

        protected void DeleteTestBtn(object sender, EventArgs e)
        {
            int testID = Convert.ToInt32(SelectTest.Value);
            try
            {
                string insertSQL = "DELETE FROM Test WHERE testID = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);



                    cmd.Parameters.AddWithValue("@testID", testID);
      

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();



                }
                notification.InnerText = "Delete Successful";
            }
            catch (OleDbException ex)
            {
                string msg = "Delete Error:";
                msg += ex.Message;
                notification.InnerText = "Delete Unsuccessful";
            }
           
        }

       
    }
}