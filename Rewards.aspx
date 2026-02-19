<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rewards.aspx.cs"
    Inherits="Respace.Rewards" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .page { padding: 20px 0; max-width: 1000px; margin: auto; }
        .card { background: #fff; border-radius: 16px; padding: 24px; box-shadow: 0 4px 20px rgba(0,0,0,0.06); margin-bottom: 24px; }
        .h2 { font-weight: 800; margin-bottom: 10px; }
        .stat { margin-top: 15px; }
        .stat__label { font-size: 14px; font-weight: 700; color: #666; text-transform: uppercase; letter-spacing: 0.5px; }
        .stat__value { font-size: 36px; font-weight: 900; color: #ff4d6d; }
        .table { width: 100%; border-collapse: collapse; margin-top: 10px; }
        .table th { text-align: left; padding: 12px; border-bottom: 2px solid #eee; color: #888; font-size: 13px; }
        .table td { padding: 12px; border-bottom: 1px solid #f9f9f9; }
        
        /* Voucher Shop Styling */
        .voucher-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 20px; margin-top: 20px; }
        .voucher-card { display: flex; border: 2px dashed #ccc; border-radius: 12px; overflow: hidden; background: #fff; transition: transform 0.2s; }
        .voucher-card:hover { transform: translateY(-3px); }
        .voucher-left { background: #f8f9fa; padding: 20px; display: flex; flex-direction: column; align-items: center; justify-content: center; border-right: 2px dashed #ccc; min-width: 100px; }
        .v-amount { font-size: 1.8rem; font-weight: 900; color: #333; }
        .v-off { font-size: 0.7rem; font-weight: bold; color: #666; }
        .voucher-right { padding: 15px; flex: 1; display: flex; flex-direction: column; justify-content: center; }
        .voucher-right h4 { margin: 0 0 5px 0; color: #333; font-size: 1.1rem; }
        .voucher-right p { margin: 0; color: #777; font-size: 0.9rem; }
        .btn-redeem { margin-top: 12px; background: #ff4d6d; color: #fff; border: none; padding: 8px 15px; border-radius: 8px; font-weight: 700; cursor: pointer; align-self: flex-start; }
        .btn-redeem:disabled { background: #ccc; cursor: not-allowed; }

        /* Gold Theme for Premium Rewards */
        .voucher-card.gold { border-color: #ffd700; background: #fffdf2; }
        .voucher-card.gold .voucher-left { background: #fff9db; border-right-color: #ffd700; }
        .voucher-card.gold .v-amount { color: #b8860b; }
        
        .alert { padding: 15px; border-radius: 10px; margin-bottom: 20px; font-weight: 600; display: block; }
    </style>

    <div class="page">
        <div class="card">
            <h2 class="h2">My Rewards</h2>
            <p class="muted">Earn points automatically when you book and review spaces.</p>
            <div class="stat">
                <div class="stat__label">Current Points Balance</div>
                <div class="stat__value"><asp:Label ID="lblPoints" runat="server" Text="0" /></div>
            </div>
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="alert" Visible="false"></asp:Label>

        <div class="card">
            <h3 class="h3" style="margin-bottom:20px;">🎁 Redeem Points for Vouchers</h3>
            <div class="voucher-grid">
                <asp:Repeater ID="rptVoucherShop" runat="server">
                    <ItemTemplate>
                        <div class='<%# "voucher-card " + (Convert.ToInt32(Eval("PointCost")) >= 1000 ? "gold" : "") %>'>
                            <div class="voucher-left">
                                <span class="v-amount">$<%# Eval("DiscountAmount") %></span>
                                <span class="v-off">OFF</span>
                            </div>
                            <div class="voucher-right">
                                <h4><%# Eval("CouponCode") %></h4>
                                <p>Required: <%# Eval("PointCost") %> Points</p>
                                <asp:Button ID="btnRedeem" runat="server" Text="Redeem" CssClass="btn-redeem" 
                                    OnClick="RedeemPoints_Click" 
                                    CommandArgument='<%# Eval("PointCost") + "|" + Eval("DiscountAmount") %>' />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div class="card">
            <h3 class="h3" style="color: #28a745;">🎟️ Ready to Use</h3>
            <asp:GridView ID="gvAvailableVouchers" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None" EmptyDataText="No active vouchers. Redeem some points above!">
                <Columns>
                    <asp:BoundField DataField="RedeemedAt" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
                    <asp:TemplateField HeaderText="Voucher">
                        <ItemTemplate>
                            <strong>$<%# Eval("DiscountAmount") %> Off</strong><br />
                            <code style="color:#0056b3; font-weight:bold; letter-spacing:1px;"><%# Eval("CouponCode") %></code>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span style="background:#d4edda; color:#155724; padding:4px 12px; border-radius:12px; font-size:0.75rem; font-weight:700;">Available</span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <div class="card">
            <h3 class="h3">Points History</h3>
            <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="false" CssClass="table" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="CreatedAt" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                            <span style='<%# Eval("TxnType").ToString() == "Earned" ? "color:#28a745; font-weight:bold;" : "color:#dc3545;" %>'>
                                <%# Eval("TxnType") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Points" HeaderText="Points" />
                    <asp:BoundField DataField="Reference" HeaderText="Details" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>