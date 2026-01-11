<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Review.aspx.cs"
    Inherits="Respace.Review" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .card {
            max-width: 520px;
            margin: 30px auto;
            background: white;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        }

        .stars { display:flex; flex-direction: row-reverse; gap: 6px; justify-content:flex-end; }
        .stars input { display:none; }
        .stars label { font-size: 32px; color:#ddd; cursor:pointer; }
        .stars input:checked ~ label { color:#ff385c; }
        .stars label:hover, .stars label:hover ~ label { color:#ff385c; }

        textarea { width:100%; border-radius: 8px; padding: 8px; border:1px solid #ddd; }
        .btn { background:#ff385c; color:white; border:none; padding:10px 16px; border-radius:8px; cursor:pointer; width:100%; }
        .muted { color:#777; }
    </style>

    <div class="card">
        <h2>Leave a Review</h2>
        <div class="muted">
            Space: <asp:Label ID="lblSpaceName" runat="server" />
        </div>
        <br />

        <div class="stars">
            <input type="radio" name="rating" id="star5" value="5" /><label for="star5">★</label>
            <input type="radio" name="rating" id="star4" value="4" /><label for="star4">★</label>
            <input type="radio" name="rating" id="star3" value="3" /><label for="star3">★</label>
            <input type="radio" name="rating" id="star2" value="2" /><label for="star2">★</label>
            <input type="radio" name="rating" id="star1" value="1" /><label for="star1">★</label>
        </div>

        <br />

        <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Rows="4"
            placeholder="Leave a review (optional)..."></asp:TextBox>

        <br /><br />

        <asp:Button ID="btnSubmit" runat="server" Text="Submit Review"
            CssClass="btn" OnClick="btnSubmit_Click" />

        <br /><br />
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
    </div>

</asp:Content>
