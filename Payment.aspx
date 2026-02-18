<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="Respace.Payment" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
    /* Centered Minimalist Layout */
    .pay-wrapper {
        display: flex;
        justify-content: center;
        align-items: center;
        padding: 60px 20px;
        background-color: #f8f9fa;
        min-height: 80vh;
    }

    .pay-container {
        max-width: 500px;
        width: 100%;
        background: #ffffff;
        border-radius: 12px;
        padding: 40px;
        box-shadow: 0 10px 30px rgba(0,0,0,0.05);
        border: 1px solid #eef0f2;
    }

    .pay-header { text-align: center; margin-bottom: 30px; }
    .pay-header h2 { font-size: 1.75rem; margin-bottom: 8px; color: #2d3436; font-weight: 700; }
    .pay-header p { color: #636e72; font-size: 0.9rem; }

    .summary-box {
        background: #fdfdfd;
        padding: 24px;
        border-radius: 8px;
        border: 1px solid #edeff1;
        margin-bottom: 30px;
    }

    .summary-label { display: block; font-size: 0.7rem; text-transform: uppercase; color: #b2bec3; letter-spacing: 1px; font-weight: 700; margin-bottom: 4px; }
    .summary-value { display: block; font-size: 1.1rem; color: #2d3436; font-weight: 600; margin-bottom: 15px; }
    
    .total-row { 
        margin-top: 10px; 
        padding-top: 15px; 
        border-top: 1px solid #f1f3f5; 
        display: flex; 
        justify-content: space-between; 
        align-items: center; 
    }

    /* Form Grid Logic */
    .input-group { margin-bottom: 20px; }
    .input-row { display: flex; gap: 15px; margin-bottom: 20px; }
    .input-row .input-group { flex: 1; margin-bottom: 0; }

    .input-group label { display: block; font-weight: 600; font-size: 0.85rem; color: #2d3436; margin-bottom: 8px; }
    
    .txt-input {
        width: 100%;
        padding: 12px 15px;
        border: 1.5px solid #dfe6e9;
        border-radius: 8px; /* Matching button roundness */
        font-size: 1rem;
        transition: border-color 0.2s;
    }

    .txt-input:focus { border-color: #ff385c; outline: none; }

    /* FIXED BUTTON STYLING */
    .btn-confirm {
        width: 100%;
        padding: 16px;
        background-color: #ff385c; /* Your requested coral/pink color */
        color: white;
        border: none;
        border-radius: 12px; /* Matches your Login button image */
        font-size: 1.1rem;
        font-weight: 700;
        cursor: pointer;
        transition: background 0.2s, transform 0.1s;
        display: block;
    }

    .btn-confirm:hover {
        background-color: #e31c5f; /* Darker shade on hover */
        opacity: 0.95;
    }

    .btn-confirm:active {
        transform: scale(0.98); /* Slight click effect */
    }

    .security-footer { text-align: center; color: #b2bec3; font-size: 0.75rem; margin-top: 25px; }
</style>

<div class="pay-wrapper">
    <div class="pay-container">
        <div class="pay-header">
            <h2>Secure Payment</h2>
            <p>Finalize your purchase to start using ReSpace.</p>
        </div>

        <div class="summary-box">
            <asp:PlaceHolder ID="phMembershipInfo" runat="server" Visible="false">
                <span class="summary-label">Membership Tier</span>
                <span class="summary-value"><asp:Label ID="lblPlanName" runat="server" /> Plan</span>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phBookingInfo" runat="server" Visible="false">
                <span class="summary-label">Booking Reference</span>
                <span class="summary-value">Order #<asp:Label ID="lblBookingId" runat="server" /></span>
            </asp:PlaceHolder>

            <div class="total-row">
                <span style="font-weight:600; color: #636e72;">Amount to Pay</span>
                <span style="font-size: 1.5rem; color: #00b894; font-weight: 800;">$<asp:Label ID="lblTotalAmount" runat="server" /></span>
            </div>
        </div>

        <div class="input-group">
            <label>Cardholder Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="txt-input" placeholder="Erfan Eddie" />
        </div>
        
        <div class="input-group">
            <label>Card Number</label>
            <asp:TextBox ID="txtCard" runat="server" CssClass="txt-input" placeholder="0000 0000 0000 0000" />
        </div>

        <div class="input-row">
            <div class="input-group">
                <label>Expiry Date</label>
                <asp:TextBox ID="txtExpiry" runat="server" CssClass="txt-input" placeholder="MM / YY" />
            </div>
            <div class="input-group">
                <label>CVC / CVV</label>
                <asp:TextBox ID="txtCVC" runat="server" CssClass="txt-input" placeholder="123" />
            </div>
        </div>

        <asp:Button ID="btnConfirm" runat="server" Text="Confirm & Pay" OnClick="btnConfirm_Click" CssClass="btn-confirm" />
        
        <div class="security-footer">
            🔒 SSL Encrypted & Secure Payment Processing
        </div>
    </div>
</div>
</asp:Content>