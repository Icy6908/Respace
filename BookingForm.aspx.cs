using System;

namespace Respace
{
    // Code-behind for BookingForm.aspx
    public partial class BookingForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // --------------------------------------------
            // This ensures the code runs ONLY on first load
            // and not again when the user clicks buttons
            // --------------------------------------------
            if (!IsPostBack)
            {
                // --------------------------------------------
                // TEMPORARY / HARDCODED ROOM DATA
                // --------------------------------------------
                // This is hardcoded for now so the page works
                // even before database / room module is merged.
                //
                // 👉 FUTURE CHANGE:
                // Replace these hardcoded values with:
                // - Database query
                // - Data passed from Search page
                // - Room object from shared model
                // --------------------------------------------

                lblRoomName.Text = "Room Name: Ocean View Meeting Room 1";
                lblRoomType.Text = "Type: Meeting Room";
                lblPrice.Text = "Price: $150 per hour";
                lblDescription.Text = "Description: Spacious room with sea view, suitable for meetings.";
            }
        }

        protected void btnBook_Click(object sender, EventArgs e)
        {
            // --------------------------------------------
            // Capture user input from booking form
            // --------------------------------------------
            string startDate = txtStartDate.Text;
            string endDate = txtEndDate.Text;

            // --------------------------------------------
            // TEMPORARY ACTION
            // --------------------------------------------
            // For now, we are NOT saving anything.
            // This prevents conflict with teammates' work.
            //
            // FUTURE CHANGE:
            // Replace this with:
            // - Booking creation logic
            // - Database insert
            // - Email confirmation trigger
            // --------------------------------------------

            // Simple confirmation message (safe placeholder)
            Response.Write("<script>alert('Booking submitted successfully (placeholder).');</script>");
        }
    }
}
