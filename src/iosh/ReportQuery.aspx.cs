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
    public partial class ReportQuery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack )
            {
                if (Session["SurveyID"]!=null)
                {
                    ViewState.Add("SurveyID", Session["SurveyID"].ToString());
                }
                else
                {
                    ViewState.Add("SurveyID", "");
                }
            }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            if (txtPaperSN.Text.Trim()=="")
            {
                Message.MsgShow(this.Page, "請輸入評估報告序號。");
            }
            else
            {
                if (txtPaperSN.Text.Trim().Length<10)
                {
                    Message.MsgShow(this.Page, "評估報告序號錯誤。");
                }
                else
                {
                    QueryReport();
                }
            }
        }

        private void QueryReport()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;
            string strSN = "";
            string strPage = "";
            bool PaperFinish = false;

            strSN = txtPaperSN.Text.Trim().Substring(0, 10);
            strSQL = "Select PaperID, SessionID, SurveyID From Paper Where PaperSN=@SN";
            DB.AddSqlParameter("@SN", strSN);
            myData = DB.GetDataTable(strSQL);

            if (myData != null)
            {
                Session["SurveyID"] = myData.Rows[0]["SurveyID"].ToString();
                Session["PaperID"] = myData.Rows[0]["PaperID"].ToString();
                Session["SessionID"] = myData.Rows[0]["SessionID"].ToString();

                switch (Convert.ToInt32(Session["SurveyID"]))
                {
                    case 9:
                        PaperFinish = BodyProcess(DB);
                        if (!PaperFinish)
                        {
                            strPage = GetBodyPage(DB);
                        }
                        break;
                    default:
                        PaperFinish = true;
                        break;
                }

                if (PaperFinish)
                {
                    // 問卷已經填寫完畢，取得報告網頁程式
                    strSQL = "Select P.PageFile "
                            + "From Survey S Join PageType P On S.PageID=P.PageID "
                            + "Where S.SurveyID=@SID";
                    DB.AddSqlParameter("@SID", Convert.ToInt32(Session["SurveyID"]));
                    strPage = DB.GetData(strSQL);
                }

                if (strPage != null)
                {
                    Response.Redirect(strPage, false);
                }
                else
                {
                    Message.MsgShow_And_Redirect(this.Page, "無法取得您要查閱的評估報告。", "Default.aspx");
                }
            }
            DB = null;
        }

        /// <summary>
        /// 檢查肌肉骨骼評估問卷是否完成基礎動作控制篩檢部分
        /// </summary>
        /// <param name="DB">資料庫連線元件</param>
        /// <returns>是否完成</returns>
        private bool BodyProcess(DBClass DB)
        {
            string strSQL = "";
            string strReturn = "";

            try
            {
                strSQL = "Select Count(*) From BodyData " +
                         "Where PaperID=@PID And QuestionID In (Select QuestionID From Question Where SurveyID=@SID And PageNo=5 And Deleted='N')";
                DB.AddSqlParameter("@PID", Convert.ToInt32(Session["PaperID"]));
                DB.AddSqlParameter("@SID", Convert.ToInt32(Session["SurveyID"]));
                strReturn = DB.GetData(strSQL);
                if (Convert.ToInt32(strReturn)>0)
                {
                    // 此份報告已經全數完成
                    return true;
                }
                else
                {
                    // 此份報告沒有做過基礎動作控制篩檢
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("BodyProcess", ex.Message);
                // 執行錯誤，直接顯示結果報告
                return true;
            }
        }

        /// <summary>
        /// 取得肌肉骨骼評估問卷的基礎動作控制篩檢網頁
        /// </summary>
        /// <param name="DB">資料庫連線元件</param>
        /// <returns>問卷程式網頁名稱</returns>
        private string GetBodyPage(DBClass DB)
        {
            string strSQL = "";
            string strPage = "";

            try
            {
                strSQL = "Select PageFile From SurveyPage Where SurveyID=9 And PageNo=4";
                strPage = DB.GetData(strSQL);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("GetBodyPage", ex.Message);
                strPage = null;
            }
            return strPage;
        }
    }
}