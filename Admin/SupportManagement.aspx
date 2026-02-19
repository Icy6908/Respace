<%@ Page Title="Support Management" Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" CodeBehind="SupportManagement.aspx.cs" Inherits="Respace.Admin.SupportManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* Modern Evidence Thumbnails */
        .support-thumb {
            width: 60px;
            height: 60px;
            object-fit: cover;
            border-radius: 8px;
            transition: all 0.3s ease;
            cursor: zoom-in;
            border: 1px solid #ddd;
        }
        .support-thumb:hover {
            transform: scale(1.8);
            box-shadow: 0 10px 20px rgba(0,0,0,0.2);
            z-index: 100;
            position: relative;
        }
        /* Delete Button Styling */
        .btn-delete {
            color: #e74a3b;
            background: none;
            border: none;
            transition: all 0.2s;
            cursor: pointer;
            padding: 0 10px;
            font-size: 1.1rem;
        }
        .btn-delete:hover {
            color: #be2617;
            transform: scale(1.2);
        }
    </style>

    <div class="container-fluid">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="h3 mb-0 text-gray-800">Support Hub Management</h2>
            
            <div class="d-flex gap-2 w-50 justify-content-end">
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-select w-25" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All Status" Value="All"></asp:ListItem>
                    <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                    <asp:ListItem Text="Resolved" Value="Resolved"></asp:ListItem>
                </asp:DropDownList>

                <asp:TextBox ID="txtSearchSupport" runat="server" CssClass="form-control w-50" 
                    Placeholder="Search user, subject or ID..." AutoPostBack="true" OnTextChanged="txtSearchSupport_TextChanged"></asp:TextBox>
            </div>
        </div>

        <div class="card shadow-sm border-0 overflow-hidden">
            <div class="table-responsive">
                <asp:GridView ID="gvSupport" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover align-middle mb-0" GridLines="None" OnRowCommand="gvSupport_RowCommand">
                    <HeaderStyle CssClass="bg-light text-muted small fw-bold text-uppercase" />
                    <Columns>
                        <%-- ID Column with Delete --%>
                        <asp:TemplateField HeaderText="ID" ItemStyle-CssClass="ps-4">
                            <ItemTemplate>
                                <div class="d-flex align-items-center">
                                    <asp:LinkButton runat="server" CommandName="DeleteTicket" 
                                        CommandArgument='<%# Eval("QueryID") %>' CssClass="btn-delete"
                                        OnClientClick="return confirm('Are you sure you want to permanently delete this ticket?');">
                                        <i class="fas fa-trash-alt"></i>
                                    </asp:LinkButton>
                                    <span class="fw-bold">#<%# Eval("QueryID") %></span>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <%-- User & Message --%>
                        <asp:TemplateField HeaderText="Inquiry Details">
                            <ItemTemplate>
                                <div class="fw-bold text-dark"><%# Eval("FullName") %></div>
                                <div class="small text-primary fw-semibold"><%# Eval("Subject") %></div>
                                <div class="text-muted mt-1 small" style="max-width: 280px; white-space: normal; line-height: 1.2;">
                                    "<%# Eval("Message") %>"
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Evidence --%>
                        <asp:TemplateField HeaderText="Evidence">
                            <ItemTemplate>
                                <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("AttachmentUrl")?.ToString()) %>'>
                                    <a href='<%# ResolveUrl(Eval("AttachmentUrl")?.ToString()) %>' target="_blank">
                                        <img src='<%# ResolveUrl(Eval("AttachmentUrl")?.ToString()) %>' class="support-thumb" />
                                    </a>
                                </asp:PlaceHolder>
                                <asp:Label runat="server" Visible='<%# string.IsNullOrEmpty(Eval("AttachmentUrl")?.ToString()) %>' 
                                    Text="No Image" CssClass="text-muted small fst-italic" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Actions --%>
                        <asp:TemplateField HeaderText="Resolution" ItemStyle-Width="320px">
                            <ItemTemplate>
                                <div class="p-2">
                                    <asp:PlaceHolder ID="phReplyArea" runat="server" Visible='<%# Eval("Status").ToString() != "Resolved" %>'>
                                        <asp:TextBox ID="txtReply" runat="server" TextMode="MultiLine" Rows="2" 
                                            CssClass="form-control form-control-sm mb-2" Placeholder="Write your response..."></asp:TextBox>
                                        <asp:Button runat="server" CommandName="SubmitReply" CommandArgument='<%# Eval("QueryID") %>' 
                                            Text="Resolve & Reply" CssClass="btn btn-primary btn-sm w-100 shadow-sm" />
                                    </asp:PlaceHolder>

                                    <asp:PlaceHolder ID="phResolvedMsg" runat="server" Visible='<%# Eval("Status").ToString() == "Resolved" %>'>
                                        <div class="small text-success fw-bold mb-1"><i class="fas fa-check-circle"></i> Resolved</div>
                                        <div class="p-2 bg-light rounded small text-muted border shadow-sm" style="font-size: 0.85rem;">
                                            <%# Eval("AdminReply") %>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Status Badge --%>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='badge rounded-pill <%# GetSupportStatusClass(Eval("Status").ToString()) %>'>
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>