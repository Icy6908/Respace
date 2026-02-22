<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HostBlockDates.aspx.cs"
    Inherits="Respace.HostBlockDates" MasterPageFile="~/Site.Master" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>

    <style>
        .hb-wrap { max-width: 980px; margin: 0 auto; padding: 14px; }
        .card { background:#fff; border-radius:16px; padding:16px; box-shadow:0 6px 18px rgba(0,0,0,0.08); margin-bottom:16px; }
        .h3 { margin:0 0 10px; font-size:20px; font-weight:800; }
        .muted { color:#777; }
        .small { font-size:0.92rem; }
        .input { width:100%; padding:10px; border-radius:10px; border:1px solid #ddd; }
        .row { display:flex; gap:12px; flex-wrap:wrap; align-items:flex-end; }
        .col { min-width: 220px; flex: 1; }
        .btn { padding:10px 14px; border-radius:12px; border:none; cursor:pointer; font-weight:800; }
        .btn-primary { background:#ff5a5f; color:#fff; }
        .btn-secondary { background:#666; color:#fff; }
        .table { width:100%; border-collapse:collapse; margin-top:10px; }
        .table th, .table td { padding:10px; border-bottom:1px solid #eee; text-align:left; }
        .link { color:#111; text-decoration:underline; cursor:pointer; font-weight:700; }
        .danger { color:#b10016; }
        .alert { display:block; margin: 10px 0 14px; padding: 10px 12px; border-radius: 10px; }
        .alert.error { background:#ffe9ea; color:#a30014; border:1px solid #ffc9cf; }
        .alert.success { background:#e9fff1; color:#0b6b2a; border:1px solid #b9f2cd; }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="hb-wrap">

        <asp:Label ID="lblMsg" runat="server" />

        <asp:HiddenField ID="hfStart" runat="server" />
        <asp:HiddenField ID="hfEnd" runat="server" />


        <asp:HiddenField ID="hfDisabledDates" runat="server" />

        <div class="card">
            <h2 class="h3">Block dates for: <asp:Label ID="lblSpaceName" runat="server" /></h2>
            <div class="muted small" style="margin-bottom:12px;">
                Grey days are unavailable (already booked or already blocked).
                End date is checkout (exclusive).
            </div>

            <div class="row">
                <div class="col">
                    <div class="muted small">START</div>
                    <asp:TextBox ID="txtStart" runat="server" ClientIDMode="Static" CssClass="input" ReadOnly="true" />
                </div>

                <div class="col">
                    <div class="muted small">END (checkout)</div>
                    <asp:TextBox ID="txtEnd" runat="server" ClientIDMode="Static" CssClass="input" ReadOnly="true" />
                </div>

                <div class="col" style="min-width:260px;">
                    <div class="muted small">REASON (optional)</div>
                    <asp:TextBox ID="txtReason" runat="server" CssClass="input" />
                </div>

                <div style="min-width:180px;">
                    <asp:Button ID="btnAdd" runat="server" Text="Block dates"
                        CssClass="btn btn-primary" OnClick="btnAdd_Click" />
                </div>
            </div>

            <div style="margin-top:14px;">
                <div id="calBlock"></div>
                <div class="muted small" style="margin-top:8px;">
                    Tip: Click 1 day to block that single day automatically.
                </div>
            </div>
        </div>

        <div class="card">
            <h3 class="h3">Existing blocks</h3>

            <asp:GridView ID="gvBlocks" runat="server"
                AutoGenerateColumns="false"
                CssClass="table"
                GridLines="None"
                DataKeyNames="BlockId"
                OnRowCommand="gvBlocks_RowCommand">
                <Columns>
                    <asp:BoundField DataField="StartDate" HeaderText="Start" DataFormatString="{0:dd MMM yyyy}" />
                    <asp:BoundField DataField="EndDate" HeaderText="End (checkout)" DataFormatString="{0:dd MMM yyyy}" />
                    <asp:BoundField DataField="Reason" HeaderText="Reason" />

                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnUnblock" runat="server"
                                Text="Unblock"
                                CssClass="link danger"
                                CommandName="Unblock"
                                CommandArgument='<%# Eval("BlockId") %>'
                                OnClientClick="return confirm('Unblock these dates?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

    </div>

    <script>
        (function () {
            var startBox = document.getElementById("txtStart");
            var endBox = document.getElementById("txtEnd");
            var hfStart = document.getElementById("<%= hfStart.ClientID %>");
            var hfEnd = document.getElementById("<%= hfEnd.ClientID %>");

         
            var rawDisabled = document.getElementById("<%= hfDisabledDates.ClientID %>").value || "";
            var disabledDays = rawDisabled.split(",").map(x => x.trim()).filter(Boolean);

            var fp = flatpickr("#calBlock", {
                inline: true,
                mode: "range",
                minDate: "today",
                dateFormat: "Y-m-d",
                disable: disabledDays,
                onChange: function (selectedDates, dateStr, instance) {
                    if (selectedDates.length >= 1) {
                        var s = instance.formatDate(selectedDates[0], "Y-m-d");
                        startBox.value = s;
                        hfStart.value = s;
                    } else {
                        startBox.value = "";
                        hfStart.value = "";
                    }

                    if (selectedDates.length >= 2) {
                        var e = instance.formatDate(selectedDates[1], "Y-m-d");
                        endBox.value = e;
                        hfEnd.value = e;
                    } else if (selectedDates.length === 1) {
                    
                        var d = new Date(selectedDates[0].getTime());
                        d.setDate(d.getDate() + 1);
                        var yyyy = d.getFullYear();
                        var mm = String(d.getMonth() + 1).padStart(2, "0");
                        var dd = String(d.getDate()).padStart(2, "0");
                        var e2 = yyyy + "-" + mm + "-" + dd;
                        endBox.value = e2;
                        hfEnd.value = e2;
                    } else {
                        endBox.value = "";
                        hfEnd.value = "";
                    }
                }
            });
        })();
    </script>

</asp:Content>
