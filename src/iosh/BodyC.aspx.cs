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
    public partial class BodyC : System.Web.UI.Page
    {
        int _SurveyID = 10;      // 問卷代碼
        int _PageNo = 4;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 暫存調查編號
                if (Session["SurveyID"] != null)
                {
                    _SurveyID = Convert.ToInt32(Session["SurveyID"]);
                }

                // 問卷頁碼
                if (Session["PageNo"] != null)
                {
                    _PageNo = Convert.ToInt32(Request.QueryString["page"]);
                }

                ViewState.Add("SurveyID", _SurveyID);
                Session["SurveyID"] = _SurveyID;
                lblPageNo.Text = _PageNo.ToString();

                if (Session["PaperID"] == null)
                {
                    // 沒有問卷編號，異常操作
                    Response.Redirect("Default.aspx", false);
                }
                else
                {
                    // 紀錄目前的問卷編號
                    ViewState.Add("PaperID", Convert.ToInt64(Session["PaperID"]));
                    //_PaperID = Convert.ToInt64(Session["PaperID"]);

                    // 開始載入題目
                    Load_Question();
                }

                //if (Session["SessionID"] == null)
                //{
                //    // 沒有用戶連線代號，讀取Cookie資料
                //    string strSessionID = UserClass.ReadCookie(this.Page, UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sMemberSession, "");
                //    if (strSessionID == "")
                //    {
                //        // Cookie沒資料，產生新的連線代號
                //        strSessionID = Guid.NewGuid().ToString();
                //        UserClass.WriteCookie(this.Page, UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sMemberSession, strSessionID);
                //    }
                //    ViewState.Add("SessionID", strSessionID);
                //}
                //else
                //{
                //    // 紀錄目前用戶的連線代號
                //    ViewState.Add("SessionID", Session["SessionID"]);
                //    //_PaperID = Convert.ToInt64(Session["PaperID"]);
                //}
            }
            else
            {
                _SurveyID = Convert.ToInt32(ViewState["SurveyID"]);
                _PageNo = Convert.ToInt32(lblPageNo.Text);
            }
        }

        /// <summary>
        /// 讀取題目載入畫面
        /// </summary>
        private void Load_Question()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select Q.QuestionID, Q.QTitle "
                        + "From Question Q "
                        + "Where Q.SurveyID=@ID And Q.PageNo=@Page And Q.Deleted='N' "
                        + "Order By Q.OrderNo";
                DB.AddSqlParameter("@ID", _SurveyID);
                DB.AddSqlParameter("@Page", _PageNo);
                myData = DB.GetDataTable(strSQL);
                if (myData == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Load_Question", DB.DBErrorMessage);
                        Message.MsgShow(this.Page, "很抱歉，系統發生異常。");
                    }
                    else
                    {
                        string strReportPage = "";
                        strReportPage = ShareCode.GetReportPage(_SurveyID);
                        Response.Redirect(strReportPage, false);
                    }
                }
                else
                {
                    Repeater1.DataSource = myData;
                    Repeater1.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Question", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 問卷送出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strValue = "";
            string strQID = "";
            long intPaperID = 0;
            string strText;
            int intCount = 0;
            string strQTitle = "";
            RadioButtonList rdbOption;
            Label lblQuestion;
            int intErrorCount = 0;
            string strPage = "";

            try
            {
                intPaperID = Convert.ToInt64(ViewState["PaperID"]);
                if (intPaperID == 0)
                {
                    // PaperID=0在問卷第二頁是異常狀態，保存資料也會造成判讀有問題，因此取消本段程式碼
                    Message.MsgShow(this.Page, "資料異常，請重新操作。");
                    return;
                }

                intErrorCount = 0;
                // 處理問卷回答資料
                foreach (RepeaterItem objItem in Repeater1.Items)
                {
                    rdbOption = (RadioButtonList)ShareFunction.FindControlEx(objItem, "rdbOption");
                    strQID = rdbOption.Attributes["QuestionID"].ToString();
                    strValue = rdbOption.SelectedValue;
                    strText = rdbOption.SelectedItem.Text;

                    lblQuestion = (Label)ShareFunction.FindControlEx(objItem, "lblQuestion");
                    strQTitle = lblQuestion.Text;

                    // 判斷是否已經填寫過
                    strSQL = "Select Count(*) From BodyData Where PaperID=@PID And QuestionID=@QID";
                    DB.AddSqlParameter("@PID", intPaperID);
                    DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                    intCount = Convert.ToInt32(DB.GetData(strSQL));

                    if (intCount > 0)
                    {
                        // 更新
                        strSQL = "Update BodyData "
                                + "Set SelectOption=@Text, "
                                + "    SelectValue=@Value "
                                + "Where PaperID=@PID And QuestionID=@QID";
                        DB.AddSqlParameter("@Text", strText);
                        DB.AddSqlParameter("@Value", strValue);
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                    }
                    else
                    {
                        // 新增
                        strSQL = "Insert Into BodyData "
                                + "(PaperID, QuestionID, QTitle, SelectOption, SelectValue) "
                                + "Values (@PID, @QID, @Title, @Text, @Value)";
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                        DB.AddSqlParameter("@Title", strQTitle);
                        DB.AddSqlParameter("@Text", strText);
                        DB.AddSqlParameter("@Value", strValue);
                    }

                    if (DB.RunSQL(strSQL) < 1)
                    {
                        intErrorCount = intErrorCount + 1;
                        ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage + "BodyData(" + intPaperID.ToString() + "," + strQID + "," + strValue);
                    }
                }

                if (strValue=="1")
                {
                    // 已檢測，進行基礎動作篩檢測試回答
                    strPage = "BodyD.aspx";     // 原為Body03.aspx
                }
                else
                {
                    // 尚未檢測，針對痠痛影響指數最高的部分進行自評
                    strPage = "BodyE.aspx";     // 原為Body031.aspx
                }

                if (intErrorCount > 0)
                {
                    Message.MsgShow_And_Redirect(this.Page, "部分回答無法儲存，資料可能會有誤差。", strPage);
                }
                else
                {
                    Response.Redirect(strPage, false);
                }
            }
            catch (Exception ex)
            {
                Message.MsgShow(this.Page, "很抱歉，系統發生異常，請稍後再試。");
                ShareFunction.PutLog("btnSubmit_Click", ex.Message);
            }
            finally
            {
                Session["PaperID"] = ViewState["PaperID"];
                Session["SessionID"] = ViewState["SessionID"];
                DB = null;
            }
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                string strQID = "";

                try
                {
                    DataRowView myRow = (System.Data.DataRowView)e.Item.DataItem;
                    RadioButtonList objRadio = (RadioButtonList)e.Item.FindControl("rdbOption");
                    strQID = objRadio.Attributes["QuestionID"];
                    string strRule = objRadio.UniqueID + ":{ required:true }";
                    if (litRule.Text != "")
                    {
                        litRule.Text += "," + Environment.NewLine;
                    }
                    litRule.Text += strRule;
                }
                catch (Exception ex)
                {
                    ShareFunction.PutLog("Repeater1_ItemDataBound", ex.Message);
                }
                DB = null; 
            }
        }
    }
}