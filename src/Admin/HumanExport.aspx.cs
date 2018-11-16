using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ferryman.Utility;

namespace Admin
{
    public partial class HumanExport : System.Web.UI.Page
    {
        int _SurveyID = 6;

        protected void Page_Load(object sender, EventArgs e)
        {

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