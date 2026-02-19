<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="CouponManager.aspx.cs" Inherits="Respace.Admin.CouponManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Reward Store Management</h2>
        
        <asp:Label ID="lblStatus" runat="server" CssClass="d-block mb-3" EnableViewState="false"></asp:Label>

        <div class="card p-4 mb-4 shadow-sm">
            <h4 class="mb-3">Create New Reward Tier</h4>
            <div class="row g-3">
                <div class="col-md-3">
                    <label class="form-label fw-bold">Internal Code (e.g. BZ10)</label>
                    <asp:TextBox ID="txtCode" runat="server" CssClass="form-control" placeholder="BZ10" />
                </div>
                <div class="col-md-3">
                    <label class="form-label fw-bold">Discount Amount ($)</label>
                    <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="10.00" />
                </div>
                <div class="col-md-3">
                    <label class="form-label fw-bold">Point Cost</label>
                    <asp:TextBox ID="txtCost" runat="server" CssClass="form-control" placeholder="500" />
                </div>
                <div class="col-md-3 d-flex align-items-end">
                    <asp:Button ID="btnSave" runat="server" Text="Add to Shop" CssClass="btn btn-primary w-100 fw-bold" OnClick="btnSave_Click" />
                </div>
            </div>
        </div>

        <div class="card shadow-sm">
            <div class="card-header bg-white">
                <h5 class="mb-0">Current Shop Inventory</h5>
            </div>
            <asp:GridView ID="gvStore" runat="server" AutoGenerateColumns="False" CssClass="table table-hover mb-0" 
                DataKeyNames="CouponDefId" OnRowDeleting="gvStore_RowDeleting" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="CouponCode" HeaderText="Internal Code" />
                    <asp:BoundField DataField="DiscountAmount" HeaderText="Guest Saves" DataFormatString="${0:F2}" />
                    <asp:BoundField DataField="PointCost" HeaderText="Cost (Points)" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" 
                                OnClientClick="return confirm('Remove this reward from the guest shop?');" 
                                CssClass="text-danger text-decoration-none">
                                <i class="fas fa-trash-alt me-1"></i>Delete
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>