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
    public partial class TheaterCityHall : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;

        private void ShowTheaterMessage(string message, string type)
        {
            lblTheaterMessage.Text = message;
            lblTheaterMessage.CssClass = $"d-block mb-2 {(type == "success" ? "success-message" : "error-message")}";
        }

        private void ShowHallMessage(string message, string type)
        {
            lblHallMessage.Text = message;
            lblHallMessage.CssClass = $"d-block mb-2 {(type == "success" ? "success-message" : "error-message")}";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTheaterDropdown();
                LoadTheaters();
                LoadHalls();
            }
        }

        private void LoadTheaters()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    string query = "SELECT * FROM theater ORDER BY theater_id";
                    OracleDataAdapter da = new OracleDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvTheaters.DataSource = dt;
                    gvTheaters.DataBind();
                }
            }
            catch
            {
                // Handle error, e.g., Response.Write("Error: ");
            }
        }

        private void LoadHalls()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    string query = "SELECT * FROM hall ORDER BY hall_id";
                    OracleDataAdapter da = new OracleDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvHalls.DataSource = dt;
                    gvHalls.DataBind();
                }
            }
            catch
            {
                // Handle error, e.g., Response.Write("Error: ");
            }
        }

        private void LoadTheaterDropdown()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    string query = "SELECT theater_id, theater_name FROM theater";
                    OracleDataAdapter da = new OracleDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlTheater.DataSource = dt;
                    ddlTheater.DataValueField = "theater_id";
                    ddlTheater.DataTextField = "theater_name";
                    ddlTheater.DataBind();
                    ddlTheater.Items.Insert(0, new ListItem("--Select Theater--", "0"));
                }
            }
            catch
            {
                // Handle error, e.g., Response.Write("Error: ");
            }
        }

        protected void gvTheaters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvTheaters.SelectedRow != null)
            {
                txtTheaterId.Text = gvTheaters.SelectedRow.Cells[0].Text;
                txtTheaterName.Text = gvTheaters.SelectedRow.Cells[1].Text;
                txtTheaterCity.Text = gvTheaters.SelectedRow.Cells[2].Text;
            }
        }

        protected void gvHalls_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvHalls.SelectedRow != null)
            {
                txtHallId.Text = gvHalls.SelectedRow.Cells[0].Text;
                ddlTheater.SelectedValue = gvHalls.SelectedRow.Cells[1].Text;
                txtHallName.Text = gvHalls.SelectedRow.Cells[2].Text;
                txtHallCapacity.Text = gvHalls.SelectedRow.Cells[3].Text;
            }
        }

        protected void btnTheaterInsert_Click(object sender, EventArgs e)
        {
            InsertTheater();
        }

        private void InsertTheater()
        {
            try
            {
                // Validate fields
                if (string.IsNullOrWhiteSpace(txtTheaterId.Text) || 
                    string.IsNullOrWhiteSpace(txtTheaterName.Text) || 
                    string.IsNullOrWhiteSpace(txtTheaterCity.Text))
                {
                    ShowTheaterMessage("All theater fields are required.", "error");
                    return;
                }

                int theaterId;
                if (!int.TryParse(txtTheaterId.Text, out theaterId))
                {
                    ShowTheaterMessage("Theater ID must be numeric.", "error");
                    return;
                }

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    string query = "INSERT INTO theater (theater_id, theater_name, theater_city) VALUES (:id, :name, :city)";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add("id", OracleDbType.Int32).Value = theaterId;
                    cmd.Parameters.Add("name", OracleDbType.Varchar2).Value = txtTheaterName.Text;
                    cmd.Parameters.Add("city", OracleDbType.Varchar2).Value = txtTheaterCity.Text;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadTheaters();
                LoadTheaterDropdown();
                ClearTheaterFields();
                ShowTheaterMessage("Theater inserted successfully.", "success");
            }
            catch (OracleException ex) when (ex.Number == 1)
            {
                ShowTheaterMessage("Duplicate Theater ID. Please use a different ID.", "error");
            }
            catch (Exception ex)
            {
                ShowTheaterMessage("Error inserting theater.", "error");
            }
        }

        protected void btnTheaterUpdate_Click(object sender, EventArgs e)
        {
            UpdateTheater();
        }

        private void UpdateTheater()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTheaterId.Text) || 
                    string.IsNullOrWhiteSpace(txtTheaterName.Text) || 
                    string.IsNullOrWhiteSpace(txtTheaterCity.Text))
                {
                    ShowTheaterMessage("All theater fields are required.", "error");
                    return;
                }

                int theaterId;
                if (!int.TryParse(txtTheaterId.Text, out theaterId))
                {
                    ShowTheaterMessage("Theater ID must be numeric.", "error");
                    return;
                }

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    string query = "UPDATE theater SET theater_name = :name, theater_city = :city WHERE theater_id = :id";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add("name", OracleDbType.Varchar2).Value = txtTheaterName.Text;
                    cmd.Parameters.Add("city", OracleDbType.Varchar2).Value = txtTheaterCity.Text;
                    cmd.Parameters.Add("id", OracleDbType.Int32).Value = theaterId;
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        ShowTheaterMessage("Theater not found.", "error");
                        return;
                    }
                }
                LoadTheaters();
                ClearTheaterFields();
                ShowTheaterMessage("Theater updated successfully.", "success");
            }
            catch (Exception ex)
            {
                ShowTheaterMessage("Error updating theater.", "error");
            }
        }

        protected void btnTheaterDelete_Click(object sender, EventArgs e)
        {
            DeleteTheater();
        }

        private void DeleteTheater()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTheaterId.Text))
                {
                    ShowTheaterMessage("Please select a theater to delete.", "error");
                    return;
                }

                int theaterId;
                if (!int.TryParse(txtTheaterId.Text, out theaterId))
                {
                    ShowTheaterMessage("Theater ID must be numeric.", "error");
                    return;
                }

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    string query = "DELETE FROM theater WHERE theater_id = :id";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add("id", OracleDbType.Int32).Value = theaterId;
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        ShowTheaterMessage("Theater not found.", "error");
                        return;
                    }
                }
                LoadTheaters();
                LoadTheaterDropdown();
                ClearTheaterFields();
                ShowTheaterMessage("Theater deleted successfully.", "success");
            }
            catch (OracleException ex) when (ex.Number == 2292)
            {
                ShowTheaterMessage("Cannot delete theater. It has associated halls. Remove halls first.", "error");
            }
            catch (Exception ex)
            {
                ShowTheaterMessage("Error deleting theater.", "error");
            }
        }

        protected void btnTheaterClear_Click(object sender, EventArgs e)
        {
            ClearTheaterFields();
            ShowTheaterMessage("", "success");
        }

        private void ClearTheaterFields()
        {
            txtTheaterId.Text = "";
            txtTheaterName.Text = "";
            txtTheaterCity.Text = "";
        }

        protected void btnHallInsert_Click(object sender, EventArgs e)
        {
            InsertHall();
        }

        private void InsertHall()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtHallId.Text) || 
                    string.IsNullOrWhiteSpace(txtHallName.Text) || 
                    string.IsNullOrWhiteSpace(txtHallCapacity.Text) ||
                    ddlTheater.SelectedValue == "0")
                {
                    ShowHallMessage("All hall fields are required, including theater selection.", "error");
                    return;
                }

                int hallId;
                if (!int.TryParse(txtHallId.Text, out hallId))
                {
                    ShowHallMessage("Hall ID must be numeric.", "error");
                    return;
                }

                int capacity;
                if (!int.TryParse(txtHallCapacity.Text, out capacity) || capacity <= 0)
                {
                    ShowHallMessage("Hall Capacity must be a positive number.", "error");
                    return;
                }

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    string query = "INSERT INTO hall (hall_id, theater_id, hall_name, hall_capacity) VALUES (:id, :tid, :name, :cap)";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add("id", OracleDbType.Int32).Value = hallId;
                    cmd.Parameters.Add("tid", OracleDbType.Int32).Value = int.Parse(ddlTheater.SelectedValue);
                    cmd.Parameters.Add("name", OracleDbType.Varchar2).Value = txtHallName.Text;
                    cmd.Parameters.Add("cap", OracleDbType.Int32).Value = capacity;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadHalls();
                ClearHallFields();
                ShowHallMessage("Hall inserted successfully.", "success");
            }
            catch (OracleException ex) when (ex.Number == 1)
            {
                ShowHallMessage("Duplicate Hall ID. Please use a different ID.", "error");
            }
            catch (OracleException ex) when (ex.Number == 2291)
            {
                ShowHallMessage("Invalid Theater selected. Please select a valid theater.", "error");
            }
            catch (Exception ex)
            {
                ShowHallMessage("Error inserting hall.", "error");
            }
        }

        protected void btnHallUpdate_Click(object sender, EventArgs e)
        {
            UpdateHall();
        }

        private void UpdateHall()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtHallId.Text) || 
                    string.IsNullOrWhiteSpace(txtHallName.Text) || 
                    string.IsNullOrWhiteSpace(txtHallCapacity.Text) ||
                    ddlTheater.SelectedValue == "0")
                {
                    ShowHallMessage("All hall fields are required, including theater selection.", "error");
                    return;
                }

                int hallId;
                if (!int.TryParse(txtHallId.Text, out hallId))
                {
                    ShowHallMessage("Hall ID must be numeric.", "error");
                    return;
                }

                int capacity;
                if (!int.TryParse(txtHallCapacity.Text, out capacity) || capacity <= 0)
                {
                    ShowHallMessage("Hall Capacity must be a positive number.", "error");
                    return;
                }

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    string query = "UPDATE hall SET theater_id = :tid, hall_name = :name, hall_capacity = :cap WHERE hall_id = :id";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add("tid", OracleDbType.Int32).Value = int.Parse(ddlTheater.SelectedValue);
                    cmd.Parameters.Add("name", OracleDbType.Varchar2).Value = txtHallName.Text;
                    cmd.Parameters.Add("cap", OracleDbType.Int32).Value = capacity;
                    cmd.Parameters.Add("id", OracleDbType.Int32).Value = hallId;
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        ShowHallMessage("Hall not found.", "error");
                        return;
                    }
                }
                LoadHalls();
                ClearHallFields();
                ShowHallMessage("Hall updated successfully.", "success");
            }
            catch (OracleException ex) when (ex.Number == 2291)
            {
                ShowHallMessage("Invalid Theater selected. Please select a valid theater.", "error");
            }
            catch (Exception ex)
            {
                ShowHallMessage("Error updating hall.", "error");
            }
        }

        protected void btnHallDelete_Click(object sender, EventArgs e)
        {
            DeleteHall();
        }

        private void DeleteHall()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtHallId.Text))
                {
                    ShowHallMessage("Please select a hall to delete.", "error");
                    return;
                }

                int hallId;
                if (!int.TryParse(txtHallId.Text, out hallId))
                {
                    ShowHallMessage("Hall ID must be numeric.", "error");
                    return;
                }

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    string query = "DELETE FROM hall WHERE hall_id = :id";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add("id", OracleDbType.Int32).Value = hallId;
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        ShowHallMessage("Hall not found.", "error");
                        return;
                    }
                }
                LoadHalls();
                ClearHallFields();
                ShowHallMessage("Hall deleted successfully.", "success");
            }
            catch (OracleException ex) when (ex.Number == 2292)
            {
                ShowHallMessage("Cannot delete hall. It has associated showtimes. Remove showtimes first.", "error");
            }
            catch (Exception ex)
            {
                ShowHallMessage("Error deleting hall.", "error");
            }
        }

        protected void btnHallClear_Click(object sender, EventArgs e)
        {
            ClearHallFields();
            ShowHallMessage("", "success");
        }

        private void ClearHallFields()
        {
            txtHallId.Text = "";
            txtHallName.Text = "";
            txtHallCapacity.Text = "";
            ddlTheater.SelectedIndex = 0;
        }
    }
}