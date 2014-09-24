<%@ Page Title="Add Word" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WordPage.aspx.cs" Inherits="FlashLanguage2.Admin.WordPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        h1, h3 {
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
    <h3>Add Word</h3>

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
            <asp:Label ID="Label2" runat="server" AssociatedControlID="EnglishWord" CssClass="col-md-2 control-label">English Word (case sensitive)</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="EnglishWord" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ControlToValidate="EnglishWord" CssClass="text-danger" ErrorMessage="The English Word field is required." />
            </div>
        </div>
        <div class="form-group">
            <asp:Label ID="Label3" runat="server" AssociatedControlID="Translation" CssClass="col-md-2 control-label">Translation</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="Translation" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Translation" CssClass="text-danger" ErrorMessage="The Translation field is required." />
            </div>
        </div>
        <div class="form-group">
            <asp:Label ID="Label4" runat="server" AssociatedControlID="SelectDiff" CssClass="col-md-2 control-label">Difficulty</asp:Label>
            <div class="col-md-10">
                <select id="SelectDiff" runat="server" name="D1">
                    <option value="1">Beginner</option>
                    <option value="2">Intermediate</option>
                    <option value="3">Advanced</option>
                </select>
            </div>
        </div>
        <div class="form-group">
            <asp:Label ID="Label5" runat="server" AssociatedControlID="ImageUpload" CssClass="col-md-2 control-label">Image (optional)</asp:Label>
            <div class="col-md-10">
                <asp:FileUpload runat="server" ID="ImageUpload" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button ID="Button1" runat="server" OnClick="AddWordBtn" Text="Add Word" CssClass="btn btn-default" />
            </div>
        </div>
    </div>
</asp:Content>
