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
    public partial class BodyB : System.Web.UI.Page
    {
        int _SurveyID = 10;      // (新)自覺肌肉骨骼症狀問卷代碼

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
                // 因為部位的痠痛位置增加左右側題目，所以增加BodyData.LineID=1的Where條件，才不會取得兩筆相同資料  2018/11/11
                strSQL = "Select Q.QuestionID, Q.QTitle, Q.IsOptional "
                        + "From Question Q Join BodyData B On Q.ParentID=B.QuestionID And Cast(B.SelectValue as int)>0 "
                        + "Where Q.SurveyID=@ID And Q.PageNo=3 And Q.Deleted='N' And B.PaperID=@PID And B.LineID=1 "
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
            bool AlertStatus = false;

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

                    if (strValue=="5")
                    {
                        // 有很大的影響，需請假休息或去看病
                        //AlertStatus = true;    // 希望還是可以針對其他部位提供建議，所以暫時取消
                    }

                    if (DB.RunSQL(strSQL) < 1)
                    {
                        intErrorCount = intErrorCount + 1;
                        ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage + "BodyData(" + intPaperID.ToString() + "," + strQID + "," + strValue);
                    }
                }

                if (AlertStatus)
                {
                    // 痠痛嚴重到影響工作，結束問卷
                    // 目前不會執行到了
                    Response.Redirect("BodyEnd.aspx", false);
                }
                else
                {
                    if (intErrorCount > 0)
                    {
                        Message.MsgShow_And_Redirect(this.Page, "部分回答無法儲存，資料可能會有誤差。", strPage);
                    }
                    else
                    {
                        Response.Redirect("BodyC.aspx", false);
                    }
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