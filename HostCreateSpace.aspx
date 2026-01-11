<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HostCreateSpace.aspx.cs"
    Inherits="Respace.HostCreateSpace" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Create Space (Host)</h2>

    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" />
    <br />

    <p>Name</p>
    <asp:TextBox ID="txtName" runat="server" Width="400" />

    <p>Location</p>
    <asp:DropDownList ID="ddlLocation" runat="server">
        <asp:ListItem Text="Central" Value="Central" />
        <asp:ListItem Text="North" Value="North" />
        <asp:ListItem Text="South" Value="South" />
        <asp:ListItem Text="East" Value="East" />
        <asp:ListItem Text="West" Value="West" />
    </asp:DropDownList>

    <p>Type</p>
    <asp:DropDownList ID="ddlType" runat="server">
        <asp:ListItem Text="Meeting Room" Value="Meeting Room" />
        <asp:ListItem Text="Event Hall" Value="Event Hall" />
        <asp:ListItem Text="Training Room" Value="Training Room" />
        <asp:ListItem Text="Conference Room" Value="Conference Room" />
        <asp:ListItem Text="Studio" Value="Studio" />
    </asp:DropDownList>

    <p>Description</p>
    <asp:TextBox ID="txtDesc" runat="server" Width="600" TextMode="MultiLine" Rows="5" />

    <p>Price Per Hour</p>
    <asp:TextBox ID="txtPrice" runat="server" Width="120" />

    <p>Capacity</p>
    <asp:TextBox ID="txtCap" runat="server" Width="120" />

    <br /><br />
    <asp:Button ID="btnSubmit" runat="server" Text="Submit for Approval" OnClick="btnSubmit_Click" />
</asp:Content>
