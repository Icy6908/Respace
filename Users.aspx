<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="Respace.Users" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>User Management</h2>
    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" 
        OnRowCommand="gvUsers_RowCommand" CssClass="table table-striped">
        <Columns>
            <asp:BoundField DataField="UserID" HeaderText="ID" />
            <asp:BoundField DataField="FullName" HeaderText="Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:BoundField DataField="UserType" HeaderText="Role (Owner/Renter)" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
        
           <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:Button ID="btnApprove" runat="server" Text="Approve Owner" 
                    CommandName="ApproveUser" CommandArgument='<%# Eval("UserID") %>' 
                    Visible='<%# Eval("UserType").ToString() == "Owner" && Convert.ToBoolean(Eval("IsApproved")) == false %>' 
                    CssClass="btn btn-success btn-sm" />

                <asp:Button ID="btnSuspend" runat="server" Text="Suspend" 
                    CommandName="SuspendUser" CommandArgument='<%# Eval("UserID") %>' 
                    Visible='<%# Eval("Status").ToString() == "Active" %>' 
                    CssClass="btn btn-warning btn-sm" />

                <asp:Button ID="btnActivate" runat="server" Text="Activate" 
                    CommandName="ActivateUser" CommandArgument='<%# Eval("UserID") %>' 
                    Visible='<%# Eval("Status").ToString() == "Suspended" %>' 
                    CssClass="btn btn-primary btn-sm" />

                <asp:Button ID="btnHistory" runat="server" Text="View History" 
                    CommandName="ViewHistory" CommandArgument='<%# Eval("UserID") %>' 
                    CssClass="btn btn-info btn-sm" />

                <asp:Button ID="btnDelete" runat="server" Text="Remove" 
                    CommandName="DeleteUser" CommandArgument='<%# Eval("UserID") %>' 
                    CssClass="btn btn-danger btn-sm" 
                    OnClientClick="return confirm('Are you sure you want to remove this user?');" />


            </ItemTemplate>
        </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
