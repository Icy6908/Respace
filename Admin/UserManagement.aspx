<%@ Page Title="User Management" Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="Respace.Admin.UserManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="h3 mb-0 text-gray-800">User Management</h2>
            <div class="w-25">
                <asp:TextBox ID="txtSearchUsers" runat="server" CssClass="form-control" 
                    Placeholder="Search users..." AutoPostBack="true" 
                    OnTextChanged="txtSearchUsers_TextChanged"></asp:TextBox>
            </div>
        </div>

        <div class="card admin-card p-0 shadow-sm border-0">
            <div class="table-responsive">
                <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover align-middle mb-0" GridLines="None" 
                    OnRowCommand="gvUsers_RowCommand">
                    <HeaderStyle CssClass="bg-light text-muted small fw-bold text-uppercase" />
                    <Columns>
                        <asp:BoundField DataField="UserId" HeaderText="ID" ItemStyle-CssClass="ps-4" />
                        <asp:TemplateField HeaderText="User Info">
                            <ItemTemplate>
                                <div class="fw-bold text-dark"><%# Eval("FullName") %></div>
                                <div class="small text-muted"><%# Eval("Email") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Role" HeaderText="Role" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='badge <%# GetStatusClass(Eval("Status").ToString()) %>'>
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="text-end pe-4">
                            <ItemTemplate>
                                <%-- UPDATED: Redirects to UserView.aspx with the ID --%>
                                <asp:HyperLink ID="lnkView" runat="server" 
                                    NavigateUrl='<%# "UserView.aspx?id=" + Eval("UserId") %>' 
                                    CssClass="btn btn-sm btn-outline-primary me-1">
                                    <i class="fas fa-eye"></i>
                                </asp:HyperLink>
                                <asp:LinkButton ID="btnSuspend" runat="server" CommandName="SuspendUser" 
                                    CommandArgument='<%# Eval("UserId") %>' CssClass="btn btn-sm btn-outline-warning me-1">
                                    <i class="fas fa-user-slash"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="RemoveUser" 
                                    CommandArgument='<%# Eval("UserId") %>' CssClass="btn btn-sm btn-outline-danger"
                                    OnClientClick="return confirm('Are you sure?');">
                                    <i class="fas fa-trash"></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>