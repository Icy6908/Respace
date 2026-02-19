<%@ Page Title="Messages - ReSpace" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Message.aspx.cs" Inherits="Respace.Message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
    .messenger-wrapper { display: flex; height: 82vh; background: #fff; border-radius: 12px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); margin-top: 20px; overflow: hidden; border: 1px solid #eee; }
    .chat-sidebar { width: 320px; border-right: 1px solid #eee; background: #fafafa; display: flex; flex-direction: column; }
    .sidebar-header { padding: 15px; border-bottom: 1px solid #eee; background: #fff; }
    .chat-item { padding: 15px; border-bottom: 1px solid #f0f0f0; text-decoration: none; color: #333; display: block; transition: background 0.2s; }
    .chat-item:hover { background: #fff1f3; }
    .chat-item.active { background: #fff1f3; border-left: 4px solid #ff4d6d; }
    .chat-main { flex: 1; display: flex; flex-direction: column; background: #fff; }
    .chat-history { flex: 1; padding: 20px; overflow-y: auto; display: flex; flex-direction: column; background: #fdfdfd; }
    .space-info { width: 280px; border-left: 1px solid #eee; padding: 20px; text-align: center; background: #fff; }
    .space-preview-img { width: 100%; height: 160px; object-fit: cover; border-radius: 8px; margin-bottom: 15px; border: 1px solid #eee; }
    .msg-bubble { max-width: 75%; padding: 12px 16px; border-radius: 18px; margin-bottom: 12px; font-size: 0.95rem; line-height: 1.4; }
    .sent { align-self: flex-end; background: #ff4d6d; color: white; border-bottom-right-radius: 4px; }
    .received { align-self: flex-start; background: #e9e9eb; color: #333; border-bottom-left-radius: 4px; }
    .chat-input-area { padding: 20px; background: #fff; border-top: 1px solid #eee; }
    .input-flex { display: flex; gap: 12px; }
    .modern-input { flex: 1; padding: 12px 20px; border: 2px solid #f0f0f0; border-radius: 25px; outline: none; }
    .btn-send { background: #ff4d6d; color: white; border: none; padding: 10px 25px; border-radius: 25px; font-weight: 600; cursor: pointer; }
    .date-badge { font-size: 0.75rem; color: #ff4d6d; font-weight: 600; }
</style>

<div class="container">
    <div class="messenger-wrapper">
        <div class="chat-sidebar">
            <div class="sidebar-header">
                <div class="fw-bold mb-2">Conversations</div>
                <div class="btn-group w-100">
                    <asp:LinkButton ID="btnShowActive" runat="server" CssClass="btn btn-sm btn-outline-secondary active" OnClick="Filter_Click" CommandArgument="Active">Active</asp:LinkButton>
                    <asp:LinkButton ID="btnShowPast" runat="server" CssClass="btn btn-sm btn-outline-secondary" OnClick="Filter_Click" CommandArgument="Past">Past</asp:LinkButton>
                </div>
            </div>
            <div style="flex: 1; overflow-y: auto;">
                <asp:Repeater ID="rptChatList" runat="server">
                    <ItemTemplate>
                        <a href='Message.aspx?bid=<%# Eval("BookingId") %>' 
                           class='chat-item <%# Request.QueryString["bid"] == Eval("BookingId").ToString() ? "active" : "" %>'>
                            <div class="d-flex justify-content-between align-items-center mb-1">
                                <div class="fw-bold text-dark"><%# Eval("PartnerName") %></div>
                                <span class="badge bg-light text-muted small">#<%# Eval("BookingId") %></span>
                            </div>
                            <div class="small text-muted text-truncate"><%# Eval("SpaceName") %></div>
                            <div class="date-badge mt-1">
                                <i class="far fa-calendar-alt me-1"></i>
                                <%# Eval("StartDateTime", "{0:MMM dd}") %> - <%# Eval("EndDateTime", "{0:MMM dd}") %>
                            </div>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div class="chat-main">
            <div class="chat-history">
                <asp:Label ID="lblNoChat" runat="server" Text="Select a conversation to start chatting" CssClass="m-auto text-muted" />
                <asp:Repeater ID="rptMessages" runat="server">
                    <ItemTemplate>
                        <div class='msg-bubble <%# Eval("SenderID").ToString() == Session["UserId"].ToString() ? "sent" : "received" %>'>
                            <%# Eval("MessageText") %>
                            <div style="font-size: 0.7rem; opacity: 0.6; margin-top: 4px; text-align: right;">
                                <%# Eval("Timestamp", "{0:HH:mm}") %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="chat-input-area">
                <div class="input-flex">
                    <asp:TextBox ID="txtNewMsg" runat="server" CssClass="modern-input" placeholder="Type a message..."></asp:TextBox>
                    <asp:Button ID="btnSend" runat="server" Text="Send" CssClass="btn-send" OnClick="btnSend_Click" />
                </div>
            </div>
        </div>

        <asp:PlaceHolder ID="phSpaceInfo" runat="server" Visible="false">
            <div class="space-info">
                <h6 class="fw-bold mb-3">Listing Details</h6>
                <asp:Image ID="imgSpaceDetail" runat="server" CssClass="space-preview-img" />
                <div class="fw-bold"><asp:Literal ID="litSpaceName" runat="server" /></div>
                <div class="text-muted small mt-1">Booking #<asp:Literal ID="litBookingId" runat="server" /></div>
                <div class="mt-3 p-2 rounded" style="background: #fff8f9; border: 1px solid #ffe6e9;">
                    <div class="small fw-bold text-uppercase text-muted" style="font-size: 0.65rem;">Booking Dates</div>
                    <div class="small">
                        <asp:Literal ID="litStartDate" runat="server" /> - <br />
                        <asp:Literal ID="litEndDate" runat="server" />
                    </div>
                </div>
                <hr />
                <p class="small text-muted">Coordinate details with your Host/Guest here.</p>
            </div>
        </asp:PlaceHolder>
    </div>
</div>
</asp:Content>