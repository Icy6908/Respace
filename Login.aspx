<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Respace.Login" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card" style="max-width:520px; margin: 0 auto;">
        <div class="h2">Welcome back</div>
        <div class="muted">Sign in to manage bookings, rewards, and your listings.</div>

        <asp:ValidationSummary ID="vsLogin" runat="server" CssClass="alert" ValidationGroup="login" />

        <div style="height:14px"></div>

        <div class="field">
            <label class="label" for="<%= txtEmail.ClientID %>">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" placeholder="name@example.com" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." CssClass="val" ValidationGroup="login" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Enter a valid email (must include @)." CssClass="val" ValidationGroup="login" Display="Dynamic"
                ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$" />
        </div>

        <div style="height:12px"></div>

        <div class="field">
            <label class="label" for="<%= txtPassword.ClientID %>">Password</label>
            <div class="password-row">
                <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" placeholder="••••••••" />
                <button class="toggle" type="button" data-toggle="pw">Show</button>
            </div>
            <asp:RequiredFieldValidator ID="rfvPw" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." CssClass="val" ValidationGroup="login" Display="Dynamic" />
            <asp:CustomValidator ID="cvPw" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password must be at least 8 characters." CssClass="val" ValidationGroup="login" Display="Dynamic"
                ClientValidationFunction="validatePwLen" />
        </div>

        <div style="height:16px"></div>

        <div class="form-actions">
            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary" ValidationGroup="login" OnClick="btnLogin_Click" />
            <a class="link" href="Register.aspx">Create an account</a>
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="alert" />
    </div>

    <script>
        function validatePwLen(sender, args) {
            args.IsValid = (args.Value || '').length >= 8;
        }

        (function () {
            var email = document.getElementById('<%= txtEmail.ClientID %>');
            var pw = document.getElementById('<%= txtPassword.ClientID %>');
            var toggle = document.querySelector('[data-toggle="pw"]');

            function mark(el, ok) {
                if (!el) return;
                el.classList.remove('valid', 'invalid');
                if (ok === true) el.classList.add('valid');
                if (ok === false) el.classList.add('invalid');
            }

            function isEmailOk(v) {
                return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test((v || '').trim());
            }

            email && email.addEventListener('input', function () { mark(email, isEmailOk(email.value)); });
            pw && pw.addEventListener('input', function () { mark(pw, (pw.value || '').length >= 8); });

            if (toggle && pw) {
                toggle.addEventListener('click', function () {
                    var show = pw.type === 'password';
                    pw.type = show ? 'text' : 'password';
                    toggle.textContent = show ? 'Hide' : 'Show';
                    pw.focus();
                });
            }
        })();
    </script>

</asp:Content>
