<%@ Page Title="Review Management" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ReviewManagement.aspx.cs" Inherits="Respace.Admin.ReviewManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .badge-pill-respace {
            background-color: #fff5f7;
            color: #ff4d6d;
            border: 1px solid #ff4d6d;
            border-radius: 12px;
            padding: 2px 8px;
            font-size: 0.75rem;
            margin-right: 4px;
            display: inline-block;
            margin-bottom: 4px;
        }
    </style>

    <div class="container-fluid">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="h3 mb-0 text-gray-800">Review Moderation Queue</h2>
            <div class="d-flex gap-2">
                <asp:DropDownList ID="ddlStarFilter" runat="server" AutoPostBack="true" 
                    OnSelectedIndexChanged="Filter_Changed" CssClass="form-select form-select-sm" style="width:130px;">
                    <asp:ListItem Text="All Ratings" Value="All"></asp:ListItem>
                    <asp:ListItem Text="5 Stars" Value="5"></asp:ListItem>
                    <asp:ListItem Text="4 Stars" Value="4"></asp:ListItem>
                    <asp:ListItem Text="3 Stars" Value="3"></asp:ListItem>
                    <asp:ListItem Text="2 Stars" Value="2"></asp:ListItem>
                    <asp:ListItem Text="1 Star" Value="1"></asp:ListItem>
                </asp:DropDownList>

                <asp:TextBox ID="txtSearchReview" runat="server" CssClass="form-control form-control-sm" 
                    Placeholder="Search Listing or User..." AutoPostBack="true" 
                    OnTextChanged="Filter_Changed" style="width:250px;"></asp:TextBox>
            </div>
        </div>

        <div class="card shadow-sm border-0">
            <div class="table-responsive">
                <asp:GridView ID="gvReviews" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover align-middle mb-0" OnRowCommand="gvReviews_RowCommand" GridLines="None">
                    <HeaderStyle CssClass="bg-light text-muted small fw-bold text-uppercase" />
                    <Columns>
                        <asp:TemplateField HeaderText="Listing">
                            <ItemTemplate>
                                <div class="fw-bold text-dark"><%# Eval("SpaceName") %></div>
                                <div class="small text-muted">ID: #<%# Eval("SpaceId") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Reviewer Profile">
                            <ItemTemplate>
                                <div class="d-flex align-items-center mb-1">
                                    <i class="fas fa-user-circle me-2 text-primary"></i>
                                    <span class="fw-bold"><%# Eval("ReviewerName") %></span>
                                </div>
                                <div class="small text-muted"><i class="fas fa-envelope me-1"></i> <%# Eval("Email") %></div>
                                <div class="small text-muted"><i class="fas fa-calendar-alt me-1"></i> Joined: <%# Eval("UserJoinedDate", "{0:MMM yyyy}") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Review Details">
                            <ItemTemplate>
                                <div class="text-warning small mb-1">
                                    <%# GenerateStars(Convert.ToInt32(Eval("Rating"))) %>
                                </div>
                                <div class="mb-2">
                                    <%# DisplayBadges(Eval("Badges")) %>
                                </div>
                                <div class="small text-dark italic">"<%# Eval("Comment") %>"</div>
                                <div class="text-muted" style="font-size: 0.75rem;"><%# Eval("CreatedAt", "{0:MMM dd, yyyy}") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

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