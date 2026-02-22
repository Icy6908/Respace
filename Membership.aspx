<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Membership.aspx.cs" Inherits="Respace.Membership" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>

    .membership-page { 
        padding: 60px 20px; 
        max-width: 1200px; 
        margin: 0 auto; 
        font-family: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
    }
    
    .membership-header { margin-bottom: 50px; text-align: center; }
    .membership-header h2 { font-size: 2.5rem; color: #111827; margin-bottom: 10px; }
    .muted { color: #6b7280; font-size: 1.1rem; }

    
    .current-plan-banner { 
        background: #f0fdf4; 
        border: 1px solid #bbf7d0; 
        border-radius: 16px; 
        padding: 24px; 
        margin-bottom: 40px; 
        display: flex; 
        align-items: center; 
        justify-content: space-between;
    }
    .badge-active { background: #22c55e; color: white; padding: 6px 16px; border-radius: 30px; font-size: 0.85rem; font-weight: 700; }

    
    .grid-3 { 
        display: grid; 
        grid-template-columns: repeat(3, 1fr); 
        gap: 30px; 
        align-items: stretch;
    }


    @media (max-width: 992px) {
        .grid-3 { grid-template-columns: 1fr; }
    }

   
    .card-plan { 
        background: white; 
        border-radius: 20px; 
        padding: 40px 30px; 
        border: 1px solid #e5e7eb;
        transition: all 0.3s ease; 
        position: relative; 
        display: flex; 
        flex-direction: column;
        box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    }
    
    .card-plan:hover { 
        transform: translateY(-10px); 
        box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1); 
    }
    
    .card-plan.featured { 
        border: 2px solid #ff385c; 
        scale: 1.05; 
        z-index: 1;
    }
    
    .benefits-list { list-style: none; padding: 0; margin: 30px 0; flex-grow: 1; }
    .benefits-list li { margin-bottom: 15px; font-size: 1rem; color: #374151; display: flex; align-items: center; }
    .benefits-list li::before { 
        content: "✓"; 
        color: #ff385c; 
        font-weight: bold; 
        margin-right: 12px; 
        background: rgba(255, 56, 92, 0.1);
        width: 24px;
        height: 24px;
        display: flex;
        align-items: center;
        justify-content: center;
        border-radius: 50%;
        font-size: 0.8rem;
    }

    .price { font-size: 2.25rem; font-weight: 800; color: #111827; margin: 15px 0; }
    .price span { font-size: 1.1rem; color: #6b7280; font-weight: 400; }

   
    .btn-plan { 
        width: 100%; 
        padding: 14px; 
        border-radius: 12px; 
        font-weight: 700; 
        cursor: pointer; 
        border: 1px solid #d1d5db; 
        background: white; 
        color: #374151;
        transition: 0.2s; 
    }
    
    .btn-plan:hover:not(:disabled) { background: #f9fafb; border-color: #9ca3af; }
    
    .btn-plan-primary { 
        background: #ff385c; 
        color: white; 
        border: none; 
    }
    
    .btn-plan-primary:hover:not(:disabled) { 
        background: #e31c5f; 
        box-shadow: 0 10px 15px -3px rgba(255, 56, 92, 0.3);
    }

    .btn-plan:disabled { 
        background: #f3f4f6; 
        color: #9ca3af; 
        cursor: not-allowed; 
        border: none; 
    }

    .popular-tag {
        position: absolute; 
        top: -15px; 
        left: 50%; 
        transform: translateX(-50%); 
        background: #ff385c; 
        color: white; 
        padding: 4px 20px; 
        border-radius: 30px; 
        font-size: 0.8rem; 
        font-weight: 800;
        letter-spacing: 0.05em;
    }
</style>

<div class="membership-page">
    <div class="membership-header">
        <h2>Upgrade Your Experience</h2>
        <p class="muted">Choose the plan that fits your lifestyle.</p>
    </div>

    <asp:PlaceHolder ID="phCurrentPlan" runat="server" Visible="false">
        <div class="current-plan-banner">
            <div>
                <span style="color: #15803d; font-size: 0.85rem; font-weight: 600; text-transform: uppercase;">Your Current Plan</span>
                <h3 style="margin: 5px 0 0 0;"><asp:Label ID="lblCurrentPlanName" runat="server" /></h3>
            </div>
            <span class="badge-active">ACTIVE</span>
        </div>
    </asp:PlaceHolder>

    <div class="grid-3">
       
        <div class="card-plan">
            <h3 class="h3">Free</h3>
            <div class="price">$0<span>/mo</span></div>
            <ul class="benefits-list">
                <li>Standard booking rates</li>
                <li>Earn 1.0× points on visits</li>
            </ul>
            <asp:Button ID="btnFree" runat="server" Text="Stay Free" CssClass="btn-plan" OnClick="btnFree_Click" />
        </div>

     
        <div class="card-plan featured">
            <div class="popular-tag">MOST POPULAR</div>
            <h3 class="h3">Plus</h3>
            <div class="price">$9.90<span>/mo</span></div>
            <ul class="benefits-list">
                <li><strong>5% Off</strong>&nbsp;all bookings</li>
                <li>Earn 1.2× points faster</li>
            </ul>
            <asp:Button ID="btnPlus" runat="server" Text="Choose Plus" CssClass="btn-plan btn-plan-primary" OnClick="btnPlus_Click" />
        </div>

        
        <div class="card-plan">
            <h3 class="h3">Pro</h3>
            <div class="price">$19.90<span>/mo</span></div>
            <ul class="benefits-list">
                <li>
                    <strong>10% Off</strong>&nbsp;all bookings
                </li>
                <li>Earn 1.5× points faster</li>
            </ul>
            <asp:Button ID="btnPro" runat="server" Text="Choose Pro" CssClass="btn-plan" OnClick="btnPro_Click" />
        </div>
    </div>
</div>
</asp:Content>