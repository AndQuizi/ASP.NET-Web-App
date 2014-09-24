<%@ Page Title="Course Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CourseRegister.aspx.cs" Inherits="FlashLanguage2.Evaluations.CourseRegister" %>
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
    

        <h1>Course Register</h1>
        <p >Enter the course code below.</p>
         
        <p>&nbsp;</p>
        
        <asp:TextBox runat="server" ID="CourseCode" CssClass="form-control" />
        <asp:Button  runat="server"  OnClick="SubmitCode" Text="Submit" CssClass="button"/>
        <p runat="server" id ="notification"></p>


</asp:Content>
