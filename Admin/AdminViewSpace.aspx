<%@ Page Title="Vet Space" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AdminViewSpace.aspx.cs" Inherits="Respace.Admin.AdminViewSpace" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="mb-4">
            <a href="SpaceManagement.aspx" class="btn btn-sm btn-outline-secondary mb-3">
                <i class="fas fa-arrow-left me-1"></i> Back to Space Management
            </a>
            <h2 class="h3 mb-0 text-gray-800"> <asp:Literal ID="litSpaceName" runat="server" /></h2>
        </div>

        <div class="row">
            <div class="col-lg-8">
                <div class="card shadow-sm border-0 mb-4">
                    <div class="card-header py-3 bg-white border-bottom">
                        <h6 class="m-0 fw-bold text-primary">Listing Information</h6>
                    </div>
                    <div class="card-body">
                        <div class="row mb-4">
                            <div class="col-md-6 mb-3">
                                <label class="small text-muted text-uppercase fw-bold">Category</label>
                                <div class="text-dark"><asp:Literal ID="litCategory" runat="server" /></div>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="small text-muted text-uppercase fw-bold">Property Type</label>
                                <div class="text-dark"><asp:Literal ID="litType" runat="server" /></div>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="small text-muted text-uppercase fw-bold">Capacity</label>
                                <div class="text-dark"><asp:Literal ID="litCapacity" runat="server" /> Guests</div>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="small text-muted text-uppercase fw-bold">Price Per Hour</label>
                                <div class="text-dark fw-bold text-success"><asp:Literal ID="litPrice" runat="server" />/hr</div>
                            </div>
                            <div class="col-12 mb-3">
                                <label class="small text-muted text-uppercase fw-bold">Location</label>
                                <div class="text-dark"><asp:Literal ID="litLocation" runat="server" /></div>
                            </div>
                        </div>
                        <hr />
                        <label class="small text-muted text-uppercase fw-bold mb-2">Description</label>
                        <p class="text-dark"><asp:Literal ID="litDescription" runat="server" /></p>
                    </div>
                </div>

                <div class="card shadow-sm border-0">
                    <div class="card-header py-3 bg-white border-bottom d-flex justify-content-between align-items-center">
                        <h6 class="m-0 fw-bold text-primary">Pending Reviews & Reviewer Verification</h6>
                        <span class="badge bg-primary text-white"><asp:Literal ID="litReviewCount" runat="server" /> Total Pending</span>
                    </div>
                    <div class="card-body p-0">
                        <asp:Repeater ID="rptReviews" runat="server" OnItemCommand="rptReviews_ItemCommand">
                            <ItemTemplate>
                                <div class="p-4 border-bottom">
                                    <div class="row">
                                        <div class="col-md-4 border-end">
                                            <h6 class="small text-uppercase fw-bold text-muted mb-3">Reviewer Profile</h6>
                                            <div class="d-flex align-items-center mb-2">
                                                <div class="bg-light text-primary rounded-circle d-flex align-items-center justify-content-center me-2" style="width:35px; height:35px; border: 1px solid #eee;">
                                                    <i class="fas fa-user-check"></i>
                                                </div>
                                                <div class="fw-bold text-dark"><%# Eval("ReviewerName") %></div>
                                            </div>
                                            <div class="small text-muted mb-1"><i class="fas fa-envelope me-1"></i> <%# Eval("Email") %></div>
                                            <div class="small text-muted mb-1"><i class="fas fa-id-badge me-1"></i> Role: <%# Eval("Role") %></div>
                                            <div class="small text-muted"><i class="fas fa-calendar-alt me-1"></i> Joined: <%# Eval("UserJoinedDate", "{0:MMM yyyy}") %></div>
                                        </div>

                                        <div class="col-md-8 ps-4">
                                            <div class="d-flex justify-content-between mb-2">
                                                <div class="text-warning small">
                                                    <%# GenerateStars(Convert.ToInt32(Eval("Rating"))) %>
                                                </div>
                                                <div class="small text-muted"><%# Eval("CreatedAt", "{0:MMM dd, yyyy}") %></div>
                                            </div>
                                            <p class="text-dark mb-3">"<%# Eval("Comment") %>"</p>
                                            
                                            <div class="d-flex gap-2">
                                                <asp:LinkButton runat="server" CommandName="ApproveReview" CommandArgument='<%# Eval("ReviewId") %>' 
                                                    CssClass="btn btn-success btn-sm"><i class="fas fa-check me-1"></i> Approve</asp:LinkButton>
                                                <asp:LinkButton runat="server" CommandName="DeleteReview" CommandArgument='<%# Eval("ReviewId") %>' 
                                                    CssClass="btn btn-outline-danger btn-sm" OnClientClick="return confirm('Delete this review?');">
                                                    <i class="fas fa-trash me-1"></i> Delete
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Panel runat="server" Visible='<%# rptReviews.Items.Count == 0 %>' CssClass="p-4 text-center text-muted">
                                    <i class="fas fa-check-circle d-block mb-2 fa-2x text-success"></i> No pending reviews for this listing.
                                </asp:Panel>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="card shadow-sm border-0 mb-4 bg-light">
                    <div class="card-body">
                        <h6 class="small text-muted text-uppercase fw-bold mb-3">Host Profile</h6>
                        <div class="d-flex align-items-center mb-3">
                            <div class="bg-primary text-white rounded-circle d-flex align-items-center justify-content-center me-3" style="width:50px; height:50px;">
                                <i class="fas fa-user-tie fa-lg"></i>
                            </div>
                            <div>
                                <h5 class="mb-0 fw-bold text-dark"><asp:Literal ID="litHostName" runat="server" /></h5>
                                <div class="small text-muted">Host ID: <asp:Literal ID="litHostId" runat="server" /></div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card shadow-sm border-0">
                    <div class="card-body p-3">
                        <asp:Button ID="btnApprove" runat="server" Text="Approve Space" CssClass="btn btn-success w-100 mb-2 py-2 fw-bold" OnClick="btnApprove_Click" />
                        <asp:Button ID="btnReject" runat="server" Text="Reject Space" CssClass="btn btn-outline-warning w-100 py-2 fw-bold" OnClick="btnReject_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>