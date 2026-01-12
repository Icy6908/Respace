<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HostBookings.aspx.cs"
    Inherits="Respace.HostBookings" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Bookings for this space</h2>
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" /><br /><br />

    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" DataKeyNames="BookingId"
        OnRowCommand="gv_RowCommand">
        <Columns>
            <asp:BoundField DataField="GuestName" HeaderText="Guest" />
            <asp:BoundField DataField="StartDateTime" HeaderText="Start" DataFormatString="{0:dd MMM yyyy HH:mm}" />
            <asp:BoundField DataField="EndDateTime" HeaderText="End" DataFormatString="{0:dd MMM yyyy HH:mm}" />
            <asp:BoundField DataField="TotalPrice" HeaderText="Total" DataFormatString="{0:C}" />
            <asp:BoundField DataField="Status" HeaderText="Status" />

            <asp:TemplateField HeaderText="Cancel Booking">
                <ItemTemplate>
                    <asp:Panel runat="server" Visible='<%# Eval("Status").ToString() == "Confirmed" %>'>
                        <asp:TextBox ID="txtReason" runat="server" Width="220" Placeholder="Reason (required)" />
                        <asp:LinkButton runat="server" Text="Cancel"
                            CommandName="CancelBooking"
                            CommandArgument='<%# Eval("BookingId") %>'
                            OnClientClick="return confirm('Cancel this booking?');" />
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
