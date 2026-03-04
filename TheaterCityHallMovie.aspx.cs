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
    public partial class TheaterCityHallMovie : System.Web.UI.Page
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
                LoadCities();
            }
        }

        private void LoadCities()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            string query = "SELECT DISTINCT theater_city FROM theater ORDER BY theater_city";

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            ddlCity.Items.Clear();
                            ddlCity.Items.Add(new ListItem("Select City", ""));
                            
                            while (reader.Read())
                            {
                                ddlCity.Items.Add(new ListItem(reader["theater_city"].ToString(), reader["theater_city"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle error (you may want to add error logging or display)
                ShowMessage("Error loading cities. Please try again later.", "error");
            }
        }

        protected void btnLoadReport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlCity.SelectedValue))
            {
                ShowMessage("Please select a city.", "error");
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            string query = @"SELECT t.theater_name,
                                   t.theater_city,
                                   h.hall_name,
                                   m.movie_title,
                                   s.show_start,
                                   COUNT(s.showtime_id) OVER (PARTITION BY h.hall_id) AS total_shows
                            FROM theater t
                            JOIN hall h ON t.theater_id = h.theater_id
                            JOIN showtime s ON h.hall_id = s.hall_id
                            JOIN movie m ON s.movie_id = m.movie_id
                            WHERE t.theater_city = :city
                            ORDER BY t.theater_name, h.hall_name, s.show_start";

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":city", OracleDbType.Varchar2).Value = ddlCity.SelectedValue;
                        
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count == 0)
                            {
                                ShowMessage("No records found for the selected city.", "error");
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
                // Handle error (you may want to add error logging or display)
                ShowMessage("Error loading report. Please try again later.", "error");
            }
        }
    }
}