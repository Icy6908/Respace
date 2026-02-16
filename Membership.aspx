<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Membership.aspx.cs"
    Inherits="Respace.Membership" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page">
        <div class="card">
            <h2 class="h2">Membership</h2>
            <asp:Label ID="lblMsg" runat="server" CssClass="muted" />
        </div>

        <div class="grid-3">
            <div class="card">
                <h3 class="h3">Free</h3>
                <p class="muted">$0 / mo</p>
                <asp:Button ID="btnFree" runat="server" Text="Choose Free" CssClass="btn" OnClick="btnFree_Click" />
            </div>

            <div class="card">
                <h3 class="h3">Plus</h3>
                <p class="muted">$9.90 / mo • 5% off • 1.2× points</p>
                <asp:Button ID="btnPlus" runat="server" Text="Choose Plus" CssClass="btn btn-primary" OnClick="btnPlus_Click" />
            </div>

            <div class="card">
                <h3 class="h3">Pro</h3>
                <p class="muted">$19.90 / mo • 10% off • 1.5× points</p>
                <asp:Button ID="btnPro" runat="server" Text="Choose Pro" CssClass="btn" OnClick="btnPro_Click" />
            </div>
        </div>
    </div>
</asp:Content>
