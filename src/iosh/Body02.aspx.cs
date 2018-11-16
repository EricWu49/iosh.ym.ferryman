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
    public partial class Body02 : System.Web.UI.Page
    {
        int _SurveyID = 7;      // 自覺肌肉骨骼症狀問卷代碼

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PaperID"] == null)
                {
                    // 沒有問卷編號，異常操作
                    Response.Redirect("Default.aspx", false);
                }
                else
                {
                    // 紀錄目前的問卷編號
                    ViewState.Add("PaperID", Convert.ToInt64(Session["PaperID"]));

                    // 暫存調查編號
                    if (Session["SurveyID"] != null)
                    {
                        _SurveyID = ShareCode.GetSurveyID(Convert.ToInt64(Session["PaperID"]));
                    }

                    ViewState.Add("SurveyID", _SurveyID);
                    Session["SurveyID"] = _SurveyID;

                    // 開始載入題目
                    Load_Question();
                }
            }
            else
            {
                _SurveyID = Convert.ToInt32(ViewState["SurveyID"]);
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
                strSQL = "Select Q.QuestionID, Q.QTitle, Q.IsOptional "
                        + "From Question Q Join BodyData B On Q.ParentID=B.QuestionID And Cast(B.SelectValue as int)>0 "
                        + "Where Q.SurveyID=@ID And Q.PageNo=3 And Q.Deleted='N' And B.PaperID=@PID "
                        + "Order By Q.OrderNo";
                DB.AddSqlParameter("@ID", _SurveyID);
                DB.AddSqlParameter("@PID", Convert.ToInt32(ViewState["PaperID"]));
                myData = DB.GetDataTable(strSQL);
                if (myData==null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Load_Question", DB.DBErrorMessage);
                        Message.MsgShow(this.Page, "很抱歉，系統發生異常。");
                    }
                    else
                    {
                        // 沒有任何部位有疼痛
                        string strReportPage = "";
                        //strReportPage = ShareCode.GetReportPage(_SurveyID);
                        strReportPage = ShareCode.GetNextWebPage(Convert.ToInt32(ViewState["SurveyID"]), 4);
                        if (strReportPage == "")
                        {
                            strReportPage = ShareCode.GetReportPage(Convert.ToInt32(ViewState["SurveyID"]));
                        }
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

                    //// ViewState的PaperID為0，表示目前處理的是第一頁問題，因此要產生新的問卷回覆資料
                    //strSQL = "Insert Into Paper "
                    //        + "(SurveyID, SessionID, UserID, CreateTime, FromIP, InValid, ParentPaper) "
                    //        + "Values (@VID, @SID, @UID, GetDate(), @IP, 'N', @PID)";
                    //DB.AddSqlParameter("@VID", Convert.ToInt32(ViewState["SurveyID"]));
                    //DB.AddSqlParameter("@SID", ViewState["SessionID"].ToString());
                    //DB.AddSqlParameter("@UID", 0);
                    //DB.AddSqlParameter("@IP", Request.UserHostAddress);
                    //DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["ParentPaper"]));
                    //intPaperID = DB.InsertOneRecord(strSQL);
                    //if (intPaperID < 1)
                    //{
                    //    ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage);
                    //    Message.MsgShow(this.Page, "很抱歉，平台無法建立您的調查資料，請稍後再試。");
                    //    return;
                    //}
                    //else
                    //{
                    //    // 將產生的問卷回覆ID保存在ViewState中
                    //    ViewState["PaperID"] = intPaperID;
                    //    if (Convert.ToInt64(ViewState["ParentPaper"]) != 0)
                    //    {
                    //        // 有ParentPaper，表示這是一分子問卷，沒有重新詢問基本資料，需要複製前一份問卷的基本資料到這一份問卷中
                    //        strSQL = "Insert Into PaperDetail "
                    //                + "(PaperID, QuestionID, SurveyID, QTitle, CategoryID, SelectOption, SelectValue, AnswerValue) "
                    //                + "Select @ID, QuestionID, @VID, QTitle, CategoryID, SelectOption, SelectValue, AnswerValue "
                    //                + "From PaperDetail "
                    //                + "Where PaperID=@PID And CategoryID=1";
                    //        DB.AddSqlParameter("@ID", intPaperID);
                    //        DB.AddSqlParameter("@VID", Convert.ToInt32(ViewState["SurveyID"]));
                    //        DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["ParentPaper"]));
                    //        if (DB.RunSQL(strSQL) < 1)
                    //        {
                    //            ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage);
                    //        }
                    //    }
                    //}
                    //// 已經處理完畢問卷關聯，所以清除ParentPaper資料，以免後續程式重複執行
                    //ViewState["ParentPaper"] = 0;
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
                    //else
                    //{
                    //    Response.Redirect("BodyFinish.aspx", false);
                    //}
                }

                strPage = ShareCode.GetNextWebPage(Convert.ToInt32(ViewState["SurveyID"]), 4);
                if (strPage=="")
                {
                    strPage = ShareCode.GetReportPage(Convert.ToInt32(ViewState["SurveyID"]));
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
                DataRowView myRow = (System.Data.DataRowView)e.Item.DataItem;
                RadioButtonList objRadio = (RadioButtonList)e.Item.FindControl("rdbOption");
                string strRule = objRadio.UniqueID + ":{ required:true }";
                if (litRule.Text != "")
                {
                    litRule.Text += "," + Environment.NewLine;
                }
                litRule.Text += strRule;
            }
        }

    }
}