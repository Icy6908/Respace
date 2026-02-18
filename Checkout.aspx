<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="Respace.Checkout" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="sd-page">
        <div class="sd-wrap">
            <h1 class="h2">Confirm and Pay</h1>
            
            <div class="listing__body">
                <div class="listing__left">
                    <div class="card section">
                        <h3 class="h3">Your Booking Summary</h3>
                        <p><strong>Space:</strong> <asp:Label ID="lblSpaceName" runat="server" /></p>
                        <p><strong>Dates:</strong> <asp:Label ID="lblDates" runat="server" /></p>
                        <p><strong>Guests:</strong> <asp:Label ID="lblGuests" runat="server" /></p>
                    </div>

                    <div class="card section">
    <h3 class="h3">Coupons & Rewards</h3>
    
    <div class="reward-group" style="margin-bottom: 20px;">
        <label><strong>Redeem Coupon:</strong></label>
        <asp:DropDownList ID="ddlUserCoupons" runat="server" CssClass="form-control" 
            AutoPostBack="true" OnSelectedIndexChanged="ddlUserCoupons_SelectedIndexChanged">
            <asp:ListItem Text="Apply coupon" Value="0" />
        </asp:DropDownList>
        <asp:Label ID="lblPointsReminder" runat="server" Visible="false" 
            Text="💡 You have enough points to redeem a new coupon! Go to Rewards page." 
            Style="font-size: 0.8em; color: #007bff; display:block; margin-top:5px;" />
    </div>

    <hr />

    <div class="manual-coupon-group" style="margin-top: 15px;">
        <label><strong>Have a promo code?</strong></label>
        <div style="display:flex; gap: 10px;">
            <asp:TextBox ID="txtCoupon" runat="server" placeholder="Enter code (e.g. SAVE5)" CssClass="form-control" style="flex:1;"></asp:TextBox>
            <asp:Button ID="btnApplyCoupon" runat="server" Text="Apply" OnClick="btnApplyCoupon_Click" CssClass="btn btn-secondary" />
        </div>
        <asp:Label ID="lblCouponMsg" runat="server" Style="display:block; margin-top:5px; font-size:0.9rem;"></asp:Label>
    </div>
</div>
                </div>

                <aside class="listing__right">
                    <asp:Label ID="lblMsg" runat="server" Text="" EnableViewState="false"></asp:Label>
                    <div class="card">
                        <h3 class="h3">Price Details</h3>
                        <div style="display:flex; justify-content:space-between; margin-bottom:10px;">
                            <span>Base Price (<asp:Label ID="lblNights" runat="server" /> nights)</span>
                            <span>$<asp:Label ID="lblSubtotal" runat="server" /></span>
                        </div>

                        <asp:PlaceHolder ID="phMemberDiscount" runat="server" Visible="false">
                            <div style="display:flex; justify-content:space-between; margin-bottom:10px; color:#0b6b2a; font-weight:bold;">
                                <span>Membership Discount (<asp:Label ID="lblTierName" runat="server" />)</span>
                                <span>-$<asp:Label ID="lblMemberDiscount" runat="server" /></span>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phCouponDiscount" runat="server" Visible="false">
                            <div style="display:flex; justify-content:space-between; margin-bottom:10px; color:#0b6b2a;">
                                <span>Coupon Discount</span>
                                <span>-$<asp:Label ID="lblCouponAmt" runat="server" /></span>
                            </div>
                        </asp:PlaceHolder>

                        <hr style="border:0; border-top:1px solid #eee; margin:15px 0;" />
                        
                        <div style="display:flex; justify-content:space-between; font-size:1.4rem; font-weight:900; margin-bottom: 10px;">
                            <span>Total</span>
                            <span>$<asp:Label ID="lblTotal" runat="server" ClientIDMode="Static" /></span>
                        </div>

                        <div class="points-row" style="background: #f0f7ff; padding: 15px; border-radius: 8px; border: 1px solid #d0e3ff; margin-top:15px;">
    <div style="font-size: 1.1rem; font-weight: bold; color: #0056b3;">
        ✨ Points you'll earn: <span id="spanPoints">0</span>
    </div>
    
    <div id="divPointsReason" style="font-size: 0.9rem; color: #555; margin-top: 5px; font-style: italic;">
        Calculating your rewards...
    </div>
</div>

<asp:HiddenField ID="hfPlanName" runat="server" ClientIDMode="Static" Value="Free" />

                        <asp:HiddenField ID="hfMultiplier" runat="server" ClientIDMode="Static" Value="1.0" />

                        <asp:Button ID="btnFinalize" runat="server" Text="Confirm Booking" CssClass="btn btn-primary btn-block" style="margin-top:20px;" OnClick="btnFinalize_Click" />
                    </div>
                </aside>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function updatePointsDisplay() {
            var totalLabel = document.getElementById('lblTotal');
            var pointsSpan = document.getElementById('spanPoints');
            var reasonDiv = document.getElementById('divPointsReason');
            var hfMult = document.getElementById('hfMultiplier');
            var hfPlan = document.getElementById('hfPlanName');

            if (totalLabel && pointsSpan && reasonDiv && hfMult) {
                var totalValue = parseFloat(totalLabel.innerText.replace(/[$,]/g, '')) || 0;
                var multiplier = parseFloat(hfMult.value) || 1.0;
                var plan = hfPlan.value || "Free";

                var earned = Math.floor(totalValue * multiplier);
                pointsSpan.innerText = earned.toLocaleString();

                // Build the reason string
                var reasonHtml = "";
                if (plan !== "Free") {
                    reasonHtml = "<strong>" + plan + " Member Reward:</strong> You're earning " +
                        multiplier + "x points on your $" + totalValue.toFixed(2) + " total!";
                } else {
                    reasonHtml = "<strong>Standard Reward:</strong> 1 point for every $1 spent.";
                }

                reasonDiv.innerHTML = reasonHtml;
            }
        }

        // Ensure it runs even if the page partially reloads
        window.onload = updatePointsDisplay;
    </script>
</asp:Content>