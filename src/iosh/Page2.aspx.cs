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
    public partial class Page2 : System.Web.UI.Page
    {
        int _SurveyID = 2;
        int _PageNo = 3;
        int _QuestionID = 332;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 暫存調查編號
                ViewState.Add("SurveyID", _SurveyID);
                Session["SurveyID"] = _SurveyID;

                if (Session["PaperID"] == null)
                {
                    // 沒有問卷編號，異常
                    Message.MsgShow_And_Redirect(this.Page, "操作異常，問卷編號不存在。", "Default.aspx");
                    return;
                }
                else
                {
                    // 紀錄目前的問卷編號
                    ViewState.Add("PaperID", Convert.ToInt64(Session["PaperID"]));
                }

                if (Session["SessionID"] == null)
                {
                    // 沒有用戶連線代號，讀取Cookie資料
                    string strSessionID = UserClass.ReadCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sMemberSession, "");
                    if (strSessionID == "")
                    {
                        // Cookie沒資料，產生新的連線代號
                        strSessionID = Guid.NewGuid().ToString();
                        UserClass.WriteCookie( UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sMemberSession, strSessionID);
                    }
                    ViewState.Add("SessionID", strSessionID);
                }
                else
                {
                    // 紀錄目前用戶的連線代號
                    ViewState.Add("SessionID", Session["SessionID"]);
                    //_PaperID = Convert.ToInt64(Session["PaperID"]);
                }

                // 開始載入題目
                Load_Question();
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
                strSQL = "Select OptionID, SelectOption, SelectValue, FreeItem From OptionData Where QuestionID=" + _QuestionID.ToString() + " And Disabled='N' Order By SortID";
                myData = DB.GetDataTable(strSQL);
                Repeater1.DataSource = myData;
                Repeater1.DataBind();
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Question", ex.Message);
            }
            DB = null;
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView myRow = (System.Data.DataRowView)e.Item.DataItem;
                CheckBox objCheck = (CheckBox)e.Item.FindControl("chkOption332");
                objCheck.Attributes.Add("onclick", "javascript: return check_click(this);");
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
            string strQTitle = "";
            CheckBox chkOption;
            string strOption;
            int intError = 0;

            try
            {
                intPaperID = Convert.ToInt64(ViewState["PaperID"]);

                // 因為是複選題，所以要先刪除所有已經存在的資料，再重新寫入
                strSQL = "Delete PaperDetail Where PaperID=@PID And QuestionID=@QID";
                DB.AddSqlParameter("@PID", intPaperID);
                DB.AddSqlParameter("@QID", _QuestionID);
                DB.RunSQL(strSQL);
                if (DB.DBErrorMessage != "")
                {
                    ShareFunction.PutLog("btnSubmit_Click(1)", DB.DBErrorMessage);
                    return;
                }

                foreach (RepeaterItem objItem in Repeater1.Items)
                {
                    chkOption = (CheckBox)ShareFunction.FindControlEx(objItem, "chkOption332");
                    if (chkOption.Checked)
                    {
                        strQID = chkOption.Attributes["QuestionID"].ToString();
                        strValue = chkOption.Attributes["Value"].ToString();
                        strOption = chkOption.Attributes["OptionID"].ToString();
                        strText = chkOption.Text;

                        strQTitle = lblQuestion332.Text;

                        // 因為是複選題，所以程式先刪除所有已經存在的資料，因此這裡只需要新增即可再重新寫入
                        strSQL = "Insert Into PaperDetail "
                                + "(PaperID, QuestionID, LineID, SurveyID, QTitle, CategoryID, SelectOption, SelectValue, AnswerValue) "
                                + "Values (@PID, @QID, @LID, @SID, @Title, 0, @Text, @Value, @Answer)";
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                        DB.AddSqlParameter("@LID", Convert.ToInt32(strValue));
                        DB.AddSqlParameter("@SID", _SurveyID);
                        DB.AddSqlParameter("@Title", strQTitle);
                        DB.AddSqlParameter("@Text", strText);
                        DB.AddSqlParameter("@Value", strValue);
                        DB.AddSqlParameter("@Answer", Convert.ToSingle(strOption));
                        if (DB.RunSQL(strSQL) < 1)
                        {
                            intError += 1;
                            ShareFunction.PutLog("btnSubmit_Click(1)", DB.DBErrorMessage);
                        }
                    }
                }
                if (intError>0)
                {
                    Message.MsgShow(this.Page, "問卷儲存發生錯誤。");
                }
                else
                {
                    Response.Redirect("Page3.aspx", false);
                    //Message.MsgShow(this.Page, "儲存成功");
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("btnSubmit_Click", ex.Message);
            }
            finally
            {
                Session["PaperID"] = ViewState["PaperID"];
                Session["SessionID"] = ViewState["SessionID"];
                DB = null;
            }
        }
    }
}