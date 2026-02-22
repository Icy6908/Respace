<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="Respace.Account" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="lblActionMsg" runat="server" CssClass="alert" />

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
                    <div style="display:flex; gap:10px; margin-top:10px;">
                        <a class="btn btn-primary" href="Membership.aspx">Upgrade</a>
                        <asp:LinkButton ID="btnCancelMembership" runat="server" CssClass="btn btn-outline danger" 
                            OnClick="btnCancelMembership_Click" OnClientClick="return confirm('Cancel subscription?');" Text="Cancel Plan" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="tabs card">
            <asp:LinkButton ID="btnTabOverview" runat="server" CssClass="tab active" OnClick="btnTabOverview_Click" Text="Overview" />
            <asp:PlaceHolder ID="phGuestTabs" runat="server" Visible="false">
                <asp:LinkButton ID="btnTabBookings" runat="server" CssClass="tab" OnClick="btnTabBookings_Click" Text="My Bookings" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phHostTabs" runat="server" Visible="false">
                <asp:LinkButton ID="btnTabSpaces" runat="server" CssClass="tab" OnClick="btnTabSpaces_Click" Text="My Listings" />
            </asp:PlaceHolder>
        </div>

        <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0">
            <asp:View ID="viewOverview" runat="server">
                <div class="grid-2">
                    <div class="card">
                        <h3 class="h3">Quick summary</h3>
                        <div class="kv">
                            <asp:PlaceHolder ID="phGuestSummary" runat="server" Visible="false">
                                <div class="kv__row"><span class="kv__k">Upcoming bookings: span><span class="kv__v"><asp:Label ID="lblUpcomingBookings" runat="server" /></span></div>
                                <div class="kv__row"><span class="kv__k">Total bookings: </span><span class="kv__v"><asp:Label ID="lblTotalBookings" runat="server" /></span></div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phHostSummary" runat="server" Visible="false">
                                <div class="kv__row"><span class="kv__k">Active listings</span><span class="kv__v"><asp:Label ID="lblActiveListings" runat="server" /></span></div>
                                <div class="kv__row"><span class="kv__k">Total earnings</span><span class="kv__v"><asp:Label ID="lblEarnings2" runat="server" /></span></div>
                            </asp:PlaceHolder>
                        </div>

                        <hr style="margin: 20px 0; border: 0; border-top: 1px solid #eee;" />
                        <h3 class="h3">Security</h3>
                        <p class="muted" style="font-size: 0.9em; margin-bottom: 15px;">Update your password to keep your account secure.</p>
                        <a href="ChangePassword.aspx" class="btn btn-outline" style="display:inline-block; width:auto;">Change Password</a>
                    </div>

                    <div class="card">
                        <h3 class="h3">Actions</h3>
                        <ul class="list">
                            <li><a href="Search.aspx">Find a space</a></li>
                            <asp:PlaceHolder ID="phHostActions" runat="server" Visible="false"><li><a href="HostCreateSpace.aspx">Create listing</a></li></asp:PlaceHolder>
                            <asp:PlaceHolder ID="phGuestActions" runat="server" Visible="false"><li><a href="Rewards.aspx">View rewards</a></li></asp:PlaceHolder>
                        </ul>
                    </div>
                </div>
            </asp:View>

            <asp:View ID="viewBookings" runat="server">
                <div class="card">
                    <h3 class="h3">My bookings</h3>
                    <asp:GridView ID="gvBookings" runat="server" AutoGenerateColumns="false" CssClass="table" GridLines="None" DataKeyNames="BookingId" OnRowCommand="gvBookings_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="BookingId" HeaderText="ID" />
                            <asp:BoundField DataField="SpaceName" HeaderText="Space" />
                            <asp:BoundField DataField="StartDateTime" HeaderText="Start" DataFormatString="{0:dd MMM HH:mm}" />
                            <asp:BoundField DataField="EndDateTime" HeaderText="End" DataFormatString="{0:dd MMM HH:mm}" />
                            <asp:BoundField DataField="TotalPrice" HeaderText="Total" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <div style="display:flex; gap:8px;">
                                        <asp:LinkButton ID="btnGuestCancel" runat="server" Text="Cancel" CssClass="btn btn-outline danger" 
                                            CommandName="GuestCancel" CommandArgument='<%# Eval("BookingId") %>'
                                            Visible='<%# Eval("Status").ToString().Trim() == "Pending" || Eval("Status").ToString().Trim() == "Confirmed" %>' 
                                            OnClientClick="return confirm('Cancel this booking?');" />
                                        <asp:LinkButton ID="btnCompleteStay" runat="server" Text="Review stay" CssClass="btn btn-primary" 
                                            Style="background-color: #ff385c; color: white;"
                                            CommandName="CompleteStay" CommandArgument='<%# Eval("BookingId") %>'
                                            Visible='<%# Eval("Status").ToString().Trim() == "Confirmed" %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:View>

            <asp:View ID="viewSpaces" runat="server">
                <div class="card">
                    <h3 class="h3">My listings</h3>
                    <asp:GridView ID="gvSpaces" runat="server" AutoGenerateColumns="false" CssClass="table" DataKeyNames="SpaceId" OnRowCommand="gvSpaces_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Name" HeaderText="Name" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <div style="display:flex; gap:12px; align-items:center;">
                                        <asp:LinkButton ID="btnEdit" runat="server" Text="Edit" CommandName="EditSpace" CommandArgument='<%# Eval("SpaceId") %>' CssClass="link" />
                                        <asp:LinkButton ID="btnBlockDates" runat="server" Text="Block Dates" CommandName="BlockDates" CommandArgument='<%# Eval("SpaceId") %>' CssClass="link" Style="color: #ff5a5f; font-weight: bold;" />
                                        <asp:LinkButton ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteSpace" CommandArgument='<%# Eval("SpaceId") %>' CssClass="link danger" />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <hr />
                    <h3 class="h3">Incoming Bookings</h3>
                    <asp:GridView ID="gvHostBookings" runat="server" AutoGenerateColumns="false" CssClass="table" OnRowCommand="gvHostBookings_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="BookingId" HeaderText="ID" />
                            <asp:BoundField DataField="SpaceName" HeaderText="Space" />
                            <asp:BoundField DataField="GuestName" HeaderText="Guest" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <div style="display:flex; gap:8px;">
                                        <asp:LinkButton ID="btnHostConfirm" runat="server" Text="Confirm" CommandName="HostConfirmBooking" CommandArgument='<%# Eval("BookingId") %>' CssClass="btn btn-primary" Visible='<%# Eval("Status").ToString().Trim() == "Pending" %>' />
                                        <asp:LinkButton ID="btnHostCancel" runat="server" Text="Cancel" CommandName="HostCancelBooking" CommandArgument='<%# Eval("BookingId") %>' CssClass="btn btn-outline danger" Visible='<%# Eval("Status").ToString().Trim() == "Pending" || Eval("Status").ToString().Trim() == "Confirmed" %>' OnClientClick="return confirm('Cancel this booking?');" />
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