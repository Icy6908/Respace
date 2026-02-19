<%@ Page Title="View User" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="UserView.aspx.cs" Inherits="Respace.Admin.UserView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="mb-4">
            <a href="UserManagement.aspx" class="btn btn-sm btn-secondary"><i class="fas fa-arrow-left"></i> Back to List</a>
        </div>

        <div class="card shadow border-0">
            <div class="card-header bg-white py-3">
                <h5 class="m-0 font-weight-bold text-primary">User Profile Details</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-4 text-center border-end">
                        <i class="fas fa-user-circle fa-6x text-light mb-3"></i>
                        <h3><asp:Literal ID="litFullName" runat="server" /></h3>
                        <span class="badge bg-primary"><asp:Literal ID="litRole" runat="server" /></span>
                    </div>
                    <div class="col-md-8 ps-md-5">
                        <table class="table table-borderless">
                            <tr>
                                <th style="width: 200px;">Email:</th>
                                <td><asp:Literal ID="litEmail" runat="server" /></td>
                            </tr>
                            <tr>
                                <th>Points Balance:</th>
                                <td class="text-success fw-bold"><asp:Literal ID="litPoints" runat="server" /> pts</td>
                            </tr>
                            <tr>
                                <th>Membership Tier:</th>
                                <td><asp:Literal ID="litTier" runat="server" /></td>
                            </tr>
                            <tr>
                                <th>Account Created:</th>
                                <td><asp:Literal ID="litJoined" runat="server" /></td>
                            </tr>
                            <tr>
                                <th>Status:</th>
                                <td><asp:Literal ID="litStatus" runat="server" /></td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <div class="card shadow border-0 mt-4">
            <div class="card-header bg-white py-3">
                <h6 class="m-0 font-weight-bold text-primary text-uppercase">Booking History</h6>
            </div>
            <div class="table-responsive">
                <%-- FIXED ID: Matches the C# variable name --%>
                <asp:GridView ID="gvUserBookings" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover align-middle mb-0" GridLines="None">
                    <HeaderStyle CssClass="bg-light text-muted small fw-bold text-uppercase" />
                    <Columns>
                        <asp:BoundField DataField="BookingId" HeaderText="ID" ItemStyle-CssClass="ps-4" />
                        <asp:BoundField DataField="SpaceName" HeaderText="Space" />
                        <asp:BoundField DataField="StartDateTime" HeaderText="Check In" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:BoundField DataField="EndDateTime" HeaderText="Check Out" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:BoundField DataField="TotalPrice" HeaderText="Paid" DataFormatString="{0:C}" />
                        <%-- FIXED PARSER ERROR: Wrapped in asp:TemplateField --%>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class="badge bg-info"><%# Eval("Status") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>