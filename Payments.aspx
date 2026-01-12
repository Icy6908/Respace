<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Payments.aspx.cs" Inherits="Respace.Payments" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Payment Tracking & Financials</h2>

    <div class="row mb-4">
        <div class="col-md-4">
            <div class="card bg-light p-3">
                <h5>Total Ingoing (Rentals)</h5>
                <asp:Label ID="lblTotalIngoing" runat="server" CssClass="h4 text-success" Text="$0.00"></asp:Label>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card bg-light p-3">
                <h5>Pending Owner Payouts</h5>
                <asp:Label ID="lblPendingPayouts" runat="server" CssClass="h4 text-primary" Text="$0.00"></asp:Label>
            </div>
        </div>
    </div>

    <asp:GridView ID="gvPayments" runat="server" CssClass="table table-bordered table-hover" 
        AutoGenerateColumns="False" DataKeyNames="PaymentID" OnRowCommand="gvPayments_RowCommand">
        <Columns>
            <asp:BoundField DataField="PaymentID" HeaderText="ID" />
            <asp:BoundField DataField="BookingID" HeaderText="Booking Ref" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:C}" />
            <asp:BoundField DataField="PaymentType" HeaderText="Type (Payout/Refund)" />
            <asp:BoundField DataField="Gateway" HeaderText="Method (PayNow/PayPal)" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button ID="btnComplete" runat="server" Text="Mark Completed" CommandName="CompletePayment" 
                        CommandArgument='<%# Eval("PaymentID") %>' CssClass="btn btn-success btn-sm" 
                        Visible='<%# Eval("Status").ToString() == "Pending" %>' />
                    <asp:Button ID="btnRefund" runat="server" Text="Issue Refund" CommandName="IssueRefund" 
                        CommandArgument='<%# Eval("PaymentID") %>' CssClass="btn btn-warning btn-sm" 
                        Visible='<%# Eval("PaymentType").ToString() == "Rental" %>' />
                    <asp:Button ID="Button1" runat="server" Text="Mark Completed" 
                        CommandName="CompletePayment" 
                        CommandArgument='<%# Eval("PaymentID") %>' 
                        Visible='<%# Eval("Status").ToString() == "Pending" %>' 
                        CssClass="btn btn-success btn-sm" />
                    <asp:Button ID="Button2" runat="server" Text="Issue Refund" 
                        CommandName="IssueRefund" 
                        CommandArgument='<%# Eval("PaymentID") %>' 
                        Visible='<%# Eval("Status").ToString() == "Completed" %>' 
                        CssClass="btn btn-danger btn-sm" 
                        OnClientClick="return confirm('Are you sure you want to refund this?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
