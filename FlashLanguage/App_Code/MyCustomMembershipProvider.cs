using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Data;
using System.Data.Odbc;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Data.OleDb;
using FlashLanguage2;

namespace FlashLangauge2.App_Code
{
    //http://msdn.microsoft.com/en-us/library/vstudio/6tc47t75(v=vs.100).aspx

    public class MyCustomMembershipProvider : MembershipProvider
    {
        private int newPasswordLength = 8;
        private string eventSource = "MyCustomMembershipProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                     System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";

        //
        // Used when determining encryption key values.
        //

        private MachineKeySection machineKey;
        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool pWriteExceptionsToEventLog;
        public bool WriteExceptionsToEventLog
        {
            get { return pWriteExceptionsToEventLog; }
            set { pWriteExceptionsToEventLog = value; }
        }

        //
        // System.Configuration.Provider.ProviderBase.Initialize Method
        //

        public override void Initialize(string name, NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "MyCustomMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample OleDB Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            pApplicationName = GetConfigValue(config["FlashLanguage"],
                                            System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }

            switch (temp_format)
            {
                case "Hashed":
                    pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            //
            // Initialize OdbcConnection.
            //

            ConnectionStringSettings ConnectionStringSettings =
              ConfigurationManager.ConnectionStrings[config["AccessFileName"]];

            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = ConnectionStringSettings.ConnectionString;


            // Get encryption and decryption key information from the configuration.
            Configuration cfg =
              WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords " +
                                                "are not supported with auto-generated keys.");
        }


