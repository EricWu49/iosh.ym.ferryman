﻿using System;
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
    public partial class SurveyPage : System.Web.UI.Page
    {
        //int _SurveyID = 0;
        //int _PageNo = 0;
        //Int64 _PaperID = 0;

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

                    if (Session["TotalPage"] == null)
                    {
                        // 沒有總頁數，取得問卷總頁數
                        ViewState.Add("TotalPage", TotalPage(Convert.ToInt32(Session["SurveyID"])));
                    }
                    else
                    {
                        // 紀錄調查總頁數
                        ViewState.Add("TotalPage", Convert.ToInt32(Session["TotalPage"]));
                    }

                    if (Session["PageNo"] == null)
                    {
                        // 沒有頁次，設為1
                        ViewState.Add("PageNo", 1);
                        //_PageNo = 1;
                    }
                    else
                    {
                        // 紀錄目前頁次號碼
                        ViewState.Add("PageNo", Convert.ToInt32(Session["PageNo"]));
                        //_PageNo = Convert.ToInt32(Session["PageNo"]);
                    }

                    if (Session["PaperID"] == null)
                    {
                        // 沒有問卷編號，先設為0
                        ViewState.Add("PaperID", 0);
                    }
                    else
                    {
                        // 紀錄目前的問卷編號
                        ViewState.Add("PaperID", Convert.ToInt64(Session["PaperID"]));
                        //_PaperID = Convert.ToInt64(Session["PaperID"]);
                    }

                    if (Session["ParentPaper"] == null)
                    {
                        // 沒有前一份問卷
                        ViewState.Add("ParentPaper", 0);
                    }
                    else
                    {
                        // 紀錄前一份問卷編號
                        ViewState.Add("ParentPaper", Convert.ToInt64(Session["ParentPaper"]));
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
                        //_PaperID = Convert.ToInt64(Session["PaperID"]);
                    }

                    // 開始載入題目
                    Load_Question();
                }
            }
        }

        /// <summary>
        /// 取得調查的頁數
        /// </summary>
        /// <param name="SurveyID">調查編號</param>
        /// <returns></returns>
        private int TotalPage(int SurveyID)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";

            try
            {
                strSQL = "Select Max(PageNo) From Question Where SurveyID=@ID And Deleted='N'";
                DB.AddSqlParameter("@ID", SurveyID);
                return Convert.ToInt32(DB.GetData(strSQL));
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("TotalPage", ex.Message);
                return 0;
            }
            finally
            {
                DB = null;
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
            string strData = "";
            //WebControl ctlQuestion;

            try
            {
                hidProgress.Value = (Math.Round((Convert.ToDouble(ViewState["PageNo"])-1) / Convert.ToDouble(ViewState["TotalPage"]), 2) * 100).ToString();
                // 因為問卷採用動態載入，SurveyPage.aspx只是一直重複PostBack，所以前一次所設定的Script會持續保存著，因此必須要先清除，才不會造成後續執行錯誤
                litScript.Text = "";
                litValidator.Text = "";

                if (Convert.ToInt64(ViewState["ParentPaper"])==0)
                {
                    // 沒有前一份問卷，需填寫SurveyID為0的基本資料
                    strSQL = "Select QuestionID, QTitle, QDescription, QType, ParentID, IsSubQuestion, IsOptional "
                            + "From Question "
                            + "Where SurveyID In (@SID, 0) And PageNo=@PNo And Deleted='N' "
                            + "Order By OrderNo";
                }
                else
                {
                    // 已經填寫過前一份問卷，直接複製前一份問卷的基本資料過來，然後直接進入第二頁
                    strSQL = "Select QuestionID, QTitle, QDescription, QType, ParentID, IsSubQuestion, IsOptional "
                            + "From Question "
                            + "Where SurveyID=@SID And PageNo=@PNo And Deleted='N' "
                            + "Order By OrderNo";
                    ViewState["PageNo"] = Convert.ToInt32(ViewState["PageNo"]) + 1;
                }
                DB.AddSqlParameter("@SID", Convert.ToInt32(ViewState["SurveyID"]));
                DB.AddSqlParameter("@PNo", Convert.ToInt32(ViewState["PageNo"]));
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
                            case 5:     // 網址
                                // 直接轉址，設定在同一頁的所有題目都會被忽略
                                strSQL = "Select SelectValue From OptionData Where QuestionID=@ID";
                                DB.AddSqlParameter("@ID", Convert.ToInt64(myData.Rows[i]["QuestionID"]));
                                strData = DB.GetData(strSQL);
                                if (strData!=null)
                                {
                                    Response.Redirect(strData, false);
                                    return;
                                }
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
                            case 19:     // 母子選項
                                QCell.Controls.Add(clsQuestion.Create_RelatedRadiobox(myData.Rows[i]));
                                QRow.Cells.Add(QCell);
                                fmPageTable.Rows.Add(QRow);
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
                                    if (Convert.ToInt64(myData.Rows[i]["ParentID"].ToString())==0)
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
                                        litValidator.Text += objControl.UniqueID + ":{" +strRules + "}";
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

                    if (Convert.ToInt32(ViewState["PageNo"]) == Convert.ToInt32(ViewState["TotalPage"]))
                    {
                        btnSubmit.Text = "結束問卷";
                    }
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
                    if (DB.DBErrorMessage!="")
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
            // Debug Info
            //ShareFunction.PutLog("Save", hidResult.Value);

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
            string strPage = "";
            string[] aryQuestionID;
            string strOption = "";
            int intLine = 0;

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
                    DB.AddSqlParameter("@UID", 0);
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
                        if (Convert.ToInt64(ViewState["ParentPaper"]) != 0)
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
                            if (DB.RunSQL(strSQL)<1)
                            {
                                ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage);
                            }
                        }
                    }
                    // 已經處理完畢問卷關聯，所以清除ParentPaper資料，以免後續程式重複執行
                    ViewState["ParentPaper"] = 0;
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
                        strQID = strElement.Substring(intIndex, strElement.Length-intIndex);
                        strQID = strQID.Replace("fm_QControl_", "");
                        aryQuestionID = strQID.Split('_');
                        if (aryQuestionID.Length>1)
                        {
                            if (ShareFunction.IsNumeric(aryQuestionID[1]))
                            {
                                strOption = aryQuestionID[1];
                            }
                            else
                            {
                                strOption = aryQuestionID[2];
                            }
                        }
                        else
                        {
                            strOption = "1";        // 預設值
                        }
                        strQID = aryQuestionID[0];

                        // 取得問題類型
                        strSQL = "Select QType From Question Where QuestionID=@QID";
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                        strType = DB.GetData(strSQL);

                        // 判斷是否已經填寫過
                        strSQL = "Select Count(*) From PaperDetail Where PaperID=@PID And QuestionID=@QID And LineID=@LID";
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                        DB.AddSqlParameter("@LID", Convert.ToInt32(strOption));
                        intCount = Convert.ToInt32(DB.GetData(strSQL));

                        if (intCount>0)
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
                                            + "Where PaperID=@PID And QuestionID=@QID ";
                                    strText = strValue;
                                    break;
                                case "18":  // 資料清單
                                    strSQL = "Update PaperDetail "
                                            + "Set SelectOption=@Text, "
                                            + "    SelectValue=@Value, "
                                            + "    AnswerValue=@Point "
                                            + "Where PaperID=@PID And QuestionID=@QID ";
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
                                case "19":   // 母子選項
                                    strSQL = "Update PaperDetail "
                                            + "Set SelectOption=@Text, "
                                            + "    SelectValue=@Value, "
                                            + "    AnswerValue=@Point "
                                            + "From PaperDetail P Join Question Q On P.QuestionID=Q.QuestionID "
                                            + "                Left Join OptionData O On Q.QuestionID=O.QuestionID "
                                            + "Where P.PaperID=@PID And Q.QuestionID=@QID And P.LineID=@LID And O.ColumnNo=@LID And O.SelectValue=@Text ";
                                    strText = strValue;
                                    DB.AddSqlParameter("@LID", Convert.ToInt32(strOption));
                                    break;
                                default:
                                    strSQL = "Update PaperDetail "
                                            + "Set SelectOption=IsNull(O.SelectOption, D.SelectOption), "
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
                                    for (int i=0; i<aryValue.Length; i++)
                                    {
                                        if (aryValue[i]!="")
                                        {
                                            strText += aryValue[i].Split('|')[1] + "|";
                                            strValue += aryValue[i].Split('|')[0] + "|";
                                            numPoint += Convert.ToSingle(aryValue[i].Split('|')[0]);
                                        }
                                    }
                                    break;
                                case "19":  // 母子選項
                                    strSQL = "Insert Into PaperDetail "
                                            + "(PaperID, QuestionID, LineID, SurveyID, QTitle, CategoryID, SelectOption, SelectValue, AnswerValue) "
                                            + "Select @PID, @QID, @LID, @SID, Q.QTitle, Q.CategoryID, O.SelectOption, @Value, @Point "
                                            + "From Question Q Left Join OptionData O On Q.QuestionID=O.QuestionID "
                                            + "Where Q.QuestionID=@QID And O.SelectValue=@Text And O.ColumnNo=@LID ";
                                    strText = strValue;
                                    numPoint = 0;   // 當Value不是數字時使用
                                    DB.AddSqlParameter("@LID", Convert.ToInt32(strOption));
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

                if (Convert.ToInt32(ViewState["PageNo"])==Convert.ToInt32(ViewState["TotalPage"]))
                {
                    strSQL = "Update Paper Set FinishTime=GetDate() Where PaperID=@PaperID";
                    DB.AddSqlParameter("@PaperID", intPaperID);
                    if (DB.RunSQL(strSQL)<1)
                    {
                        ShareFunction.PutLog("btnSubmit_Click(Finish)", DB.DBErrorMessage);
                    }

                    strSQL = "Insert Into PaperResult "
                            + "(PaperID, DimensionID, SumofValue, SurveyID, Point) "
                            + "Select P.PaperID, D.DimensionID, Sum(P.AnswerValue), P.SurveyID, dbo.udf_GetSurveyPoint(P.SurveyID, D.DimensionID, Sum(P.AnswerValue)) "
                            + "From PaperDetail P Join Question Q On P.QuestionID=Q.QuestionID " 
                            + "                   Join QuestionDimension D On Q.QuestionID=D.QuestionID "
                            + "Where P.PaperID=@PaperID And Q.CategoryID=2 "
                            + "Group By P.PaperID, D.DimensionID, P.SurveyID";
                    DB.AddSqlParameter("@PaperID", intPaperID);
                    DB.RunSQL(strSQL);
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("btnSubmit_Click(Result)", DB.DBErrorMessage);
                    }
                    else
                    {
                        strSQL = "Select P.PageFile "
                                + "From Survey S Join PageType P On S.PageID=P.PageID "
                                + "Where S.SurveyID=@SID";
                        DB.AddSqlParameter("@SID", Convert.ToInt32(ViewState["SurveyID"]));
                        strPage = DB.GetData(strSQL);
                        if (strPage == null)
                        {
                            Message.MsgShow_And_Redirect(this.Page, "問卷已完成，謝謝您。", "Default.aspx");
                            ShareFunction.PutLog("btnSubmit_Click(Redirect)", DB.DBErrorMessage);
                        }
                        else
                        {
                            if (strPage=="")
                            {
                                Message.MsgShow_And_Redirect(this.Page, "問卷已完成，謝謝您。", "Default.aspx");
                            }
                            else
                            {
                                Session["SurveyID"] = ViewState["SurveyID"].ToString();
                                Session["PaperID"] = ViewState["PaperID"].ToString();
                                Session["SessionID"] = ViewState["SessionID"].ToString();
                                Response.Redirect(strPage, false);
                            }
                        }
                    }
                }
                else
                {
                    ViewState["PageNo"] = Convert.ToInt32(ViewState["PageNo"]) + 1;
                    Load_Question();
                }
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

        protected void btnBack_Click(object sender, EventArgs e)
        {

        }
    }
}