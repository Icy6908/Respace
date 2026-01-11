<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="SpaceDetails.aspx.cs"
    Inherits="Respace.SpaceDetails"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Space Details</h2>

    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" />
    <br /><br />

    <asp:Panel ID="pnlDetails" runat="server" Visible="false">

        <h3><asp:Label ID="lblName" runat="server" /></h3>

        <p><strong>Location:</strong> <asp:Label ID="lblLocation" runat="server" /></p>
        <p><strong>Type:</strong> <asp:Label ID="lblType" runat="server" /></p>
        <p><strong>Capacity:</strong> <asp:Label ID="lblCap" runat="server" /></p>
        <p><strong>Price / Hour:</strong> $<asp:Label ID="lblPrice" runat="server" /></p>

        <p>
            <strong>Description:</strong><br />
            <asp:Label ID="lblDesc" runat="server" />
        </p>

        <hr />

        <h3>Book this space</h3>

        <table>
            <tr>
                <td>Start Date</td>
                <td>
                    <asp:TextBox ID="txtStartDate" runat="server" />
                </td>
            </tr>
            <tr>
                <td>Start Time</td>
                <td>
                    <asp:TextBox ID="txtStartTime" runat="server" />
                </td>
            </tr>
            <tr>
                <td>End Date</td>
                <td>
                    <asp:TextBox ID="txtEndDate" runat="server" />
                </td>
            </tr>
            <tr>
                <td>End Time</td>
                <td>
                    <asp:TextBox ID="txtEndTime" runat="server" />
                </td>
            </tr>
        </table>

        <br />

        <asp:Button ID="btnBook"
                    runat="server"
                    Text="Confirm Booking"
                    OnClick="btnBook_Click" />

    </asp:Panel>

</asp:Content>
