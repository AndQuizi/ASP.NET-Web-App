<%@ Page Title="Practise" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PractisePage.aspx.cs" Inherits="FlashLanguage2.Practise.PractisePage" %>

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
            text-align: center;

        }

        .table {
            text-align: center;
        }
        #btns{
            text-align: center;
        }
    </style>

    <h1><%: Title %></h1>
     
    <!-- align is not used in html5. <div style="text-align:center"> does not work  -->
    <div align="center">
           <table align: "center" cellpadding="20">
        <tr>
            <td >
                <asp:Button runat="server" OnClick ="End" Text="End" CssClass="button" /></td>
            <td  >
                <asp:Button runat="server" OnClick ="Skip" Text="Skip" CssClass="button" /></td>
        </tr>
         </table>
    </div>

    <!-- The picture and english word -->
    <div style="text-align: center">
            <asp:ListView ID="PractiseImage"
                ItemType="FlashLanguage2.Helpers.Word"
                runat="server"
                SelectMethod="WordSequence">
                <ItemTemplate>          
                             <image src='/Images/<%#: Item._imagePath %>' width="300" height="200" border="1" />
                        <h1><%#: Item._englishWord %></h1>
                </ItemTemplate>
            </asp:ListView>
        </div>

    <!-- The choices -->
    <div id="PractiseAnswers" style="text-align: center">
                   <table align="center" style="margin: 0px auto;" cellpadding="5">
        <tr>
            <td >
                <asp:Button  runat="server" OnClick ="AnswerBtn" ID="btn1"  CssClass="button" style="text-align: center;"/>
            <td  >
                <asp:Button  runat="server" OnClick ="AnswerBtn" ID="btn2"  CssClass="button" style="text-align: center;"/>
        </tr>
<tr>
            <td >
                  <asp:Button  runat="server" OnClick ="AnswerBtn" ID="btn3"  CssClass="button" style="text-align: center;"/>
            <td  >
                  <asp:Button  runat="server" OnClick ="AnswerBtn" ID="btn4"  CssClass="button" style="text-align: center;"/>
        </tr>
         </table>
                             <!-- Buttons-->
                    
      
      
               

                
            
        </div>
       <div id="score" class="navbar-collapse collapse"  >
                    <ul class="nav navbar-nav">
                         <li><div>Correct Answers: <%=strScoreValue%></div></li>

                    </ul>
                </div>
</asp:Content>
