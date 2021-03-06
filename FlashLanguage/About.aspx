﻿<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="FlashLanguage2.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        h2 {
            text-align: center;
        }

        h3 {
            text-align: center;
        }

        

        .button {
            width: 150px;
            height: 50px;
        }
      
    </style>

    <h2><%: Title %> Flash Language</h2>
    <h3 text-align: center>A convenient and simple language learning tool</h3>

    <p>&nbsp;</p>

    <h4>What is it?</h4>
    
    <div>
   <p>Flash language was developed as a fourth year academic project by programmer Andrew Quizi. </p>
        <p>Designed as a language learning tool for both recreational and academic purposes, Flash Language is used practise and evaluate individual language skills.</p>
    </div>
    <br>
     <h4>How does it work?</h4>
    <div>
        <p>One can practise their skills at a particular language by entering "Practise" mode for that language. You do not need an account to do this.</p>

        <p>There is also a "Test" mode that allows users to take tests for some specific language and have their marks recorded. This feature is used by teachers to assign homework to their students via Language Leaner, then be able to extract and evaluate their marks. To be able to enter Test mode for some language, the user/student must first enter a password set by the course instructor. If the user entered password is correct then that user can now take tests for that language.
        </p>
    </div>
    <br>
    <h4>What is the purpose?</h4>
    
    <div>
        <p>Flash Language was created as a tool that allows linguistic teachers to assign homework to their students and able to their extract marks to be used for their final grade. </p>
    <p>Using Flash Language these teachers can create/delete tests, add new words to the database (to be used by both practise and test mode), set course passwords, and extract student test marks.</p>
        <p>In the end Flash Language provides an easy to use language learning tool to be used both recreationally and academically</p>
</div>
    <br>
     <h4>Documentaion:</h4>
    <p>http://www.mediafire.com/view/9n3bi64170vd498/Quizi_-_Flash_Language_-_Final_Report.docx</p>
</asp:Content>
