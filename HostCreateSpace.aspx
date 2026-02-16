<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HostCreateSpace.aspx.cs"
    Inherits="Respace.HostCreateSpace" MasterPageFile="~/Site.Master" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .wrap { max-width: 980px; margin: 0 auto; padding: 14px; }
        .card { background:#fff; border-radius:16px; padding:16px; box-shadow:0 6px 18px rgba(0,0,0,0.08); margin-bottom:16px; }
        .h2 { margin:0 0 6px; font-size:26px; font-weight:900; }
        .h3 { margin:0 0 10px; font-size:20px; font-weight:900; }
        .muted { color:#777; }
        .small { font-size:0.92rem; }

        .alert { display:block; margin: 10px 0 14px; padding: 10px 12px; border-radius: 10px; }
        .alert.error { background:#ffe9ea; color:#a30014; border:1px solid #ffc9cf; }
        .alert.success { background:#e9fff1; color:#0b6b2a; border:1px solid #b9f2cd; }

        .grid { display:grid; grid-template-columns: 1fr 1fr; gap: 12px; }
        @media (max-width: 820px) { .grid { grid-template-columns: 1fr; } }

        .field { margin-bottom: 12px; }
        .label { font-weight: 900; margin-bottom: 6px; display:block; }
        .input, .select, textarea { width:100%; padding:10px; border-radius:12px; border:1px solid #ddd; outline:none; }
        textarea { min-height: 110px; resize: vertical; }

        .amenities { display:grid; grid-template-columns: repeat(2, minmax(0,1fr)); gap: 6px 12px; }
        @media (max-width: 600px) { .amenities { grid-template-columns: 1fr; } }
        .amenities label { font-weight:700; }

        .btn { padding:10px 14px; border-radius: 12px; border:none; cursor:pointer; font-weight:900; }
        .btn-primary { background:#ff5a5f; color:#fff; }
        .btn-outline { background:#fff; color:#111; border:1px solid #ddd; }
        .btn-danger { background:#fff; color:#b10016; border:1px solid #ffc9cf; }
        .btn-row { display:flex; gap:10px; flex-wrap:wrap; }

        .photos-grid { display:grid; grid-template-columns: repeat(4, minmax(0,1fr)); gap: 10px; margin-top: 10px; }
        @media (max-width: 820px) { .photos-grid { grid-template-columns: repeat(2, minmax(0,1fr)); } }
        .photo-card { border:1px solid #eee; border-radius: 14px; overflow:hidden; }
        .photo-card img { width:100%; height:140px; object-fit:cover; display:block; background:#f2f2f2; }
        .photo-card__meta { padding: 10px; display:flex; justify-content:space-between; align-items:center; gap: 8px; }
        .pill { display:inline-block; padding:6px 10px; border-radius:999px; background:#f3f3f3; font-weight:900; font-size: 12px; }
        .link { text-decoration: underline; font-weight:800; cursor:pointer; border:none; background:transparent; padding:0; }
        .danger { color:#b10016; }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="wrap">

        <asp:Label ID="lblMsg" runat="server" />

        <div class="card">
            <div class="h2">
                <asp:Label ID="lblTitle" runat="server" Text="Create listing" />
            </div>
            <div class="muted small">
                Add details, amenities, photos and address. When ready, click <b>Complete listing creation</b> to review everything on a confirmation page.
            </div>
        </div>

        <div class="card">
            <h3 class="h3">Basic info</h3>

            <div class="grid">
                <div class="field">
                    <span class="label">Name</span>
                    <asp:TextBox ID="txtName" runat="server" CssClass="input" />
                </div>

                <div class="field">
                    <span class="label">Location (Region)</span>
                    <asp:DropDownList ID="ddlLocation" runat="server" CssClass="select">
                        <asp:ListItem>North</asp:ListItem>
                        <asp:ListItem>South</asp:ListItem>
                        <asp:ListItem>East</asp:ListItem>
                        <asp:ListItem>West</asp:ListItem>
                        <asp:ListItem>Central</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="field">
                    <span class="label">Type</span>
                    <asp:DropDownList ID="ddlType" runat="server" CssClass="select">
                        <asp:ListItem>Meeting Room</asp:ListItem>
                        <asp:ListItem>Conference Room</asp:ListItem>
                        <asp:ListItem>Training Room</asp:ListItem>
                        <asp:ListItem>Event Hall</asp:ListItem>
                        <asp:ListItem>Studio</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="field">
                    <span class="label">Capacity (max guests)</span>
                    <asp:TextBox ID="txtCapacity" runat="server" CssClass="input" TextMode="Number" />
                </div>

                <div class="field">
                    <span class="label">Price per day</span>
                    <asp:TextBox ID="txtPrice" runat="server" CssClass="input" TextMode="Number" />
                </div>

                <div class="field">
                    <span class="label">Status</span>
                    <asp:Label ID="lblStatus" runat="server" CssClass="pill" Text="Draft" />
                    <div class="muted small" style="margin-top:6px;">
                        <asp:Label ID="lblStatusHint" runat="server" Text="(Draft is not visible in Search until submitted)" />
                    </div>
                </div>
            </div>

            <div class="field">
                <span class="label">Description</span>
                <asp:TextBox ID="txtDesc" runat="server" TextMode="MultiLine" />
            </div>
        </div>

        <div class="card">
            <h3 class="h3">What this place offers</h3>
            <div class="muted small">Select amenities to show on the listing.</div>

            <div style="margin-top:10px;">
                <asp:CheckBoxList ID="cblAmenities" runat="server" CssClass="amenities" RepeatLayout="Table" />
            </div>
        </div>

        <div class="card">
            <h3 class="h3">Address + Map</h3>
            <div class="muted small">This will show under Location on SpaceDetails.</div>

            <div class="grid" style="margin-top:10px;">
                <div class="field">
                    <span class="label">Address line</span>
                    <asp:TextBox ID="txtAddressLine" runat="server" CssClass="input" />
                </div>
                <div class="field">
                    <span class="label">City</span>
                    <asp:TextBox ID="txtCity" runat="server" CssClass="input" />
                </div>
                <div class="field">
                    <span class="label">State</span>
                    <asp:TextBox ID="txtState" runat="server" CssClass="input" />
                </div>
                <div class="field">
                    <span class="label">Postcode</span>
                    <asp:TextBox ID="txtPostcode" runat="server" CssClass="input" />
                </div>
                <div class="field">
                    <span class="label">Country</span>
                    <asp:TextBox ID="txtCountry" runat="server" CssClass="input" />
                </div>

                <div class="field">
                    <span class="label">Latitude (optional)</span>
                    <asp:TextBox ID="txtLat" runat="server" CssClass="input" />
                </div>
                <div class="field">
                    <span class="label">Longitude (optional)</span>
                    <asp:TextBox ID="txtLng" runat="server" CssClass="input" />
                </div>
            </div>
        </div>

        <div class="card">
            <h3 class="h3">Photos</h3>
            <div class="muted small">Upload multiple photos. You can set a cover photo after upload.</div>

            <div style="margin-top:10px;">
                <asp:FileUpload ID="fuPhotos" runat="server" AllowMultiple="true" />
            </div>

            <div class="btn-row" style="margin-top:12px;">
                <asp:Button ID="btnSave" runat="server" Text="Save listing" CssClass="btn btn-outline" OnClick="btnSave_Click" />
                <asp:Button ID="btnUpload" runat="server" Text="Upload photos" CssClass="btn btn-outline" OnClick="btnUpload_Click" />

                <!-- ✅ This is the button you want -->
                <asp:Button ID="btnComplete" runat="server" Text="Complete listing creation" CssClass="btn btn-primary" OnClick="btnComplete_Click" />

                <a class="btn btn-outline" href="Account.aspx">Back</a>
            </div>

            <!-- Existing photos -->
            <asp:Panel ID="pnlExistingPhotos" runat="server" Visible="false" style="margin-top:14px;">
                <h3 class="h3">Current photos</h3>

                <asp:Repeater ID="rptPhotos" runat="server" OnItemCommand="rptPhotos_ItemCommand">
                    <HeaderTemplate><div class="photos-grid"></HeaderTemplate>
                    <ItemTemplate>
                        <div class="photo-card">
                            <img src="<%# Eval("DisplayUrl") %>" alt="photo" />
                            <div class="photo-card__meta">
                                <span class="pill" style='<%# (Convert.ToBoolean(Eval("IsCover")) ? "" : "display:none;") %>'>COVER</span>

                                <div style="display:flex; gap:10px; align-items:center;">
                                    <asp:LinkButton runat="server"
                                        CssClass="link"
                                        CommandName="SetCover"
                                        CommandArgument='<%# Eval("PhotoId") %>'
                                        Text="Set cover" />

                                    <asp:LinkButton runat="server"
                                        CssClass="link danger"
                                        CommandName="DeletePhoto"
                                        CommandArgument='<%# Eval("PhotoId") %>'
                                        Text="Delete"
                                        OnClientClick="return confirm('Delete this photo?');" />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
            </asp:Panel>

        </div>

    </div>
</asp:Content>
