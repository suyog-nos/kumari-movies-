using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace kumari
{
    public partial class MovieOccupancyReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadOccupancy();
            }
        }

        private void LoadOccupancy()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    
                    string sql = @"SELECT m.movie_title,
                                   t.theater_name,
                                   h.hall_name,
                                   h.hall_capacity,
                                   COUNT(s.showtime_id) AS total_shows,
                                   COUNT(CASE WHEN p.payment_status = 'PAID' THEN 1 END) AS paid_tickets,
                                   ROUND(
                                       (COUNT(CASE WHEN p.payment_status = 'PAID' THEN 1 END)
                                        * 100.0 / h.hall_capacity),
                                       2
                                   ) AS occupancy_percentage
                                   FROM showtime s
                                   JOIN movie m ON s.movie_id = m.movie_id
                                   JOIN hall h ON s.hall_id = h.hall_id
                                   JOIN theater t ON h.theater_id = t.theater_id
                                   LEFT JOIN booking b ON s.showtime_id = b.showtime_id
                                   LEFT JOIN ticket tk ON b.booking_id = tk.booking_id
                                   LEFT JOIN payment p ON tk.ticket_id = p.ticket_id
                                   GROUP BY m.movie_title, t.theater_name, h.hall_name, h.hall_capacity
                                   ORDER BY occupancy_percentage DESC";
                    
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            gvOccupancy.DataSource = dt;
                            gvOccupancy.DataBind();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception (you may want to log or display error message)
                    System.Diagnostics.Debug.WriteLine("Error loading occupancy data: " + ex.Message);
                }
            }
        }
    }
}