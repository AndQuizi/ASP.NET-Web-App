using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2.Admin
{
    public partial class WordPage : System.Web.UI.Page
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
        protected void AddWordBtn(object sender, EventArgs e)
        {

            int langID = Convert.ToInt32(SelectLanguage.Value);
            string englishWord = EnglishWord.Text;
            string translation = Translation.Text;
            int diff = Convert.ToInt32(SelectDiff.Value);

            //make sure image is valid before adding DB data
            if (CheckImage())
            {
                int wordID = GetWord(englishWord);
                //english word does not already exist in Word table
                if (wordID == -1)
                {
                    //add new word to Word table
                    if (AddWord(englishWord, ImageUpload.FileName.ToLower()))
                    {
                        if (AddImage())
                        {
                            //add to word WordLanguage table
                            if (AddWordLanguage(GetWord(englishWord), langID, translation, diff))
                            {

                                notification.InnerText = "Word Successfully Added!";

                            }
                            else
                            {
                                notification.InnerText = "Could not add word to WordLanguage table.";
                            }
                        }
                    }
                    else
                    {
                        notification.InnerText = "Could not add word to Word table.";
                    }
                }
                //english word already exists in Word table
                else
                {
                    //add to word WordLanguage table
                    if (AddWordLanguage(wordID, langID, translation, diff))
                    {

                        notification.InnerText = "Word Successfully Added!";

                    }
                    else
                    {
                        notification.InnerText = "Could not add word to WordLanguage table.";
                    }
                }


            }
        }

        //gets word from db
        public int GetWord(string word)
        {
            int id = -1;
            try
            {
                string insertSQL = "SELECT WordID FROM Words WHERE EnglishWord = ?";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@EnglishWord", word);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();


                        id = Convert.ToInt32(reader[0]);

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

        public bool AddWord(string englishWord, string image)
        {
            if (image == "")
            {
                image = "default.jpg";
            }
            try
            {
                string insertSQL = "INSERT INTO Words (EnglishWord, ImagePath) values (?,?)";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@EnglishWord", englishWord);
                    cmd.Parameters.AddWithValue("@ImagePath", image);

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

        public bool AddWordLanguage(int wordID, int langID, string word, int diff)
        {

            try
            {
                string insertSQL = "INSERT INTO WordLanguages (WLWordID, WLLanguageID, WLWord, Difficulty) values (?,?,?,?)";

                using (OleDbConnection conn = new OleDbConnection(conString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(insertSQL, conn);

                    cmd.Parameters.AddWithValue("@WLWordID", wordID);
                    cmd.Parameters.AddWithValue("@WLLanguageID", langID);
                    cmd.Parameters.AddWithValue("@WLWord", word);
                    cmd.Parameters.AddWithValue("@Difficulty", diff);
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

        protected bool AddImage()
        {
            int fileSize = 5096000; string fileExt = ""; string pathAndFile = "";
            if (ImageUpload.FileName.ToLower() != "")
            {
                if (ImageUpload.PostedFile.ContentLength < fileSize)
                {
                    int length = ImageUpload.PostedFile.ContentLength;
                    byte[] pic = new byte[length];

                    fileExt = System.IO.Path.GetExtension(ImageUpload.FileName).ToLower();

                    pathAndFile = "/Images/";
                    pathAndFile += ImageUpload.FileName.ToLower();


                    if (fileExt == ".jpg" || fileExt == ".png")
                    {

                        if (File.Exists(Server.MapPath(pathAndFile)))
                        {
                            notification.InnerText = "An image with the same name already exists.";
                            return false;
                        }
                        else
                        {
                            ImageUpload.SaveAs(Server.MapPath(pathAndFile));

                        }

                    }
                    else
                    {
                        notification.InnerText = "Error Only .jpg and .png are allowed";
                        return false;
                    }


                }
                else
                {
                    notification.InnerText = "ERROR: The file is needs to be less than 4MB (4096 KB).";
                    return false;
                }
                return true;
            }
            else return true;
        }
        protected bool CheckImage()
        {
            int fileSize = 5096000; string fileExt = ""; string pathAndFile = "";

            if (ImageUpload.FileName.ToLower() != "")
            {
                if (ImageUpload.PostedFile.ContentLength < fileSize)
                {
                    int length = ImageUpload.PostedFile.ContentLength;
                    byte[] pic = new byte[length];

                    fileExt = System.IO.Path.GetExtension(ImageUpload.FileName).ToLower();

                    pathAndFile = "/Images/";
                    pathAndFile += ImageUpload.FileName.ToLower();


                    if (fileExt == ".jpg" || fileExt == ".png")
                    {
                        return true;



                    }
                    else
                    {
                        notification.InnerText = "Error Only .jpg and .png are allowed";
                        return false;
                    }


                }
                else
                {
                    notification.InnerText = "ERROR: The file is needs to be less than 4MB (4096 KB).";
                    return false;
                }
            }
            else return true;
        }
    }
}