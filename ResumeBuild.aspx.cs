using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace WebApplication18.User
{
    public partial class ResumeBuild : Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader sdr;
        string str = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        string query;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    showUserInfo();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }


            }
        }

        private void showUserInfo()
        {
            try
            {
                con = new SqlConnection(str);
                string query = "Select *  from [User] where UserId = @userId";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", Request.QueryString["id"]);
                con.Open();
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    if (sdr.Read())
                    {
                        // Populate user information on the form fields
                        txtUserName.Text = sdr["Username"].ToString();
                        txtFullName.Text = sdr["Name"].ToString();
                        txtEmail.Text = sdr["Email"].ToString();
                        txtMobile.Text = sdr["Mobile"].ToString();
                        txtTenth.Text = sdr["TenthGrade"].ToString();
                        txtTwelfth.Text = sdr["TwelfthGrade"].ToString();
                        txtGraduation.Text = sdr["GraduationGrade"].ToString();
                        txtPostGraduation.Text = sdr["PostGraduationGrade"].ToString();
                        txtPhd.Text = sdr["Phd"].ToString();
                        txtWork.Text = sdr["WorksOn"].ToString();
                        txtExperience.Text = sdr["Experience"].ToString();
                        txtAddress.Text = sdr["Address"].ToString();
                        ddlCountry.SelectedValue = sdr["Country"].ToString();
                    }
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "User not found";
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message.ToString() + "');</script>");
            }
            finally
            {
                con.Close();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString["id"] != null)
                {
                    string concatQuery = string.Empty;
                    string filePath = string.Empty;
                    bool isValid = false;
                    con = new SqlConnection(str);
                    if (fuResume.HasFile)
                    {
                        if (Utils.IsValidExtensionResume(fuResume.FileName))
                        {
                            concatQuery = "Resume=@resume";
                            isValid = true;
                        }
                        else
                        {
                            lblMsg.Visible = true;
                            lblMsg.Text = ".doc, .docx, .pdf only";
                            lblMsg.CssClass = "alert alert-danger";
                        }
                    }
                    else
                    {
                        concatQuery = string.Empty;
                    }
                    //query = @"Update [User] set Username = @Username,Name=@Name,Email=@Email,Mobile=@Mobile,TenthGrade=@TenthGrade
                    //    TwelfthGrade=@TwelfthGrade,GraduationGrade=@GraduationGrade,PostGraduationGrade=@PostGraduationGrade,Phd=@Phd,
                    //    WorksOn=@WorksOn,Experience=@Experience," + concatQuery + ",Address=@Address,Country=@Country where UserId=@UserId ";
                    query = @"Update [User] set Username = @Username, Name=@Name, Email=@Email, Mobile=@Mobile, TenthGrade=@TenthGrade,
                    TwelfthGrade=@TwelfthGrade, GraduationGrade=@GraduationGrade, PostGraduationGrade=@PostGraduationGrade, Phd=@Phd,
                    WorksOn=@WorksOn, Experience=@Experience," + concatQuery + ", Address=@Address, Country=@Country where UserId=@UserId ";


                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Username", txtUserName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Name", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Mobile", txtMobile.Text.Trim());
                    cmd.Parameters.AddWithValue("@TenthGrade", txtTenth.Text.Trim());
                    cmd.Parameters.AddWithValue("@TwelfthGrade", txtTwelfth.Text.Trim());
                    cmd.Parameters.AddWithValue("@GraduationGrade", txtGraduation.Text.Trim());
                    cmd.Parameters.AddWithValue("@PostGraduationGrade", txtPostGraduation.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phd", txtPhd.Text.Trim());
                    cmd.Parameters.AddWithValue("@workson", txtWork.Text.Trim());
                    cmd.Parameters.AddWithValue("@Experience", txtExperience.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@Country", ddlCountry.SelectedValue);
                    cmd.Parameters.AddWithValue("@UserId", Request.QueryString["id"]);
                    if (isValid)
                    {
                        Guid obj = Guid.NewGuid();
                        string fileName = obj.ToString() + "_" + Path.GetFileName(fuResume.FileName);
                        filePath = Path.Combine(Server.MapPath("~/Resumes/"), fileName);
                        fuResume.SaveAs(filePath);
                        cmd.Parameters.AddWithValue("@resume", filePath);
                    }

                    con.Open();
                    int r = cmd.ExecuteNonQuery();
                    if (r > 0)
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = "Resume Details updated Successfully";
                        lblMsg.CssClass = "alert alert-success";
                    }
                    else
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = "Resume could not be updated, please try again later";
                        lblMsg.CssClass = "alert alert-danger";
                    }
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Cannot update the record, please try to login again";
                    lblMsg.CssClass = "alert alert-danger";
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
                    lblMsg.Text = "An error occurred: " + ex.Message;
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "An error occurred: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
            finally
            {
                con.Close();
            }
        }
    }
}
