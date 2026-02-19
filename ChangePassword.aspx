<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Respace.ChangePassword" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card" style="max-width:520px; margin:0 auto;">
        <div class="h2">Change password</div>
        <div class="muted">For security, we’ll email you an OTP before changing your password.</div>

        <div style="height:14px"></div>

        <div class="field">
            <label class="label" for="<%= txtCurrent.ClientID %>">Current password</label>
            <asp:TextBox ID="txtCurrent" runat="server" CssClass="input" TextMode="Password" placeholder="Current password" />
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

        <div style="height:12px"></div>

        <div class="field">
            <label class="label" for="<%= txtOtp.ClientID %>">OTP (sent to your email)</label>
            <asp:TextBox ID="txtOtp" runat="server" CssClass="input" placeholder="123456" />
        </div>

        <div style="height:16px"></div>

        <div class="form-actions">
            <asp:Button ID="btnSendOtp" runat="server" Text="Send OTP" CssClass="btn btn-outline" OnClick="btnSendOtp_Click" />
            <asp:Button ID="btnChange" runat="server" Text="Change Password" CssClass="btn btn-primary" OnClick="btnChange_Click" />
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="alert" />
    </div>
</asp:Content>
