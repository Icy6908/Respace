<%@ Page Title="Financial Management" Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" CodeBehind="Financials.aspx.cs" Inherits="Respace.Admin.Financials" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="h3 mb-0 text-gray-800">Financial Management</h2>
            <div class="d-flex gap-2 align-items-center">
                <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-control form-control-sm" style="width:160px;"></asp:TextBox>
                <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control form-control-sm" style="width:160px;"></asp:TextBox>
                <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-primary btn-sm px-3" OnClick="btnFilter_Click" />
                <asp:Button ID="btnExport" runat="server" Text="Export CSV" CssClass="btn btn-outline-success btn-sm px-3" OnClick="btnExport_Click" />
            </div>
        </div>

        <div class="row mb-4">
            <div class="col-md-4">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-body">
                        <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">Total Amount Received</div>
                        <div class="h5 mb-0 font-weight-bold text-gray-800"><asp:Literal ID="litTotalReceived" runat="server" Text="$0.00" /></div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card border-left-info shadow h-100 py-2">
                    <div class="card-body">
                        <div class="text-xs font-weight-bold text-info text-uppercase mb-1">Host Payouts (90%)</div>
                        <div class="h5 mb-0 font-weight-bold text-gray-800"><asp:Literal ID="litHostPayout" runat="server" Text="$0.00" /></div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card border-left-success shadow h-100 py-2">
                    <div class="card-body">
                        <div class="text-xs font-weight-bold text-success text-uppercase mb-1">Commission Earned (10%)</div>
                        <div class="h5 mb-0 font-weight-bold text-gray-800"><asp:Literal ID="litCommission" runat="server" Text="$0.00" /></div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card admin-card p-0 shadow-sm border-0">
            <div class="card-header bg-white py-3">
                <h6 class="m-0 font-weight-bold text-primary">Transaction History</h6>
            </div>
            <div class="table-responsive">
                <asp:GridView ID="gvFinancials" runat="server" AutoGenerateColumns="False" CssClass="table table-hover align-middle mb-0" GridLines="None">
                    <HeaderStyle CssClass="bg-light text-muted small fw-bold text-uppercase" />
                    <Columns>
                        <asp:BoundField DataField="PaymentID" HeaderText="Ref ID" ItemStyle-CssClass="ps-4" />
                        <asp:TemplateField HeaderText="Space & Renter">
                            <ItemTemplate>
                                <div class="fw-bold text-dark"><%# Eval("SpaceName") %></div>
                                <div class="small text-muted">User: <%# Eval("RenterName") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="PaymentDate" HeaderText="Date" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:BoundField DataField="Amount" HeaderText="Total Paid" DataFormatString="{0:C}" ItemStyle-CssClass="fw-bold" />
                        
                        <asp:TemplateField HeaderText="Host (90%)">
                            <ItemTemplate>
                                <span class="text-muted"><%# (Convert.ToDecimal(Eval("Amount")) * 0.90m).ToString("C") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Earned (10%)">
                            <ItemTemplate>
                                <span class="text-success fw-bold">+ <%# (Convert.ToDecimal(Eval("Amount")) * 0.10m).ToString("C") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='badge <%# GetPaymentStatusClass(Eval("Status").ToString()) %>'>
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>