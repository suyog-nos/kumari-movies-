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
    public partial class ShowtimeDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadShowtimes();
                LoadMovies();
                LoadHalls();
            }
        }

        private void LoadShowtimes()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    string query = @"SELECT s.showtime_id,
       m.movie_title,
       h.hall_name,
       s.show_start
FROM showtime s
JOIN movie m ON s.movie_id = m.movie_id
JOIN hall h ON s.hall_id = h.hall_id
ORDER BY s.showtime_id";
                    using (OracleDataAdapter da = new OracleDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvShowtimes.DataSource = dt;
                        gvShowtimes.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Database connection failed. Could not load showtimes.');", true);
            }
        }

        private void LoadMovies()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT movie_id, movie_title FROM movie";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            ddlMovie.DataSource = dr;
                            ddlMovie.DataTextField = "movie_title";
                            ddlMovie.DataValueField = "movie_id";
                            ddlMovie.DataBind();
                            ddlMovie.Items.Insert(0, new ListItem("--Select Movie--", "0"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Database connection failed. Could not load movies.');", true);
            }
        }

        private void LoadHalls()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT hall_id, hall_name FROM hall";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            ddlHall.DataSource = dr;
                            ddlHall.DataTextField = "hall_name";
                            ddlHall.DataValueField = "hall_id";
                            ddlHall.DataBind();
                            ddlHall.Items.Insert(0, new ListItem("--Select Hall--", "0"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Database connection failed. Could not load halls.');", true);
            }
        }

        protected void gvShowtimes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvShowtimes.SelectedDataKey == null)
                return;

            int id = Convert.ToInt32(gvShowtimes.SelectedDataKey.Value);
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT * FROM showtime WHERE showtime_id = :showtime_id";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("showtime_id", id));
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                txtShowtimeId.Text = dr["showtime_id"].ToString();
                                ddlMovie.SelectedValue = dr["movie_id"].ToString();
                                ddlHall.SelectedValue = dr["hall_id"].ToString();
                                DateTime dt = (DateTime)dr["show_start"];
                                txtShowStart.Text = dt.ToString("yyyy-MM-ddTHH:mm");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        private void ClearForm()
        {
            txtShowtimeId.Text = "";
            ddlMovie.SelectedIndex = 0;
            ddlHall.SelectedIndex = 0;
            txtShowStart.Text = "";
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            // Validation: Check if all fields are filled
            if (string.IsNullOrEmpty(txtShowtimeId.Text) || ddlMovie.SelectedValue == "0" || ddlHall.SelectedValue == "0" || string.IsNullOrEmpty(txtShowStart.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation", "alert('All showtime fields are required.');", true);
                return;
            }

            // Validation: Check if Showtime ID is numeric
            int showtimeId;
            if (!int.TryParse(txtShowtimeId.Text, out showtimeId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation", "alert('Showtime ID must be numeric.');", true);
                return;
            }

            // Validation: Check if date is valid
            DateTime showStart;
            if (!DateTime.TryParse(txtShowStart.Text, out showStart))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation", "alert('Please enter a valid date and time for show start.');", true);
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    string query = "INSERT INTO showtime (showtime_id, movie_id, hall_id, show_start) VALUES (:showtime_id, :movie_id, :hall_id, :show_start)";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("showtime_id", showtimeId));
                        cmd.Parameters.Add(new OracleParameter("movie_id", int.Parse(ddlMovie.SelectedValue)));
                        cmd.Parameters.Add(new OracleParameter("hall_id", int.Parse(ddlHall.SelectedValue)));
                        cmd.Parameters.Add(new OracleParameter("show_start", OracleDbType.TimeStamp)).Value = showStart;
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadShowtimes();
                ClearForm();
                ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Showtime scheduled successfully.');", true);
            }
            catch (OracleException ex) when (ex.Number == 1)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Showtime conflict detected in this hall.');", true);
            }
            catch (Exception)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Data insertion failed. Please try again.');", true);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            // Validation: Check if all fields are filled
            if (string.IsNullOrEmpty(txtShowtimeId.Text) || ddlMovie.SelectedValue == "0" || ddlHall.SelectedValue == "0" || string.IsNullOrEmpty(txtShowStart.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation", "alert('All showtime fields are required.');", true);
                return;
            }

            // Validation: Check if Showtime ID is numeric
            int showtimeId;
            if (!int.TryParse(txtShowtimeId.Text, out showtimeId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation", "alert('Showtime ID must be numeric.');", true);
                return;
            }

            // Validation: Check if date is valid
            DateTime showStart;
            if (!DateTime.TryParse(txtShowStart.Text, out showStart))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation", "alert('Please enter a valid date and time for show start.');", true);
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    string query = "UPDATE showtime SET movie_id=:movie_id, hall_id=:hall_id, show_start=:show_start WHERE showtime_id=:showtime_id";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("movie_id", int.Parse(ddlMovie.SelectedValue)));
                        cmd.Parameters.Add(new OracleParameter("hall_id", int.Parse(ddlHall.SelectedValue)));
                        cmd.Parameters.Add(new OracleParameter("show_start", OracleDbType.TimeStamp)).Value = showStart;
                        cmd.Parameters.Add(new OracleParameter("showtime_id", showtimeId));
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Showtime updated successfully.');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Showtime not found. Update failed.');", true);
                        }
                    }
                }
                LoadShowtimes();
                ClearForm();
            }
            catch (OracleException ex) when (ex.Number == 1)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Showtime conflict detected in this hall.');", true);
            }
            catch (Exception)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Data update failed. Please try again.');", true);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            // Validation: Check if Showtime ID is provided
            if (string.IsNullOrEmpty(txtShowtimeId.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation", "alert('Please select a showtime to delete.');", true);
                return;
            }

            // Validation: Check if Showtime ID is numeric
            int showtimeId;
            if (!int.TryParse(txtShowtimeId.Text, out showtimeId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation", "alert('Showtime ID must be numeric.');", true);
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    string query = "DELETE FROM showtime WHERE showtime_id=:showtime_id";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("showtime_id", showtimeId));
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Showtime deleted successfully.');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Showtime not found. Delete failed.');", true);
                        }
                    }
                }
                LoadShowtimes();
                ClearForm();
            }
            catch (OracleException)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Cannot delete record because dependent records exist.');", true);
            }
            catch (Exception)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Data deletion failed. Please try again.');", true);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}