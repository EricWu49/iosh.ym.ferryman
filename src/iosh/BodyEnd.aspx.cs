using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Ferryman.DATA;
using Ferryman.Utility;

namespace iosh
{
    public partial class BodyEnd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PaperID"] == null)
                {
                    Response.Redirect("Default.aspx", false);
                }
                else
                {
                    ViewState.Add("PaperID", Session["PaperID"]);
                    ShareCode.Close_Report(Convert.ToInt64(Session["PaperID"]));
                }
            }
        }
    }
}