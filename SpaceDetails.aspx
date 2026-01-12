<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="SpaceDetails.aspx.cs"
    Inherits="Respace.SpaceDetails"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Space Details</h2>

    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" />
    <br /><br />

    <asp:Panel ID="pnlDetails" runat="server" Visible="false">

        <h3><asp:Label ID="lblName" runat="server" /></h3>

        <p><strong>Location:</strong> <asp:Label ID="lblLocation" runat="server" /></p>
        <p><strong>Type:</strong> <asp:Label ID="lblType" runat="server" /></p>
        <p><strong>Capacity:</strong> <asp:Label ID="lblCap" runat="server" /></p>
        <p><strong>Price / Hour:</strong> $<asp:Label ID="lblPrice" runat="server" /></p>

        <p>
            <strong>Description:</strong><br />
            <asp:Label ID="lblDesc" runat="server" />
        </p>

        <hr />

        <h3>Book this space</h3>

        <asp:Panel ID="pnlUnavailable" runat="server" Visible="false">
    <h4>Unavailable (confirmed bookings)</h4>

    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <div>
                <%# Eval("StartDateTime", "{0:dd MMM yyyy HH:mm}") %>
                to
                <%# Eval("EndDateTime", "{0:dd MMM yyyy HH:mm}") %>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <br />
</asp:Panel>

            <asp:Repeater ID="rptBooked" runat="server">
                <ItemTemplate>
                    <div>
                        <%# Eval("StartDateTime", "{0:dd MMM yyyy HH:mm}") %>
                        to
                        <%# Eval("EndDateTime", "{0:dd MMM yyyy HH:mm}") %>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <br />


        <table>
            <tr>
                <td>Start Date</td>
                <td><asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" /></td>
            </tr>
            <tr>
                <td>Start Time</td>
                <td><asp:TextBox ID="txtStartTime" runat="server" TextMode="Time" /></td>
            </tr>
            <tr>
                <td>End Date</td>
                <td><asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" /></td>
            </tr>
            <tr>
                <td>End Time</td>
                <td><asp:TextBox ID="txtEndTime" runat="server" TextMode="Time" /></td>
            </tr>
        </table>

        <br />

        <asp:Button ID="btnBook"
            runat="server"
            Text="Confirm Booking"
            OnClick="btnBook_Click" />

        <br /><br />

        <!-- Leave Review Button -->
        <a runat="server" id="lnkReview" class="review-btn">Write Review</a>

        <hr />

        <h3>Reviews</h3>
        <asp:Repeater ID="rptReviews" runat="server">
            <ItemTemplate>
                <div style="margin-bottom:12px; border-bottom:1px solid #eee; padding-bottom:10px;">
                    <strong><%# Eval("GuestName") %></strong><br />
                    <span style="color:#ff385c;"><%# Eval("Stars") %></span><br />
                    <%# Eval("Comment") %><br />
                    <small style="color:#777;"><%# Eval("CreatedAt", "{0:dd MMM yyyy}") %></small>
                </div>
            </ItemTemplate>
        </asp:Repeater>

    </asp:Panel>

    <style>
        .review-btn{
            display:inline-block;
            margin-top:12px;
            padding:10px 14px;
            border-radius:10px;
            background:#ffb6c1;
            color:#000;
            font-weight:bold;
            text-decoration:none;
        }
        .review-btn:hover{ opacity:0.9; }
    </style>

</asp:Content>
