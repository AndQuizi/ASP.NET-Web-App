<%@ Page Title="Extract Marks" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ExtractMarksPage.aspx.cs" Inherits="FlashLanguage2.Admin.ExtractMarksPage" %>
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
    <h3>Extract Marks</h3>

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
                <asp:Button ID="Button1" runat="server" OnClick="updateTests" Text="Update" CssClass="btn btn-default" Height="31px" Width="69px" />
            </div>
        </div>
          <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
            </div>
        </div>
         <div class="form-group">
            <asp:Label ID="Label2" runat="server" AssociatedControlID="SelectTest" CssClass="col-md-2 control-label">Test</asp:Label>
            <div class="col-md-10">
                <select id="SelectTest" runat="server" name="SelectTest">
                </select>
            </div>
        </div>
          <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button ID="Button4" runat="server" OnClick="ExtractMarksBtn" Text="Extract Marks" CssClass="btn btn-default" />
            </div>
        </div>
     </div>
</asp:Content>
