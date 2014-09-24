using FlashLanguage2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlashLanguage2.Practise
{
    public partial class PractiseResults : System.Web.UI.Page
    {
        public String strFinalScore;
        public String totalQuestions;

        public List<Word> questionQueue;
        public List<UserResults> userResults;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["currLanguage"] == null || Session["difficulty"] == null)
            {
                Response.Redirect("../Default.aspx");
            }

            strFinalScore = (String)Session["finalScore"];
            totalQuestions = Convert.ToString((int)Session["currQuestion"]);

            questionQueue = Session["ogQueue"] as List<Word>;
            userResults = Session["userResults"] as List<UserResults>;

     
        }

        //called when finish button is clicked
        //redirects to users language page
        protected void Finish(object sender, EventArgs e)
        {
            
            int id = (int)Session["currLanguageID"];
            Session.Remove("difficulty");
            Session.Remove("finalScore");
            Session.Remove("wordQueue");
            Session.Remove("ogQueue");
            Session.Remove("userResults");
            Session.Remove("currQuestion");
            if (id == 1)
            {
                Response.Redirect("../Italian.aspx");
            }
            else
            {
                Response.Redirect("../French.aspx");
            }
             
        }

        //called when try again button is clicked
        //returns to language practise page
        protected void TryAgain(object sender, EventArgs e)
        {
            Session.Remove("difficulty");
            Session.Remove("finalScore");
            Session.Remove("wordQueue");
            Session.Remove("ogQueue");
            Session.Remove("userResults");
            Session.Remove("currQuestion");
            Response.Redirect("PractiseIntro.aspx");
        }
    
        public IQueryable<UserResults> GetResults()
        {
            //need list empty check
            IQueryable<UserResults> query = Queryable.AsQueryable(userResults);
            return query;
        } 
    }
}