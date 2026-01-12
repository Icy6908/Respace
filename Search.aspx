x<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Respace.Search" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Search Venues</title>

    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #fafafa;
            padding: 30px;
        }

        .search-bar {
            display: flex;
            gap: 10px;
            margin-bottom: 15px;
        }

        input, select, button {
            padding: 10px;
            border-radius: 8px;
            border: 1px solid #ddd;
        }

        button {
            background-color: #ff5a5f;
            color: white;
            border: none;
            cursor: pointer;
        }

            button.secondary {
                background-color: #666;
            }

        .filter-panel {
            background: white;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            margin-bottom: 25px;
        }

        .filter-group {
            margin-bottom: 20px;
        }

        .venue-card {
            background: white;
            border-radius: 14px;
            box-shadow: 0 6px 16px rgba(0,0,0,0.1);
            padding: 20px;
            margin-bottom: 20px;
        }

        .price {
            color: #ff5a5f;
            font-weight: bold;
        }

        .venue-card {
            max-width: 720px;
            margin: 0 auto 24px auto;
        }
    </style>
</head>

<body>
    <form runat="server">


        <div class="search-bar">
            <asp:TextBox ID="txtSearch" runat="server" Width="280"
                Placeholder="Search by name, location, or month (e.g. March)" />

            <asp:Button ID="btnSearch" runat="server" Text="Search"
                OnClick="btnSearch_Click" />

            <asp:Button ID="btnToggleFilter" runat="server" Text="Filter"
                CssClass="secondary"
                OnClick="btnToggleFilter_Click" />
        </div>


        <asp:Panel ID="pnlFilter" runat="server" CssClass="filter-panel" Visible="false">

            <h3>Simple Search</h3>


            <div class="filter-group">
                <strong>Location</strong><br />
                <asp:CheckBoxList ID="cblLocation" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem>North</asp:ListItem>
                    <asp:ListItem>South</asp:ListItem>
                    <asp:ListItem>East</asp:ListItem>
                    <asp:ListItem>West</asp:ListItem>
                    <asp:ListItem>Central</asp:ListItem>
                </asp:CheckBoxList>
            </div>

            <div class="filter-group">
                <strong>Type</strong><br />
                <asp:CheckBoxList ID="cblType" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem>Meeting Room</asp:ListItem>
                    <asp:ListItem>Conference Room</asp:ListItem>
                    <asp:ListItem>Training Room</asp:ListItem>
                    <asp:ListItem>Event Hall</asp:ListItem>
                    <asp:ListItem>Studio</asp:ListItem>
                </asp:CheckBoxList>
            </div>


            <div class="filter-group">
                <strong>Price Range ($)</strong><br />
                Min:
            <asp:TextBox ID="txtMinPrice" runat="server" TextMode="Number" Width="100" />
                Max:
            <asp:TextBox ID="txtMaxPrice" runat="server" TextMode="Number" Width="100" />
            </div>

            <div class="filter-group">
                <strong>Available Date</strong><br />
                From:
            <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" />
                To:
            <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" />
            </div>


            <div class="filter-group">
                <strong>Sort By</strong><br />
                <asp:DropDownList ID="ddlSort" runat="server">
                    <asp:ListItem Value="">-- Select --</asp:ListItem>
                    <asp:ListItem Value="price_asc">Price: Low to High</asp:ListItem>
                    <asp:ListItem Value="price_desc">Price: High to Low</asp:ListItem>
                    <asp:ListItem Value="date_asc">Date: Earliest</asp:ListItem>
                    <asp:ListItem Value="date_desc">Date: Latest</asp:ListItem>
                </asp:DropDownList>
            </div>

            <asp:Button ID="btnApplyFilter" runat="server" Text="Apply Filter"
                OnClick="btnApplyFilter_Click" />

            <asp:Button ID="btnClearFilter" runat="server" Text="Clear All"
                CssClass="secondary"
                OnClick="btnClearFilter_Click" />

        </asp:Panel>

        <asp:Repeater ID="rptSpaces" runat="server">
            <ItemTemplate>
                <div class="venue-card">

                    <h3><%# Eval("Name") %></h3>
                    <div><%# Eval("Location") %> • <%# Eval("Type") %></div>
                    <div class="price">$<%# Eval("Price") %> / day</div>
                    <div>Available from <%# Eval("AvailableDate", "{0:dd MMM yyyy}") %></div>
                    <p><%# Eval("Description") %></p>

                    <div style="margin-top: 12px;">
                        <asp:Button
                            runat="server"
                            Text="View Reviews"
                            CssClass="secondary"
                            CommandName="ToggleReviews"
                            CommandArgument='<%# Eval("RoomId") %>'
                            OnCommand="ToggleReviews_Click" />

                        &nbsp;

                <asp:HyperLink
                    runat="server"
                    NavigateUrl='<%# "review.aspx?roomId=" + Eval("RoomId") %>'
                    Text="Review this venue" />
                    </div>

                    <asp:Panel
                        ID="pnlReviews"
                        runat="server"
                        Visible="false"
                        Style="margin-top: 12px; border-top: 1px solid #eee; padding-top: 10px;">

                        <asp:Repeater
                            runat="server"
                            DataSource='<%# GetApprovedReviews(Convert.ToInt32(Eval("RoomId"))) %>'>
                            <ItemTemplate>
                                <div style="margin-bottom: 10px;">
                                    <strong style="color: #ff5a5f">
                                        <%# new string('★', Convert.ToInt32(Eval("Rating"))) %>
                                    </strong>
                                    <br />
                                    <%# Eval("Comment") %><br />
                                    <small style="color: #777">
                                        <%# Eval("ReviewDate", "{0:dd MMM yyyy}") %>
                                    </small>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                    </asp:Panel>

                </div>
            </ItemTemplate>
        </asp:Repeater>


    </form>
</body>
</html>
