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

        <div class="card">
            <h3 class="h3">Points History</h3>

            <asp:GridView ID="gvTransactions" runat="server"
                AutoGenerateColumns="false"
                CssClass="table"
                GridLines="None">

                <Columns>
                    <asp:BoundField DataField="TxnType" HeaderText="Type" />
                    <asp:BoundField DataField="Points" HeaderText="Points" />
                    <asp:BoundField DataField="Reference" HeaderText="Reference" />
                    <asp:BoundField DataField="CreatedAt"
                        HeaderText="Date"
                        DataFormatString="{0:dd MMM yyyy HH:mm}" />
                </Columns>

            </asp:GridView>

        </div>

    </div>

</asp:Content>
