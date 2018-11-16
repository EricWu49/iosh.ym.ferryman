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
    public partial class Suggest : System.Web.UI.Page
    {
        DBClass DB;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PaperID"] == null)
                {
                    // 沒有問卷編號
                    Response.Redirect("Default.aspx", false);
                }
                else
                {
                    // 紀錄目前的問卷編號
                    ViewState.Add("PaperID", Convert.ToInt64(Session["PaperID"]));

                    if (Session["SurveyID"] == null)
                    {
                        // 沒有問卷編號，先設為0
                        ViewState.Add("PaperID", 0);
                    }
                    else
                    {
                        ViewState.Add("SurveyID", Session["SurveyID"]);
                    }

                    if (Session["SessionID"] == null)
                    {
                        // 沒有用戶連線代號，讀取Cookie資料
                        string strSessionID = UserClass.ReadCookie( UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sMemberSession, "");
                        if (strSessionID == "")
                        {
                            // Cookie沒資料，產生新的連線代號
                            strSessionID = Guid.NewGuid().ToString();
                            UserClass.WriteCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sMemberSession, strSessionID);
                        }
                        ViewState.Add("SessionID", strSessionID);
                    }
                    else
                    {
                        // 紀錄目前用戶的連線代號
                        ViewState.Add("SessionID", Session["SessionID"]);
                    }

                    ShareCode.Close_Report(Convert.ToInt64(Session["PaperID"]));
                    DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                    Init_Page();
                    DB = null;
                }
            }
        }

        private void Init_Page()
        {
            string strSQL = "";
            DataTable myData = null;
            Literal myLiteral;

            try
            {
                strSQL = "Select SituationID, SituationName, Suggest "
                        + "From Situation "
                        + "Where SituationID In "
                        + "(Select Distinct B.SituationID "
                        + " From(Select SurveyID, DimensionID, Point "
                        + "      From PaperResult "
                        + "      Where PaperID = @PID) as A "
                        + " JOIN "
                        + "(Select SurveyID, RuleID, DimensionID, MinValue, MaxValue, SituationID "
                        + " From SituationRule) as B On A.DimensionID = B.DimensionID And A.SurveyID = B.SurveyID "
                        + " Where A.Point Between B.MinValue And B.MaxValue)";
                DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["PaperID"]));
                myData = DB.GetDataTable(strSQL);
                if (myData == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "很抱歉，系統無法載入診斷報告。");
                        ShareFunction.PutLog("Init_Page", DB.DBErrorMessage);
                    }
                    else
                    {
                        myLiteral = new Literal();
                        myLiteral.Text = "<h4>您的睡眠足夠且品質良好、有良好運動習慣、沒有吸菸、喝酒習慣</h4>";
                        Panel_Situation.Controls.Add(myLiteral);

                        Panel_Good.Visible = true;
                        rptSuggest.Visible = false;
                    }
                }
                else
                {
                    myLiteral = new Literal();
                    myLiteral.Text = "";
                    myLiteral.Text += "<ul class='situation_list'>";
                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        myLiteral.Text += "<li>" + myData.Rows[i]["SituationName"].ToString() + "</li>";
                    }
                    myLiteral.Text += "</ul>";
                    Panel_Situation.Controls.Add(myLiteral);

                    rptSuggest.DataSource = myData;
                    rptSuggest.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Init_Page", ex.Message);
            }
        }

        protected void rptSuggest_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Literal myLiteral = (Literal)e.Item.FindControl("litStrategy");
                string strID = ((System.Data.DataRowView)e.Item.DataItem).Row["SituationID"].ToString().Trim();
                string strSQL = "";
                DataTable myData = null;

                try
                {
                    strSQL = "Select Distinct S.StrategyID, S.StrategyName, S.StrategyType, S.StrategySource "
                            + "From ResourceDB R Join Strategy S On R.StrategyID = S.StrategyID "
                            + "Where R.SituationID=" + strID;
                    myData = DB.GetDataTable(strSQL);

                    if (myData == null)
                    {
                        if (DB.DBErrorMessage != "")
                        {
                            ShareFunction.PutLog("rptSuggest_ItemDataBound", DB.DBErrorMessage);
                        }
                        myLiteral.Text = "";
                    }
                    else
                    {
                        myLiteral.Text = "";
                        myLiteral.Text += "<ul class='strategy_list'>";
                        for (int i = 0; i < myData.Rows.Count; i++)
                        {
                            switch (myData.Rows[i]["StrategyType"].ToString().ToUpper())
                            {
                                case "URL":
                                    myLiteral.Text += "<li class='url_link'><a href='" + myData.Rows[i]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[i]["StrategyName"].ToString() + "</a></li>";
                                    break;
                                case "VIDEO":
                                    myLiteral.Text += "<li class='video_link'><a href='" + myData.Rows[i]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[i]["StrategyName"].ToString() + "</a></li>";
                                    break;
                                case "IMAGE":
                                    myLiteral.Text += "<li class='image_link'><a href='" + myData.Rows[i]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[i]["StrategyName"].ToString() + "</a></li>";
                                    break;
                                case "FILE":
                                    myLiteral.Text += "<li class='file_link'><a href='" + myData.Rows[i]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[i]["StrategyName"].ToString() + "</a></li>";
                                    break;
                            }
                        }
                        myLiteral.Text += "</ul>";
                    }
                }
                catch (Exception ex)
                {
                    ShareFunction.PutLog("rptSuggest_ItemDataBound", ex.Message);
                }
            }
        }

        private void StrategyResult(DBClass DB, string SituationIDs)
        {
            string strSQL = "";
            DataTable myData = null;
            Literal myLiteral;

            try
            {
                strSQL = "Select Distinct S.StrategyID, S.StrategyName, S.StrategyType, S.StrategySource "
                        + "From ResourceDB R Join Strategy S On R.StrategyID = S.StrategyID "
                        + "Where R.SituationID In (" + SituationIDs + ")";
                myData = DB.GetDataTable(strSQL);

                myLiteral = new Literal();

                if (myData==null)
                {
                    if (DB.DBErrorMessage!="")
                    {
                        ShareFunction.PutLog("StrategyResult", DB.DBErrorMessage);
                    }
                    myLiteral.Text = "<strong>建議對策資料建構中...</strong>";
                }
                else
                {
                    myLiteral.Text = "<h4>針對您的情況，提供您下列資源來協助您改善工作適能：</h4>";
                    myLiteral.Text += "<ul class='strategy_list'>";
                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        switch (myData.Rows[i]["StrategyType"].ToString().ToUpper())
                        {
                            case "URL":
                                myLiteral.Text += "<li class='url_link'><a href='" + myData.Rows[i]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[i]["StrategyName"].ToString() + "</a></li>";
                                break;
                            case "VIDEO":
                                myLiteral.Text += "<li class='video_link'><a href='" + myData.Rows[i]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[i]["StrategyName"].ToString() + "</a></li>";
                                break;
                            case "IMAGE":
                                myLiteral.Text += "<li class='image_link'><a href='" + myData.Rows[i]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[i]["StrategyName"].ToString() + "</a></li>";
                                break;
                            case "FILE":
                                myLiteral.Text += "<li class='file_link'><a href='" + myData.Rows[i]["StrategySource"].ToString() + "' target='_blank'>" + myData.Rows[i]["StrategyName"].ToString() + "</a></li>";
                                break;
                        }
                    }
                    myLiteral.Text += "</ul>";
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("StrategyResult", ex.Message);
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