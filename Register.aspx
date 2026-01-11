<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Respace.Register" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Create Account</h2>

    <p>Full Name</p>
    <asp:TextBox ID="txtName" runat="server" />

    <p>Email</p>
    <asp:TextBox ID="txtEmail" runat="server" />

    <p>Password</p>
    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />

    <p>Register As</p>
    <asp:DropDownList ID="ddlRole" runat="server">
        <asp:ListItem Text="Guest" Value="Guest" />
        <asp:ListItem Text="Host" Value="Host" />
    </asp:DropDownList>

    <br /><br />
    <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" />
    <br /><br />
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" />
</asp:Content>
