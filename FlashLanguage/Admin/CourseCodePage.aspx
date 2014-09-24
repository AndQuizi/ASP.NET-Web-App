<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CourseCodePage.aspx.cs" Inherits="FlashLanguage2.Admin.CourseCodePage" %>
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
    <h3>Change Course Codes</h3>

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
            <asp:Label ID="Label2" runat="server" AssociatedControlID="CourseCode" CssClass="col-md-2 control-label">New Course Code</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="CourseCode" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ControlToValidate="CourseCode" CssClass="text-danger" ErrorMessage="The Course Code field is required." />
            </div>
        </div>
         <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button ID="Button4" runat="server" OnClick="UpdateCourseCode" OnClientClick="if(!confirm('Are you sure? Currently registered students will have to re-regsiter for the course.')) return false;" Text="Update" CssClass="btn btn-default" />
            </div>
        </div>
        </div>


</asp:Content>
