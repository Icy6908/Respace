<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Respace.Search" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>ReSpace</title>

    <style>
        body {
            font-family: Arial;
            background: #2f2f3f;
            color: white;
            padding: 30px;
        }

        .top-bar {
            display: flex;
            gap: 10px;
            margin-bottom: 20px;
        }

        .filter-panel {
            background: #3a3a4f;
            padding: 20px;
            border-radius: 12px;
            width: 600px;
            margin-bottom: 30px;
        }

        .filter-title {
            margin-top: 15px;
            margin-bottom: 10px;
            font-weight: bold;
        }

        .space-card {
            border: 1px solid #555;
            padding: 15px;
            border-radius: 10px;
            margin-bottom: 15px;
        }

        .btn {
            padding: 8px 16px;
            border-radius: 8px;
            border: none;
            cursor: pointer;
        }

        .btn-primary { background: #ffb6c1; }
        .btn-secondary { background: #777; color: white; }
    </style>
</head>

<body>
<form runat="server">

    <!-- SEARCH BAR -->
    <div class="top-bar">
        <asp:TextBox ID="txtSearch" runat="server"
            Width="300px"
            Placeholder="Search venue..." />

        <asp:Button ID="btnSearch" runat="server"
            Text="Search"
            CssClass="btn btn-primary"
            OnClick="btnSearch_Click" />

        <asp:Button ID="btnToggleFilter" runat="server"
            Text="Filter"
            CssClass="btn btn-secondary"
            OnClick="btnToggleFilter_Click" />
    </div>

    <!-- FILTER PANEL (ONLY SHOWS WHEN FILTER CLICKED) -->
    <asp:Panel ID="pnlFilter" runat="server" CssClass="filter-panel">

        <div class="filter-title">Location</div>
        <asp:CheckBoxList ID="cblLocation" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem>North</asp:ListItem>
            <asp:ListItem>South</asp:ListItem>
            <asp:ListItem>East</asp:ListItem>
            <asp:ListItem>West</asp:ListItem>
            <asp:ListItem>Central</asp:ListItem>
        </asp:CheckBoxList>

        <div class="filter-title">Type of Space</div>
        <asp:CheckBoxList ID="cblType" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem>Meeting Room</asp:ListItem>
            <asp:ListItem>Event Hall</asp:ListItem>
            <asp:ListItem>Training Room</asp:ListItem>
            <asp:ListItem>Conference Room</asp:ListItem>
            <asp:ListItem>Studio</asp:ListItem>
        </asp:CheckBoxList>

        <br />

        <asp:Button ID="btnApplyFilter" runat="server"
            Text="Filter"
            CssClass="btn btn-primary"
            OnClick="btnApplyFilter_Click" />

        <asp:Button ID="btnClearFilter" runat="server"
            Text="Clear All Filters"
            CssClass="btn btn-secondary"
            OnClick="btnClearFilter_Click" />

    </asp:Panel>

    <!-- RESULTS -->
    <asp:Repeater ID="rptSpaces" runat="server">
        <ItemTemplate>
            <div class="space-card">
                <strong><%# Eval("Name") %></strong><br />
                Location: <%# Eval("Location") %><br />
                Type: <%# Eval("Type") %>
            </div>
        </ItemTemplate>
    </asp:Repeater>

</form>
</body>
</html>
