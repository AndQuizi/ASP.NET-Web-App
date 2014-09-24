<%@ Page Title="Practise" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PractiseIntro.aspx.cs" Inherits="FlashLanguage2.Practise.PractiseIntro" %>

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

    <h1>
        <%=currLanguage%>
    </h1>

    <p><%: Title %></p>

    <p>&nbsp;</p>

    <div class="form-group" style="text-align: center;">
        <asp:Button runat="server" OnClick="PractiseBtn" Text="Beginner" CssClass="button" />
        <p>&nbsp;</p>
        <asp:Button runat="server"  OnClick="PractiseBtn" Text="Intermediate" CssClass="button" />
        <p>&nbsp;</p>
        <asp:Button runat="server"  OnClick="PractiseBtn" Text="Advanced" CssClass="button" />
        <!-- Future Feature
        <p>&nbsp;</p>
        <asp:Button runat="server" OnClick="PractiseBtn" Text="Readings" CssClass="button" />
        -->
    </div>

</asp:Content>
