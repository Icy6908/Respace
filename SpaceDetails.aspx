<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SpaceDetails.aspx.cs"
    Inherits="Respace.SpaceDetails" MasterPageFile="~/Site.Master" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>

    <style>
        .sd-page {
            padding: 12px 0;
        }

        .sd-wrap {
            max-width: 1100px;
            margin: 0 auto;
            padding: 0 14px;
        }

        .alert {
            display: block;
            margin: 10px 0 14px;
            padding: 10px 12px;
            border-radius: 10px;
        }

            .alert.error {
                background: #ffe9ea;
                color: #a30014;
                border: 1px solid #ffc9cf;
            }

            .alert.success {
                background: #e9fff1;
                color: #0b6b2a;
                border: 1px solid #b9f2cd;
            }

        .card {
            background: #fff;
            border-radius: 16px;
            padding: 16px;
            box-shadow: 0 6px 18px rgba(0,0,0,0.08);
        }

        .muted {
            color: #777;
        }

        .small {
            font-size: 0.92rem;
        }

        .center {
            text-align: center;
        }

        .listing__header {
            margin-bottom: 14px;
        }

        .listing__title {
            margin: 0;
            font-size: 28px;
            font-weight: 800;
        }

        .listing__sub {
            margin-top: 6px;
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            align-items: center;
        }

        .dot {
            color: #999;
        }

        /* Photo gallery */
        .photo-grid {
            display: grid;
            grid-template-columns: 2fr 1fr 1fr;
            grid-template-rows: 160px 160px;
            gap: 10px;
            margin: 10px 0 18px;
            position: relative;
        }

        .photo-grid__big, .photo-grid__small {
            border-radius: 16px;
            background: #eee;
            overflow: hidden;
        }

        .photo-grid__big {
            grid-row: 1 / span 2;
        }

        .photo-grid img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            display: block;
        }

        .photo-grid__action {
            position: absolute;
            right: 14px;
            bottom: 14px;
            background: #fff;
            padding: 10px 12px;
            border-radius: 12px;
            font-weight: 700;
            box-shadow: 0 6px 18px rgba(0,0,0,0.12);
        }

        .photo-empty {
            height: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #777;
            font-weight: 700;
        }

        .listing__body {
            display: grid;
            grid-template-columns: 2fr 1fr;
            gap: 18px;
            align-items: start;
        }

        @media (max-width: 980px) {
            .listing__body {
                grid-template-columns: 1fr;
            }
        }

        .section {
            margin-bottom: 16px;
        }

        .h3 {
            margin: 0 0 10px;
            font-size: 20px;
            font-weight: 800;
        }

        .text {
            margin: 0;
            line-height: 1.5;
        }

        .section-tabs {
            display: flex;
            gap: 14px;
            margin: 0 0 12px;
            flex-wrap: wrap;
        }

        .section-tabs__link {
            text-decoration: none;
            font-weight: 700;
            color: #222;
            padding: 8px 10px;
            border-radius: 10px;
            background: #f3f3f3;
        }

        .features {
            display: flex;
            gap: 16px;
            flex-wrap: wrap;
            margin-top: 12px;
        }

        .feature {
            min-width: 170px;
        }

        .feature__title {
            font-weight: 800;
            margin-bottom: 4px;
        }

        .amenities {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 10px;
            margin-top: 10px;
        }

        @media (max-width: 600px) {
            .amenities {
                grid-template-columns: 1fr;
            }
        }

        .amenity {
            border: 1px solid #eee;
            border-radius: 14px;
            padding: 10px 12px;
            display: flex;
            gap: 10px;
            align-items: center;
        }

        .amenity__icon {
            width: 34px;
            height: 34px;
            border-radius: 12px;
            background: #f3f3f3;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 900;
            color: #444;
        }

        .amenity__name {
            font-weight: 700;
            color: #222;
        }

        .map-frame {
            width: 100%;
            height: 260px;
            border: 0;
            border-radius: 14px;
            overflow: hidden;
        }

        .map-box {
            margin-top: 10px;
        }

        .avail-cal {
            margin-top: 10px;
        }

        .flatpickr-calendar.inline {
            box-shadow: none;
            border: 1px solid #eee;
            border-radius: 14px;
            overflow: hidden;
        }

        .reviews-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }

        .review {
            border-top: 1px solid #eee;
            padding-top: 12px;
            margin-top: 12px;
        }

        .review__top {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .review__name {
            font-weight: 800;
        }

        .review__stars {
            color: #ff385c;
            letter-spacing: 1px;
        }

        .review__comment {
            margin-top: 6px;
        }

        .booking-card {
            position: sticky;
            top: 14px;
        }

        .booking-card__price {
            display: flex;
            justify-content: space-between;
            align-items: baseline;
            margin-bottom: 12px;
        }

        .price__amt {
            font-weight: 900;
            font-size: 22px;
        }

        .booking-box {
            border: 1px solid #eee;
            border-radius: 14px;
            overflow: hidden;
            margin-bottom: 12px;
        }

        .booking-box__row {
            display: grid;
            grid-template-columns: 1fr 1fr;
        }

        .booking-field {
            padding: 10px 12px;
        }

            .booking-field + .booking-field {
                border-left: 1px solid #eee;
            }

        .booking-field__label {
            font-size: 12px;
            font-weight: 900;
            letter-spacing: .5px;
            margin-bottom: 6px;
            color: #333;
        }

        .input {
            width: 100%;
            padding: 10px 10px;
            border-radius: 10px;
            border: 1px solid #ddd;
            outline: none;
            background: #fff;
        }

        .select {
            width: 100%;
            padding: 10px;
            border-radius: 10px;
            border: 1px solid #ddd;
            background: #fff;
        }

        .cal-wrap {
            border: 1px solid #eee;
            border-radius: 14px;
            padding: 10px;
        }

        .cal-actions {
            margin-top: 8px;
        }

        .link {
            color: #111;
            text-decoration: underline;
            cursor: pointer;
        }

        .btn {
            padding: 10px 14px;
            border-radius: 12px;
            border: none;
            cursor: pointer;
            font-weight: 800;
        }

        .btn-primary {
            background: #ff5a5f;
            color: #fff;
        }

        .btn-outline {
            background: #fff;
            color: #111;
            border: 1px solid #ddd;
        }

        .btn-block {
            width: 100%;
        }

        .reviewMeta {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            align-items: center;
            margin-top: 6px;
        }

        .badgePill {
            display: inline-block;
            padding: 4px 10px;
            border: 1px solid #e5e5e5;
            border-radius: 999px;
            background: #f8f8f8;
            font-weight: 700;
            font-size: 0.85em;
            line-height: 1.2;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="sd-page">
        <div class="sd-wrap">

            <asp:Label ID="lblMsg" runat="server" CssClass="alert" />

            <!-- Calendar disable lists -->
            <asp:HiddenField ID="hfBookedDates" runat="server" />
            <asp:HiddenField ID="hfBlockedDates" runat="server" />

            <!-- Postback-safe selected dates -->
            <asp:HiddenField ID="hfStart" runat="server" />
            <asp:HiddenField ID="hfEnd" runat="server" OnValueChanged="hfEnd_ValueChanged" />

            <asp:Panel ID="pnlDetails" runat="server" Visible="false">

                <!-- Title -->
                <div class="listing__header">
                    <h1 class="listing__title">
                        <asp:Label ID="lblName" runat="server" />
                    </h1>

                    <div class="listing__sub">
                        <span class="muted">
                            <asp:Label ID="lblLocation" runat="server" /></span>
                        <span class="dot">•</span>
                        <span class="muted">
                            <asp:Label ID="lblType" runat="server" /></span>
                        <span class="dot">•</span>
                        <span class="muted">Capacity:
                            <asp:Label ID="lblCap" runat="server" /></span>
                    </div>
                </div>

                <!-- ✅ Photo gallery (cover + 4 thumbnails) -->
                <div class="photo-grid">
                    <div class="photo-grid__big">
                        <asp:Image ID="imgCover" runat="server" Visible="false" />
                        <asp:Panel ID="pnlNoPhotos" runat="server" CssClass="photo-empty" Visible="false">
                            No photos yet
                        </asp:Panel>
                    </div>

                    <div class="photo-grid__small">
                        <asp:Image ID="img1" runat="server" Visible="false" />
                    </div>
                    <div class="photo-grid__small">
                        <asp:Image ID="img2" runat="server" Visible="false" />
                    </div>
                    <div class="photo-grid__small">
                        <asp:Image ID="img3" runat="server" Visible="false" />
                    </div>
                    <div class="photo-grid__small">
                        <asp:Image ID="img4" runat="server" Visible="false" />
                    </div>

                    <div class="photo-grid__action">Show all photos</div>
                </div>

                <div class="listing__body">

                    <!-- LEFT -->
                    <div class="listing__left">

                        <div class="section-tabs">
                            <a class="section-tabs__link" href="#about">About</a>
                            <a class="section-tabs__link" href="#offers">What this place offers</a>
                            <a class="section-tabs__link" href="#availability">Availability</a>
                            <a class="section-tabs__link" href="#location">Location</a>
                            <a class="section-tabs__link" href="#reviews">Reviews</a>
                        </div>

                        <!-- About -->
                        <section id="about" class="section card">
                            <h2 class="h3">About this space</h2>
                            <p class="text">
                                <asp:Label ID="lblDesc" runat="server" />
                            </p>

                            <div class="features">
                                <div class="feature">
                                    <div class="feature__title">Type</div>
                                    <div class="muted">
                                        <asp:Label ID="lblType2" runat="server" />
                                    </div>
                                </div>
                                <div class="feature">
                                    <div class="feature__title">Capacity</div>
                                    <div class="muted">
                                        <asp:Label ID="lblCap2" runat="server" />
                                        people
                                    </div>
                                </div>
                                <div class="feature">
                                    <div class="feature__title">Rate</div>
                                    <div class="muted">
                                        $<asp:Label ID="lblPrice2" runat="server" />
                                        / day
                                    </div>
                                </div>
                            </div>
                        </section>

                        <!-- ✅ Amenities -->
                        <section id="offers" class="section card">
                            <h2 class="h3">What this place offers</h2>
                            <asp:Repeater ID="rptAmenities" runat="server">
                                <HeaderTemplate>
                                    <div class="amenities">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="amenity">
                                        <div class="amenity__icon"><%# Eval("IconText") %></div>
                                        <div class="amenity__name"><%# Eval("AmenityName") %></div>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>

                            <asp:Panel ID="pnlNoAmenities" runat="server" Visible="false" CssClass="muted small">
                                No amenities listed yet.
                            </asp:Panel>
                        </section>

                        <!-- Availability -->
                        <section id="availability" class="section card">
                            <h2 class="h3">Availability</h2>
                            <div class="muted">
                                Grey days are unavailable (already booked or blocked by host).
                                Use the booking panel on the right to select dates.
                            </div>

                            <div class="avail-cal">
                                <div id="calAvail"></div>
                            </div>
                        </section>

                        <!-- ✅ Map / Address -->
                        <section id="location" class="section card">
                            <h2 class="h3">Location</h2>
                            <div class="muted">
                                <asp:Label ID="lblAddress" runat="server" />
                            </div>

                            <div class="map-box">
                                <asp:Literal ID="litMap" runat="server" />
                            </div>
                        </section>

                        <!-- Reviews -->
                        <section id="reviews" class="section card">
                            <div class="reviews-header">
                                <h2 class="h3">Reviews</h2>
                                <a id="lnkReview" runat="server" class="btn btn-outline" href="#">Write a review</a>
                            </div>

                            <asp:Repeater ID="rptReviews" runat="server">
                                <ItemTemplate>
                                    <div class="review">
                                        <div class="review__top">
                                            <div class="review__name"><%# Eval("GuestName") %></div>
                                            <div class="review__stars"><%# Eval("Stars") %></div>
                                        </div>
                                        <div class="review__comment"><%# Eval("Comment") %></div>
                                        <div class="reviewMeta">
                                            <span class="reviewDate muted"><%# Convert.ToDateTime(Eval("CreatedAt")).ToString("dd MMM yyyy") %></span>

                                            <asp:Repeater runat="server"
                                                DataSource='<%# GetBadgesWithEmoji((Eval("Badges") ?? "").ToString()) %>'>
                                                <ItemTemplate>
                                                    <span class="badgePill"><%# Container.DataItem %></span>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>

                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </section>

                    </div>

                    <!-- RIGHT -->
                    <aside class="listing__right">
                        <div class="booking-card card">

                            <div class="booking-card__price">
                                <div class="price">
                                    <span class="price__amt">$<asp:Label ID="lblPrice" runat="server" /></span>
                                    <span class="muted">/ day</span>
                                </div>
                                <div class="muted small">Prices include all fees</div>
                            </div>

                            <!-- ✅ Guest count -->
                            <div class="booking-field" style="padding: 0 0 12px;">
                                <div class="booking-field__label">GUESTS</div>
                                <asp:DropDownList ID="ddlGuests" runat="server" CssClass="select" />
                                <div class="muted small" style="margin-top: 6px;">
                                    Max:
                                    <asp:Label ID="lblCap3" runat="server" />
                                </div>
                            </div>

                            <!-- Selected dates -->
                            <div class="booking-box">
                                <div class="booking-box__row">
                                    <div class="booking-field">
                                        <div class="booking-field__label">CHECK-IN</div>
                                        <asp:TextBox ID="txtStartDate" runat="server"
                                            ClientIDMode="Static"
                                            CssClass="input"
                                            ReadOnly="true" />
                                    </div>
                                    <div class="booking-field">
                                        <div class="booking-field__label">CHECK-OUT</div>
                                        <asp:TextBox ID="txtEndDate" runat="server"
                                            ClientIDMode="Static"
                                            CssClass="input"
                                            ReadOnly="true" />
                                    </div>
                                </div>
                            </div>

                            <div class="cal-wrap">
                                <div id="calBook"></div>
                                <div class="cal-actions">
                                    <a href="javascript:void(0)" id="btnClearDates" class="link">Clear dates</a>
                                </div>
                            </div>

                            <div class="muted small" style="margin: 10px 2px 14px">
                                <span id="jsNights">0</span> night(s) • Estimated total:
                                <strong id="jsTotal">$0.00</strong>
                            </div>

                            <asp:Button ID="btnBook" runat="server" Text="Reserve"
                                CssClass="btn btn-primary btn-block" OnClick="btnBook_Click" />

                            <div class="muted small center" style="margin-top: 10px">You won’t be charged yet</div>

                        </div>
                    </aside>

                </div>

            </asp:Panel>

        </div>
    </div>

    <script>
        (function () {
            var rawBooked = document.getElementById("<%= hfBookedDates.ClientID %>").value || "";
            var rawBlocked = document.getElementById("<%= hfBlockedDates.ClientID %>").value || "";

            var bookedDays = rawBooked.split(",").map(x => x.trim()).filter(Boolean);
            var blockedDays = rawBlocked.split(",").map(x => x.trim()).filter(Boolean);

            var disabled = bookedDays.concat(blockedDays);

            var startBox = document.getElementById("txtStartDate");
            var endBox = document.getElementById("txtEndDate");

            var hfStart = document.getElementById("<%= hfStart.ClientID %>");
            var hfEnd = document.getElementById("<%= hfEnd.ClientID %>");

            var price = parseFloat("<%= lblPrice.Text %>") || 0;

            function updateSummary() {
                var s = (hfStart && hfStart.value) ? hfStart.value : "";
                var e = (hfEnd && hfEnd.value) ? hfEnd.value : "";
                var nights = 0;

                if (s && e) {
                    var sd = new Date(s + "T00:00:00");
                    var ed = new Date(e + "T00:00:00");
                    nights = Math.floor((ed - sd) / (1000 * 60 * 60 * 24));
                    if (nights < 0) nights = 0;
                }

                var nightsEl = document.getElementById("jsNights");
                var totalEl = document.getElementById("jsTotal");
                if (nightsEl) nightsEl.innerText = nights;
                if (totalEl) totalEl.innerText = "$" + (price * nights).toFixed(2);
            }

            flatpickr("#calAvail", {
                inline: true,
                mode: "single",
                minDate: "today",
                showMonths: 2,
                dateFormat: "Y-m-d",
                disable: disabled,
                clickOpens: false,
                onChange: function (selectedDates, dateStr, instance) {
                    instance.clear();
                }
            });

            var fpBook = flatpickr("#calBook", {
                inline: true,
                mode: "range",
                minDate: "today",
                showMonths: 1,
                dateFormat: "Y-m-d",
                disable: disabled,
                onChange: function (selectedDates, dateStr, instance) {
                    if (selectedDates.length >= 1) {
                        var s = instance.formatDate(selectedDates[0], "Y-m-d");
                        if (startBox) startBox.value = s;
                        if (hfStart) hfStart.value = s;
                    } else {
                        if (startBox) startBox.value = "";
                        if (hfStart) hfStart.value = "";
                    }

                    if (selectedDates.length >= 2) {
                        var e = instance.formatDate(selectedDates[1], "Y-m-d");
                        if (endBox) endBox.value = e;
                        if (hfEnd) hfEnd.value = e;
                    } else {
                        if (endBox) endBox.value = "";
                        if (hfEnd) hfEnd.value = "";
                    }

                    updateSummary();
                }
            });

            var clearBtn = document.getElementById("btnClearDates");
            if (clearBtn) {
                clearBtn.addEventListener("click", function () {
                    fpBook.clear();
                    if (startBox) startBox.value = "";
                    if (endBox) endBox.value = "";
                    if (hfStart) hfStart.value = "";
                    if (hfEnd) hfEnd.value = "";
                    updateSummary();
                });
            }

            updateSummary();
        })();
    </script>

</asp:Content>
