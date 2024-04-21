using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebApplication18.User
{
    public partial class Contact : Page
    {
        SqlConnection con;
        SqlCommand cmd;

        string str = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // No specific initialization required in this method.
        }

        protected void btnsend_Click(object sender, EventArgs e)
        {
            Console.WriteLine("TEST");
            try
            {
                con = new SqlConnection(str);
                string query = @"INSERT INTO Contact VALUES (@name, @email, @subject, @message)";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name.Value.Trim());
                cmd.Parameters.AddWithValue("@email", email.Value.Trim()); // Corrected parameter name
                cmd.Parameters.AddWithValue("@subject", subject.Value.Trim());
                cmd.Parameters.AddWithValue("@message", message.Value.Trim());

                con.Open();
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Thanks for reaching out. We will look into your query!";
                    lblMsg.CssClass = "alert alert-success";
                    Clear(); // Corrected method name
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Cannot save record right now. Please try again later.";
                    lblMsg.CssClass = "alert alert-warning";
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
            finally
            {
                con.Close();
            }
        }

        private void Clear()
        {
            name.Value = string.Empty;
            email.Value = string.Empty;
            subject.Value = string.Empty;
            message.Value = string.Empty;
        }
    }
}
