<%@ Page Title="Review Management" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ReviewManagement.aspx.cs" Inherits="Respace.Admin.ReviewManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <h2 class="h3 mb-4 text-gray-800">Review Moderation Queue</h2>

        <div class="card shadow-sm border-0">
            <div class="table-responsive">
                <asp:GridView ID="gvReviews" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover align-middle mb-0" OnRowCommand="gvReviews_RowCommand" GridLines="None">
                    <HeaderStyle CssClass="bg-light text-muted small fw-bold text-uppercase" />
                    <Columns>
                        <%-- Listing Info --%>
                        <asp:TemplateField HeaderText="Listing">
                            <ItemTemplate>
                                <div class="fw-bold text-dark"><%# Eval("SpaceName") %></div>
                                <div class="small text-muted">ID: #<%# Eval("SpaceId") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- NEW: Reviewer Verification Profile --%>
                        <asp:TemplateField HeaderText="Reviewer Profile">
                            <ItemTemplate>
                                <div class="d-flex align-items-center mb-1">
                                    <i class="fas fa-user-circle me-2 text-primary"></i>
                                    <span class="fw-bold"><%# Eval("ReviewerName") %></span>
                                </div>
                                <div class="small text-muted"><i class="fas fa-envelope me-1"></i> <%# Eval("Email") %></div>
                                <div class="small text-muted"><i class="fas fa-id-badge me-1"></i> Role: <%# Eval("Role") %></div>
                                <div class="small text-muted"><i class="fas fa-calendar-alt me-1"></i> Joined: <%# Eval("UserJoinedDate", "{0:MMM yyyy}") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Review Content --%>
                        <asp:TemplateField HeaderText="Review Details">
                            <ItemTemplate>
                                <div class="text-warning small mb-1">
                                    <%# GenerateStars(Convert.ToInt32(Eval("Rating"))) %>
                                </div>
                                <div class="small text-dark italic">"<%# Eval("Comment") %>"</div>
                                <div class="text-muted" style="font-size: 0.75rem;"><%# Eval("CreatedAt", "{0:MMM dd, yyyy}") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Actions --%>
                        <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="text-end pe-4">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CommandName="ApproveReview" CommandArgument='<%# Eval("ReviewId") %>' 
                                    CssClass="btn btn-success btn-sm me-1"><i class="fas fa-check"></i></asp:LinkButton>
                                <asp:LinkButton runat="server" CommandName="DeleteReview" CommandArgument='<%# Eval("ReviewId") %>' 
                                    CssClass="btn btn-outline-danger btn-sm" OnClientClick="return confirm('Delete this review?');">
                                    <i class="fas fa-trash"></i></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>