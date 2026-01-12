<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Support.aspx.cs" Inherits="Respace.Support" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid mt-4">
        <div class="row" style="height: 80vh;">
            <div class="col-md-4 border-end overflow-auto h-100">
                <h4 class="mb-3">Support Inbox</h4>
                <div class="list-group list-group-flush">
                    <asp:Repeater ID="rptTickets" runat="server" OnItemCommand="rptTickets_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkSelect" runat="server" CommandName="SelectTicket" CommandArgument='<%# Eval("QueryID") %>' 
                                CssClass="list-group-item list-group-item-action py-3 lh-tight">
                                <div class="d-flex w-100 align-items-center justify-content-between">
                                    <strong class="mb-1"><%# Eval("Subject") %></strong>
                                    <small class="text-muted"><%# Eval("SubmittedAt", "{0:MMM dd}") %></small>
                                </div>
                                <div class="col-10 mb-1 small text-truncate"><%# Eval("Issue") %></div>
                                <span class='badge <%# GetStatusClass(Eval("Status").ToString()) %>'><%# Eval("Status") %></span>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <div class="col-md-8 d-flex flex-column h-100">
                <asp:Panel ID="pnlMessageDetail" runat="server" Visible="false" CssClass="flex-grow-1 d-flex flex-column">
                    <div class="p-3 border-bottom bg-light">
                        <h5><asp:Label ID="lblSubject" runat="server" /></h5>
                        <small>From: <asp:Label ID="lblUserEmail" runat="server" /></small>
                    </div>
                    
                    <div class="p-4 flex-grow-1 overflow-auto bg-white">
                        <div class="card mb-3 border-0 bg-light p-3">
                            <asp:Literal ID="litMessageContent" runat="server" />
                        </div>
                    </div>

                    <div class="p-3 border-top mt-auto">
                        <div class="d-flex gap-2 mb-3">
                            <asp:Button ID="btnMarkInProgress" runat="server" Text="Mark In-Progress" CssClass="btn btn-outline-primary" OnClick="btnStatus_Click" />
                            <asp:Button ID="btnResolve" runat="server" Text="Resolve Ticket" CssClass="btn btn-success" OnClick="btnStatus_Click" />
                        </div>
                        <asp:TextBox ID="txtReply" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control mb-2" Placeholder="Type your reply here..."></asp:TextBox>
                        <asp:Button ID="btnSendReply" runat="server" Text="Send Reply" CssClass="btn btn-primary" />
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlNoSelection" runat="server" CssClass="text-center my-auto">
                    <p class="text-muted">Select a ticket from the inbox to view details.</p>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
