<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserHistory.aspx.cs" Inherits="Respace.UserHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Activity History for User ID: <asp:Label ID="lblUserId" runat="server" /></h2>
    <hr />

    <h3>Owned Listings (Spaces)</h3>
    <asp:GridView ID="gvUserListings" runat="server" CssClass="table table-bordered" AutoGenerateColumns="True">
        <EmptyDataTemplate>No listings owned by this user.</EmptyDataTemplate>
    </asp:GridView>

    <br />

    <h3>Booking History (As Renter)</h3>
    <asp:GridView ID="gvUserBookings" runat="server" CssClass="table table-bordered" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="BookingID" HeaderText="Ref Number" />
            <asp:BoundField DataField="ListingID" HeaderText="Space ID" />
            <asp:BoundField DataField="BookingDate" HeaderText="Date Reserved" DataFormatString="{0:d}" />
            <asp:BoundField DataField="TotalPrice" HeaderText="Amount Paid" DataFormatString="{0:C}" />
            <asp:BoundField DataField="Status" HeaderText="Current Status" />
        </Columns>
        <EmptyDataTemplate>No bookings found for this user.</EmptyDataTemplate>
    </asp:GridView>
    
    <br />
    <asp:Button ID="btnBack" runat="server" Text="Back to Users" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
</asp:Content>
