<%@ Page Title="Space Management" Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" CodeBehind="SpaceManagement.aspx.cs" Inherits="Respace.Admin.SpaceManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="container-fluid">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="h3 mb-0 text-gray-800">Space Management</h2>
            <div class="d-flex">
                <asp:TextBox ID="txtSearchSpace" runat="server" CssClass="form-control form-control-sm me-2" 
                    Placeholder="Search by Space or Host..." AutoPostBack="true" OnTextChanged="txtSearchSpace_TextChanged"></asp:TextBox>
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

                <div class="modal fade" id="detailsModal" tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog modal-lg modal-dialog-centered">
                        <div class="modal-content border-0 shadow">
                            <div class="modal-header bg-light">
                                <h5 class="modal-title fw-bold">Vetting Listing: <asp:Literal ID="litSpaceName" runat="server" /></h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body p-4">
                                <div class="row">
                                    <div class="col-md-7">
                                        <h6 class="text-uppercase small fw-bold text-muted mb-3">Listing Description</h6>
                                        <p class="text-dark"><asp:Literal ID="litDescription" runat="server" /></p>
                                    
                                        <h6 class="text-uppercase small fw-bold text-muted mt-4 mb-3">Property Details</h6>
                                        <div class="bg-light p-3 rounded">
                                            <div class="row g-2">
                                                <div class="col-6 small"><strong>Type:</strong> <asp:Literal ID="litType" runat="server" /></div>
                                                <div class="col-6 small"><strong>Category:</strong> <asp:Literal ID="litCategory" runat="server" /></div>
                                                <div class="col-6 small"><strong>Capacity:</strong> <asp:Literal ID="litCapacity" runat="server" /> Guests</div>
                                                <div class="col-6 small"><strong>Listed Price:</strong> <asp:Literal ID="litPrice" runat="server" /></div>
                                                <div class="col-12 small mt-2"><strong>Location:</strong> <asp:Literal ID="litLocation" runat="server" /></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-5 border-start">
                                        <h6 class="text-uppercase small fw-bold text-muted mb-3">Host Verification</h6>
                                        <div class="d-flex align-items-center mb-3">
                                            <div class="bg-primary text-white rounded-circle d-flex align-items-center justify-content-center me-2" style="width:40px; height:40px;">
                                                <i class="fas fa-user"></i>
                                            </div>
                                            <div>
                                                <div class="fw-bold"><asp:Literal ID="litHostName" runat="server" /></div>
                                                <div class="small text-muted">Verified Respace Host</div>
                                            </div>
                                        </div>
                                        <div class="card bg-light border-0">
                                            <div class="card-body p-3">
                                                <div class="small text-muted mb-1">Total Community Reviews</div>
                                                <div class="h4 mb-0 fw-bold text-primary"><asp:Literal ID="litReviewCount" runat="server" /></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer bg-light border-0">
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close Review</button>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>