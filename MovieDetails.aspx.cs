using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace kumari
{
    public partial class MovieDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMovies();
            }
        }

        private void LoadMovies()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM movie ORDER BY movie_id";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            gvMovies.DataSource = reader;
                            gvMovies.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (you may want to add logging or display error message)
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error loading movies: " + ex.Message + "');", true);
            }
        }

        private void InsertMovie()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO movie VALUES(:id,:title,:duration,:language,:genre,:releaseDate)";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("id", OracleDbType.Decimal).Value = Convert.ToDecimal(txtMovieId.Text);
                        cmd.Parameters.Add("title", OracleDbType.Varchar2).Value = txtTitle.Text;
                        cmd.Parameters.Add("duration", OracleDbType.Decimal).Value = Convert.ToDecimal(txtDuration.Text);
                        cmd.Parameters.Add("language", OracleDbType.Varchar2).Value = txtLanguage.Text;
                        cmd.Parameters.Add("genre", OracleDbType.Varchar2).Value = txtGenre.Text;
                        cmd.Parameters.Add("releaseDate", OracleDbType.Date).Value = Convert.ToDateTime(txtReleaseDate.Text);
                        
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadMovies();
                ClearForm();
                ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Movie inserted successfully!');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error inserting movie: " + ex.Message + "');", true);
            }
        }

        private void UpdateMovie()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString))
                {
                    conn.Open();
                    string query = "UPDATE movie SET movie_title=:title, duration_minutes=:duration, language=:language, genre=:genre, release_date=:releaseDate WHERE movie_id=:id";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("title", OracleDbType.Varchar2).Value = txtTitle.Text;
                        cmd.Parameters.Add("duration", OracleDbType.Decimal).Value = Convert.ToDecimal(txtDuration.Text);
                        cmd.Parameters.Add("language", OracleDbType.Varchar2).Value = txtLanguage.Text;
                        cmd.Parameters.Add("genre", OracleDbType.Varchar2).Value = txtGenre.Text;
                        cmd.Parameters.Add("releaseDate", OracleDbType.Date).Value = Convert.ToDateTime(txtReleaseDate.Text);
                        cmd.Parameters.Add("id", OracleDbType.Decimal).Value = Convert.ToDecimal(txtMovieId.Text);
                        
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadMovies();
                ClearForm();
                ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Movie updated successfully!');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error updating movie: " + ex.Message + "');", true);
            }
        }

        private void DeleteMovie()
        {
            if (!TryGetMovieId(out decimal movieId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Please select a valid movie to delete.');", true);
                return;
            }

            try
            {
                using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM movie WHERE movie_id=:id";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("id", OracleDbType.Decimal).Value = movieId;
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Movie not found or already deleted.');", true);
                            return;
                        }
                    }
                }

                LoadMovies();
                ClearForm();
                gvMovies.SelectedIndex = -1;
                ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Movie deleted successfully!');", true);
            }
            catch (OracleException ex) when (ex.Number == 2292)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Cannot delete movie because it has related showtimes. Remove dependent showtimes first.');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error deleting movie: " + ex.Message + "');", true);
            }
        }

        private bool TryGetMovieId(out decimal movieId)
        {
            movieId = 0;
            if (gvMovies.SelectedDataKey != null && decimal.TryParse(gvMovies.SelectedDataKey.Value.ToString(), out decimal selectedId))
            {
                movieId = selectedId;
                return true;
            }

            if (!string.IsNullOrEmpty(txtMovieId.Text) && decimal.TryParse(txtMovieId.Text, out decimal textId))
            {
                movieId = textId;
                return true;
            }

            return false;
        }

        private void ClearForm()
        {
            txtMovieId.Text = "";
            txtTitle.Text = "";
            txtDuration.Text = "";
            txtLanguage.Text = "";
            txtGenre.Text = "";
            txtReleaseDate.Text = "";
        }

        protected void gvMovies_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow row = gvMovies.SelectedRow;
                txtMovieId.Text = row.Cells[0].Text;
                txtTitle.Text = row.Cells[1].Text;
                txtDuration.Text = row.Cells[2].Text;
                txtLanguage.Text = row.Cells[3].Text;
                txtGenre.Text = row.Cells[4].Text;
                
                // Handle date conversion from grid to date input
                string dateStr = row.Cells[5].Text;
                if (!string.IsNullOrEmpty(dateStr) && dateStr != "&nbsp;")
                {
                    DateTime dateValue = Convert.ToDateTime(dateStr);
                    txtReleaseDate.Text = dateValue.ToString("yyyy-MM-dd");
                }
                else
                {
                    txtReleaseDate.Text = "";
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error selecting movie: " + ex.Message + "');", true);
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                InsertMovie();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (ValidateForm() && !string.IsNullOrEmpty(txtMovieId.Text))
            {
                UpdateMovie();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Please select a movie to update.');", true);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteMovie();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(txtTitle.Text) || string.IsNullOrEmpty(txtDuration.Text) ||
                string.IsNullOrEmpty(txtLanguage.Text) || string.IsNullOrEmpty(txtGenre.Text) ||
                string.IsNullOrEmpty(txtReleaseDate.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Please fill in all required fields.');", true);
                return false;
            }

            try
            {
                Convert.ToDecimal(txtDuration.Text);
                Convert.ToDateTime(txtReleaseDate.Text);
                return true;
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Please enter valid numeric values for duration and a valid date for release date.');", true);
                return false;
            }
        }
    }
}