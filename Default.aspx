<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Respace.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mb-4">Admin Dashboard Overview</h2>
        
        <div class="row">
            <div class="col-md-3">
                <div class="card text-white bg-primary mb-3">
                    <div class="card-body">
                        <h5 class="card-title">Total Users</h5>
                        <p class="card-text fs-2"><asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label></p>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card text-white bg-warning mb-3">
                    <div class="card-body">
                        <h5 class="card-title">Pending Spaces</h5>
                        <p class="card-text fs-2"><asp:Label ID="lblPendingSpaces" runat="server" Text="0"></asp:Label></p>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card text-white bg-success mb-3">
                    <div class="card-body">
                        <h5 class="card-title">Total Revenue</h5>
                        <p class="card-text fs-2"><asp:Label ID="lblRevenue" runat="server" Text="$0.00"></asp:Label></p>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card text-white bg-danger mb-3">
                    <div class="card-body">
                        <h5 class="card-title">Open Queries</h5>
                        <p class="card-text fs-2"><asp:Label ID="lblOpenQueries" runat="server" Text="0"></asp:Label></p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>