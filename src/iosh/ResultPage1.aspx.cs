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
    public partial class ResultPage1 : System.Web.UI.Page
    {
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
                        ViewState.Add("SurveyID", 0);
                    }
                    else
                    {
                        ViewState.Add("SurveyID", Session["SurveyID"]);
                    }

                    if (Session["SessionID"] == null)
                    {
                        // 沒有用戶連線代號，讀取Cookie資料
                        string strSessionID = UserClass.ReadCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sMemberSession, "");
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

                    Init_Page();

                }
            }
        }

        private void Init_Page()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData = null;

            try
            {
                strSQL = "Select ReportID,  ReportTitle, Case When Printable='N' Then 'true' Else 'false' End as Printable From SurveyReport Where SurveyID=@SID";
                DB.AddSqlParameter("@SID", Convert.ToInt32(ViewState["SurveyID"]));
                myData = DB.GetDataTable(strSQL);
                if (myData == null)
                {
                    Message.MsgShow(this.Page, "很抱歉，系統無法載入診斷報告。");
                }
                else
                {
                    Repeater1.DataSource = myData;
                    Repeater1.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Init_Page", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            Response.Redirect("ResultReport1.aspx", true);
        }
    }
}