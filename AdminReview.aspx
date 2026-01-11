<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminReview.aspx.cs" Inherits="Respace.adminreview" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Admin Reviews</title>

    <style>
        body {
            font-family: Arial;
            background: #f7f7f7;
        }

        table {
            width: 90%;
            margin: auto;
            background: white;
            border-radius: 10px;
        }

        th, td {
            padding: 10px;
        }

        .msg {
            text-align: center;
            color: green;
            font-weight: bold;
        }
    </style>
</head>

<body>
    <form runat="server">

        Search:
        <asp:TextBox ID="txtSearch" runat="server" />
        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="Bind" />
        <br /><br />

        Sort:
        <asp:DropDownList ID="ddlSort" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Bind">
            <asp:ListItem Value="date_desc">Newest</asp:ListItem>
            <asp:ListItem Value="rating_desc">Highest Rating</asp:ListItem>
        </asp:DropDownList>

        <br /><br />

        <asp:Label ID="lblMessage" runat="server" CssClass="msg" />

        <br /><br />

        <asp:GridView ID="gvReviews" runat="server"
            AutoGenerateColumns="false"
            OnRowCommand="gvReviews_RowCommand">

            <Columns>
                <asp:BoundField DataField="VenueName" HeaderText="Venue" />

                <asp:TemplateField HeaderText="Rating">
                    <ItemTemplate>
                        <span style="color:#ff385c">
                            <%# new string('★', (int)Eval("Rating")) %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="Comment" HeaderText="Comment" />
                <asp:BoundField DataField="ReviewDate" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" />

                <asp:ButtonField Text="Approve" CommandName="Approve" />
                <asp:ButtonField Text="Delete" CommandName="DeleteReview" />
            </Columns>
        </asp:GridView>

    </form>
</body>
</html>
