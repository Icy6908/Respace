<%@ Page Title="Payment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="Respace.Payment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-width: 500px; margin: 40px auto;">
        <div class="card">
            <h2 class="h2">Secure Checkout</h2>
            <p class="muted">You are subscribing to the <strong><asp:Literal ID="litPlanName" runat="server" /></strong> plan.</p>
            <hr />
            
            <div style="margin-top:20px;">
                <label>Card Number</label>
                <input type="text" class="input" placeholder="1234 5678 1234 5678" style="width:100%" />
            </div>

            <div style="display:flex; gap:10px; margin-top:15px;">
                <div style="flex:1">
                    <label>Expiry</label>
                    <input type="text" class="input" placeholder="MM/YY" style="width:100%" />
                </div>
                <div style="flex:1">
                    <label>CVV</label>
                    <input type="text" class="input" placeholder="123" style="width:100%" />
                </div>
            </div>

            <div style="margin-top:30px;">
                <asp:Button ID="btnConfirm" runat="server" Text="Pay & Activate Plan" 
                    CssClass="btn btn-primary" Width="100%" OnClick="btnConfirm_Click" />
            </div>
            
            <p style="text-align:center; margin-top:15px;">
                <a href="Membership.aspx" class="muted">Cancel and go back</a>
            </p>
        </div>
    </div>
</asp:Content>