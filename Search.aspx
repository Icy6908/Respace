<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Search.aspx.cs"
    Inherits="Respace.Search"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body { font-family: Arial, sans-serif; background:#fafafa; }

        .search-wrap { max-width: 980px; margin: 0 auto; }
        .search-bar { display:flex; gap:10px; margin: 10px 0 15px; flex-wrap: wrap; }
        .search-bar input { padding:10px; border-radius:10px; border:1px solid #ddd; }

        .btn { padding:10px 14px; border-radius:10px; border:none; cursor:pointer; }
        .btn-primary { background:#ff5a5f; color:#fff; }
        .btn-secondary { background:#666; color:#fff; }

        .filter-panel {
            background:#fff;
            padding:16px;
            border-radius:14px;
            box-shadow:0 4px 12px rgba(0,0,0,0.08);
            margin-bottom:18px;
        }
        .filter-grid { display:flex; gap:18px; flex-wrap:wrap; }
        .filter-block { min-width: 220px; }
        .filter-title { font-weight:700; margin-bottom:8px; }

        .venue-card {
            background:#fff;
            border-radius:16px;
            box-shadow:0 6px 16px rgba(0,0,0,0.1);
            padding:18px;
            margin-bottom:16px;
        }
        .meta { color:#666; margin-top:4px; }
        .price { color:#ff5a5f; font-weight:700; margin-top:8px; }
        .desc { margin-top:10px; color:#333; }

        .book-link {
            display:inline-block;
            margin-top:12px;
            padding:10px 14px;
            border-radius:12px;
            background:#ffb6c1;
            color:#000;
            text-decoration:none;
            font-weight:700;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="search-wrap">
        <h2>Search Venues</h2>

        <div class="search-bar">
            <asp:TextBox ID="txtSearch" runat="server" Width="300"
                Placeholder="Search by name, location, type..." />

            <asp:Button ID="btnSearch" runat="server" Text="Search"
                CssClass="btn btn-primary"
                OnClick="btnSearch_Click" />

            <asp:Button ID="btnToggleFilter" runat="server" Text="Filter"
                CssClass="btn btn-secondary"
                OnClick="btnToggleFilter_Click" />
        </div>

        <asp:Panel ID="pnlFilter" runat="server" CssClass="filter-panel" Visible="false">
            <div class="filter-grid">
                <div class="filter-block">
                    <div class="filter-title">Location</div>
                    <asp:CheckBoxList ID="cblLocation" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem>North</asp:ListItem>
                        <asp:ListItem>South</asp:ListItem>
                        <asp:ListItem>East</asp:ListItem>
                        <asp:ListItem>West</asp:ListItem>
                        <asp:ListItem>Central</asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div class="filter-block">
                    <div class="filter-title">Type</div>
                    <asp:CheckBoxList ID="cblType" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem>Meeting Room</asp:ListItem>
                        <asp:ListItem>Conference Room</asp:ListItem>
                        <asp:ListItem>Training Room</asp:ListItem>
                        <asp:ListItem>Event Hall</asp:ListItem>
                        <asp:ListItem>Studio</asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div class="filter-block">
                    <div class="filter-title">Price Range ($/hour)</div>
                    Min:
                    <asp:TextBox ID="txtMinPrice" runat="server" TextMode="Number" Width="90" />
                    &nbsp; Max:
                    <asp:TextBox ID="txtMaxPrice" runat="server" TextMode="Number" Width="90" />
                </div>

                <div class="filter-block">
                    <div class="filter-title">Availability (optional)</div>
                    From:
                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" />
                    <br />
                    To:
                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" />
                    <div style="color:#777; font-size:0.9em; margin-top:6px;">
                        (Shows spaces with no confirmed bookings overlapping this date range)
                    </div>
                </div>

                <div class="filter-block">
                    <div class="filter-title">Sort By</div>
                    <asp:DropDownList ID="ddlSort" runat="server">
                        <asp:ListItem Value="">-- Select --</asp:ListItem>
                        <asp:ListItem Value="price_asc">Price: Low to High</asp:ListItem>
                        <asp:ListItem Value="price_desc">Price: High to Low</asp:ListItem>
                        <asp:ListItem Value="date_asc">Newest: Oldest</asp:ListItem>
                        <asp:ListItem Value="date_desc">Oldest: Newest</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <br />
            <asp:Button ID="btnApplyFilter" runat="server" Text="Apply Filter"
                CssClass="btn btn-primary"
                OnClick="btnApplyFilter_Click" />

            <asp:Button ID="btnClearFilter" runat="server" Text="Clear All"
                CssClass="btn btn-secondary"
                OnClick="btnClearFilter_Click" />
        </asp:Panel>

        <asp:Repeater ID="rptSpaces" runat="server">
            <ItemTemplate>
                <div class="venue-card">
                    <h3><%# Eval("Name") %></h3>
                    <div class="meta">
                        <%# Eval("Location") %> • <%# Eval("Type") %> • Capacity: <%# Eval("Capacity") %>
                    </div>
                    <div class="price">$<%# Eval("PricePerHour", "{0:0.00}") %> / hour</div>
                    <div class="desc"><%# Eval("Description") %></div>

                    <a class="book-link" href='<%# "SpaceDetails.aspx?id=" + Eval("SpaceId") %>'>
                        View / Book
                    </a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

</asp:Content>
