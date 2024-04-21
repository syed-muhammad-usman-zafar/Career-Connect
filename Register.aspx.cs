using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace WebApplication18.User
{
    public partial class Register : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Optional: You can add initialization logic here.
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"INSERT INTO [User](Username, Password, Mobile, Email, Country) 
                                    VALUES (@Username, @Password, @Mobile, @Email, @Country)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", txtUserName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Password", HashPassword(txtConfirmPassword.Text.Trim())); // Securely store password
                        cmd.Parameters.AddWithValue("@Mobile", txtMobile.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Country", ddlCountry.SelectedValue);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            lblMsg.Visible = true;
                            lblMsg.Text = "Registered Successfully";
                            lblMsg.CssClass = "alert alert-success";
                            ClearFields(); // Corrected method name
                        }
                        else
                        {
                            lblMsg.Visible = true;
                            lblMsg.Text = "No rows affected. Registration might not have been successful.";
                            lblMsg.CssClass = "alert alert-warning";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = $"<b>{txtUserName.Text.Trim()}</b> username already exists, try a new one.";
                    lblMsg.CssClass = "alert alert-danger";
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "An error occurred during registration. Please try again later.";
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
        }

        private string HashPassword(string password)
        {
            // Implement password hashing algorithm (e.g., using BCrypt, PBKDF2, etc.)
            // Example: return BCrypt.Net.BCrypt.HashPassword(password);
            return password; // Dummy implementation, replace with actual hashing logic
        }

        private void ClearFields()
        {
            txtUserName.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtMobile.Text = string.Empty;
            txtFullName.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
            ddlCountry.ClearSelection();
        }
    }
}
