<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Respace.Login" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Login</h2>

    <p>Email</p>
    <asp:TextBox ID="txtEmail" runat="server" />

    <p>Password</p>
    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />

    <br /><br />
    <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
    <br /><br />
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" />
</asp:Content>
