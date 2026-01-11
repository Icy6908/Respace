<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BookingForm.aspx.cs" Inherits="Respace.BookingForm" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <style>
    body {
        font-family: Arial;
        background: #2f2f3f;
        color: white;
        padding: 30px;
    }

    .top-bar {
        display: flex;
        gap: 10px;
        margin-bottom: 20px;
    }

    .filter-panel {
        background: #3a3a4f;
        padding: 20px;
        border-radius: 12px;
        width: 600px;
        margin-bottom: 30px;
    }

    .filter-title {
        margin-top: 15px;
        margin-bottom: 10px;
        font-weight: bold;
    }

    .space-card {
        border: 1px solid #555;
        padding: 15px;
        border-radius: 10px;
        margin-bottom: 15px;
    }

    .btn {
        padding: 8px 16px;
        border-radius: 8px;
        border: none;
        cursor: pointer;
    }

    .btn-primary { background: #ffb6c1; }
    .btn-secondary { background: #777; color: white; }
</style>
    <title>Booking Form</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Book This Room</h2>
        
        <asp:Label ID="lblRoomName" runat="server" Text=""></asp:Label><br />
        <asp:Label ID="lblRoomType" runat="server" Text=""></asp:Label><br />
        <asp:Label ID="lblPrice" runat="server" Text=""></asp:Label><br />
        <asp:Label ID="lblDescription" runat="server" Text=""></asp:Label><br />

        <asp:TextBox ID="txtStartDate" runat="server" placeholder="Start Date"></asp:TextBox><br />
        <asp:TextBox ID="txtEndDate" runat="server" placeholder="End Date"></asp:TextBox><br />

        <asp:Button ID="btnBook" runat="server" Text="Book Now" OnClick="btnBook_Click" />
    </form>
</body>
</html>
