<%@ Page Title="Support Management" Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" CodeBehind="SupportManagement.aspx.cs" Inherits="Respace.Admin.SupportManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="h3 mb-0 text-gray-800">Support Queries</h2>
            
            <div class="d-flex gap-2 w-50 justify-content-end">
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-select w-25" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All Status" Value="All"></asp:ListItem>
                    <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                    <asp:ListItem Text="Resolved" Value="Resolved"></asp:ListItem>
                </asp:DropDownList>

                <asp:TextBox ID="txtSearchSupport" runat="server" CssClass="form-control w-50" 
                    Placeholder="Search by User or Subject..." AutoPostBack="true" OnTextChanged="txtSearchSupport_TextChanged"></asp:TextBox>
            </div>
        </div>

        <div class="card admin-card p-0 shadow-sm border-0">
            <div class="table-responsive">
                <asp:GridView ID="gvSupport" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover align-middle mb-0" GridLines="None" OnRowCommand="gvSupport_RowCommand">
                    <HeaderStyle CssClass="bg-light text-muted small fw-bold text-uppercase" />
                    <Columns>
                        <asp:BoundField DataField="QueryID" HeaderText="ID" ItemStyle-CssClass="ps-4" />
                        
                        <asp:TemplateField HeaderText="User & Subject">
                            <ItemTemplate>
                                <div class="fw-bold text-dark"><%# Eval("FullName") %></div>
                                <div class="small text-primary"><%# Eval("Subject") %></div>
                                <div class="text-muted mt-1 small" style="max-width: 300px; white-space: normal;">
                                    "<%# Eval("Message") %>"
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Admin Action">
                            <ItemTemplate>
                                <div class="p-2">
                                    <asp:PlaceHolder ID="phReplyArea" runat="server" Visible='<%# Eval("Status").ToString() != "Resolved" %>'>
                                        <asp:TextBox ID="txtReply" runat="server" TextMode="MultiLine" Rows="2" 
                                            CssClass="form-control form-control-sm mb-2" Placeholder="Type reply here..."></asp:TextBox>
                
                                        <div style="height: 20px; margin-bottom: 5px;">
                                            <asp:RequiredFieldValidator ID="rfvReply" runat="server" 
                                                ControlToValidate="txtReply" ErrorMessage="Reply cannot be empty" 
                                                ForeColor="#e74a3b" Display="Dynamic" CssClass="small fw-bold"
                                                ValidationGroup='<%# "ReplyGroup_" + Eval("QueryID") %>'></asp:RequiredFieldValidator>
                                        </div>

                                        <asp:Button runat="server" CommandName="SubmitReply" CommandArgument='<%# Eval("QueryID") %>' 
                                            Text="Send Reply" CssClass="btn btn-primary btn-sm w-100" 
                                            ValidationGroup='<%# "ReplyGroup_" + Eval("QueryID") %>' />
                                    </asp:PlaceHolder>

                                    <asp:PlaceHolder ID="phResolvedMsg" runat="server" Visible='<%# Eval("Status").ToString() == "Resolved" %>'>
                                        <div class="small text-success fw-bold mb-1"><i class="fas fa-check-circle"></i> Resolved</div>
                                        <div class="p-2 bg-light rounded small text-muted border shadow-sm" style="min-height: 60px;">
                                            <%# string.IsNullOrEmpty(Eval("AdminReply")?.ToString()) ? "No reply recorded." : Eval("AdminReply") %>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='badge <%# GetSupportStatusClass (Eval("Status").ToString()) %>'>
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