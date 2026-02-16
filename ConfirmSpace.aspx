<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfirmSpace.aspx.cs"
    Inherits="Respace.ConfirmSpace" MasterPageFile="~/Site.Master" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .wrap{max-width:1100px;margin:22px auto;padding:0 16px;}
        .card{background:#fff;border-radius:16px;box-shadow:0 6px 16px rgba(0,0,0,.08);padding:18px;margin-bottom:16px;}
        .h2{font-size:30px;margin:0 0 6px;}
        .muted{color:#777;}
        .grid{display:grid;grid-template-columns:1.2fr .8fr;gap:18px;}
        @media(max-width:900px){.grid{grid-template-columns:1fr;}}
        .kv{display:grid;grid-template-columns:160px 1fr;gap:10px 14px;margin-top:10px;}
        .kv div{padding:6px 0;border-bottom:1px solid #eee;}
        .kv .k{color:#666;font-weight:700;}
        .btn{display:inline-block;padding:10px 14px;border-radius:10px;border:none;cursor:pointer;text-decoration:none;font-weight:700;}
        .btn-primary{background:#ff5a5f;color:#fff;}
        .btn-outline{background:#fff;border:1px solid #ddd;color:#111;}
        .btn-row{display:flex;gap:10px;flex-wrap:wrap;margin-top:14px;}
        .alert{display:block;padding:12px 14px;border-radius:12px;margin:0 0 14px;}
        .alert.error{background:#ffecec;border:1px solid #ffb3b3;color:#8a1f1f;}
        .alert.success{background:#eafff0;border:1px solid #9de9b1;color:#1a6b2a;}
        .photo-grid{display:grid;grid-template-columns:2fr 1fr 1fr;grid-auto-rows:160px;gap:10px;margin-top:10px;}
        .photo-grid img{width:100%;height:100%;object-fit:cover;border-radius:14px;background:#f2f2f2;}
        .photo-grid .big{grid-row:span 2;grid-column:span 1;min-height:330px;}
        @media(max-width:900px){.photo-grid{grid-template-columns:1fr 1fr;grid-auto-rows:150px}.photo-grid .big{grid-column:span 2;grid-row:span 1;min-height:220px}}
        .pill{display:inline-flex;align-items:center;gap:8px;padding:10px 12px;border:1px solid #eee;border-radius:14px;background:#fafafa;margin:6px 8px 0 0;}
        .map-frame{width:100%;height:320px;border:0;border-radius:14px;background:#f3f3f3;}
        .section-title{font-size:18px;margin:0 0 8px;font-weight:800;}
    </style>
</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="MainContent" runat="server">
    <div class="wrap">

        <asp:Label ID="lblMsg" runat="server" />

        <div class="card">
            <div class="h2">Confirm your listing</div>
            <div class="muted">Review everything below. This page is read-only.</div>
        </div>

        <div class="grid">

            <!-- LEFT -->
            <div>

                <div class="card">
                    <div class="section-title">Photos</div>

                    <asp:Panel ID="pnlNoPhotos" runat="server" Visible="false" CssClass="muted">
                        No photos added yet.
                    </asp:Panel>

                    <asp:Repeater ID="rptPhotos" runat="server">
                        <HeaderTemplate><div class="photo-grid"></HeaderTemplate>
                        <ItemTemplate>
                            <img class='<%# (Container.ItemIndex==0 ? "big" : "") %>'
                                 src='<%# Eval("ResolvedUrl") %>' alt="photo" />
                        </ItemTemplate>
                        <FooterTemplate></div></FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="card">
                    <div class="section-title">Basic info</div>

                    <div class="h2" style="font-size:26px;margin-top:4px;">
                        <asp:Label ID="lblName" runat="server" />
                    </div>
                    <div class="muted">
                        <asp:Label ID="lblLocation" runat="server" /> •
                        <asp:Label ID="lblType" runat="server" /> •
                        Capacity: <asp:Label ID="lblCap" runat="server" />
                    </div>

                    <div style="margin-top:10px;font-weight:800;color:#ff5a5f;font-size:20px;">
                        $<asp:Label ID="lblPrice" runat="server" /> <span class="muted" style="font-weight:400;">/ day</span>
                    </div>

                    <div style="margin-top:12px;">
                        <div class="section-title">Description</div>
                        <div style="white-space:pre-wrap;">
                            <asp:Label ID="lblDesc" runat="server" />
                        </div>
                    </div>
                </div>

                <div class="card">
                    <div class="section-title">What this place offers</div>

                    <asp:Panel ID="pnlNoAmenities" runat="server" Visible="false" CssClass="muted">
                        No amenities selected.
                    </asp:Panel>

                    <asp:Repeater ID="rptAmenities" runat="server">
                        <ItemTemplate>
                            <span class="pill">
                                <span><%# Eval("IconText") %></span>
                                <span><%# Eval("AmenityName") %></span>
                            </span>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="card">
                    <div class="section-title">Location</div>
                    <div class="muted" style="margin-bottom:10px;">
                        <asp:Label ID="lblAddress" runat="server" />
                    </div>
                    <asp:Literal ID="litMap" runat="server" />
                </div>

            </div>

            <!-- RIGHT -->
            <div>
                <div class="card">
                    <div class="section-title">Submit</div>
                    <div class="muted">
                        When you confirm, your listing will be submitted to Admin as <b>Pending</b>.
                        It will appear in Search only after Admin approves it.
                    </div>

                    <div class="btn-row">
                        <a class="btn btn-outline" runat="server" id="lnkBackToEdit">Back to edit</a>

                        <asp:Button ID="btnConfirmSubmit" runat="server"
                            CssClass="btn btn-primary"
                            Text="Confirm & Submit"
                            OnClick="btnConfirmSubmit_Click" />
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>
