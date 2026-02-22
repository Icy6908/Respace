<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="Respace.Admin.AdminDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <h2 class="mb-4">Admin Dashboard Overview</h2>
        
        <div class="row">
            <div class="col-md-3 mb-4">
                <div class="card admin-card bg-primary text-white p-3 shadow-sm h-100">
                    <h6 class="text-uppercase small">Total Users</h6>
                    <h2><asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label></h2>
                </div>
            </div>
            <div class="col-md-3 mb-4">
                <div class="card admin-card bg-info text-white p-3 shadow-sm h-100">
                    <h6 class="text-uppercase small">Host-to-Guest Ratio</h6>
                    <h2><asp:Label ID="lblUserRatio" runat="server" Text="0:0"></asp:Label></h2>
                </div>
            </div>
            <div class="col-md-3 mb-4">
                <div class="card admin-card bg-danger text-white p-3 shadow-sm h-100">
                    <h6 class="text-uppercase small">User Churn Rate</h6>
                    <h2><asp:Label ID="lblChurnRate" runat="server" Text="0%"></asp:Label></h2>
                </div>
            </div>
            <div class="col-md-3 mb-4">
                <a href="SupportManagement.aspx?filter=Pending" style="text-decoration: none;">
                    <div class="card admin-card bg-secondary text-white p-3 shadow-sm h-100 border-0">
                        <h6 class="text-uppercase small">Open Support Tickets</h6>
                        <div class="d-flex justify-content-between align-items-center">
                            <h2><asp:Label ID="lblOpenQueries" runat="server" Text="0"></asp:Label></h2>
                            <i class="fas fa-external-link-alt small opacity-50"></i>
                        </div>
                    </div>
                </a>
            </div>
        </div>

        <div class="row">
            <div class="col-md-4 mb-4">
                <div class="card border-0 shadow-sm p-3" style="border-left: 5px solid #f1c40f;">
                    <h6 class="text-muted text-uppercase small">Pending Approvals</h6>
                    <h3 class="text-warning"><asp:Label ID="lblPendingSpaces" runat="server" Text="0"></asp:Label></h3>
                </div>
            </div>
            <div class="col-md-4 mb-4">
                <div class="card border-0 shadow-sm p-3" style="border-left: 5px solid #3498db;">
                    <h6 class="text-muted text-uppercase small">Confirmed Bookings</h6>
                    <h3 class="text-primary"><asp:Label ID="lblConfirmedBookings" runat="server" Text="0"></asp:Label></h3>
                </div>
            </div>
            <div class="col-md-4 mb-4">
                <div class="card border-0 shadow-sm p-3" style="border-left: 5px solid #9b59b6;">
                    <h6 class="text-muted text-uppercase small">Space Utilization</h6>
                    <h3 class="text-purple"><asp:Label ID="lblUtilization" runat="server" Text="0%"></asp:Label></h3>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-4 mb-4">
                <div class="card border-0 shadow-sm p-3" style="border-left: 5px solid #27ae60;">
                    <h6 class="text-muted text-uppercase small">Total Transaction Vol.</h6>
                    <h3 class="text-success">$<asp:Label ID="lblTotalRevenue" runat="server" Text="0.00"></asp:Label></h3>
                </div>
            </div>
            <div class="col-md-4 mb-4">
                <div class="card border-0 shadow-sm p-3" style="border-left: 5px solid #2ecc71;">
                    <h6 class="text-muted text-uppercase small">Net Commission (10%)</h6>
                    <h3 class="text-success">$<asp:Label ID="lblNetCommission" runat="server" Text="0.00"></asp:Label></h3>
                </div>
            </div>
            <div class="col-md-4 mb-4">
                <div class="card border-0 shadow-sm p-3" style="border-left: 5px solid #e67e22;">
                    <h6 class="text-muted text-uppercase small">Avg Order Value (AOV)</h6>
                    <h3 class="text-warning">$<asp:Label ID="lblAOV" runat="server" Text="0.00"></asp:Label></h3>
                </div>
            </div>
        </div>

        <div class="row mt-2">
            <div class="col-md-8 mb-4">
                <div class="card admin-card p-4 shadow-sm h-100">
                    <h5 class="mb-3 font-weight-bold">Financial Growth Trend</h5>
                    <div style="height: 300px; position: relative;">
                        <canvas id="revenueChart"></canvas>
                    </div>
                </div>
            </div>
            <div class="col-md-4 mb-4">
                <div class="card admin-card p-4 shadow-sm h-100">
                    <h5 class="mb-3 font-weight-bold">Space Mix (By Category)</h5>
                    <div style="height: 300px; position: relative;">
                        <canvas id="categoryChart"></canvas>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-2">
            <div class="col-md-7 mb-4">
                <div class="card admin-card p-4 shadow-sm h-100">
                    <h5 class="mb-3 font-weight-bold">Gross Revenue by Space Type</h5>
                    <div style="height: 300px; position: relative;">
                        <canvas id="revenueByTypeChart"></canvas>
                    </div>
                </div>
            </div>
            <div class="col-md-5 mb-4">
                <div class="card admin-card p-4 shadow-sm h-100">
                    <h5 class="mb-3 font-weight-bold">Booking Success Rate (%)</h5>
                    <div style="height: 300px; position: relative;">
                        <canvas id="successRateChart"></canvas>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            
            const revCtx = document.getElementById('revenueChart').getContext('2d');
            new Chart(revCtx, {
                type: 'line',
                data: {
                    labels: ['Sep', 'Oct', 'Nov', 'Dec', 'Jan', 'Feb'],
                    datasets: [{
                        label: 'Gross Vol.',
                        data: [500, 800, 600, 1200, 1500, 1800],
                        borderColor: '#2ecc71',
                        backgroundColor: 'rgba(46, 204, 113, 0.1)',
                        fill: true,
                        tension: 0.4
                    }, {
                        label: 'Net Comm.',
                        data: [50, 80, 60, 120, 150, 180],
                        borderColor: '#3498db',
                        borderDash: [5, 5],
                        fill: false
                    }]
                },
                options: { maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
            });

     
            const catCtx = document.getElementById('categoryChart').getContext('2d');
            new Chart(catCtx, {
                type: 'doughnut',
                data: {
                    labels: <%= CategoryLabels %>,
                    datasets: [{
                        data: <%= CategoryData %>,
                        backgroundColor: ['#3498db', '#f1c40f', '#9b59b6', '#e74c3c', '#2ecc71'],
                        borderWidth: 0
                    }]
                },
                options: { maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
            });

            
            const revTypeCtx = document.getElementById('revenueByTypeChart').getContext('2d');
            new Chart(revTypeCtx, {
                type: 'bar',
                data: {
                    labels: <%= RevenueTypeLabels %>, 
                    datasets: [{
                        label: 'Gross Revenue ($)',
                        data: <%= RevenueTypeData %>,
                        backgroundColor: '#3498db',
                        borderRadius: 5
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: { y: { beginAtZero: true } }
                }
            });

       
            const successCtx = document.getElementById('successRateChart').getContext('2d');
            new Chart(successCtx, {
                type: 'line',
                data: {
                    labels: ['Sep', 'Oct', 'Nov', 'Dec', 'Jan', 'Feb'],
                    datasets: [{
                        label: 'Success %',
                        data: [75, 82, 78, 85, 89, 94],
                        borderColor: '#e67e22',
                        backgroundColor: 'rgba(230, 126, 34, 0.1)',
                        tension: 0.3,
                        fill: true
                    }]
                },
                options: { maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
            });
        });
    </script>
</asp:Content>