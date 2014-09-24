<%@ Page Title="Administration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="FlashLanguage2.Admin.AdminPage" %>
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
         
        <p>&nbsp;</p>
      <div class="form-group" style="text-align: center;">
        <asp:Button runat="server" OnClick="AddWord" Text="Add Word" CssClass="button" />
        <p>&nbsp;</p>
        <asp:Button runat="server" OnClick="ManageTest" Text="Manage Test" CssClass="button" />
        <p>&nbsp;</p>
        <asp:Button runat="server" OnClick="ExtractMarks" Text="Extract Marks" CssClass="button" />
    </div>



</asp:Content>
