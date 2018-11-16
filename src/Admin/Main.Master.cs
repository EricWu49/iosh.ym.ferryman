using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class Main : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.Path.ToLower().IndexOf("default.aspx")>0)
                {
                    pnlMenu.Visible = false;
                    lblLoginUser.Visible = false;
                }
                else
                {
                    if (Session["IsLogin"] == null)
                    {
                        Response.Redirect("Default.aspx");
                        //pnlMenu.Visible = false;
                        //lblLoginUser.Visible = false;
                    }
                    else
                    {
                        pnlMenu.Visible = true;
                        lblLoginUser.Visible = true;
                        lblLoginUser.Text = Session["UserName"].ToString();
                    }
                }
            }
        }
    }
}