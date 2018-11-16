﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iosh.Console
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["IsLogin"] = null;
            Session["UserName"] = null;
            Response.Redirect("../Default.aspx", false);
        }
    }
}