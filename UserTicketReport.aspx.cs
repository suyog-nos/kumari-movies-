using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;

namespace kumari
{
    public partial class UserTicketReport : System.Web.UI.Page
    {
        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = $"d-block mb-2 {(type == "success" ? "alert alert-success" : "alert alert-danger")}";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT user_id FROM app_user";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            ddlUser.DataSource = reader;
                            ddlUser.DataTextField = "user_id";
                            ddlUser.DataValueField = "user_id";
                            ddlUser.DataBind();
                            ddlUser.Items.Insert(0, new ListItem("--Select User--", ""));
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle exception, e.g., log or display error message
            }
        }

        protected void btnLoadReport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlUser.SelectedValue))
            {
                ShowMessage("Please select a user.", "error");
                gvReport.DataSource = null;
                gvReport.DataBind();
                return;
            }

            int userId;
            if (!int.TryParse(ddlUser.SelectedValue, out userId))
            {
                ShowMessage("Invalid user selection.", "error");
                gvReport.DataSource = null;
                gvReport.DataBind();
                return;
            }

            try
            {
                string query = @"
SELECT b.booking_id,
       m.movie_title,
       h.hall_name,
       s.show_start,
       st.seat_label,
       t.final_price,
       p.payment_status
FROM booking b
JOIN ticket t ON b.booking_id = t.booking_id
LEFT JOIN payment p ON t.ticket_id = p.ticket_id
JOIN showtime s ON b.showtime_id = s.showtime_id
JOIN movie m ON s.movie_id = m.movie_id
JOIN hall h ON s.hall_id = h.hall_id
JOIN seat st ON t.seat_id = st.seat_id
WHERE b.user_id = :user_id
AND s.show_start >= ADD_MONTHS(SYSDATE, -6)
ORDER BY s.show_start DESC
";

                using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32)).Value = userId;
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count == 0)
                            {
                                ShowMessage("No tickets found for the selected user in the last 6 months.", "error");
                                gvReport.DataSource = null;
                                gvReport.DataBind();
                                return;
                            }
                            gvReport.DataSource = dt;
                            gvReport.DataBind();
                            ShowMessage("Report loaded successfully.", "success");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading report. Please try again later.", "error");
            }
        }
    }
}