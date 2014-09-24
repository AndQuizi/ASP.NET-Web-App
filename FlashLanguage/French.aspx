<%@ Page Title="French" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="French.aspx.cs" Inherits="FlashLanguage2.French" %>
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
                height: 50px;
            }
          

        </style>
    

        <h1> <%: Title %></h1>
        <p>Bienvenue à la langue française!</p>
         
        <p>&nbsp;</p>
                            
        <div class="form-group" style="text-align:center;">
         <asp:Button  runat="server" OnClick="Practise" Text="Practise" CssClass="button"/>
            <p>&nbsp;</p>
         <asp:Button runat="server" OnClick="Test" Text="Test" CssClass="button"/>
       </div>

</asp:Content>
