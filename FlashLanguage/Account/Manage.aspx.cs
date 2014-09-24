using FlashLangauge2.App_Code;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2.Account
{
    public partial class Manage : System.Web.UI.Page
    {
        private string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                             System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
        protected string SuccessMessage
        {
            get;
            private set;
        }

        protected bool CanRemoveExternalLogins
        {
            get;
            private set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User.Identity.Name == "")
            {
                Response.Redirect("../Default.aspx");
            }

            if (!IsPostBack)
            {
               
                // Render success message
                var message = Request.QueryString["m"];
                if (message != null)
                {
                    // Strip the query string from action
                    Form.Action = ResolveUrl("~/Account/Manage");

                    SuccessMessage =
                        message == "ChangePwdSuccess" ? "Your password has been changed."
                        : message == "SetPwdSuccess" ? "Your password has been set."
                        : message == "SetPwdFail" ? "Error: Your password has not been changed."
                        : message == "RemoveLoginSuccess" ? "The account was removed."
                        : String.Empty;
                    successMessage.Visible = !String.IsNullOrEmpty(SuccessMessage);
                }
            }
        }

        protected void ChangePassword_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                
                bool result = ChangePassword(Context.User.Identity.Name, CurrentPassword.Text, NewPassword.Text);
                if (result)
                {
                    Response.Redirect("~/Account/Manage?m=ChangePwdSuccess");
                }
                else
                {
                    Response.Redirect("~/Account/Manage?m=SetPwdFail");
                }
            }
        }

        public bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            
            try
            {


                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand("UPDATE aspnet_Membership SET [Password] = ? WHERE [Username] = ?", conn);

                    cmd.Parameters.AddWithValue("@Password", EncodePassword(newPwd));
                    cmd.Parameters.AddWithValue("@Username", username);

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
        private string EncodePassword(string password)
        {

            MyCustomMembershipProvider provider = new MyCustomMembershipProvider();

            string encodedPassword = provider.EncodePassword(password);

            return encodedPassword;
        }
     

      
    }
}