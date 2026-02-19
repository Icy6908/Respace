<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Search.aspx.cs"
    Inherits="Respace.Search"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
            background: #fafafa;
        }

        .wrap {
            max-width: 1100px;
            margin: 0 auto;
            padding: 0 16px;
        }

        /* ===== HERO ===== */
        .hero {
            padding: 34px 0 18px;
            border-bottom: 1px solid #eee;
            background: radial-gradient(900px 360px at 18% 10%, rgba(255,56,92,.18), transparent 60%),
                        radial-gradient(900px 360px at 82% 30%, rgba(0,140,255,.12), transparent 55%),
                        #fff;
        }

        .hero__inner {
            display: grid;
            grid-template-columns: 1.2fr 0.8fr;
            gap: 18px;
            align-items: center;
        }

        @media (max-width: 900px) {
            .hero__inner { grid-template-columns: 1fr; }
        }

        .hero__title {
            margin: 0;
            font-size: 40px;
            line-height: 1.05;
        }

        .hero__subtitle {
            margin: 10px 0 0;
            color: #666;
            font-size: 15px;
        }

        .card {
            background: #fff;
            border-radius: 18px;
            box-shadow: 0 10px 26px rgba(0,0,0,.08);
        }

        /* ===== SEARCH BAR ===== */
        .searchCard {
            margin-top: 16px;
            padding: 14px;
        }

        .searchRow {
            display: grid;
            grid-template-columns: 1fr auto auto;
            gap: 10px;
            align-items: end;
        }

        @media (max-width: 760px) {
            .searchRow { grid-template-columns: 1fr; }
        }

        .label {
            display: block;
            font-weight: 800;
            margin-bottom: 6px;
        }

        .input {
            width: 100%;
            padding: 12px 14px;
            border-radius: 14px;
            border: 1px solid #e3e3e3;
            outline: none;
            transition: box-shadow .12s, border-color .12s;
            background: #fff;
            box-sizing: border-box;
        }

        .input:focus {
            border-color: rgba(255,56,92,.45);
            box-shadow: 0 0 0 4px rgba(255,56,92,.14);
        }

        .btn {
            padding: 12px 16px;
            border-radius: 14px;
            border: none;
            cursor: pointer;
            font-weight: 800;
        }

        .btn-primary {
            background: #ff385c;
            color: #fff;
        }

        .btn-secondary {
            background: #111;
            color: #fff;
            opacity: .9;
        }

        .btn-secondary:hover { opacity: 1; }

        .chips {
            margin-top: 10px;
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }

        .chip {
            border: 1px solid #ececec;
            background: #fff;
            padding: 8px 12px;
            border-radius: 999px;
            cursor: pointer;
            font-weight: 800;
        }

        .chip:hover { background: #f7f7f7; }

        /* right preview */
        .preview {
            padding: 16px;
            border-radius: 18px;
            background: rgba(255,255,255,.75);
            backdrop-filter: blur(6px);
            border: 1px solid rgba(0,0,0,.06);
        }

        .preview__badge {
            display: inline-block;
            padding: 6px 10px;
            border-radius: 999px;
            background: rgba(255,56,92,.12);
            color: #ff385c;
            font-weight: 900;
            font-size: 12px;
        }

        .preview__title {
            margin: 10px 0 4px;
            font-weight: 900;
            font-size: 16px;
        }

        .muted { color: #777; }

        /* ===== FILTER PANEL ===== */
        .filter-panel {
            background: #fff;
            padding: 16px;
            border-radius: 18px;
            box-shadow: 0 10px 26px rgba(0,0,0,0.08);
            margin: 18px 0;
        }

        /* ===== RESULTS ===== */
        .resultsHead {
            display: flex;
            justify-content: space-between;
            align-items: flex-end;
            gap: 12px;
            margin: 18px 0 10px;
        }

        .resultsTitle {
            margin: 0;
            font-size: 18px;
        }

        /* ==========================
           CONSISTENT CARD GRID FIX
           ========================== */
        .gridCards {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 16px;
            margin: 16px 0 40px;
            align-items: stretch; /* important */
        }

        @media (max-width:1100px) { .gridCards { grid-template-columns: repeat(3, 1fr); } }
        @media (max-width:860px)  { .gridCards { grid-template-columns: repeat(2, 1fr); } }
        @media (max-width:520px)  { .gridCards { grid-template-columns: 1fr; } }

        /* anchor fills grid cell */
        .cardLink {
            text-decoration: none;
            color: inherit;
            display: block;
            height: 100%;
        }

        /* card has fixed height -> every card consistent across ALL rows */
        .listingCard {
            background: #fff;
            border-radius: 18px;
            overflow: hidden;
            box-shadow: 0 10px 26px rgba(0,0,0,.08);
            transition: transform .12s ease, box-shadow .12s ease;

            display: flex;
            flex-direction: column;
            /*height: 420px;*/ /* ✅ adjust if you want taller/shorter */
        }

        .listingCard:hover {
            transform: translateY(-2px);
            box-shadow: 0 14px 34px rgba(0,0,0,.12);
        }

        /* image area always same height */
        .thumb {
            position: relative;
            width: 100%;
            height: 220px; /* ✅ consistent image height */
            background: #f3f3f3;
        }

        .thumb img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            display: block;
        }

        .badgeType {
            position: absolute;
            left: 10px;
            top: 10px;
            background: rgba(255,255,255,.92);
            border-radius: 999px;
            padding: 6px 10px;
            font-weight: 900;
            font-size: 12px;
        }

        .cardBody {
            padding: 12px 14px 14px;
            display: flex;
            flex-direction: column;
            flex: 1;
        }

        /* Reserve exactly 2 lines height (cross-browser reliable) */
        .cardTitle {
            font-weight: 900;
            font-size: 15px;
            margin: 0;

            line-height: 1.25;
            height: 2.5em; /* ~2 lines */
            overflow: hidden;

            /* keep clamp for browsers that support it */
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
        }

        /* reserve a stable line so layout doesn't shift */
        .cardMeta {
            color: #666;
            margin-top: 4px;
            font-size: 13px;
            min-height: 1.2em;
        }

        .cardPriceRow {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 10px;
            margin-top: auto; /* ✅ stick to bottom */
        }

        .cardPrice { font-weight: 900; }
        .stars { color: #ff385c; letter-spacing: 1px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- ===== HERO ===== -->
    <section class="hero">
        <div class="wrap">
            <div class="hero__inner">
                <div>
                    <h1 class="hero__title">Book spaces like a pro.</h1>
                    <p class="hero__subtitle">
                        Search meeting rooms, studios, event halls and more — compare prices, capacity, and reviews.
                    </p>

                    <div class="card searchCard">
                        <div class="searchRow">
                            <div>
                                <label class="label" for="<%= txtSearch.ClientID %>">Where / what are you looking for?</label>
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="input"
                                    Placeholder="Search by name, location, type..." />
                            </div>

                            <asp:Button ID="btnSearch" runat="server" Text="Search"
                                CssClass="btn btn-primary"
                                OnClick="btnSearch_Click" />

                            <asp:Button ID="btnToggleFilter" runat="server" Text="Filters"
                                CssClass="btn btn-secondary"
                                OnClick="btnToggleFilter_Click" />
                        </div>

                        <div class="chips">
                            <button type="button" class="chip" data-fill="Meeting Room">Meeting Room</button>
                            <button type="button" class="chip" data-fill="Studio">Studio</button>
                            <button type="button" class="chip" data-fill="Event Hall">Event Hall</button>
                            <button type="button" class="chip" data-fill="Central">Central</button>
                            <button type="button" class="chip" data-fill="North">North</button>
                        </div>
                    </div>
                </div>

                <div class="preview">
                    <span class="preview__badge">Popular this week</span>
                    <div class="preview__title">Creative Studio</div>
                    <div class="muted">Central • 10–15 pax • from $35/hr</div>
                    <div style="height: 10px"></div>
                    <div class="muted">Tip: try “studio” or “meeting room” to see quick results.</div>
                </div>
            </div>
        </div>
    </section>

    <div class="wrap">

        <!-- ===== FILTER PANEL ===== -->
        <asp:Panel ID="pnlFilter" runat="server" CssClass="filter-panel" Visible="false">
            <div style="margin-bottom:10px;font-weight:800;">Filters</div>

            <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px;">
                <div>
                    <div style="font-weight:800;margin-bottom:6px;">Location</div>
                    <asp:CheckBoxList ID="cblLocation" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem>North</asp:ListItem>
                        <asp:ListItem>South</asp:ListItem>
                        <asp:ListItem>East</asp:ListItem>
                        <asp:ListItem>West</asp:ListItem>
                        <asp:ListItem>Central</asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div>
                    <div style="font-weight:800;margin-bottom:6px;">Type</div>
                    <asp:CheckBoxList ID="cblType" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem>Meeting Room</asp:ListItem>
                        <asp:ListItem>Conference Room</asp:ListItem>
                        <asp:ListItem>Training Room</asp:ListItem>
                        <asp:ListItem>Event Hall</asp:ListItem>
                        <asp:ListItem>Studio</asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div>
                    <div style="font-weight:800;margin-bottom:6px;">Price Range ($/hour)</div>
                    Min:
                    <asp:TextBox ID="txtMinPrice" runat="server" TextMode="Number" Width="90" CssClass="input" />
                    &nbsp; Max:
                    <asp:TextBox ID="txtMaxPrice" runat="server" TextMode="Number" Width="90" CssClass="input" />
                </div>

                <div>
                    <div style="font-weight:800;margin-bottom:6px;">Availability (optional)</div>
                    From:
                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="input" />
                    <br />
                    To:
                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="input" />
                    <div class="muted" style="font-size: 0.9em; margin-top: 6px;">
                        (Shows spaces with no confirmed bookings overlapping this date range)
                    </div>
                </div>

                <div>
                    <div style="font-weight:800;margin-bottom:6px;">Sort By</div>
                    <asp:DropDownList ID="ddlSort" runat="server" CssClass="input">
                        <asp:ListItem Value="">-- Select --</asp:ListItem>
                        <asp:ListItem Value="price_asc">Price: Low to High</asp:ListItem>
                        <asp:ListItem Value="price_desc">Price: High to Low</asp:ListItem>
                        <asp:ListItem Value="date_asc">Newest: Oldest</asp:ListItem>
                        <asp:ListItem Value="date_desc">Oldest: Newest</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div style="height: 12px"></div>

            <asp:Button ID="btnApplyFilter" runat="server" Text="Apply Filter"
                CssClass="btn btn-primary"
                OnClick="btnApplyFilter_Click" />

            <asp:Button ID="btnClearFilter" runat="server" Text="Clear All"
                CssClass="btn btn-secondary"
                OnClick="btnClearFilter_Click" />
        </asp:Panel>

        <!-- ===== RESULTS ===== -->
        <div class="resultsHead">
            <h2 class="resultsTitle">Available spaces</h2>
            <div class="muted">Scroll to explore • Use filters to refine</div>
        </div>

        <div class="gridCards">
            <asp:Repeater ID="rptSpaces" runat="server">
                <ItemTemplate>
                    <a class="cardLink" href='<%# "SpaceDetails.aspx?id=" + Eval("SpaceId") %>'>
                        <div class="listingCard">
                            <div class="thumb">
                                <div class="badgeType"><%# Eval("Type") %></div>

                                <!-- Optional fallback for broken images:
                                     Put a placeholder at /Images/placeholder.jpg -->
                                <img alt="Space photo"
                                     src='<%# GetListingImage(Eval("CoverPhotoUrl")) %>'
                                     onerror="this.onerror=null;this.src='Images/placeholder.jpg';" />
                            </div>

                            <div class="cardBody">
                                <p class="cardTitle" title='<%# Eval("Name") %>'><%# Eval("Name") %></p>

                                <div class="cardMeta">
                                    <%# Eval("Location") %> • <%# Eval("Capacity") %> pax
                                </div>

                                <div class="cardPriceRow">
                                    <div class="cardPrice">
                                        $<%# Eval("PricePerHour", "{0:0.00}") %>
                                        <span style="font-weight: 600; color: #666">/ hr</span>
                                    </div>

                                    <div class="cardMeta">
                                        <span class="stars"><%# GetStars(Convert.ToInt32(Eval("AvgRating"))) %></span>
                                        <span>(<%# Eval("ReviewCount") %>)</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <script>
        // Chips fill the search box
        (function () {
            var input = document.getElementById('<%= txtSearch.ClientID %>');
            if (!input) return;

            document.querySelectorAll('.chip[data-fill]').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    input.value = btn.getAttribute('data-fill');
                    input.focus();
                });
            });
        })();
    </script>

</asp:Content>
