using System.Data.OleDb;
using System.Globalization;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Web;
using System.Web.UI;
using FlashLanguage2;
using FlashLangauge2.App_Code;

namespace FlashLanguage2.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User.Identity.Name != "")
            {
                Response.Redirect("../Default.aspx");
            }
          
        }
        protected void LogIn(object sender, EventArgs e)
        {
            if (ValidateUser(UserName.Text, Password.Text))
            {
                SetupFormsAuthTicket(UserName.Text, true);

                //Boolean x = HttpContext.Current.User.Identity.IsAuthenticated;
                //String y = HttpContext.Current.User.Identity.Name;
                FormsAuthentication.RedirectFromLoginPage(UserName.Text, true);
            }
            else
                notification.InnerText = "Incorrect login information.";
        }
        /// <summary>
        /// Filter out the fat fingers who get their passwords wrong
        /// </summary>
        bool ValidateUser(string user, string pass)
        {
            string connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                      System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();
                string sql = "Select Username from aspnet_Membership where Username = ? and Password = ?";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", user);
                cmd.Parameters.AddWithValue("@Password", EncodePassword(pass));
                return cmd.ExecuteScalar() is string;
            }
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

        private string EncodePassword(string password)
        {
      
        MyCustomMembershipProvider provider = new MyCustomMembershipProvider();

           string encodedPassword = provider.EncodePassword(password);

          return encodedPassword;
            //return password;
        }
    }
}