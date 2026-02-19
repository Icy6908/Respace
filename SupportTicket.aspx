<%@ Page Title="Support" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SupportTicket.aspx.cs" Inherits="Respace.SupportTicket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* Matches your site's soft gradient background */
        body {
            background: linear-gradient(135deg, #fdfcfb 0%, #e2d1f9 100%);
            min-height: 100vh;
        }

        .support-card {
            background: rgba(255, 255, 255, 0.9);
            backdrop-filter: blur(10px);
            border-radius: 20px;
            border: none;
            box-shadow: 0 10px 30px rgba(0,0,0,0.05);
            transition: transform 0.3s ease;
        }

        .support-card:hover {
            transform: translateY(-5px);
        }

        /* Signature Pink/Red Accent Color */
        .btn-submit {
            background-color: #ff4d6d;
            border: none;
            border-radius: 12px;
            padding: 12px;
            font-weight: 600;
            color: white;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(255, 77, 109, 0.3);
        }

        .btn-submit:hover {
            background-color: #ff1a4a;
            box-shadow: 0 6px 20px rgba(255, 77, 109, 0.5);
            color: white;
        }

        .form-control {
            border-radius: 12px;
            border: 1px solid #eee;
            padding: 12px 15px;
            background-color: #fcfcfc;
        }

        .form-control:focus {
            border-color: #ff4d6d;
            box-shadow: 0 0 0 0.25 margin rgba(255, 77, 109, 0.1);
        }

        .brand-dot {
            height: 10px;
            width: 10px;
            background-color: #ff4d6d;
            border-radius: 50%;
            display: inline-block;
            margin-right: 8px;
        }
    </style>

    <div class="container py-5">
        <div class="row justify-content-center">
            <div class="col-md-5">
                <div class="card support-card p-4">
                    <div class="text-center mb-4">
                        <div class="brand-dot"></div>
                        <h2 class="d-inline fw-bold" style="color: #333;">Support Hub</h2>
                        <p class="text-muted mt-2">How can we help you today, <%= Session["FullName"] %>?</p>
                    </div>

                    <div class="mb-3">
                        <label class="form-label small fw-bold text-uppercase text-muted">Subject</label>
                        <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" placeholder="e.g., Booking Issue, Refund Query" />
                    </div>

                    <div class="mb-4">
                        <label class="form-label small fw-bold text-uppercase text-muted">Detailed Message</label>
                        <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control" placeholder="Describe your issue in detail..." />
                    </div>

                    <asp:Button ID="btnSubmit" runat="server" Text="Send Ticket" CssClass="btn btn-submit w-100" OnClick="btnSubmit_Click" />
                    
                    <div class="text-center mt-3">
                         <asp:Label ID="lblStatus" runat="server" CssClass="small fw-bold" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>