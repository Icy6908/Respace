<%@ Page Title="Listings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Listings.aspx.cs" Inherits="Respace.Listings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Listing & Space Management</h2>
    
    <div class="row mb-3">
        <div class="col-md-4">
            <asp:DropDownList ID="ddlCategoryFilter" runat="server" AutoPostBack="true" 
                OnSelectedIndexChanged="ddlCategoryFilter_SelectedIndexChanged" CssClass="form-control">
                <asp:ListItem Text="-- All Categories --" Value="All"></asp:ListItem>
                <asp:ListItem Text="Rooms" Value="Rooms"></asp:ListItem>
                <asp:ListItem Text="Studios" Value="Studios"></asp:ListItem>
                <asp:ListItem Text="Offices" Value="Offices"></asp:ListItem> <asp:ListItem Text="Event Spaces" Value="Event Spaces"></asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <asp:GridView ID="gvListings" runat="server" AutoGenerateColumns="False" DataKeyNames="ListingID" OnRowCommand="gvListings_RowCommand"
    CssClass="table table-hover table-striped custom-listing-style">
        <Columns>
            <asp:BoundField DataField="ListingID" HeaderText="ID" />
            <asp:BoundField DataField="Title" HeaderText="Space Name" />
            <asp:BoundField DataField="Category" HeaderText="Category" />
            <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <asp:Label ID="lblStatus" runat="server" 
                        Text='<%# Eval("Status") %>' 
                        CssClass='<%# Eval("Status").ToString() == "Approved" ? "badge bg-success" : 
                                     Eval("Status").ToString() == "Rejected" ? "badge bg-danger" : 
                                     "badge bg-warning text-dark" %>'>
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
           <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:PlaceHolder ID="phPendingActions" runat="server" 
                        Visible='<%# Eval("Status").ToString() == "Pending" %>'>
            
                        <asp:Button ID="btnApprove" runat="server" Text="Approve" 
                            CommandName="ApproveListing" CommandArgument='<%# Eval("ListingID") %>' 
                            CssClass="btn btn-success btn-sm" />
            
                        <asp:Button ID="btnReject" runat="server" Text="Reject" 
                            CommandName="RejectListing" CommandArgument='<%# Eval("ListingID") %>' 
                            CssClass="btn btn-warning btn-sm" />
                    </asp:PlaceHolder>

                    <asp:Button ID="btnDelete" runat="server" Text="Remove" 
                        CommandName="DeleteListing" CommandArgument='<%# Eval("ListingID") %>' 
                        CssClass="btn btn-danger btn-sm" 
                        OnClientClick="return confirm('Are you sure you want to remove this listing?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
