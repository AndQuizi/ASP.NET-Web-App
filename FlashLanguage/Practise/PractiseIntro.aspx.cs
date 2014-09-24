using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2.Practise
{
    public partial class PractiseIntro : System.Web.UI.Page
    {
        public static string currLanguage;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["currLanguage"] == null)
            {
                Response.Redirect("../Default.aspx");
            }
            else
            {
                currLanguage = (String)Session["currLanguage"];
                if (currLanguage == "Italian")
                {
                    Session["currLanguageID"] = 1;
                }
                else
                {
                    Session["currLanguageID"] = 2;
                }
            }
        }

        protected void PractiseBtn(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            String choice = clickedButton.Text;
            if (choice == "Beginner")
            {
                Session["difficulty"] = 1;
            }
            else if (choice == "Intermediate")
            {
                Session["difficulty"] = 2;
            }
            else{
                Session["difficulty"] = 3;
            }

            Session["currQuestion"] = 1;
            Response.Redirect("PractisePage.aspx");

        }

    }
}