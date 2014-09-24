<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TestPage.aspx.cs" Inherits="FlashLanguage2.Evaluations.TestPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

     <style>
        h1 {
            text-align: center;
        }

        p {
            text-align: center;
        }

        .button {
            width: 250px;
            height: 40px;
        }

        .table {
            text-align: center;
        }
    </style>

    <h1>Test</h1>
    <div><p>Attempt: <%=attempt%>/<%=totalAttempts%></p></div>

    <!-- align is not used in html5. <div style="text-align:center"> does not work  -->
    <div align="center">
           <table align: "center" cellpadding="20">
        <tr>
            <td  >
                <asp:Button runat="server" OnClick ="Skip" Text="Skip" CssClass="button" /></td>
        </tr>
         </table>
    </div>


    <!-- The picture and english word -->
    <div id="testpage" style="text-align: center">
            <asp:ListView ID="TestImage"
                ItemType="FlashLanguage2.Helpers.Word"
                runat="server"
                SelectMethod="WordSequence">
                <ItemTemplate>          
                             <image src='/Images/<%#: Item.ImagePath %>' width="300" height="200" border="1" />
                        <h1><%#: Item.EnglishWord %></h1>
                </ItemTemplate>
            </asp:ListView>
        </div>

    <!-- The choices -->
    <div id="PractiseAnswers" style="text-align: center">
        <table align="center" style="margin: 0px auto;" cellpadding="5">
        <tr>
            <td >
                <asp:Button  runat="server" OnClick ="AnswerBtn" ID="btn1"   CssClass="button"/>
            <td  >
                <asp:Button  runat="server" OnClick ="AnswerBtn" ID="btn2"    CssClass="button"/>
        </tr>
<tr>
            <td >
                  <asp:Button  runat="server" OnClick ="AnswerBtn" ID="btn3"  CssClass="button"/>
            <td  >
                  <asp:Button  runat="server" OnClick ="AnswerBtn" ID="btn4"   CssClass="button"/>
        </tr>
         </table>

          
        </div>
      <div id="score" class="navbar-collapse collapse"  >
                    <ul class="nav navbar-nav">
                         <li><div><%=strScoreValue%>/<%=totalQuestions%></div></li>

                    </ul>
                </div>

</asp:Content>
