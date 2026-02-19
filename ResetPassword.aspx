<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="Respace.ResetPassword" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card" style="max-width:520px; margin:0 auto;">
        <div class="h2">Reset password</div>
        <div class="muted">Enter the OTP sent to your email, then set a new password.</div>

        <div style="height:14px"></div>

        <div class="field">
            <label class="label" for="<%= txtOtp.ClientID %>">OTP (6 digits)</label>
            <asp:TextBox ID="txtOtp" runat="server" CssClass="input" placeholder="123456" />
        </div>

        <div style="height:12px"></div>

        <div class="field">
            <label class="label" for="<%= txtNewPw.ClientID %>">New password</label>
            <asp:TextBox ID="txtNewPw" runat="server" CssClass="input" TextMode="Password" placeholder="8+ characters" />
        </div>

        <div style="height:12px"></div>

        <div class="field">
            <label class="label" for="<%= txtConfirm.ClientID %>">Confirm new password</label>
            <asp:TextBox ID="txtConfirm" runat="server" CssClass="input" TextMode="Password" placeholder="Repeat password" />
        </div>

        <div style="height:16px"></div>

        <div class="form-actions">
            <asp:Button ID="btnReset" runat="server" Text="Reset Password" CssClass="btn btn-primary" OnClick="btnReset_Click" />
            <a class="link" href="ForgotPassword.aspx">Resend OTP</a>
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="alert" />
    </div>
</asp:Content>
