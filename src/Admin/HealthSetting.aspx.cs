using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class HealthSetting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] == null)
                {
                    // 預設為健康狀況，暫時沒有其他問卷使用
                    hidSurveyID.Value = "4";
                }
                else
                {
                    hidSurveyID.Value = Request.QueryString["id"];
                }
            }
        }
    }
}