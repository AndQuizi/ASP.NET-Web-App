<%@ Page Title="Manage Tests" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageTests.aspx.cs" Inherits="FlashLanguage2.Admin.ManageTests" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

       <style>
            h1,h3 {
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
    

        <h1>Administration</h1>
        <h3>Manage Tests</h3>
     <p>&nbsp;</p>
     <div class="form-group" style="text-align: center;">
        <asp:Button runat="server" OnClick="CreateTest" Text="Create Test" CssClass="button" />
        <p>&nbsp;</p>
        <asp:Button runat="server" OnClick="DeleteTest" Text="Delete Test" CssClass="button" />
        <p>&nbsp;</p>
         </div>
</asp:Content>
