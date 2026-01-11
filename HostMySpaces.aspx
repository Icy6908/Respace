<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HostMySpaces.aspx.cs"
    Inherits="Respace.HostMySpaces" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>My Spaces</h2>
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" />
    <br /><br />

    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="true" />
</asp:Content>
