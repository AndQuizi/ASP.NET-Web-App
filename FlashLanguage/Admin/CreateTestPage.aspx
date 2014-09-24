<%@ Page Title="Create Test" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateTestPage.aspx.cs" Inherits="FlashLanguage2.Admin.CreateTestPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        h1, h3 {
            text-align: center;
        }

        p {
            text-align: center;
        }
         .firstLabel  {
            margin-right: 50px;
        }
        .button {
            width: 150px;
            height: 50px;
        }
    </style>


    <h1>Administration</h1>
    <h3>Create Test</h3>

    <p>&nbsp;</p>
    <p runat="server" id="notification"></p>
    <div class="form-horizontal">
        <hr />
        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
            <p class="text-danger">
                <asp:Literal runat="server" ID="FailureText" />
            </p>
        </asp:PlaceHolder>
        <div class="form-group">
            <asp:Label ID="Label1" runat="server" AssociatedControlID="SelectLanguage" CssClass="col-md-2 control-label">Language</asp:Label>
            <div class="col-md-10">
                <select id="SelectLanguage" runat="server" name="D1">
                </select>
            </div>
        </div>
        <div class="form-group">
            <asp:Label ID="Label2" runat="server" AssociatedControlID="TestName" CssClass="col-md-2 control-label">Test Name</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="TestName" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ControlToValidate="TestName" CssClass="text-danger" ErrorMessage="The Test Name field is required." />
            </div>
        </div>
        <div class="form-group">
            <asp:Label ID="Label3" runat="server" AssociatedControlID="EnglishWord" CssClass="col-md-2 control-label">English Word</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="EnglishWord" CssClass="form-control" />

            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button ID="Button1" runat="server" OnClick="AddWordBtn" Text="Add Word" CssClass="btn btn-default" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label ID="Label4" runat="server" AssociatedControlID="StartDate" CssClass="col-md-2 control-label">Start/End Date</asp:Label>
            <div class="col-md-10">
                <asp:Button ID="Button2" runat="server" OnClick="Button1_Click" Text="Start Date" />
                <asp:Calendar ID="StartDate" runat="server"></asp:Calendar>
                <asp:Button ID="Button3" runat="server" OnClick="Button2_Click" Text="End Date" />
                <asp:Calendar ID="EndDate" runat="server"></asp:Calendar>
            </div>
        </div>

      
  <div class="form-group">
            <asp:Label ID="Label5" runat="server" AssociatedControlID="Attempts" CssClass="col-md-2 control-label">Attempts</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" type="number" ID="Attempts" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Attempts" CssClass="text-danger" ErrorMessage="The Attempts field is required." />
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button ID="Button4" runat="server" OnClick="CreateTestBtn" Text="Finish" CssClass="btn btn-default" />
            </div>
        </div>
    </div>

    <p>&nbsp;</p>
    <p>Current Test Questions</p>
    <p>&nbsp;</p>
    <div style="text-align:center">
  <asp:ListView ID="ListView1" runat="server">
        <ItemTemplate>
            <div>
                <span class="firstLabel"><asp:Label ID="EnglishWordLabel" runat="server" Text='<%# Eval("EnglishWord") %>'></asp:Label></span>
                <span class="secondLabel"><asp:Label ID="TrnaslationLabel" runat="server"  Text='<%# Eval("Translation") %>'></asp:Label></span>
            </div>
        </ItemTemplate>
    </asp:ListView>
   </div>

</asp:Content>
