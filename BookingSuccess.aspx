<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BookingSuccess.aspx.cs"
    Inherits="Respace.BookingSuccess" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page" style="max-width:980px;margin:0 auto;">
        <div class="card" style="padding:22px;">
            <div style="display:flex;align-items:center;gap:14px;">
                <div style="width:46px;height:46px;border-radius:999px;background:#e9f7ef;display:flex;align-items:center;justify-content:center;font-size:22px;">
                    ✓
                </div>
                <div>
                    <h1 class="h2" style="margin:0;">Booking request received</h1>
                    <div class="muted">
                        Your booking has been created. Status may be <strong>Pending</strong> until host confirms.
                    </div>
                </div>
            </div>

            <asp:Label ID="lblMsg" runat="server" CssClass="alert" />

            <asp:Panel ID="pnlDetails" runat="server" Visible="false" style="margin-top:18px;">
                <div class="grid-2" style="display:grid;grid-template-columns:1.2fr .8fr;gap:16px;">
                    <div class="card" style="padding:18px;">
                        <h3 class="h3" style="margin-top:0;">Booking details</h3>

                        <div style="display:grid;grid-template-columns:140px 1fr;row-gap:10px;column-gap:10px;">
                            <div class="muted">Space</div>
                            <div><strong><asp:Label ID="lblSpaceName" runat="server" /></strong></div>

                            <div class="muted">Location</div>
                            <div><asp:Label ID="lblLocation" runat="server" /></div>

                            <div class="muted">Check-in</div>
                            <div><asp:Label ID="lblCheckIn" runat="server" /></div>

                            <div class="muted">Check-out</div>
                            <div><asp:Label ID="lblCheckOut" runat="server" /></div>

                            <div class="muted">Nights</div>
                            <div><asp:Label ID="lblNights" runat="server" /></div>

                            <div class="muted">Total</div>
                            <div><strong><asp:Label ID="lblTotal" runat="server" /></strong></div>

                            <div class="muted">Status</div>
                            <div>
                                <span class="pill"><asp:Label ID="lblStatus" runat="server" /></span>
                            </div>
                        </div>
                    </div>

                    <div class="card" style="padding:18px;">
                        <h3 class="h3" style="margin-top:0;">Next steps</h3>
                        <div class="muted" style="margin-bottom:12px;">
                            You can view/cancel your booking in your Account page.
                        </div>

                        <asp:Button ID="btnViewBookings" runat="server"
                            Text="View my bookings"
                            CssClass="btn btn-primary btn-block"
                            OnClick="btnViewBookings_Click" />

                        <a href="Search.aspx" class="btn btn-outline btn-block" style="margin-top:10px;text-decoration:none;display:block;text-align:center;">
                            Find another space
                        </a>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>

</asp:Content>
