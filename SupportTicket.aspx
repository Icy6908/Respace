<%@ Page Title="Support Hub" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SupportTicket.aspx.cs" Inherits="Respace.SupportTicket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style type="text/css">
        /* Your existing styles remain... */
        .support-wrapper { padding: 60px 0; background: linear-gradient(135deg, #f8f9ff 0%, #e2e8f0 100%); min-height: 90vh; }
        .premium-card { background: #ffffff; border-radius: 30px; box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.08); overflow: hidden; margin: 0 auto; max-width: 900px; }
        .card-inner { padding: 60px; }
        .section-title { color: #1a202c; font-weight: 700; font-size: 2.5rem; }
        .professional-input { width: 100%; border: 2px solid #edf2f7; border-radius: 16px; padding: 20px 25px; background-color: #f7fafc; }
        .label-text { font-weight: 600; color: #4a5568; text-transform: uppercase; letter-spacing: 0.5px; }
        .btn-premium { background: linear-gradient(90deg, #ff4d6d, #ff758c); color: white; border-radius: 16px; padding: 22px; font-weight: 700; width: 100%; cursor: pointer; border:none; }

        /* NEW: History Styles */
        .history-section { margin-top: 50px; border-top: 2px solid #f1f5f9; padding-top: 40px; }
        .ticket-card { background: #f8fafc; border-radius: 20px; padding: 25px; margin-bottom: 20px; border: 1px solid #e2e8f0; }
        .status-badge { padding: 6px 12px; border-radius: 10px; font-size: 0.8rem; font-weight: 700; text-transform: uppercase; }
        .status-pending { background: #fef3c7; color: #92400e; }
        .status-resolved { background: #dcfce7; color: #166534; }
        .admin-reply-box { background: #ffffff; border-left: 4px solid #ff4d6d; padding: 15px; margin-top: 15px; border-radius: 8px; font-style: italic; }
    </style>

    <div class="container support-wrapper">
        <div class="row justify-content-center">
            <div class="col-lg-10"> 
                <div class="card premium-card">
                    <div class="card-inner">
                        <div class="text-center mb-5">
                            <h1 class="section-title">Support Hub</h1>
                            <p class="text-muted">Hello, <strong><%= Session["FullName"] %></strong>. Please describe your issue below.</p>
                        </div>

                        <div class="mb-4">
                            <label class="label-text">Subject</label>
                            <asp:TextBox ID="txtSubject" runat="server" CssClass="professional-input" placeholder="e.g. Missing Function" required="required" />
                        </div>

                        <div class="mb-4">
                            <label class="label-text">Message</label>
                            <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Rows="6" CssClass="professional-input" placeholder="Elaborate on the problem..." required="required" />
                        </div>

                        <div class="mb-4">
                            <label class="label-text">Screenshots (Optional)</label>
                            <div class="file-upload-wrapper">
                                <asp:FileUpload ID="fileAttachment" runat="server" CssClass="form-control" />
                            </div>
                        </div>

                        <asp:Button ID="Button1" runat="server" Text="Submit Request" CssClass="btn-premium" OnClick="btnSubmit_Click" />
                        
                        <div class="text-center mt-3">
                            <asp:Label ID="lblStatus" runat="server" />
                        </div>

                        <div class="history-section">
                            <h3 class="mb-4 fw-bold">My Previous Tickets</h3>
                            <asp:Repeater ID="rptHistory" runat="server">
                                <ItemTemplate>
                                    <div class="ticket-card shadow-sm mb-4">
                                        <h5 class="fw-bold mb-1"><%# Eval("Subject") %></h5>
                
                                        <p class="mb-3 text-dark"><%# Eval("Message") %></p>
                
                                        <div class="d-flex align-items-center gap-3 mb-2">
                                            <span class='status-badge <%# Eval("Status").ToString() == "Pending" ? "status-pending" : "status-resolved" %>'>
                                                <%# Eval("Status") %>
                                            </span>
                                            <span class="text-muted small">
                                                <i class="far fa-calendar-alt me-1"></i><%# Eval("SubmittedAt", "{0:MMM dd, yyyy HH:mm}") %>
                                            </span>
                                        </div>

                                        <%# Eval("AdminReply") != DBNull.Value ? 
                                            "<div class='admin-reply-box mt-3'><strong><i class='fas fa-reply me-2'></i>Admin Response:</strong><br/><div class='mt-1'>" + Eval("AdminReply") + "</div></div>" : "" %>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>