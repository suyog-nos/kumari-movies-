using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace kumari
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDashboardStats();
        }

        private void LoadDashboardStats()
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            
            try
            {
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.Open();

                    // Total Movies
                    string movieQuery = "SELECT COUNT(*) FROM movie";
                    using (OracleCommand cmd = new OracleCommand(movieQuery, conn))
                    {
                        lblTotalMovies.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Total Users
                    string userQuery = "SELECT COUNT(*) FROM app_user";
                    using (OracleCommand cmd = new OracleCommand(userQuery, conn))
                    {
                        lblTotalUsers.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Total Bookings
                    string bookingQuery = "SELECT COUNT(*) FROM booking";
                    using (OracleCommand cmd = new OracleCommand(bookingQuery, conn))
                    {
                        lblTotalBookings.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Total Paid Tickets
                    string paidTicketsQuery = @"SELECT COUNT(*) FROM payment p 
                                              JOIN ticket t ON p.ticket_id = t.ticket_id 
                                              WHERE p.payment_status = 'PAID'";
                    using (OracleCommand cmd = new OracleCommand(paidTicketsQuery, conn))
                    {
                        lblTotalPaidTickets.Text = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (this uses the ex variable)
                System.Diagnostics.Debug.WriteLine("Dashboard error: " + ex.ToString());
                
                // Set default values and optionally show error
                lblTotalMovies.Text = "Error";
                lblTotalUsers.Text = "Error";
                lblTotalBookings.Text = "Error";
                lblTotalPaidTickets.Text = "Error";
                
                // Show error message for debugging
                Response.Write("Dashboard loading error: " + ex.Message);
            }
        }
    }
}