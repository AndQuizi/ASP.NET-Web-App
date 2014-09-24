using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace FlashLanguage2.Helpers
{
    /// <summary>
    /// Summary description for FileDownload
    /// </summary>
    public class FileDownload : IHttpHandler
    {
        //not used
        public void ProcessRequest(HttpContext context)
        {
            //Track your id
            string id = context.Request.QueryString["id"];
            //save into the database 
            string fileName = "YOUR-FILE.pdf";
            context.Response.Clear();
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            context.Response.End();
            //download the file
        }
        public void ProcessRequest(HttpContext context, string testName, List<StudentScore> scores, int totalScore)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(testName);
            sb.AppendLine();
            foreach (StudentScore student in scores)
            {
                sb.AppendLine(student.firstName + "         " + student.lastName + "            " + student.score + "/" + totalScore);

            }
            sb.AppendLine();
            sb.AppendLine("=============================================");

            string filename = "TestScores.txt";
            context.Response.Buffer = false;
            context.Response.Clear();
            context.Response.AddHeader("content-disposition", "attachment; filename=" + filename);
            context.Response.ContentType = "text/plain";
            context.Response.Write(sb.ToString());
           
            
            context.Response.WriteFile("~/Helpers/" + filename);
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}