<%@ Page Title="Flash Language Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FlashLanguage2._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

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
    <h1>Welcome to Flash Language!</h1>
    <p>Choose a language and lets get started on your vocabulary!</p>
    <p>&nbsp;</p>
    <div class="nav3" style="height: 300px; text-align: center">
        <a runat="server" href="~/Italian" title="Italian">
            <img src="/Images/italian_symbol.jpg"></a>
        <a runat="server" href="~/French" title="French">
            <img src="/Images/french_symbol2.jpg"></a>
    </div>

     <h2 class="text-center">A convenient and simple language learning tool</h2>

</asp:Content>
