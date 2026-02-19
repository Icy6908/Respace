<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rewards.aspx.cs"
    Inherits="Respace.Rewards" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page">

        <div class="card">
            <h2 class="h2">My Rewards</h2>
            <p class="muted">Earn points when you book spaces.</p>

            <div class="stat">
                <div class="stat__label">Current Points Balance</div>
                <div class="stat__value">
                    <asp:Label ID="lblPoints" runat="server" />
                </div>
            </div>
        </div>

        <asp:Label ID="lblMsg" runat="server" Text="" EnableViewState="false"></asp:Label>

<div class="card section">
    <h3 class="h3">Points History</h3>
    <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="false" CssClass="table" GridLines="None">
        <Columns>
            <asp:BoundField DataField="CreatedAt" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
            <asp:TemplateField HeaderText="Type">
                <ItemTemplate>
                    <span style='<%# Eval("TxnType").ToString() == "Earned" ? "color:#28a745;" : "color:#dc3545;" %>'>
                        <%# Eval("TxnType") %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Points" HeaderText="Points" />
            <asp:BoundField DataField="Reference" HeaderText="Details" />
        </Columns>
    </asp:GridView>
</div>

        <div class="card section">
    <h3 class="h3" style="color: #28a745;">🎟️ Ready to Use</h3>
    <asp:GridView ID="gvAvailableVouchers" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None">
        <Columns>
            <asp:BoundField DataField="RedeemedAt" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
            <asp:TemplateField HeaderText="Voucher">
                <ItemTemplate>
                    <strong>$<%# Eval("DiscountAmount") %> Off</strong><br />
                    <code style="color:#0056b3;"><%# Eval("CouponCode") %></code>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <span class="badge" style="background:#d4edda; color:#155724; padding:5px 10px; border-radius:15px; font-size:0.8rem;">Available</span>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <hr style="margin: 30px 0; border: 0; border-top: 1px solid #eee;" />

    <h3 class="h3" style="color: #666;">📜 Used History</h3>
    <asp:GridView ID="gvUsedVouchers" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None" Opacity="0.7">
        <Columns>
            <asp:BoundField DataField="RedeemedAt" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
            <asp:TemplateField HeaderText="Voucher">
                <ItemTemplate>
                    <span style="text-decoration: line-through; color: #999;">$<%# Eval("DiscountAmount") %> Off</span><br />
                    <small style="color:#999;"><%# Eval("CouponCode") %></small>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <span style="color:#999; font-style:italic;">Redeemed</span>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>

    <div class="card">
    <h3 class="h3" style="margin-bottom:20px;">🎁 Redeem Points for Vouchers</h3>
    <div class="voucher-grid">
        <asp:Repeater ID="rptVoucherShop" runat="server">
            <ItemTemplate>
                <div class='<%# "voucher-card " + (Convert.ToInt32(Eval("PointsCost")) >= 1000 ? "gold" : "") %>'>
                    <div class="voucher-left">
                        <span class="v-amount">$<%# Eval("DiscountAmount") %></span>
                        <span class="v-off">OFF</span>
                    </div>
                    <div class="voucher-right">
                        <h4><%# Eval("VoucherName") %></h4>
                        <p>Cost: <%# Eval("PointsCost") %> Points</p>
                        <asp:Button ID="btnRedeem" runat="server" Text="Redeem" CssClass="btn-redeem" 
                            OnClick="RedeemPoints_Click" 
                            CommandArgument='<%# Eval("PointsCost") + "|" + Eval("DiscountAmount") %>' />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
</div>

<style>
    .voucher-grid { display: flex; gap: 20px; flex-wrap: wrap; margin-top: 20px; }
    .voucher-card { 
        display: flex; border: 2px dashed #ccc; border-radius: 10px; 
        overflow: hidden; width: 300px; background: #fff;
    }
    .voucher-left { 
        background: #f8f9fa; padding: 20px; display: flex; 
        flex-direction: column; align-items: center; border-right: 2px dashed #ccc;
    }
    .v-amount { font-size: 2rem; font-weight: 900; color: #333; }
    .v-off { font-size: 0.8rem; font-weight: bold; }
    .voucher-right { padding: 15px; flex: 1; }
    .voucher-right h4 { margin: 0; color: #333; }
    .btn-redeem { 
        margin-top: 10px; background: #ff4d6d; color: #fff; 
        border: none; padding: 5px 15px; border-radius: 5px; cursor: pointer;
    }
    /* Gold Theme for higher rewards */
    .voucher-card.gold { border-color: #ffd700; }
    .voucher-card.gold .voucher-left { background: #fff9db; border-right-color: #ffd700; }
    .voucher-card.gold .v-amount { color: #d4af37; }
</style>
</div>

    </div>

</asp:Content>
