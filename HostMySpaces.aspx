<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HostMySpaces.aspx.cs"
    Inherits="Respace.HostMySpaces" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>My Spaces</h2>

    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" />
    <br /><br />

    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" DataKeyNames="SpaceId"
        OnRowCommand="gv_RowCommand">
        <Columns>
            <asp:BoundField DataField="SpaceId" HeaderText="SpaceId" />
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="Location" HeaderText="Location" />
            <asp:BoundField DataField="Type" HeaderText="Type" />
            <asp:BoundField DataField="PricePerHour" HeaderText="Price/Hour" DataFormatString="{0:C}" />
            <asp:BoundField DataField="Capacity" HeaderText="Capacity" />
            <asp:BoundField DataField="Status" HeaderText="Status" />

            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:LinkButton runat="server" Text="Manage Bookings"
                        CommandName="ManageBookings"
                        CommandArgument='<%# Eval("SpaceId") %>' />
                    &nbsp;|&nbsp;
                    <asp:LinkButton runat="server" Text="Remove Listing"
                        CommandName="RemoveListing"
                        CommandArgument='<%# Eval("SpaceId") %>'
                        OnClientClick="return confirm('Remove this listing?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
