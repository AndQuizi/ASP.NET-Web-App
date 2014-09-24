using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2.Admin
{
    public partial class AdminPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //todo:
        //add word function
        //create/remove test function
        //extract marks function
        protected void AddWord(object sender, EventArgs e)
        {
            Response.Redirect("WordPage.aspx");
        }
        protected void ManageTest(object sender, EventArgs e)
        {
            Response.Redirect("ManageTests.aspx");
        }
        protected void ExtractMarks(object sender, EventArgs e)
        {
            Response.Redirect("ExtractMarksPage.aspx");
        }

    }
}