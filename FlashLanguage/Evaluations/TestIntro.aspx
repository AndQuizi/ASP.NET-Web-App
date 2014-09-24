<%@ Page Title="Test Intro" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TestIntro.aspx.cs" Inherits="FlashLanguage2.Evaluations.TestInto" %>
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

        th, td {
            padding: 15px;
            text-align: center;
        }

        table {
            margin-left: auto;
            margin-right: auto;
        }
    </style>


    <h1>Test</h1>
    <p>You can only retake a test so many times. Once you start a test it counts as an attempt.</p>
    <p>&nbsp;</p>
    <p runat="server" id="notification"></p>
    <p>&nbsp;</p>



    <asp:ListView runat="server" ID="ListView1"
        ItemType="FlashLanguage2.Helpers.TestAttempts" SelectMethod="GetTests">
         <EmptyDataTemplate>
                    <table>
                        <tr>
                            <td>There are no current tests.</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
        <LayoutTemplate>
            <table runat="server" id="table1">
                <tr>
                    <th>Test</th>
                    <th>Attempts</th>
                </tr>
                <tr runat="server" id="itemPlaceholder"></tr>
            </table>
        </LayoutTemplate>
        <ItemTemplate>

            <tr runat="server">
                <td>
                    <asp:Button  runat="server"  OnClick="StartTest" Text="<%#:Item.testName%>" CssClass="button"/>
                </td>
                <td>
                    <p><%#:Item.userAttempts%>/<%#:Item.testAttempts%> </p>
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>
