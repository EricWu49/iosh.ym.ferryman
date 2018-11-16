using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class SuggestSetting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"]==null)
                {
                    // 預設為個人生活狀況，暫時沒有其他問卷使用
                    hidSurveyID.Value = "3";
                }
                else
                {
                    hidSurveyID.Value = Request.QueryString["id"];
                }
            }
        }
    }
}