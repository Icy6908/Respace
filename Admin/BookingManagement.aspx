<%@ Page Title="Booking Management" Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" CodeBehind="BookingManagement.aspx.cs" Inherits="Respace.Admin.BookingManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="h3 mb-0 text-gray-800">Booking Management</h2>
            <div class="d-flex gap-2">
                <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" 
                    OnSelectedIndexChanged="Filter_Changed" CssClass="form-select form-select-sm" style="width:150px;">
                    <asp:ListItem Text="All Status" Value="All"></asp:ListItem>
                    <asp:ListItem Text="Confirmed" Value="Confirmed"></asp:ListItem>
                    <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                    <asp:ListItem Text="Cancelled" Value="Cancelled"></asp:ListItem>
                    <asp:ListItem Text="Refunded" Value="Refunded"></asp:ListItem>
                </asp:DropDownList>

                <asp:TextBox ID="txtSearchBooking" runat="server" CssClass="form-control form-control-sm me-2" 
                    Placeholder="Search Renter or Space..." AutoPostBack="true" 
                    OnTextChanged="txtSearchBooking_TextChanged" style="width:250px;"></asp:TextBox>
            </div>
        </div>

        <div class="card admin-card p-0 shadow-sm border-0">
            <div class="table-responsive">
                <asp:GridView ID="gvBookings" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover align-middle mb-0" GridLines="None">
                    <HeaderStyle CssClass="bg-light text-muted small fw-bold text-uppercase" />
                    <Columns>
                        <asp:BoundField DataField="BookingId" HeaderText="ID" ItemStyle-CssClass="ps-4" HeaderStyle-CssClass="ps-4" />
                        <asp:BoundField DataField="SpaceName" HeaderText="Space" />
                        <asp:BoundField DataField="RenterName" HeaderText="Renter" />
                        <asp:BoundField DataField="StartDateTime" HeaderText="Date" DataFormatString="{0:MMM dd, yyyy}" />
        
                        <asp:TemplateField HeaderText="Total">
                            <ItemTemplate>
                                <span class="fw-bold"><%# Eval("TotalPrice", "{0:C}") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='badge <%# GetBookingStatusClass(Eval("Status")?.ToString()) %>'>
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