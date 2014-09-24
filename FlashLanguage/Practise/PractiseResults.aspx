<%@ Page Title="Practise Results" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PractiseResults.aspx.cs" Inherits="FlashLanguage2.Practise.PractiseResults" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        h1 {
            text-align: center;
        }

        p {
            text-align: center;
        }

        .button {
            width: 150px;
            height: 40px;
        }

        th, td {
            padding: 15px;
            text-align: center;
        }
        table {       
             margin-left: auto;
            margin-right: auto;
        }
        </style>
    

        <h1> <%: Title %></h1>

        <!-- Score -->
      <div><p><%=strFinalScore%>/<%=totalQuestions%></p></div>

        <!-- align is not used in html5. <div style="text-align:center"> does not work  -->
    <div align="center">
           <table align: "center" cellpadding="20">
        <tr>
            <td >
                <asp:Button runat="server"  OnClick ="Finish" Text="Finish" CssClass="button" /></td>
            <td  >
                <asp:Button runat="server" OnClick ="TryAgain" Text="Try Again" CssClass="button" /></td>
        </tr>
         </table>
    </div>

    <asp:ListView runat="server" ID="ListView1" 
        ItemType="FlashLanguage2.Helpers.UserResults" SelectMethod="GetResults" >
        <LayoutTemplate>
            <table runat="server" id="table1">
                <tr>
                <th>Question #</th>
                <th>Word</th>
                <th>Your Answer</th>
                <th>Correct Answer</th>
            </tr>
                <tr runat="server" id="itemPlaceholder"></tr>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            
              <tr runat="server">
                <td>
                    <p><%#:Item.qNumber%> </p>
                </td>
                <td>
                    <p><%#:Item.word.EnglishWord%> </p>
                </td>
                <td>
                    <p><%#:Item.userAnswer%> </p>
                </td>
                <td>
                    <p><%#:Item.correctAnswer%> </p>
                </td>


            </tr>
        </ItemTemplate>
    </asp:ListView>

</asp:Content>
