using System.Data.OleDb;
using System.Globalization;
using System.Web;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.UI;
using FlashLanguage2;
using FlashLangauge2.App_Code;

namespace FlashLanguage2.Account
{
    public partial class Register : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User.Identity.Name != "")
            {
                Response.Redirect("../Default.aspx");
            }

        }
        protected void CreateUser_Click(object sender, EventArgs e)
        {
            //var manager = new UserManager();
            //var user = new ApplicationUser() { UserName = UserName.Text };
            //IdentityResult result = manager.Create(user, Password.Text);
            //if (result.Succeeded)
            //{
            //    IdentityHelper.SignIn(manager, user, isPersistent: false);
            //    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            //}
            //else
            //{
            //    ErrorMessage.Text = result.Errors.FirstOrDefault();
            //}

            MembershipCreateStatus status;
            MyCustomMembershipProvider provider = new MyCustomMembershipProvider();
            if (provider.CreateUser(UserName.Text, Password.Text, Email.Text, "", "", false, null, out status, FirstName.Text, LastName.Text))
            {
                SetupFormsAuthTicket(UserName.Text, true);

                //this page call is a workaround to initilize the newly created accounts courses
                Response.Redirect("../InitializePage.aspx");

            }
            else
            {

                notification.InnerText = "Incorrect login";
            }
        }
        private void SetupFormsAuthTicket(string userName, bool persistanceFlag)
        {
            //OleDbMembershipUser user;

            string userId = GetUserID(userName);

            var userData = userId.ToString(CultureInfo.InvariantCulture);
            var authTicket = new FormsAuthenticationTicket(1, //version
                                userName, // user name
                                DateTime.Now,             //creation
                                DateTime.Now.AddMinutes(30), //Expiration
                                persistanceFlag, //Persistent
                                userData);

            var encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie cookie = (new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            cookie.Path = FormsAuthentication.FormsCookiePath;
            // Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            if (!persistanceFlag)
            {
                cookie.Expires = DateTime.Now.AddYears(-1);
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        string GetUserID(string user)
        {
            string connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                      System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
            string userID = "";
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();
                string sql = "Select UserId from aspnet_Membership where Username = ?";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", user);
                //cmd.Parameters.AddWithValue("@Password", EncodePassword(pass));
                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    reader.Read();
                    userID = Convert.ToString(reader[0]);
                }
                return userID;

            }

        }
        private string EncodePassword(string password)
        {

            MyCustomMembershipProvider provider = new MyCustomMembershipProvider();

            string encodedPassword = provider.EncodePassword(password);

            return encodedPassword;
        }
      
    }
}