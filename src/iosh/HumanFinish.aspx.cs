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
    public partial class HumanFinish : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PaperID"]==null)
                {
                    Response.Redirect("Default.aspx", false);
                }
                else
                {
                    ViewState.Add("PaperID", Session["PaperID"]);
                    ShareCode.Close_Report(Convert.ToInt64(Session["PaperID"]));
                    Load_Result();
                }
            }
        }

        private void Load_Result()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable dt1, dt2;
            int i, j;
            Literal litContent;
            string strID = "";

            try
            {
                strSQL = "Select QuestionID, QTitle "
                        + "From HumanData "
                        + "Where PaperID = @ID And QuestionID In(Select QuestionID "
                        + "                                      From Question "
                        + "                                      Where SurveyID=6 And PageNo=2) "
                        + "       And SelectValue='1'";
                DB.AddSqlParameter("@ID", Convert.ToInt64(ViewState["PaperID"]));
                dt1 = DB.GetDataTable(strSQL);

                if (dt1==null)
                {
                    // 沒有任何一項回答"有"
                    //Message.MsgShow(this.Page, "恭喜您，您的工作並沒有不良的姿勢或動作。");
                    litContent = new Literal();
                    litContent.Text = "<div style='width: 100%; text-align: center; height: 40px; margin: 10px;' class='bg-primary '><h2 style='color: #ffffff;'>恭喜您，您的工作並沒有不良的姿勢或動作。</h2></div>";
                    Panel1.Controls.Add(litContent);
                }
                else
                {
                    for (i=0; i<dt1.Rows.Count; i++)
                    {
                        //strSQL = "Select S.UniqueID, P.SourcePage "
                        //       + "From HumanData D Join Question Q On D.QuestionID = Q.QuestionID "
                        //       + "                  Join HumanSuggest S On D.QuestionID = S.QuestionID And D.SelectValue = S.SelectValue "
                        //       + "                  Join Suggest P On S.SuggestID = P.SuggestID "
                        //       + "Where D.PaperID = @PID And Q.ParentID = @QID";
                        strSQL = "Select Distinct S.SuggestID, P.SourcePage "
                               + "From HumanData D Join Question Q On D.QuestionID = Q.QuestionID "
                               + "                  Join HumanSuggest S On D.QuestionID = S.QuestionID And D.SelectValue = S.SelectValue "
                               + "                  Join Suggest P On S.SuggestID = P.SuggestID "
                               + "Where D.PaperID = @PID And Q.ParentID = @QID";
                        DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["PaperID"]));
                        DB.AddSqlParameter("@QID", Convert.ToInt64(dt1.Rows[i]["QuestionID"]));
                        dt2 = DB.GetDataTable(strSQL);

                        if (dt2==null)
                        {
                            // 系統沒有建議方案，或者選擇無此現象
                            //Message.MsgShow(this.Page, "很抱歉，系統目前尚未有關於" + dt1.Rows[i]["QTitle"].ToString() + "的建議改善方案。");
                        }
                        else
                        {
                            litContent = new Literal();
                            litContent.Text = "<div style='width: 100%; text-align: center; height: 40px;' class='bg-primary '><h2 style='color: #ffffff;'>" + dt1.Rows[i]["QTitle"].ToString() + "的改善方案</h2></div>";
                            Panel1.Controls.Add(litContent);
                            for (j = 0; j < dt2.Rows.Count; j++)
                            {
                                litContent = new Literal();
                                //strID = "div_suggest_" + dt1.Rows[i]["QuestionID"].ToString() + "_" + dt2.Rows[j]["UniqueID"].ToString();
                                strID = "div_suggest_" + dt1.Rows[i]["QuestionID"].ToString() + "_" + dt2.Rows[j]["SuggestID"].ToString();
                                litContent.Text = "<div id=\"" + strID + "\" style=\"width:100%; text-align: left;\"></div>";
                                Panel1.Controls.Add(litContent);
                                litScript.Text += "$(\"#" + strID + "\").load(\"/object/html/human/" + dt2.Rows[j]["SourcePage"].ToString() + "\");";
                            }
                        }
                    }
                    litScript.Text = "<script type=\"text/javascript\">" + litScript.Text + "</script>";
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Result", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            if (ViewState["PaperID"] != null)
            {
                string strPage = "";
                strPage = ShareCode.GoBackt_Report(Convert.ToInt64(ViewState["PaperID"].ToString()));
                Response.Redirect(strPage, false);
            }
            else
            {
                Response.Redirect("Default.aspx", false);
            }
        }
    }
}