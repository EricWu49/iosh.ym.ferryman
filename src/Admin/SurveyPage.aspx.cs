using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class SurveyPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"]!=null)
                {
                    hidSurveyID.Value = Request.QueryString["id"].ToString();
                    FormView1.DefaultMode = FormViewMode.Edit;
                    Load_Data();
                }
                else
                {
                    hidSurveyID.Value = "";
                    FormView1.DefaultMode = FormViewMode.Insert;
                }
            }
        }

        private void Load_Data()
        {
            SurveyClass clsSurvey = new SurveyClass();
            System.Data.DataTable myData;

            myData = clsSurvey.GetSurveyList("SurveyID=" + hidSurveyID.Value.Trim());
            FormView1.DataSource = myData;
            FormView1.DataBind();
        }
    }
}