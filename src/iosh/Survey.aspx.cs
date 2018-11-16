using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ferryman.DATA;
using System.Data;
using Ferryman.Utility;

namespace iosh
{
    public partial class Survey : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] == null)
                {
                    Response.Redirect("Default.aspx", false);
                }
                else
                {
                    ViewState.Add("SurveyID", Request.QueryString["id"]);
                    //if (Session["ParentPaper"]!=null)
                    //{
                    //    ViewState.Add("ParentPaper", Session["ParentPaper"].ToString());
                    //}
                    if (Request.QueryString["paper"]!=null)
                    {
                        ViewState.Add("ParentPaper", Request.QueryString["paper"]);
                    }
                    else
                    {
                        ViewState["ParentPaper"] = null;
                    }
                    Load_Survey(Request.QueryString["id"].ToString());
                }
            }
        }

        private void Load_Survey(string id)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData = null;
            string strPage = "";   // 2017/8/31 新增

            try
            {
                // 2017/8/31 調整問題顯示頁的設定方式，定義於SurveyPage資料表，由這個資料表來決定每一個問卷的每一頁要使用哪一個WebPage
                strSQL = "Select PageFile From SurveyPage Where SurveyID=@ID And PageNo=2";
                DB.AddSqlParameter("@ID", Convert.ToInt32(id));
                strPage = DB.GetData(strSQL);
                // End

                // 2017/9/19 增加ReviewFlag，提供後續憑報告序號來查詢報告
                strSQL = "Select SurveyName, Instruction, SurveyPage, ReviewFlag From Survey Where SurveyID=@ID And Closed='N'";
                DB.AddSqlParameter("@ID", Convert.ToInt32(id));
                myData = DB.GetDataTable(strSQL);
                if (myData==null)
                {
                    if (DB.DBErrorMessage!="")
                    {
                        ShareFunction.PutLog("Load_Survey", DB.DBErrorMessage);
                        Message.MsgShow_And_Redirect(this.Page, "資料庫發生異常", "Default.aspx");
                    }
                    else
                    {
                        Message.MsgShow_And_Redirect(this.Page, "系統查無指定的調查資料。", "Default.aspx");
                    }
                }
                else
                {
                    // 為了保持舊程式碼的相容性，當SurveyPage還沒有定義時，仍維持以Survey資料表所定義的WebPage為主
                    if (strPage!=null)
                    {
                        lblSurveyPage.Text = strPage;
                    }
                    else
                    {
                        lblSurveyPage.Text = myData.Rows[0]["SurveyPage"].ToString();
                    }
                    // End

                    if (myData.Rows[0]["Instruction"].ToString()=="")
                    {
                        // 沒有調查描述，直接進入問卷題目
                        PaperStart();
                    }
                    else
                    {
                        litSurveyName.Text = myData.Rows[0]["SurveyName"].ToString();
                        litInstruction.Text = HttpUtility.HtmlDecode(myData.Rows[0]["Instruction"].ToString());
                        // 2017/9/19 增加查詢報告功能
                        if (myData.Rows[0]["ReviewFlag"].ToString()=="Y")
                        {
                            btnQuery.Visible = true;
                        }
                        else
                        {
                            btnQuery.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Survey", ex.Message );
                Message.MsgShow_And_Redirect(this.Page, "系統執行過程發生異常", "Default.aspx");
            }
            finally
            {
                DB = null;
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            PaperStart();
        }

        private void PaperStart()
        {
            //if (Session["PaperID"]!=null)
            //{
            //    // Session有PaperID，暫存成ParentPaper，供進一步診斷時保存兩份診斷問卷的關聯性使用
            //    Session["ParentPaper"] = Session["PaperID"];
            //}
            Session["SurveyID"] = ViewState["SurveyID"].ToString();
            Session["PaperID"] = null;
            Session["TotalPage"] = null;
            Session["PageNo"] = null;
            Session["SurveyPage"] = lblSurveyPage.Text;
            if (ViewState["ParentPaper"] != null)
            {
                Session["ParentPaper"] = ViewState["ParentPaper"].ToString();
            }
            else
            {
                Session["ParentPaper"] = null;
            }

            //Response.Redirect("SurveyPage.aspx", false);
            //Response.Redirect(lblSurveyPage.Text, false);
            Response.Redirect("Profile.aspx", false);
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            Session["SurveyID"] = ViewState["SurveyID"].ToString();
            Response.Redirect("ReportQuery.aspx", false);
        }
    }
}