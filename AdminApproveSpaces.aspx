<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminApproveSpaces.aspx.cs"
    Inherits="Respace.AdminApproveSpaces" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Admin - Approve Spaces</h2>

    <asp:Label ID="lblMsg" runat="server" ForeColor="Green" />
    <br /><br />

    <asp:GridView ID="gvPending" runat="server" AutoGenerateColumns="False" DataKeyNames="SpaceId"
        OnRowCommand="gvPending_RowCommand">
        <Columns>
            <asp:BoundField DataField="SpaceId" HeaderText="ID" />
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="Location" HeaderText="Location" />
            <asp:BoundField DataField="Type" HeaderText="Type" />
            <asp:BoundField DataField="HostName" HeaderText="Host" />
            <asp:BoundField DataField="PricePerHour" HeaderText="Price/Hour" DataFormatString="{0:0.00}" />
            <asp:BoundField DataField="Capacity" HeaderText="Cap" />
            <asp:BoundField DataField="CreatedAt" HeaderText="Created" DataFormatString="{0:yyyy-MM-dd HH:mm}" />

            <asp:TemplateField HeaderText="Remarks (optional)">
                <ItemTemplate>
                    <asp:TextBox ID="txtRemarks" runat="server" Width="180" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button ID="btnApprove" runat="server" Text="Approve"
                        CommandName="ApproveSpace" CommandArgument='<%# Eval("SpaceId") %>' />
                    <asp:Button ID="btnReject" runat="server" Text="Reject"
                        CommandName="RejectSpace" CommandArgument='<%# Eval("SpaceId") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <hr />

    <h3>Recently Approved/Rejected (latest 20)</h3>
    <asp:GridView ID="gvRecent" runat="server" AutoGenerateColumns="true" />
</asp:Content>