        //
        // A helper function to retrieve config values from the configuration file.
        //

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }


        //
        // System.Web.Security.MembershipProvider properties.
        //


        private string pApplicationName;
        private bool pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;
        private int pMinRequiredNonAlphanumericCharacters;
        private int pMinRequiredPasswordLength;
        private string pPasswordStrengthRegularExpression;

        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return pEnablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return pEnablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return pRequiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return pRequiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return pMaxInvalidPasswordAttempts; }
        }


        public override int PasswordAttemptWindow
        {
            get { return pPasswordAttemptWindow; }
        }


        public override MembershipPasswordFormat PasswordFormat
        {
            get { return pPasswordFormat; }
        }


        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return pMinRequiredNonAlphanumericCharacters; }
        }


        public override int MinRequiredPasswordLength
        {
            get { return pMinRequiredPasswordLength; }
        }


        public override string PasswordStrengthRegularExpression
        {
            get { return pPasswordStrengthRegularExpression; }
        }

        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            if (!ValidateUser(username, oldPwd))
                return false;


            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, newPwd, false);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");


            OleDbConnection conn = new OleDbConnection(connectionString);

            OleDbCommand cmd = new OleDbCommand("UPDATE aspnet_Membership SET [Password] = ? WHERE [Username] = ?", conn);

            cmd.Parameters.AddWithValue("@Password", EncodePassword(newPwd));
            cmd.Parameters.AddWithValue("@Username", username);

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (OleDbException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePassword");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username,
                  string password,
                  string newPwdQuestion,
                  string newPwdAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand("UPDATE aspnet_Membership " +
                    " SET PasswordQuestion = ?, PasswordAnswer = ?" +
                    " WHERE Username = ?", conn);

            cmd.Parameters.Add("@Question", OleDbType.VarChar, 255).Value = newPwdQuestion;
            cmd.Parameters.Add("@Answer", OleDbType.VarChar, 255).Value = EncodePassword(newPwdAnswer);
            cmd.Parameters.Add("@Username", OleDbType.VarChar, 255).Value = username;
            //cmd.Parameters.Add("@ApplicationName", OleDbType.VarChar, 255).Value = pApplicationName;


            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (OleDbException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Dispose();
            }

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }

        //this create user is not used
        //made a custom one that also accepts first and last name
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args =
        new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }



            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = GetUser(username, false);

            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                if (providerUserKey == null)
                {
                    providerUserKey = Guid.NewGuid();
                }
                else
                {
                    if (!(providerUserKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return null;
                    }
                }

                OleDbConnection conn = new OleDbConnection(connectionString);
                OleDbCommand cmd = new OleDbCommand("INSERT INTO aspnet_Membership " +
                      " (UserId, Username, Password, Email, " +
                      " LastLoginDate, CreateDate" +
                      " Values(?, ?, ?, ?, ?, ?)", conn);

                cmd.Parameters.Add("@UserId", OleDbType.Guid).Value = providerUserKey;
                cmd.Parameters.Add("@Username", OleDbType.VarChar, 255).Value = username;
                cmd.Parameters.Add("@Password", OleDbType.VarChar, 255).Value = EncodePassword(password);
                cmd.Parameters.Add("@Email", OleDbType.VarChar, 128).Value = email;
                cmd.Parameters.Add("@CreateDate", OleDbType.Date).Value = createDate;
                cmd.Parameters.Add("@LastLoginDate", OleDbType.Date).Value = createDate;

                try
                {
                    conn.Open();

                    int recAdded = cmd.ExecuteNonQuery();

                    if (recAdded > 0)
                    {
                        status = MembershipCreateStatus.Success;
                    }
                    else
                    {
                        status = MembershipCreateStatus.UserRejected;
                    }
                }
                catch (OleDbException e)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "CreateUser");
                    }

                    status = MembershipCreateStatus.ProviderError;
                }
                finally
                {
                    conn.Close();
                }


                return GetUser(username, false);
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }


            return null;
        }

        //This is the Create user function that is actually used. Accepts First and Last name as extra parameters
        public Boolean CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status, string firstName, string lastName)
        {
            ValidatePasswordEventArgs args =
        new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return false;
            }



            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return false;
            }
            OleDbMembershipUser u = GetUser2(username, false);

            //if username doesnt already exist
            //currently allows dupe emails
            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                if (providerUserKey == null)
                {
                    providerUserKey = Guid.NewGuid();
                }
                else
                {
                    if (!(providerUserKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return false;
                    }
                }
                connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                      System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
                OleDbConnection conn = new OleDbConnection(connectionString);
                try
                {
                    conn.Open();
                //OleDbCommand cmd = new OleDbCommand("INSERT INTO aspnet_Membership " +
                      //" Values('" + providerUserKey + "', '" + EncodePassword(password) + "', '" + email + "', #" + createDate + "#, #" + createDate + "#, '" + username + "', '" + firstName + "', '" + lastName + "')", conn);

               
                    //Password is a reserved keyword in MS Access. Needs square brackets
                OleDbCommand cmd = new OleDbCommand("INSERT INTO aspnet_Membership " +
                      " (UserId, [Password], Email," +
                      " CreateDate, LastLoginDate, Username, FirstName, LastName)" +
                      " values(?,?,?,?,?,?,?,?)", conn);

                cmd.Parameters.Add("@UserId", OleDbType.Guid).Value = providerUserKey;  
                cmd.Parameters.Add("@Password", OleDbType.VarChar, 255).Value = EncodePassword(password);
                cmd.Parameters.Add("@Email", OleDbType.VarChar, 128).Value = email;
                cmd.Parameters.Add("@CreateDate", OleDbType.Date).Value = createDate;
                cmd.Parameters.Add("@LastLoginDate", OleDbType.Date).Value = createDate;
                cmd.Parameters.Add("@Username", OleDbType.VarChar, 255).Value = username;
                cmd.Parameters.Add("@FirstName", OleDbType.VarChar).Value = firstName;
                cmd.Parameters.Add("@LastName", OleDbType.VarChar).Value = lastName;
               
                

                    int recAdded = cmd.ExecuteNonQuery();

                    if (recAdded > 0)
                    {
                        status = MembershipCreateStatus.Success;
                    }
                    else
                    {
                        status = MembershipCreateStatus.UserRejected;
                    }
                }
                catch (OleDbException e)
                {
                    
                        WriteToEventLog(e, "CreateUser");
                    

                    status = MembershipCreateStatus.ProviderError;
                }
                finally
                {
                    conn.Close();
                }


                return true;
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return false;
            }


            
        }


        //not yet tested
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand("DELETE FROM aspnet_Membership " +
                    " WHERE Username = ?", conn);

            cmd.Parameters.Add("@Username", OleDbType.VarChar, 255).Value = username;
            //cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName;

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();

                if (deleteAllRelatedData)
                {
                    // Process commands to delete all data for the user in the database.
                }
            }
            catch (OleDbException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteUser");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
                return true;

            return false;
        }



        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot retrieve Hashed passwords.");
            }

            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand("SELECT Password FROM Users " +
                  " WHERE Username = ?", conn);

            cmd.Parameters.Add("@Username", OleDbType.VarChar, 255).Value = username;
            //cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName;

            string password = "";
            string passwordAnswer = "";
            OleDbDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    password = reader.GetString(0);
                    passwordAnswer = reader.GetString(1);
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }
            }
            catch (OleDbException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetPassword");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }


            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {


                throw new MembershipPasswordException("Incorrect password answer.");
            }


            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = UnEncodePassword(password);
            }

            return password;
        }


        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand("SELECT UserId, Username, Email, LastLoginDate" +
                " FROM aspnet_Membership WHERE Username = ?", conn);

            cmd.Parameters.Add("@Username", OleDbType.VarChar, 255).Value = username;
            //cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName;

            OleDbMembershipUser u = null;
            OleDbDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);

                    if (userIsOnline)
                    {
                        OleDbCommand updateCmd = new OleDbCommand("UPDATE Users " +
                                  "SET LastLoginDate = ? " +
                                  "WHERE Username = ?", conn);

                        updateCmd.Parameters.Add("@LastLoginDate", OleDbType.Date).Value = DateTime.Now;
                        updateCmd.Parameters.Add("@Username", OleDbType.VarChar, 255).Value = username;
                        //updateCmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName;

                        updateCmd.ExecuteNonQuery();
                    }
                }

            }
            catch (OleDbException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            return u;
        }
        public OleDbMembershipUser GetUser2(object providerUserKey, bool userIsOnline)
        {
            //connectionString = ConfigurationManager.ConnectionStrings["AccessFileName"].ToString();
            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                      System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand("SELECT UserId, Username, Email, LastLoginDate" +
                  " FROM aspnet_Membership WHERE UserId = ?", conn);

            cmd.Parameters.Add("@UserId", OleDbType.VarChar).Value = providerUserKey;

            OleDbMembershipUser u = null;
            OleDbDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);

                    if (userIsOnline)
                    {
                        OleDbCommand updateCmd = new OleDbCommand("UPDATE aspnet_Membership " +
                                  "SET LastLoginDate = ? " +
                                  "WHERE UserId = ?", conn);

                        updateCmd.Parameters.Add("@LastLoginDate", OleDbType.Date).Value = DateTime.Now;
                        updateCmd.Parameters.Add("@UserId", OleDbType.Guid).Value = providerUserKey;

                        updateCmd.ExecuteNonQuery();
                    }
                }

            }
            catch (OleDbException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(Object, Boolean)");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            return u;
        }

        public object GetUserID(string userName, bool userIsOnline)
        {
            //connectionString = ConfigurationManager.ConnectionStrings["AccessFileName"].ToString();
            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                      System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand("SELECT UserId " +
                  " FROM aspnet_Membership WHERE Username = ?", conn);

            cmd.Parameters.Add("@Username", OleDbType.VarChar).Value = userName;

            OleDbMembershipUser u = null;
            OleDbDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader();

            }
            catch (OleDbException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(Object, Boolean)");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }
            String x = reader.GetValue(0).ToString();
            return reader.GetValue(0);
        }
        //this function is not used
        //made a custom Getuser that returns OleDbMembershipUser
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand("SELECT UserId, Username, Email, LastLoginDate" +
                  " FROM aspnet_Membership WHERE UserId = ?", conn);

            cmd.Parameters.Add("@UserId", OleDbType.Guid).Value = providerUserKey;

            OleDbMembershipUser u = null;
            OleDbDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);

                    if (userIsOnline)
                    {
                        OleDbCommand updateCmd = new OleDbCommand("UPDATE aspnet_Membership " +
                                  "SET LastLoginDate = ? " +
                                  "WHERE UserId = ?", conn);

                        updateCmd.Parameters.Add("@LastLoginDate", OleDbType.Date).Value = DateTime.Now;
                        updateCmd.Parameters.Add("@UserId", OleDbType.Guid).Value = providerUserKey;

                        updateCmd.ExecuteNonQuery();
                    }
                }

            }
            catch (OleDbException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(Object, Boolean)");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            return u;
        }

        //
        // GetUserFromReader
        //    A helper function that takes the current row from the OleDbDataReader
        // and hydrates a MembershiUser from the values. Called by the 
        // MembershipUser.GetUser implementation.
        //

        private OleDbMembershipUser GetUserFromReader(OleDbDataReader reader)
        {
            object providerUserKey = reader.GetValue(0);
            string username = reader.GetString(1);
            string email = reader.GetString(2);


            DateTime lastLoginDate = new DateTime();
            if (reader.GetValue(3) != DBNull.Value)
                lastLoginDate = reader.GetDateTime(3);
            string firstname = reader.GetString(4);
            string lastname = reader.GetString(5);

            OleDbMembershipUser u = new OleDbMembershipUser(this.Name,
                                                  username,
                                                  providerUserKey,
                                                  email,
                                                  "",
                                                  "",
                                                  true,
                                                  true,
                                                  DateTime.MinValue,
                                                  lastLoginDate,
                                                  DateTime.MinValue,
                                                  DateTime.MinValue,
                                                  DateTime.MinValue,
                                                  firstname,
                                                  lastname);

            return u;
        }

        //not tested
        public override string GetUserNameByEmail(string email)
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand("SELECT Username" +
                  " FROM aspnet_Membership WHERE Email = ?", conn);

            cmd.Parameters.Add("@Email", OleDbType.VarChar, 128).Value = email;
            //cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName;

            string username = "";

            try
            {
                conn.Open();

                username = (string)cmd.ExecuteScalar();
            }
            catch (OleDbException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUserNameByEmail");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            if (username == null)
                username = "";

            return username;
        }


        //I do not use answer parameter
        //not tested
        public override string ResetPassword(string username, string asnwer)
        {

            string newPassword =
              System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            int rowsAffected = 0;
            OleDbDataReader reader = null;
            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");


            OleDbConnection conn = new OleDbConnection(connectionString);
            try
            {
                conn.Open();
                OleDbCommand updateCmd = new OleDbCommand("UPDATE aspnet_Membership " +
                        " SET Password = ?" +
                        " WHERE Username = ?", conn);

                updateCmd.Parameters.Add("@Password", OleDbType.VarChar, 255).Value = EncodePassword(newPassword);
                updateCmd.Parameters.Add("@Username", OleDbType.VarChar, 255).Value = username;
                //updateCmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName;

                rowsAffected = updateCmd.ExecuteNonQuery();
            }
            catch (OleDbException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ResetPassword");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }

            if (rowsAffected > 0)
            {
                return newPassword;
            }
            else
            {
                throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
            }
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        //not tested
        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

           string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                     System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data") + "\\ASPNetDB.mdb ";
           string pwd = "";
           try
           {

               OleDbConnection conn = new OleDbConnection(conString);
               conn.Open();
               OleDbCommand cmd = new OleDbCommand("SELECT Password FROM aspnet_Membership WHERE Username = ?", conn);

               cmd.Parameters.AddWithValue("@Username", username);
               // cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName;

               OleDbDataReader reader = cmd.ExecuteReader();
              


               if (reader != null && reader.HasRows)
               {
                   while (reader.Read())
                   {
                       pwd = Convert.ToString(reader[0]);
                   }

               }
               else
               {
                   return false;
               }

               reader.Close();
           }
           catch (OleDbException ex)
           {
               string msg = "Select Error:";
               msg += ex.Message;
               
           }
           if (EncodePassword(password) == pwd)
           {
               isValid = true;
           }

            return isValid;
        }

        //
        // CheckPassword
        //   Compares password values based on the MembershipPasswordFormat.

        //not tested
        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            
           pass2 = UnEncodePassword(dbpassword);
            

            if (pass1 == pass2)
            {
                return true;
            }

            return false;
        }

        //
        // EncodePassword
        //   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        //
        public string EncodePassword(string password)
        {
            string encodedPassword = password;
            //todo
            encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
            /*
            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    HMACSHA1 hash = new HMACSHA1();
                    hash.Key = HexToByte(machineKey.ValidationKey);
                    encodedPassword =
                      Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            */
            return encodedPassword;
        }


        //
        // UnEncodePassword
        //   Decrypts or leaves the password clear based on the PasswordFormat.
        //

        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;
            password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
            /*
            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            */
            return password;
        }
        //

        // HexToByte
        //   Converts a hexadecimal string to a byte array. Used to convert encryption
        // key values from the configuration.
        //

        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        //
        // WriteToEventLog
        //   A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also 
        // thrown by the caller.
        //

        //not 100% sure how to use or find my logged events
        //used to debug exceptions
        //just put a breakpoint instead
        private void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            string message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e.ToString();
            int i = 0;
            //log.WriteEntry(message);
        }

    }

}