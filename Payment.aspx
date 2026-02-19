<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="Respace.Payment" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
    .pay-wrapper { display: flex; justify-content: center; align-items: center; padding: 60px 20px; background-color: #f8f9fa; min-height: 80vh; }
    .pay-container { max-width: 500px; width: 100%; background: #ffffff; border-radius: 12px; padding: 40px; border: 1px solid #eef0f2; box-shadow: 0 10px 30px rgba(0,0,0,0.05); }
    .pay-header { text-align: center; margin-bottom: 30px; }
    .pay-header h2 { font-size: 1.75rem; margin-bottom: 8px; color: #2d3436; font-weight: 700; }
    .summary-box { background: #fdfdfd; padding: 24px; border-radius: 8px; border: 1px solid #edeff1; margin-bottom: 30px; }
    .total-row { border-top: 1px solid #f1f3f5; padding-top: 15px; display: flex; justify-content: space-between; align-items: center; }
    .input-group { margin-bottom: 20px; }
    .input-row { display: flex; gap: 15px; }
    .input-row .input-group { flex: 1; }
    .txt-input { width: 100%; padding: 12px 15px; border: 1.5px solid #dfe6e9; border-radius: 8px; font-size: 1rem; box-sizing: border-box; }
    .txt-input:focus { border-color: #ff385c; outline: none; box-shadow: 0 0 0 3px rgba(255, 56, 92, 0.1); }
    .val-error { color: #ff385c; font-size: 0.75rem; font-weight: 600; margin-top: 4px; display: block; }
    .btn-confirm { width: 100%; padding: 16px; background-color: #ff385c; color: white; border: none; border-radius: 12px; font-size: 1.1rem; font-weight: 700; cursor: pointer; transition: 0.2s; }
    .btn-confirm:hover { background-color: #e31c5f; }
</style>

<div class="pay-wrapper">
    <div class="pay-container">
        <div class="pay-header">
            <h2>Secure Payment</h2>
            <p>Finalize your purchase securely.</p>
        </div>

        <div class="summary-box">
            <%-- ID matched to phCurrentPlan --%>
            <asp:PlaceHolder ID="phCurrentPlan" runat="server" Visible="false">
                <div style="font-size: 0.7rem; color: #b2bec3; text-transform: uppercase;">Membership</div>
                <div style="font-weight: 600; margin-bottom: 10px;"><asp:Label ID="lblPlanName" runat="server" /> Plan</div>
            </asp:PlaceHolder>
            
            <asp:PlaceHolder ID="phBookingInfo" runat="server" Visible="false">
                <div style="font-size: 0.7rem; color: #b2bec3; text-transform: uppercase;">Booking Reference</div>
                <div style="font-weight: 600; margin-bottom: 10px;">Order #<asp:Label ID="lblBookingId" runat="server" /></div>
            </asp:PlaceHolder>

            <div class="total-row">
                <span style="color: #636e72;">Total Amount</span>
                <span style="font-size: 1.5rem; color: #00b894; font-weight: 800;">$<asp:Label ID="lblTotalAmount" runat="server" /></span>
            </div>
        </div>

        <div class="input-group">
            <label style="font-weight:600; font-size:0.85rem;">Cardholder Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="txt-input" placeholder="Your Name" />
            <asp:RequiredFieldValidator ControlToValidate="txtName" ErrorMessage="Name required" Display="Dynamic" CssClass="val-error" runat="server" ValidationGroup="PayGroup" />
        </div>
        
        <div class="input-group">
            <label style="font-weight:600; font-size:0.85rem;">Card Number</label>
            <asp:TextBox ID="txtCard" runat="server" CssClass="txt-input" placeholder="16 Digits" MaxLength="16" />
            <asp:RequiredFieldValidator ControlToValidate="txtCard" ErrorMessage="Required" Display="Dynamic" CssClass="val-error" runat="server" ValidationGroup="PayGroup" />
            <asp:RegularExpressionValidator ControlToValidate="txtCard" ErrorMessage="16 digits" ValidationExpression="^\d{16}$" Display="Dynamic" CssClass="val-error" runat="server" ValidationGroup="PayGroup" />
        </div>

        <div class="input-row">
            <div class="input-group">
                <label style="font-weight:600; font-size:0.85rem;">Expiry (MM/YY)</label>
                <asp:TextBox ID="txtExpiry" runat="server" CssClass="txt-input" MaxLength="5" placeholder="05/28" />
                <asp:RequiredFieldValidator ControlToValidate="txtExpiry" ErrorMessage="Required" Display="Dynamic" CssClass="val-error" runat="server" ValidationGroup="PayGroup" />
            </div>
            <div class="input-group">
                <label style="font-weight:600; font-size:0.85rem;">CVC</label>
                <asp:TextBox ID="txtCVC" runat="server" CssClass="txt-input" MaxLength="3" placeholder="123" />
                <asp:RequiredFieldValidator ControlToValidate="txtCVC" ErrorMessage="Required" Display="Dynamic" CssClass="val-error" runat="server" ValidationGroup="PayGroup" />
            </div>
        </div>

        <asp:Button ID="btnConfirm" runat="server" Text="Confirm & Pay" 
            OnClick="btnConfirm_Click" 
            OnClientClick="if(Page_ClientValidate('PayGroup')) { return confirm('Process payment?'); } else { return false; }"
            CssClass="btn-confirm" 
            ValidationGroup="PayGroup" />
    </div>
</div>
</asp:Content>