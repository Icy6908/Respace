<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="Respace.ForgotPassword" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card" style="max-width:520px; margin:0 auto;">
        <div class="h2">Forgot password</div>
        <div class="muted">Enter your email. We'll send you a 6-digit OTP.</div>

        <div style="height:14px"></div>

        <div class="field">
            <label class="label" for="<%= txtEmail.ClientID %>">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" placeholder="name@example.com" />
        </div>

        <div style="height:16px"></div>

        <div class="form-actions">
            <asp:Button ID="btnSendOtp" runat="server" Text="Send OTP" CssClass="btn btn-primary" OnClick="btnSendOtp_Click" />
            <a class="link" href="Login.aspx">Back to login</a>
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="alert" />
    </div>
</asp:Content>
