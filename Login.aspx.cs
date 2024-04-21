using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
namespace WebApplication18.User
{
    public partial class Login : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader sdr;
        string str = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        string username, password = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if(ddlLoginType.SelectedValue=="Admin")
                {
                    username = ConfigurationManager.AppSettings["username"];
                    password = ConfigurationManager.AppSettings["password"];

                    if(username == txtUserName.Text.Trim() && password== txtPassword.Text.Trim())
                    {
                        Session["admin"] =username;
                        Response.Redirect("../Admin1/Dashboard.aspx", false);

                    }
                    else
                    {
                        showErrorMsg("Admin");
                    }
                }
                else
                {

                    con = new SqlConnection(str);

                   // string query = @"INSERT INTO [User](Username, Password, Address, Mobile, Email, Country) VALUES (@Username, @Password, @Address, @Mobile, @Email, @Country)";
                    string query = @"Select* from [User] where Username=@username and Password = @Password";
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Username", txtUserName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                    
                    con.Open();
                    sdr = cmd.ExecuteReader();
                    if (sdr.Read())
                    {
                        Session["user"] = sdr["Username"].ToString();
                        Session["userId"] = sdr["UserId"].ToString();
                        Response.Redirect("Deafult.aspx", false);
                    }
                    else
                    {
                        showErrorMsg("User");
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Response.Write("<script>alert('Exception: " + ex.Message + "');</script>");
            }
        }

        private void showErrorMsg(string userType)
        {
            lblMsg.Visible = true;
            lblMsg.Text = "<b>" + userType + "</b> cresidentyials are incorrect....";
            lblMsg.CssClass = "alert alert-danger";
        }
    }
}