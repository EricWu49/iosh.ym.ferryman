using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Ferryman.DATA;
using Ferryman.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace iosh
{
    /// <summary>
    /// 將主選單的每個調查項目設定此頁為第一頁，只要進入此頁，就先清除UserID，重新建立
    /// 若是在完成一個調查後，透過診斷報告連結進度其他的調查，設定為直接進入SurveyPage，以使UserID維持有效，不會被消除
    /// 以使資料庫中，紀錄為同一個User，使基本資料達到兩個調查共用的目的
    /// </summary>
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["SurveyID"] == null)
                {
                    // 沒有傳入調查編號
                    Response.Redirect("Default.aspx", false);
                }
                else
                {
                    // 暫存調查編號
                    ViewState.Add("SurveyID", Session["SurveyID"]);

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

                    if (Session["SurveyPage"] == null)
                    {
                        // 預設值，以便相容之前已經開發完成的問卷
                        lblSurveyPage.Text = "SurveyPage.aspx";
                    }
                    else
                    {
                        lblSurveyPage.Text = Session["SurveyPage"].ToString();
                    }

                    // 進入這一頁就先清除UserID
                    // Session["UserID"] = null;        2018/9/30 啟用註冊用戶登入方式，需要保存UserID
                    if (Session["UserID"] == null)
                    {
                        Session["UserID"] = UserClass.ReadCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserID, "0");
                    }
                    ViewState.Add("UserID", Session["UserID"]);

                    if (Session["ParentPaper"] == null)
                    {
                        // 沒有前一份問卷
                        ViewState.Add("ParentPaper", 0);
                        Load_Question();
                        if (Session["UserID"] != null)
                        {
                            if (Session["UserID"].ToString() != "0")
                            {
                                Load_Profile();
                            }
                        }
                    }
                    else
                    {
                        // 有前一份問卷編號
                        ViewState.Add("ParentPaper", Convert.ToInt64(Session["ParentPaper"]));
                        Copy_And_Skip();
                    }
                }
            }
        }

        /// <summary>
        /// 載入問題
        /// </summary>
        private void Load_Question()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData = null;
            TableRow QRow;
            TableCell QCell;
            QuestionClass clsQuestion;
            string strRules = "";
            Control objControl = null;
            //WebControl ctlQuestion;

            try
            {
                //hidProgress.Value = (Math.Round((Convert.ToDouble(ViewState["PageNo"]) - 1) / Convert.ToDouble(ViewState["TotalPage"]), 2) * 100).ToString();
                // 因為問卷採用動態載入，SurveyPage.aspx只是一直重複PostBack，所以前一次所設定的Script會持續保存著，因此必須要先清除，才不會造成後續執行錯誤
                litScript.Text = "";
                litValidator.Text = "";

                strSQL = "Select QuestionID, QTitle, QDescription, QType, ParentID, IsSubQuestion, IsOptional "
                        + "From Question "
                        + "Where SurveyID=0 And Deleted='N' "
                        + "Order By OrderNo";
                //DB.AddSqlParameter("@SID", Convert.ToInt32(ViewState["SurveyID"]));
                //DB.AddSqlParameter("@PNo", Convert.ToInt32(ViewState["PageNo"]));
                myData = DB.GetDataTable(strSQL);
                if (myData == null)
                {
                    Message.MsgShow_And_Redirect(this.Page, "問卷設定異常。", "Default.aspx");
                    ShareFunction.PutLog("Load_Question", DB.DBErrorMessage);
                }
                else
                {
                    clsQuestion = new QuestionClass(ref DB);

                    fmPageTable.Style.Add("width", "100%");

                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        QRow = new TableRow();
                        QCell = new TableCell();
                        QCell.Style.Add("padding", "3px");
                        QCell.Style.Add("width", "100%");
                        switch (Convert.ToInt32(myData.Rows[i]["QType"]))
                        {
                            case 1:     // 標題
                                QCell.Controls.Add(clsQuestion.Create_TitleText(Convert.ToInt64(myData.Rows[i]["QuestionID"]), myData.Rows[i]["QTitle"].ToString(), myData.Rows[i]["QDescription"].ToString()));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            case 2:     // 文字顯示
                                QCell.Controls.Add(clsQuestion.Create_NormalText(Convert.ToInt64(myData.Rows[i]["QuestionID"]), myData.Rows[i]["QTitle"].ToString(), myData.Rows[i]["QDescription"].ToString()));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            case 3:     // 圖片
                                QCell.Controls.Add(clsQuestion.Create_Image(Convert.ToInt64(myData.Rows[i]["QuestionID"]), myData.Rows[i]["QTitle"].ToString(), myData.Rows[i]["QDescription"].ToString()));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            case 11:    // 文字輸入
                                QCell.Controls.Add(clsQuestion.Create_TextBox(myData.Rows[i]));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            case 12:    // 單選鈕
                                QCell.Controls.Add(clsQuestion.Create_Radiobox(myData.Rows[i]));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            case 13:    // 複選
                                QCell.Controls.Add(clsQuestion.Create_Checkbox(myData.Rows[i]));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            case 14:    // 單選下拉選單
                                QCell.Controls.Add(clsQuestion.Create_DropdownList(myData.Rows[i], false));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            case 15:    // 多選下拉選單
                                QCell.Controls.Add(clsQuestion.Create_DropdownList(myData.Rows[i], true));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            case 16:    // 日期輸入
                                break;
                            case 17:    // 量尺
                                QCell.Controls.Add(clsQuestion.Create_Slider(myData.Rows[i]));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            case 18:    // 資料清單
                                QCell.Controls.Add(clsQuestion.Create_Datalist(Convert.ToInt64(myData.Rows[i]["QuestionID"]), myData.Rows[i]["QTitle"].ToString(), myData.Rows[i]["QDescription"].ToString()));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                litScript.Text = "var rows=$('#fm_QControl_" + myData.Rows[i]["QuestionID"].ToString() + "').datalist('getSelections'); "
                                                + "var ss = ''; "
                                                + "for(var i=0; i<rows.length; i++){ "
                                                + "var row = rows[i]; "
                                                + "ss += row.value + '$'; "
                                                + "} "
                                                + "$('#fm_QControl_" + myData.Rows[i]["QuestionID"].ToString() + "').val(ss);";
                                break;
                            case 101:    // 性別選項
                                QCell.Controls.Add(clsQuestion.Create_PreDefineInput(Convert.ToInt64(myData.Rows[i]["QuestionID"]), myData.Rows[i]["QTitle"].ToString(), myData.Rows[i]["QDescription"].ToString(), "GENDER"));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            case 105:    // 教育程度選項
                                QCell.Controls.Add(clsQuestion.Create_PreDefineInput(Convert.ToInt64(myData.Rows[i]["QuestionID"]), myData.Rows[i]["QTitle"].ToString(), myData.Rows[i]["QDescription"].ToString(), "EDUCATION"));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
                                break;
                            default:
                                break;
                        }

                        if (Convert.ToInt32(myData.Rows[i]["QType"]) > 10)
                        {
                            objControl = ShareFunction.FindControlEx(this.Page, "fm_QControl_" + myData.Rows[i]["QuestionID"].ToString());
                            if (objControl != null)
                            {
                                strRules = Validate_Rule(DB, Convert.ToInt64(myData.Rows[i]["QuestionID"].ToString()));
                                if (myData.Rows[i]["IsOptional"].ToString() == "N")
                                {
                                    if (litValidator.Text != "")
                                    {
                                        litValidator.Text += "," + Environment.NewLine;
                                    }
                                    if (Convert.ToInt64(myData.Rows[i]["ParentID"].ToString()) == 0)
                                    {
                                        if (strRules == "")
                                        {
                                            litValidator.Text += objControl.UniqueID + ":{ required:true}";
                                        }
                                        else
                                        {
                                            litValidator.Text += objControl.UniqueID + ":{ required:true, " + strRules + "}";
                                        }
                                    }
                                    else
                                    {
                                        if (strRules == "")
                                        {
                                            litValidator.Text += objControl.UniqueID + ":{ required:!this.disabled}";
                                        }
                                        else
                                        {
                                            litValidator.Text += objControl.UniqueID + ":{ required:!this.disabled, " + strRules + "}";
                                        }
                                    }
                                }
                                else
                                {
                                    if (strRules != "")
                                    {
                                        if (litValidator.Text != "")
                                        {
                                            litValidator.Text += "," + Environment.NewLine;
                                        }
                                        litValidator.Text += objControl.UniqueID + ":{" + strRules + "}";
                                    }
                                }
                            }
                        }
                    }

                    litValidator.Text = " var validator = $(\"#form1\").validate({" + Environment.NewLine
                                       + "rules:{" + Environment.NewLine
                                       + litValidator.Text + Environment.NewLine
                                       + "}" + Environment.NewLine
                                       + "})";

                    //if (Convert.ToInt32(ViewState["PageNo"]) == Convert.ToInt32(ViewState["TotalPage"]))
                    //{
                    //    btnSubmit.Text = "結束問卷";
                    //}
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
        /// 檢查規則設定
        /// </summary>
        /// <param name="DB">資料庫連線物件</param>
        /// <param name="QuestionID">問題編號</param>
        /// <returns></returns>
        private string Validate_Rule(DBClass DB, long QuestionID)
        {
            string strSQL = "";
            string strRules = "";

            try
            {
                strSQL = "Select AttrValue From QuestionAttribute Where QuestionID=@ID And AttrName=N'rules'";
                DB.AddSqlParameter("@ID", QuestionID);
                strRules = DB.GetData(strSQL);
                if (strRules == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Validate_Rule", DB.DBErrorMessage);
                    }
                    strRules = "";
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Validate_Rule", ex.Message);
                strRules = "";
            }
            return strRules;
        }

        /// <summary>
        /// 送出問卷處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string strResult = "";
            Dictionary<string, string> objResult;
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strElement = "";
            string strValue = "";
            string strQID = "";
            int intIndex = 0;
            long intPaperID = 0;
            string strText, strType;
            int intCount = 0;
            float numPoint = 0;
            string[] aryValue;

            try
            {
                intPaperID = Convert.ToInt64(ViewState["PaperID"]);
                if (intPaperID == 0)
                {
                    strSQL = "Insert Into Paper "
                            + "(SurveyID, SessionID, UserID, CreateTime, FromIP, InValid, ParentPaper) "
                            + "Values (@VID, @SID, @UID, GetDate(), @IP, 'N', @PID)";
                    DB.AddSqlParameter("@VID", Convert.ToInt32(ViewState["SurveyID"]));
                    DB.AddSqlParameter("@SID", ViewState["SessionID"].ToString());
                    DB.AddSqlParameter("@UID", Convert.ToInt32(ViewState["UserID"]));
                    DB.AddSqlParameter("@IP", Request.UserHostAddress);
                    DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["ParentPaper"]));
                    intPaperID = DB.InsertOneRecord(strSQL);
                    if (intPaperID < 1)
                    {
                        ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage);
                        Message.MsgShow(this.Page, "很抱歉，平台無法建立您的調查資料，請稍後再試。");
                        return;
                    }
                    else
                    {
                        ViewState["PaperID"] = intPaperID;
                    }
                }

                strResult = hidResult.Value;
                objResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(strResult);
                foreach (var objElement in objResult)
                {
                    strElement = objElement.Key;
                    strValue = objElement.Value;
                    intIndex = strElement.IndexOf("fm_QControl_");
                    if (intIndex > 0)
                    {
                        strQID = strElement.Substring(intIndex, strElement.Length - intIndex);
                        strQID = strQID.Replace("fm_QControl_", "");

                        // 取得問題類型
                        strSQL = "Select QType From Question Where QuestionID=@QID";
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                        strType = DB.GetData(strSQL);

                        // 判斷是否已經填寫過
                        strSQL = "Select Count(*) From PaperDetail Where PaperID=@PID And QuestionID=@QID";
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                        intCount = Convert.ToInt32(DB.GetData(strSQL));

                        if (intCount > 0)
                        {
                            // 更新
                            switch (strType)
                            {
                                case "11":  // 文字輸入
                                case "17":  // 量尺
                                    strSQL = "Update PaperDetail "
                                            + "Set SelectOption=@Text, "
                                            + "    SelectValue=@Value, "
                                            + "    AnswerValue=@Point "
                                            + "Where UserID=@UID And QuestionID=@QID ";
                                    strText = strValue;
                                    break;
                                case "18":  // 資料清單
                                    strSQL = "Update PaperDetail "
                                            + "Set SelectOption=@Text, "
                                            + "    SelectValue=@Value, "
                                            + "    AnswerValue=@Point "
                                            + "Where UserID=@UID And QuestionID=@QID ";
                                    aryValue = strValue.Split('$');
                                    strText = "";
                                    strValue = "";
                                    numPoint = 0;
                                    for (int i = 0; i < aryValue.Length; i++)
                                    {
                                        if (aryValue[i] != "")
                                        {
                                            strText += aryValue[i].Split('|')[1] + "|";
                                            strValue += aryValue[i].Split('|')[0] + "|";
                                            numPoint += Convert.ToSingle(aryValue[i].Split('|')[0]);
                                        }
                                    }
                                    break;
                                default:
                                    strSQL = "Update PaperDetail "
                                            + "Set SelectOption=@Text, "
                                            + "    SelectValue=@Value, "
                                            + "    AnswerValue=@Point "
                                            + "From PaperDetail P Join Question Q On P.QuestionID=Q.QuestionID "
                                            + "                Left Join OptionData O On Q.QuestionID=O.QuestionID "
                                            + "                Left Join DefaultOption D On Q.QType=D.QType "
                                            + "Where P.PaperID=@PID And Q.QuestionID=@QID And (O.SelectValue=@Text Or D.SelectValue=@Text) ";
                                    strText = strValue;
                                    break;
                            }
                            DB.AddSqlParameter("@PID", intPaperID);
                            DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                            DB.AddSqlParameter("@SID", Convert.ToInt32(ViewState["SurveyID"]));
                            DB.AddSqlParameter("@Text", strText);
                            DB.AddSqlParameter("@Value", strValue);
                            if (ShareFunction.IsNumeric(strValue))
                            {
                                DB.AddSqlParameter("@Point", Convert.ToSingle(strValue));
                            }
                            else
                            {
                                DB.AddSqlParameter("@Point", numPoint);
                            }
                        }
                        else
                        {
                            // 新增
                            switch (strType)
                            {
                                case "11":  // 文字輸入
                                case "17":  // 量尺
                                    strSQL = "Insert Into PaperDetail "
                                            + "(PaperID, QuestionID, SurveyID, QTitle, CategoryID, SelectOption, SelectValue, AnswerValue) "
                                            + "Select @PID, @QID, @SID, Q.QTitle, Q.CategoryID, @Text, @Value, @Point "
                                            + "From Question Q "
                                            + "Where Q.QuestionID=@QID ";
                                    strText = strValue;
                                    break;
                                case "18":  // 資料清單
                                    strSQL = "Insert Into PaperDetail "
                                            + "(PaperID, QuestionID, SurveyID, QTitle, CategoryID, SelectOption, SelectValue, AnswerValue) "
                                            + "Select @PID, @QID, @SID, Q.QTitle, Q.CategoryID, @Text, @Value, @Point "
                                            + "From Question Q "
                                            + "Where Q.QuestionID=@QID ";
                                    aryValue = strValue.Split('$');
                                    strText = "";
                                    strValue = "";
                                    numPoint = 0;
                                    for (int i = 0; i < aryValue.Length; i++)
                                    {
                                        if (aryValue[i] != "")
                                        {
                                            strText += aryValue[i].Split('|')[1] + "|";
                                            strValue += aryValue[i].Split('|')[0] + "|";
                                            numPoint += Convert.ToSingle(aryValue[i].Split('|')[0]);
                                        }
                                    }
                                    break;
                                default:
                                    strSQL = "Insert Into PaperDetail "
                                            + "(PaperID, QuestionID, SurveyID, QTitle, CategoryID, SelectOption, SelectValue, AnswerValue) "
                                            + "Select @PID, @QID, @SID, Q.QTitle, Q.CategoryID, IsNull(O.SelectOption, D.SelectOption), @Value, @Point "
                                            + "From Question Q Left Join OptionData O On Q.QuestionID=O.QuestionID "
                                            + "                Left Join DefaultOption D On Q.QType=D.QType "
                                            + "Where Q.QuestionID=@QID And (O.SelectValue=@Text Or D.SelectValue=@Text) ";
                                    strText = strValue;
                                    numPoint = 0;   // 當Value不是數字時使用
                                    break;
                            }
                            DB.AddSqlParameter("@PID", intPaperID);
                            DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                            DB.AddSqlParameter("@SID", Convert.ToInt32(ViewState["SurveyID"]));
                            DB.AddSqlParameter("@Text", strText);
                            DB.AddSqlParameter("@Value", strValue);
                            if (ShareFunction.IsNumeric(strValue))
                            {
                                DB.AddSqlParameter("@Point", Convert.ToSingle(strValue));
                            }
                            else
                            {
                                DB.AddSqlParameter("@Point", numPoint);
                            }
                        }
                        if (DB.RunSQL(strSQL) < 1)
                        {
                            ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage);
                        }
                    }
                }


                //Response.Redirect("SurveyPage.aspx", false);
                Session["PaperID"] = intPaperID;
                Response.Redirect(lblSurveyPage.Text, false);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("btnSubmit_Click(Exception)", ex.Message);
                Message.MsgShow(this.Page, "系統發生錯誤，請稍後再試。");
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 回上一頁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBack_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 複製工作適能指數所填寫的個人資料
        /// </summary>
        private void Copy_And_Skip()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            long intPaperID = 0;

            try
            {
                intPaperID = Convert.ToInt64(ViewState["PaperID"]);
                if (intPaperID == 0)
                {
                    strSQL = "Insert Into Paper "
                            + "(SurveyID, SessionID, UserID, CreateTime, FromIP, InValid, ParentPaper) "
                            + "Values (@VID, @SID, @UID, GetDate(), @IP, 'N', @PID)";
                    DB.AddSqlParameter("@VID", Convert.ToInt32(ViewState["SurveyID"]));
                    DB.AddSqlParameter("@SID", ViewState["SessionID"].ToString());
                    DB.AddSqlParameter("@UID", Convert.ToInt32(ViewState["UserID"]));
                    DB.AddSqlParameter("@IP", Request.UserHostAddress);
                    DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["ParentPaper"]));
                    intPaperID = DB.InsertOneRecord(strSQL);
                    if (intPaperID < 1)
                    {
                        ShareFunction.PutLog("Copy_And_Skip", DB.DBErrorMessage);
                        Message.MsgShow(this.Page, "很抱歉，平台無法建立您的調查資料，請稍後再試。");
                        return;
                    }
                    else
                    {
                        // 複製前一份問卷的基本資料到這一份問卷中
                        strSQL = "Insert Into PaperDetail "
                                + "(PaperID, QuestionID, SurveyID, QTitle, CategoryID, SelectOption, SelectValue, AnswerValue) "
                                + "Select @ID, QuestionID, @VID, QTitle, CategoryID, SelectOption, SelectValue, AnswerValue "
                                + "From PaperDetail "
                                + "Where PaperID=@PID And CategoryID=1";
                        DB.AddSqlParameter("@ID", intPaperID);
                        DB.AddSqlParameter("@VID", Convert.ToInt32(ViewState["SurveyID"]));
                        DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["ParentPaper"]));
                        if (DB.RunSQL(strSQL) < 1)
                        {
                            ShareFunction.PutLog("Copy_And_Skip", DB.DBErrorMessage);
                        }
                    }
                }
                Session["PaperID"] = intPaperID;
                Response.Redirect(lblSurveyPage.Text, false);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Copy_And_Skip", ex.Message );
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 載入會員的個人資料
        /// </summary>
        private void Load_Profile()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData = null;
            TableRow QRow;
            TableCell QCell;
            QuestionClass clsQuestion;
            string strRules = "";
            Control objControl = null;

            try
            {
                strSQL = "Select U.QuestionID, U.SelectValue, Q.QType " +
                         "From UserProfile U Join Question Q On U.QuestionID=Q.QuestionID " +
                         "Where U.UserID=@UID";
                DB.AddSqlParameter("@UID", Convert.ToInt32(Session["UserID"]));
                myData = DB.GetDataTable(strSQL);

                if (DB!=null)
                {
                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        QRow = new TableRow();
                        QCell = new TableCell();
                        QCell.Style.Add("padding", "3px");
                        QCell.Style.Add("width", "100%");
                        switch (Convert.ToInt32(myData.Rows[i]["QType"]))
                        {
                            case 11:    // 文字輸入
                                objControl = ShareFunction.FindControlEx(fmPageTable, "fm_QControl_" + myData.Rows[i]["QuestionID"].ToString());
                                if (objControl!=null)
                                {
                                    TextBox myText = (TextBox)objControl;
                                    myText.Text = myData.Rows[i]["SelectValue"].ToString();
                                }
                                break;
                            case 12:    // 單選鈕
                                objControl = ShareFunction.FindControlEx(fmPageTable, "fm_QControl_" + myData.Rows[i]["QuestionID"].ToString());
                                if (objControl != null)
                                {
                                    RadioButtonList myRadio = (RadioButtonList)objControl;
                                    myRadio.SelectedValue = myData.Rows[i]["SelectValue"].ToString();
                                }
                                break;
                            case 13:    // 複選
                                break;
                            case 14:    // 單選下拉選單
                                objControl = ShareFunction.FindControlEx(fmPageTable, "fm_QControl_" + myData.Rows[i]["QuestionID"].ToString());
                                if (objControl != null)
                                {
                                    DropDownList myList = (DropDownList)objControl;
                                    myList.SelectedValue = myData.Rows[i]["SelectValue"].ToString();
                                }
                                break;
                            case 15:    // 多選下拉選單
                                // 不支援，所以不處理
                                break;
                            case 16:    // 日期輸入
                                break;
                            case 17:    // 量尺
                                break;
                            case 18:    // 資料清單
                                break;
                            case 101:    // 性別選項
                                objControl = ShareFunction.FindControlEx(fmPageTable, "fm_QControl_" + myData.Rows[i]["QuestionID"].ToString());
                                if (objControl != null)
                                {
                                    RadioButtonList myRadio = (RadioButtonList)objControl;
                                    myRadio.SelectedValue = myData.Rows[i]["SelectValue"].ToString();
                                }
                                break;
                            case 105:    // 教育程度選項
                                objControl = ShareFunction.FindControlEx(fmPageTable, "fm_QControl_" + myData.Rows[i]["QuestionID"].ToString());
                                if (objControl != null)
                                {
                                    RadioButtonList myRadio = (RadioButtonList)objControl;
                                    myRadio.SelectedValue = myData.Rows[i]["SelectValue"].ToString();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    // 沒有個人資料
                    if (DB.DBErrorMessage!="")
                    {
                        ShareFunction.PutLog("Load_Profile", DB.DBErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                DB = null;
            }
        }
    }
}