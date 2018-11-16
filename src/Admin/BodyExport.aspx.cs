using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ferryman.Utility;

namespace Admin
{
    public partial class BodyExport : System.Web.UI.Page
    {
        int _SurveyID = 7;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (Request.QueryString["id"] == null)
                    {
                        _SurveyID = 7;
                        hidSurveyName.Value = "自覺肌肉骨骼症狀 資料匯出";
                    }
                    else
                    {
                        _SurveyID = Convert.ToInt32(Request.QueryString["id"]);
                        hidSurveyName.Value = "肌肉骨骼健康評估 資料匯出";
                    }
                    ViewState.Add("SurveyID", _SurveyID);
                }
                catch (Exception ex)
                {
                    btnSearch.Enabled = false;
                }
            }
            else
            {
                _SurveyID = Convert.ToInt32(ViewState["SurveyID"]);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string strDate1, strDate2;

            if (txtStartDate.Text.Trim() != "")
            {
                strDate1 = txtStartDate.Text.Trim();
            }
            else
            {
                strDate1 = "";
            }

            if (txtEndDate.Text.Trim() != "")
            {
                strDate2 = txtEndDate.Text.Trim();
            }
            else
            {
                strDate2 = "";
            }
            ShareFunction.Register_Open_Script(this.Page, "Export.aspx?id=" + _SurveyID.ToString() + "&date1=" + strDate1 + "&date2=" + strDate2);
        }
    }
}