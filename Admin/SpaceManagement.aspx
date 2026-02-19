<%@ Page Title="Space Management" Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" CodeBehind="SpaceManagement.aspx.cs" Inherits="Respace.Admin.SpaceManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="h3 mb-0 text-gray-800">Space Management</h2>
            <div class="d-flex gap-2">
                <asp:DropDownList ID="ddlTypeFilter" runat="server" AutoPostBack="true" 
                    OnSelectedIndexChanged="Filter_Changed" CssClass="form-select form-select-sm" style="width:160px;">
                    <asp:ListItem Text="All Types" Value="All"></asp:ListItem>
                    <asp:ListItem Text="Meeting Room" Value="Meeting Room"></asp:ListItem>
                    <asp:ListItem Text="Conference Room" Value="Conference Room"></asp:ListItem>
                    <asp:ListItem Text="Training Room" Value="Training Room"></asp:ListItem>
                    <asp:ListItem Text="Event Hall" Value="Event Hall"></asp:ListItem>
                    <asp:ListItem Text="Studio" Value="Studio"></asp:ListItem>
                </asp:DropDownList>

                <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" 
                    OnSelectedIndexChanged="Filter_Changed" CssClass="form-select form-select-sm" style="width:130px;">
                    <asp:ListItem Text="All Status" Value="All"></asp:ListItem>
                    <asp:ListItem Text="Approved" Value="Approved"></asp:ListItem>
                    <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                    <asp:ListItem Text="Rejected" Value="Rejected"></asp:ListItem>
                </asp:DropDownList>

                <asp:TextBox ID="txtSearchSpace" runat="server" CssClass="form-control form-control-sm me-2" 
                    Placeholder="Search by Space or Host..." AutoPostBack="true" OnTextChanged="txtSearchSpace_TextChanged" style="width:200px;"></asp:TextBox>
            </div>
        </div>

        <asp:UpdatePanel ID="upSpaceGrid" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="card admin-card p-0 shadow-sm border-0">
                    <div class="table-responsive">
                        <asp:GridView ID="gvSpaces" runat="server" AutoGenerateColumns="False" 
                            CssClass="table table-hover align-middle mb-0" GridLines="None" OnRowCommand="gvSpaces_RowCommand">
                            <HeaderStyle CssClass="bg-light text-muted small fw-bold text-uppercase" />
                            <Columns>
                                <asp:BoundField DataField="SpaceId" HeaderText="ID" ItemStyle-CssClass="ps-4 fw-bold" HeaderStyle-CssClass="ps-4" />
                                
                                <asp:TemplateField HeaderText="Space & Host Details">
                                    <ItemTemplate>
                                        <div class="fw-bold text-dark"><%# Eval("Name") %></div>
                                        <div class="small text-primary">Host: <%# Eval("HostName") %> (ID: <%# Eval("HostUserId") %>)</div>
                                        <div class="small text-muted"><%# Eval("Type") %> • <%# Eval("PricePerHour", "{0:C}") %>/hr</div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <span class='badge <%# GetStatusClass(Eval("Status").ToString()) %>'>
                                            <%# Eval("Status") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="text-end pe-4" HeaderStyle-CssClass="text-end pe-4">
                                    <ItemTemplate>
                                        <div class="btn-group">
                                            <asp:LinkButton runat="server" 
                                                PostBackUrl='<%# "AdminViewSpace.aspx?id=" + Eval("SpaceId") %>' 
                                                CssClass="btn btn-outline-info btn-sm me-1">
                                                <i class="fas fa-eye"></i> View
                                            </asp:LinkButton>

                                            <asp:PlaceHolder runat="server" Visible='<%# Eval("Status").ToString() == "Pending" %>'>
                                                <asp:LinkButton runat="server" CommandName="Approve" CommandArgument='<%# Eval("SpaceId") %>' 
                                                    CssClass="btn btn-outline-success btn-sm me-1">
                                                    <i class="fas fa-check"></i> Approve
                                                </asp:LinkButton>
                                                <asp:LinkButton runat="server" CommandName="Reject" CommandArgument='<%# Eval("SpaceId") %>' 
                                                    CssClass="btn btn-outline-warning btn-sm me-1">
                                                    <i class="fas fa-times"></i> Reject
                                                </asp:LinkButton>
                                            </asp:PlaceHolder>

                                            <asp:LinkButton runat="server" CommandName="DeleteSpace" CommandArgument='<%# Eval("SpaceId") %>' 
                                                CssClass="btn btn-outline-danger btn-sm" OnClientClick="return confirm('Permanently delete this listing?');">
                                                <i class="fas fa-trash"></i>
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>