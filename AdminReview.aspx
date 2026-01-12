<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminReview.aspx.cs"
    Inherits="Respace.AdminReview" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .wrap { max-width: 1000px; margin: 0 auto; }
        .panel { background:#fff; padding:16px; border-radius:12px; box-shadow:0 4px 12px rgba(0,0,0,0.08); }
        .msg { text-align:center; color:green; font-weight:bold; margin: 10px 0; }
        .ctrl { display:flex; gap:10px; flex-wrap:wrap; margin-bottom:10px; align-items:flex-end; }
        input, select { padding:10px; border-radius:10px; border:1px solid #ddd; }
        .btn { padding:10px 14px; border-radius:10px; border:none; cursor:pointer; background:#ff385c; color:#fff; }
        .btn-secondary { background:#666; }
    </style>

    <div class="wrap">
        <h2>Admin Review Approvals</h2>

        <div class="panel">
            <div class="ctrl">
                <div>
                    Search space name:
                    <asp:TextBox ID="txtSearch" runat="server" />
                </div>

                <div>
                    Sort:
                    <asp:DropDownList ID="ddlSort" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Bind">
                        <asp:ListItem Value="date_desc">Newest</asp:ListItem>
                        <asp:ListItem Value="rating_desc">Highest Rating</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn" OnClick="Bind" />
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="msg" />

            <asp:GridView ID="gvReviews" runat="server"
                AutoGenerateColumns="false"
                OnRowCommand="gvReviews_RowCommand"
                DataKeyNames="ReviewId">

                <Columns>
                    <asp:BoundField DataField="SpaceName" HeaderText="Space" />
                    <asp:BoundField DataField="GuestName" HeaderText="Guest" />

                    <asp:TemplateField HeaderText="Rating">
                        <ItemTemplate>
                            <span style="color:#ff385c">
                                <%# new string("\u2605"[0], Convert.ToInt32(Eval("Rating"))) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Comment" HeaderText="Comment" />
                    <asp:BoundField DataField="CreatedAt" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:ButtonField Text="Approve" CommandName="Approve" />
                    <asp:ButtonField Text="Delete" CommandName="DeleteReview" />
                </Columns>

            </asp:GridView>
        </div>
    </div>

</asp:Content>
