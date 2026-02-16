<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Respace.Register" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card" style="max-width:620px; margin: 0 auto;">
        <div class="h2">Create your account</div>
        <div class="muted">Join to book spaces, earn points, or host your own listings.</div>

        <asp:ValidationSummary ID="vsRegister" runat="server" CssClass="alert" ValidationGroup="reg" />

        <div style="height:14px"></div>

        <div class="form-row">
            <div class="field">
                <label class="label" for="<%= txtName.ClientID %>">Full name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="input" placeholder="e.g. Aina Ahmad" />
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ErrorMessage="Full name is required." CssClass="val" ValidationGroup="reg" Display="Dynamic" />
            </div>

            <div class="field">
                <label class="label" for="<%= ddlRole.ClientID %>">Register as</label>
                <asp:DropDownList ID="ddlRole" runat="server" CssClass="input">
                    <asp:ListItem Text="Guest" Value="Guest" />
                    <asp:ListItem Text="Host" Value="Host" />
                </asp:DropDownList>
            </div>
        </div>

        <div style="height:12px"></div>

        <div class="field">
            <label class="label" for="<%= txtEmail.ClientID %>">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" placeholder="name@example.com" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." CssClass="val" ValidationGroup="reg" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Enter a valid email (must include @)." CssClass="val" ValidationGroup="reg" Display="Dynamic"
                ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$" />
        </div>

        <div style="height:12px"></div>

        <div class="form-row">
            <div class="field">
                <label class="label" for="<%= txtPassword.ClientID %>">Password</label>
                <div class="password-row">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" placeholder="At least 8 characters" />
                    <button class="toggle" type="button" data-toggle="pw">Show</button>
                </div>
                <div class="strength" aria-hidden="true"><span id="pwBar"></span></div>
                <div id="pwHint" class="help">Use 8+ chars. Add a number + symbol for extra strength.</div>

                <asp:RequiredFieldValidator ID="rfvPw" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." CssClass="val" ValidationGroup="reg" Display="Dynamic" />
                <asp:CustomValidator ID="cvPw" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password must be at least 8 characters." CssClass="val" ValidationGroup="reg" Display="Dynamic"
                    ClientValidationFunction="validatePwLen" />
            </div>

            <div class="field">
                <label class="label" for="<%= txtConfirm.ClientID %>">Confirm password</label>
                <div class="password-row">
                    <asp:TextBox ID="txtConfirm" runat="server" CssClass="input" TextMode="Password" placeholder="Repeat password" />
                    <button class="toggle" type="button" data-toggle="confirm">Show</button>
                </div>
                <asp:RequiredFieldValidator ID="rfvConfirm" runat="server" ControlToValidate="txtConfirm" ErrorMessage="Please confirm your password." CssClass="val" ValidationGroup="reg" Display="Dynamic" />
                <asp:CompareValidator ID="cmpPw" runat="server" ControlToCompare="txtPassword" ControlToValidate="txtConfirm" ErrorMessage="Passwords do not match." CssClass="val" ValidationGroup="reg" Display="Dynamic" />
            </div>
        </div>

        <div style="height:16px"></div>

        <div class="form-actions">
            <asp:Button ID="btnRegister" runat="server" Text="Create account" CssClass="btn btn-primary" ValidationGroup="reg" OnClick="btnRegister_Click" />
            <a class="link" href="Login.aspx">I already have an account</a>
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
            var confirm = document.getElementById('<%= txtConfirm.ClientID %>');
            var pwBar = document.getElementById('pwBar');
            var pwHint = document.getElementById('pwHint');

            function mark(el, ok) {
                if (!el) return;
                el.classList.remove('valid', 'invalid');
                if (ok === true) el.classList.add('valid');
                if (ok === false) el.classList.add('invalid');
            }

            function isEmailOk(v) {
                return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test((v || '').trim());
            }

            function scorePw(v) {
                v = v || '';
                var score = 0;
                if (v.length >= 8) score++;
                if (/[A-Z]/.test(v)) score++;
                if (/[0-9]/.test(v)) score++;
                if (/[^A-Za-z0-9]/.test(v)) score++;
                return score; // 0..4
            }

            function updateStrength() {
                if (!pwBar || !pw) return;
                var s = scorePw(pw.value);
                var pct = [0, 25, 55, 80, 100][s];
                pwBar.style.width = pct + '%';
                if (pwHint) {
                    pwHint.textContent = (pw.value || '').length === 0
                        ? 'Use 8+ chars. Add a number + symbol for extra strength.'
                        : (s <= 1 ? 'Weak — add length, numbers, or symbols.' : (s === 2 ? 'Okay — add a symbol or uppercase.' : (s === 3 ? 'Strong — nice.' : 'Very strong — great.')));
                }
                mark(pw, (pw.value || '').length >= 8);
                if (confirm) mark(confirm, confirm.value.length > 0 && confirm.value === pw.value);
            }

            email && email.addEventListener('input', function () { mark(email, isEmailOk(email.value)); });
            pw && pw.addEventListener('input', updateStrength);
            confirm && confirm.addEventListener('input', function () {
                mark(confirm, confirm.value.length > 0 && confirm.value === (pw ? pw.value : ''));
            });

            document.querySelectorAll('button.toggle[data-toggle]').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    var which = btn.getAttribute('data-toggle');
                    var el = which === 'confirm' ? confirm : pw;
                    if (!el) return;
                    var show = el.type === 'password';
                    el.type = show ? 'text' : 'password';
                    btn.textContent = show ? 'Hide' : 'Show';
                    el.focus();
                });
            });

            updateStrength();
        })();
    </script>
</asp:Content>
