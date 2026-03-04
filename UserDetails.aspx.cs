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
    public partial class UserDetails : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "SELECT * FROM app_user ORDER BY user_id";
                OracleCommand cmd = new OracleCommand(query, conn);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    da.Fill(dt);
                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Database connection failed. Could not load users.');", true);
                }
            }
        }

        private bool InsertUser()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "INSERT INTO app_user VALUES(:id,:name,:address)";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32)).Value = Convert.ToInt32(txtUserId.Text);
                cmd.Parameters.Add(new OracleParameter("name", OracleDbType.Varchar2)).Value = txtUserName.Text;
                cmd.Parameters.Add(new OracleParameter("address", OracleDbType.Varchar2)).Value = txtUserAddress.Text;
                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    // Log exception if logging available
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error adding user. Please contact administrator.');", true);
                    return false;
                }
            }
        }

        private bool UpdateUser()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "UPDATE app_user SET user_name=:name, user_address=:address WHERE user_id=:id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("name", OracleDbType.Varchar2)).Value = txtUserName.Text;
                cmd.Parameters.Add(new OracleParameter("address", OracleDbType.Varchar2)).Value = txtUserAddress.Text;
                cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32)).Value = Convert.ToInt32(txtUserId.Text);
                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error updating user. Please contact administrator.');", true);
                    return false;
                }
            }
        }

        private bool DeleteUser()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "DELETE FROM app_user WHERE user_id=:id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("id", Convert.ToInt32(txtUserId.Text)));
                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error deleting user. Please contact administrator.');", true);
                    return false;
                }
            }
        }

        private void ClearForm()
        {
            txtUserId.Text = "";
            txtUserName.Text = "";
            txtUserAddress.Text = "";
        }

        private bool ValidateUserFields()
        {
            if (string.IsNullOrWhiteSpace(txtUserId.Text) || 
                string.IsNullOrWhiteSpace(txtUserName.Text) || 
                string.IsNullOrWhiteSpace(txtUserAddress.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('All user fields are required');", true);
                return false;
            }

            int userId;
            if (!int.TryParse(txtUserId.Text, out userId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('User ID must be numeric');", true);
                return false;
            }

            return true;
        }

        private bool IsDuplicateUserId(int userId)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM app_user WHERE user_id = :id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("id", userId));
                
                try
                {
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        private bool UserExists(int userId)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM app_user WHERE user_id = :id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("id", userId));
                
                try
                {
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "message-label message-" + type;
        }

        protected void gvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvUsers.SelectedDataKey != null)
            {
                txtUserId.Text = gvUsers.SelectedDataKey.Value.ToString();
            }
            GridViewRow row = gvUsers.SelectedRow;
            if (row != null)
            {
                // Use cell indexes carefully; fallback to empty if not present
                txtUserName.Text = row.Cells.Count > 1 ? row.Cells[1].Text : string.Empty;
                txtUserAddress.Text = row.Cells.Count > 2 ? row.Cells[2].Text : string.Empty;
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (!ValidateUserFields())
                return;

            int userId = Convert.ToInt32(txtUserId.Text);
            if (IsDuplicateUserId(userId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Duplicate User ID is not allowed');", true);
                return;
            }

            try
            {
                if (InsertUser())
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('User added successfully');", true);
                    LoadUsers();
                    ClearForm();
                }
                else
                {
                    // InsertUser already shows a friendly message on failure
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error adding user. Please contact administrator.');", true);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateUserFields())
                return;

            int userId = Convert.ToInt32(txtUserId.Text);
            if (!UserExists(userId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('User ID does not exist');", true);
                return;
            }

            try
            {
                if (UpdateUser())
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('User updated successfully');", true);
                    LoadUsers();
                    ClearForm();
                }
                else
                {
                    // UpdateUser shows friendly message on failure
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error updating user. Please contact administrator.');", true);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('User ID is required for deletion');", true);
                return;
            }

            int userId;
            if (!int.TryParse(txtUserId.Text, out userId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('User ID must be numeric');", true);
                return;
            }

            if (!UserExists(userId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('User ID does not exist');", true);
                return;
            }

            try
            {
                if (DeleteUser())
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('User deleted successfully');", true);
                    LoadUsers();
                    ClearForm();
                }
                else
                {
                    // DeleteUser shows friendly message on failure
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Error deleting user. Please contact administrator.');", true);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            // Clear message display - no popup needed for clear
        }
    }
}