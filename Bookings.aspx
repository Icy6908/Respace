<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Bookings.aspx.cs" Inherits="Respace.Bookings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2>Booking Management</h2>
        <p>Monitor and manage all space reservations across the platform.</p>

        <asp:GridView ID="gvBookings" runat="server" CssClass="table table-striped table-bordered" 
            AutoGenerateColumns="False" DataKeyNames="BookingID" OnRowDeleting="gvBookings_RowDeleting">
            <Columns>
                <asp:BoundField DataField="BookingID" HeaderText="ID" />
                <asp:BoundField DataField="ListingID" HeaderText="Space ID" />
                <asp:BoundField DataField="RenterID" HeaderText="User ID" />
                <asp:BoundField DataField="BookingDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Status" HeaderText="Status" />
                
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" 
                            OnClientClick="return confirm('Are you sure you want to cancel/remove this booking?');" 
                            CssClass="btn btn-outline-danger btn-sm">Remove</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
