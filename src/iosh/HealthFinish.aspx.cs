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
    public partial class HealthFinish : System.Web.UI.Page
    {
        int _SurveyID = 4;
        DBClass DB;

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
                    DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                    Load_Result();
                    DB = null;
                }
            }
        }

        private void Load_Result()
        {
            string strSQL = "";
            DataTable dt1;

            try
            {
                strSQL = "Select QuestionID, QTitle, SelectValue "
                        + "From PaperDetail "
                        + "Where PaperID = @ID And LineID=1 And CategoryID=2 And SelectValue<>'1' ";
                DB.AddSqlParameter("@ID", Convert.ToInt64(ViewState["PaperID"]));
                dt1 = DB.GetDataTable(strSQL);

                if (dt1 == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Load_Result", DB.DBErrorMessage);
                        Message.MsgShow(this.Page, "系統發生錯誤。");
                    }
                    else
                    {
                        // 健康狀況沒有影響工作適能的問題
                        PlaceHolder1.Visible = false;
                        PlaceHolder2.Visible = true;
                    }
                }
                else
                {
                    Repeater1.DataSource = dt1;
                    Repeater1.DataBind();

                    Get_Resource(dt1);
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

        private void Get_Resource(DataTable dt)
        {
            string strSQL = "";
            DataTable myData;
            string strCode = "";
            Literal litElement;
            int i, j;

            try
            {
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    litElement = new Literal();

                    strCode = dt.Rows[i]["QuestionID"].ToString();
                    strSQL = "Select S.StrategyName, S.StrategyType, S.StrategySource "
                           + "From HealthSuggest H Join Strategy S On H.StrategyID=S.StrategyID "
                           + "Where H.QuestionID=@Code And H.LineID=@Line And H.SelectValue=@Value ";
                    DB.AddSqlParameter("@Code", Convert.ToInt64(strCode));
                    DB.AddSqlParameter("@Line", 1);     // 目前僅針對有沒有不舒服提供建議，若有針對是否影響工作提供建議時，須調整為動態
                    DB.AddSqlParameter("@Value", dt.Rows[i]["SelectValue"].ToString());
                    myData = DB.GetDataTable(strSQL);

                    litElement.Text = "<div id=\"tab-" + strCode + "\">" + Environment.NewLine;
                    if (myData != null)
                    {
                        litElement.Text += "<p>健康問題衛教資訊供您參考或繼續評估</p>";
                        litElement.Text += "<ul>" + Environment.NewLine;
                        for (j = 0; j < myData.Rows.Count; j++)
                        {
                            switch (myData.Rows[j]["StrategyType"].ToString().ToUpper())
                            {
                                case "URL":
                                    litElement.Text += "<li class=\"url_link\"><a href='" + myData.Rows[j]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[j]["StrategyName"].ToString() + "</a></li>" +
                                                        Environment.NewLine;
                                    break;
                                case "VIDEO":
                                    litElement.Text += "<li class=\"video_link\"><video width='480' height='360' controls>" +
                                                        "<source src='" + myData.Rows[j]["StrategySource"].ToString() + "' type='video/mp4'>您的瀏覽器不支援此 HTML5 影片標籤</video></li>" +
                                                        Environment.NewLine;
                                    break;
                                case "TEXT":
                                    litElement.Text += "<li class=\"strategy_list\">" + myData.Rows[j]["StrategyName"].ToString() + "</li>" + Environment.NewLine;
                                    break;
                                case "IMAGE":
                                    litElement.Text += "<li class=\"image_link\"><a href='" + myData.Rows[j]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[j]["StrategyName"].ToString() + "</a></li>" +
                                                        Environment.NewLine;
                                    break;
                                case "FILE":
                                    litElement.Text += "<li class=\"file_link\"><a href='" + myData.Rows[j]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[j]["StrategyName"].ToString() + "</a></li>" +
                                                        Environment.NewLine;
                                    break;
                            }
                        }   // for j
                        litElement.Text += "</ul>" + Environment.NewLine;
                    }   // if (myData != null)
                    else
                    {
                        ShareFunction.PutLog("Get_Resource", DB.DBErrorMessage);
                    }   // if (myData != null) else
                    litElement.Text += "</div>" + Environment.NewLine;
                    Panel1.Controls.Add(litElement);
                }   // for i
            }   // try
            catch (Exception ex)
            {
                ShareFunction.PutLog("Get_Resource", ex.Message);
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