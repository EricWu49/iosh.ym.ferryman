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
    public partial class Place01 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 暫存調查編號
                ViewState.Add("SurveyID", 6);
                Session["SurveyID"] = 6;

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
            Load_Question_186(ref DB);
            Load_Question_258(ref DB);
            Load_Question_187(ref DB);
            DB = null;
        }

        private void Load_Question_186(ref DBClass DB)
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select OptionID, SelectOption, FreeItem, ColumnNo, SortID From OptionData Where QuestionID=186 And Disabled='N' Order By SortID";
                myData = DB.GetDataTable(strSQL);
                Repeater1.DataSource = myData;
                Repeater1.DataBind();
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Question_186", ex.Message);
            }
        }

        private void Load_Question_258(ref DBClass DB)
        {
            string strSQL = "";
            string strRule = "";

            try
            {
                strSQL = "Select AttrValue From QuestionAttribute Where QuestionID=258 And AttrName='rules'";
                strRule = DB.GetData(strSQL);
                strRule = txtQuestion258.UniqueID + ":{ " + strRule + " }";
                if (litRule.Text != "")
                {
                    litRule.Text += "," + Environment.NewLine;
                }
                litRule.Text += strRule;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Question_258", ex.Message);
            }
        }

        private void Load_Question_187(ref DBClass DB)
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select QuestionID, QTitle, IsOptional "
                        + "From Question "
                        + "Where ParentID=187 And Deleted='N' "
                        + "Order By OrderNo";
                myData = DB.GetDataTable(strSQL);
                if (DB.DBErrorMessage != "")
                {
                    ShareFunction.PutLog("Load_Question_187", DB.DBErrorMessage);
                }
                Repeater2.DataSource = myData;
                Repeater2.DataBind();
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Question_187", ex.Message);
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
            CheckBox chkOption;
            TextBox txtOption;
            RadioButtonList rdbOption;
            Label lblQuestion;
            int intError = 0;

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
                            if (DB.RunSQL(strSQL) < 1)
                            {
                                ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage);
                            }
                        }
                    }
                    // 已經處理完畢問卷關聯，所以清除ParentPaper資料，以免後續程式重複執行
                    ViewState["ParentPaper"] = 0;
                }

                // 第一題
                // 因為是複選題，所以要先刪除所有已經存在的資料，再重新寫入
                strSQL = "Delete PlaceData Where PaperID=@PID And QuestionID=@QID";
                DB.AddSqlParameter("@PID", intPaperID);
                DB.AddSqlParameter("@QID", 186);
                DB.RunSQL(strSQL);
                if (DB.DBErrorMessage!="")
                {
                    ShareFunction.PutLog("btnSubmit_Click(1)", DB.DBErrorMessage);
                }

                foreach (RepeaterItem objItem in Repeater1.Items)
                {
                    chkOption = (CheckBox)ShareFunction.FindControlEx(objItem, "chkOption186");
                    if (chkOption.Checked)
                    {
                        strQID = chkOption.Attributes["QuestionID"].ToString();
                        strValue = chkOption.Attributes["OptionID"].ToString();
                        strText = chkOption.Text;

                        txtOption = (TextBox)ShareFunction.FindControlEx(objItem, "txtOption186");
                        if (txtOption.Visible)
                        {
                            strText = txtOption.Text;
                        }

                        strQTitle = lblQuestion186.Text;

                        // 因為是複選題，所以程式先刪除所有已經存在的資料，因此這裡只需要新增即可再重新寫入
                        strSQL = "Insert Into PlaceData "
                                + "(PaperID, QuestionID, QTitle, SelectOption, SelectValue) "
                                + "Values (@PID, @QID, @Title, @Text, @Value)";
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                        DB.AddSqlParameter("@Title", strQTitle);
                        DB.AddSqlParameter("@Text", strText);
                        DB.AddSqlParameter("@Value", strValue);
                        if (DB.RunSQL(strSQL) < 1)
                        {
                            intError += 1;
                            ShareFunction.PutLog("btnSubmit_Click(1)", DB.DBErrorMessage);
                        }
                    }
                }

                // 第二題
                strQID = txtQuestion258.Attributes["QuestionID"].ToString();
                strValue = txtQuestion258.Text;
                strText = txtQuestion258.Text;
                strQTitle = lblQuestion258.Text;

                // 判斷是否已經填寫過
                strSQL = "Select Count(*) From PlaceData Where PaperID=@PID And QuestionID=@QID";
                DB.AddSqlParameter("@PID", intPaperID);
                DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                intCount = Convert.ToInt32(DB.GetData(strSQL));

                if (intCount > 0)
                {
                    // 更新
                    strSQL = "Update PlaceData "
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
                    strSQL = "Insert Into PlaceData "
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
                    intError += 1;
                    ShareFunction.PutLog("btnSubmit_Click(2)", DB.DBErrorMessage);
                }

                // 第三題
                foreach (RepeaterItem objItem in Repeater2.Items)
                {
                    rdbOption = (RadioButtonList)ShareFunction.FindControlEx(objItem, "rdbOption187");
                    strQID = rdbOption.Attributes["QuestionID"].ToString();
                    strValue = rdbOption.SelectedValue;
                    strText = rdbOption.SelectedItem.Text;

                    lblQuestion = (Label)ShareFunction.FindControlEx(objItem, "lblQuestion187");
                    strQTitle = lblQuestion.Text;

                    // 判斷是否已經填寫過
                    strSQL = "Select Count(*) From PlaceData Where PaperID=@PID And QuestionID=@QID";
                    DB.AddSqlParameter("@PID", intPaperID);
                    DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                    intCount = Convert.ToInt32(DB.GetData(strSQL));

                    if (intCount > 0)
                    {
                        // 更新
                        strSQL = "Update PlaceData "
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
                        strSQL = "Insert Into PlaceData "
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
                        intError += 1;
                        ShareFunction.PutLog("btnSubmit_Click(3)", DB.DBErrorMessage);
                    }

                    if (intError==0)
                    {
                        Response.Redirect("SurveyFinish.aspx", false);
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "資料儲存過程發生錯誤。");
                    }
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

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView myRow = (System.Data.DataRowView)e.Item.DataItem;
                CheckBox  objCheck = (CheckBox)e.Item.FindControl("chkOption186");
                TextBox objText;
                if (myRow.Row["FreeItem"].ToString()=="Y")
                {
                    objText = (TextBox)e.Item.FindControl("txtOption186");
                    objText.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    objText.Visible = true;
                    objText.Attributes.Add("disabled", "disabled");
                    objCheck.Attributes.Add("onclick", "others_check(this, '" + objText.ClientID + "', $(this).prop('checked'));");
                    string strRule = objText.UniqueID + ":{ required:!this.disabled }";
                    if (litRule.Text != "")
                    {
                        litRule.Text += "," + Environment.NewLine;
                    }
                    litRule.Text += strRule;
                }
                else
                {
                    objCheck.Attributes.Add("onclick", "javascript: return check_click(this);");
                }
            }
        }

        protected void Repeater2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataTable myData;
                DataRowView myRow = (System.Data.DataRowView)e.Item.DataItem;
                RadioButtonList objRadio = (RadioButtonList)e.Item.FindControl("rdbOption187");
                myData = Get_Option(Convert.ToInt32(myRow.Row["QuestionID"]));
                if (myData != null)
                {
                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        objRadio.Items.Add(new ListItem(myData.Rows[i]["SelectOption"].ToString(), myData.Rows[i]["SelectValue"].ToString()));
                        if (myData.Rows[i]["DefaultItem"].ToString() == "Y")
                        {
                            objRadio.Items[i].Selected = true;
                        }
                    }

                    string strRule = objRadio.UniqueID + ":{ required:true }";
                    if (litRule.Text != "")
                    {
                        litRule.Text += "," + Environment.NewLine;
                    }
                    litRule.Text += strRule;
                }
            }
        }

        private DataTable Get_Option(int QuestionID)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select SelectOption, SelectValue, DefaultItem "
                        + "From OptionData "
                        + "Where QuestionID=@ID And Disabled='N' "
                        + "Order By SortID";
                DB.AddSqlParameter("@ID", QuestionID);
                myData = DB.GetDataTable(strSQL);
                if (DB.DBErrorMessage != "")
                {
                    ShareFunction.PutLog("Get_Option", DB.DBErrorMessage);
                }
                return myData;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Get_Option", ex.Message);
                return null;
            }
            finally
            {
                DB = null;
            }
        }
    }
}