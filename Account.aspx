<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Account.aspx.cs"
    Inherits="Respace.Account" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Label ID="lblActionMsg" runat="server" />

    <div class="page">

        <div class="account-hero card">
            <div class="account-hero__left">
                <h1 class="h2">Account</h1>
                <div class="muted">
                    <asp:Label ID="lblName" runat="server" /> •
                    <asp:Label ID="lblEmail" runat="server" /> •
                    <span class="pill"><asp:Label ID="lblRole" runat="server" /></span>
                </div>
            </div>

            <div class="account-hero__right">
                <asp:PlaceHolder ID="phGuestPointsUI" runat="server" Visible="false">
                    <div class="stat">
                        <div class="stat__label">Points</div>
                        <div class="stat__value"><asp:Label ID="lblPoints" runat="server" /></div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phHostEarningsUI" runat="server" Visible="false">
                    <div class="stat">
                        <div class="stat__label">Total earnings</div>
                        <div class="stat__value"><asp:Label ID="lblEarnings" runat="server" /></div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phMembershipUI" runat="server" Visible="false">
                    <div class="stat">
                        <div class="stat__label">Membership</div>
                        <div class="stat__value"><asp:Label ID="lblMembership" runat="server" /></div>
                    </div>
                    <a class="btn btn-primary" href="Membership.aspx">Manage Membership</a>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="tabs card">
            <asp:LinkButton ID="btnTabOverview" runat="server" CssClass="tab active"
                OnClick="btnTabOverview_Click" Text="Overview" />

            <asp:PlaceHolder ID="phGuestTabs" runat="server" Visible="false">
                <asp:LinkButton ID="btnTabBookings" runat="server" CssClass="tab"
                    OnClick="btnTabBookings_Click" Text="My Bookings" />
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phHostTabs" runat="server" Visible="false">
                <asp:LinkButton ID="btnTabSpaces" runat="server" CssClass="tab"
                    OnClick="btnTabSpaces_Click" Text="My Listings" />
            </asp:PlaceHolder>
        </div>

        <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0">

            <!-- OVERVIEW -->
            <asp:View ID="viewOverview" runat="server">
                <div class="grid-2">
                    <div class="card">
                        <h3 class="h3">Quick summary</h3>
                        <div class="kv">

                            <asp:PlaceHolder ID="phGuestSummary" runat="server" Visible="false">
                                <div class="kv__row">
                                    <span class="kv__k">Upcoming bookings</span>
                                    <span class="kv__v"><asp:Label ID="lblUpcomingBookings" runat="server" /></span>
                                </div>
                                <div class="kv__row">
                                    <span class="kv__k">Total bookings</span>
                                    <span class="kv__v"><asp:Label ID="lblTotalBookings" runat="server" /></span>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="phHostSummary" runat="server" Visible="false">
                                <div class="kv__row">
                                    <span class="kv__k">Active listings</span>
                                    <span class="kv__v"><asp:Label ID="lblActiveListings" runat="server" /></span>
                                </div>
                                <div class="kv__row">
                                    <span class="kv__k">Total earnings</span>
                                    <span class="kv__v"><asp:Label ID="lblEarnings2" runat="server" /></span>
                                </div>
                            </asp:PlaceHolder>

                        </div>
                    </div>

                    <div class="card">
                        <h3 class="h3">Recommended actions</h3>
                        <ul class="list">
                            <li><a href="Search.aspx">Find a space</a></li>

                            <asp:PlaceHolder ID="phHostActions" runat="server" Visible="false">
                                <li><a href="HostCreateSpace.aspx">Create a new listing</a></li>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="phGuestActions" runat="server" Visible="false">
                                <li><a href="Rewards.aspx">View rewards</a></li>
                            </asp:PlaceHolder>
                        </ul>
                    </div>
                </div>
            </asp:View>

            <!-- GUEST BOOKINGS -->
            <asp:View ID="viewBookings" runat="server">
                <div class="card">
                    <h3 class="h3">My bookings</h3>
                    <asp:Label ID="lblBookingsMsg" runat="server" CssClass="muted" />

                    <asp:GridView ID="gvBookings" runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table"
                        GridLines="None"
                        DataKeyNames="BookingId"
                        OnRowCommand="gvBookings_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="BookingId" HeaderText="ID" />
                            <asp:BoundField DataField="SpaceName" HeaderText="Space" />
                            <asp:BoundField DataField="StartDateTime" HeaderText="Start" DataFormatString="{0:dd MMM yyyy HH:mm}" />
                            <asp:BoundField DataField="EndDateTime" HeaderText="End" DataFormatString="{0:dd MMM yyyy HH:mm}" />
                            <asp:BoundField DataField="TotalPrice" HeaderText="Total" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />

                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnGuestCancel" runat="server"
                                        Text="Cancel"
                                        CssClass="btn btn-outline"
                                        CommandName="GuestCancel"
                                        CommandArgument='<%# Eval("BookingId") %>'
                                        Visible='<%# (Eval("Status") ?? "").ToString().Trim().Equals("Pending", StringComparison.OrdinalIgnoreCase) %>'
                                        OnClientClick="return confirm('Cancel this booking request?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:View>

            <!-- HOST LISTINGS + HOST BOOKINGS -->
            <asp:View ID="viewSpaces" runat="server">
                <div class="card">
                    <h3 class="h3">My listings</h3>
                    <asp:Label ID="lblSpacesMsg" runat="server" CssClass="muted" />

                    <asp:GridView ID="gvSpaces" runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table"
                        GridLines="None"
                        DataKeyNames="SpaceId"
                        OnRowCommand="gvSpaces_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEditSpace" runat="server"
                                        Text="Edit"
                                        CssClass="link"
                                        CommandName="EditSpace"
                                        CommandArgument='<%# Eval("SpaceId") %>' />
                                    &nbsp;&nbsp;

                                    <!-- ✅ NEW: Block dates -->
                                    <asp:LinkButton ID="btnBlockDates" runat="server"
                                        Text="Block dates"
                                        CssClass="link"
                                        CommandName="BlockDates"
                                        CommandArgument='<%# Eval("SpaceId") %>' />
                                    &nbsp;&nbsp;

                                    <asp:LinkButton ID="btnDeleteSpace" runat="server"
                                        Text="Delete"
                                        CssClass="link danger"
                                        CommandName="DeleteSpace"
                                        CommandArgument='<%# Eval("SpaceId") %>'
                                        OnClientClick="return confirm('Hide this listing? (Bookings will be kept)');" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="SpaceId" HeaderText="ID" />
                            <asp:BoundField DataField="Name" HeaderText="Name" />
                            <asp:BoundField DataField="Location" HeaderText="Location" />
                            <asp:BoundField DataField="Type" HeaderText="Type" />
                            <asp:BoundField DataField="PricePerHour" HeaderText="Price/Hour" DataFormatString="{0:0.00}" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />
                        </Columns>
                    </asp:GridView>

                    <hr />

                    <h3 class="h3">Bookings on my spaces</h3>
                    <asp:Label ID="lblHostBookingsMsg" runat="server" CssClass="muted" />

                    <asp:GridView ID="gvHostBookings" runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table"
                        GridLines="None"
                        DataKeyNames="BookingId"
                        OnRowCommand="gvHostBookings_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="BookingId" HeaderText="Booking ID" />
                            <asp:BoundField DataField="SpaceName" HeaderText="Space" />
                            <asp:BoundField DataField="GuestName" HeaderText="Guest" />
                            <asp:BoundField DataField="StartDateTime" HeaderText="Start" DataFormatString="{0:dd MMM yyyy HH:mm}" />
                            <asp:BoundField DataField="EndDateTime" HeaderText="End" DataFormatString="{0:dd MMM yyyy HH:mm}" />
                            <asp:BoundField DataField="TotalPrice" HeaderText="Total" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />

                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <%# "" %>
                                    <div style="display:flex; gap:10px; flex-wrap:wrap; align-items:center;">

                                        <asp:LinkButton ID="btnConfirm" runat="server"
                                            Text="Confirm"
                                            CssClass="btn btn-primary"
                                            CommandName="HostConfirmBooking"
                                            CommandArgument='<%# Eval("BookingId") %>'
                                            Visible='<%# (Eval("Status") ?? "").ToString().Trim().Equals("Pending", StringComparison.OrdinalIgnoreCase) %>' />

                                        <asp:LinkButton ID="btnReject" runat="server"
                                            Text="Reject"
                                            CssClass="btn btn-outline"
                                            CommandName="HostRejectBooking"
                                            CommandArgument='<%# Eval("BookingId") %>'
                                            Visible='<%# (Eval("Status") ?? "").ToString().Trim().Equals("Pending", StringComparison.OrdinalIgnoreCase) %>' />

                                        <asp:TextBox ID="txtHostCancelReason" runat="server"
                                            CssClass="input"
                                            Width="220"
                                            Placeholder="Cancel reason (optional)"
                                            Visible='<%# !(Eval("Status") ?? "").ToString().Trim().Equals("Cancelled", StringComparison.OrdinalIgnoreCase) %>' />

                                        <asp:LinkButton ID="btnCancel" runat="server"
                                            Text="Cancel"
                                            CssClass="btn btn-outline"
                                            CommandName="HostCancelBooking"
                                            CommandArgument='<%# Eval("BookingId") %>'
                                            OnClientClick="return confirm('Cancel this booking?');"
                                            Visible='<%# !(Eval("Status") ?? "").ToString().Trim().Equals("Cancelled", StringComparison.OrdinalIgnoreCase) %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </div>
            </asp:View>

        </asp:MultiView>
    </div>

</asp:Content>
