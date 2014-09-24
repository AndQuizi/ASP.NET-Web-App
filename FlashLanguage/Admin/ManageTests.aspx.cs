using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2.Admin
{
    public partial class ManageTests : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CreateTest(object sender, EventArgs e)
        {
            Response.Redirect("CreateTestPage.aspx");
        }
        protected void DeleteTest(object sender, EventArgs e)
        {
            Response.Redirect("DeleteTestPage.aspx");
        }

    }
}