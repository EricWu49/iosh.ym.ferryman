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
    public partial class Human02 : System.Web.UI.Page
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
                    // 沒有問卷編號，異常狀況，回到首頁
                    Response.Redirect("Default.aspx", false);
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

                Get_List();

                // 開始載入題目
                if (lstQuestion.Items.Count>0)
                {
                    Load_Question();
                }
            }
            else
            {
                // 維持Session紀錄
                Session["PaperID"] = ViewState["PaperID"];
                Session["SessionID"] = ViewState["SessionID"];
            }
        }

        private void Get_List()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select QuestionID "
                        + "From HumanData "
                        + "Where QuestionID In (Select QuestionID From Question Where SurveyID=6 And PageNo=2 And Deleted='N') "
                        + "      And PaperID=@ID And SelectValue='1' "
                        + "Order By UniqueID";
                DB.AddSqlParameter("@ID", Convert.ToInt64(ViewState["PaperID"]));
                myData = DB.GetDataTable(strSQL);
                if (DB.DBErrorMessage != "")
                {
                    ShareFunction.PutLog("Get_List", DB.DBErrorMessage);
                    Message.MsgShow(this.Page, "系統發生異常。");
                    lstQuestion.Items.Clear();
                    lstQuestion.SelectedIndex = -1;
                }
                else
                {
                    if (myData == null)
                    {
                        // 全部回答沒有
                        //Message.MsgShow_And_Redirect(this.Page, "恭喜您，您工作時的姿勢並沒有異常。", "Default.aspx");
                        lstQuestion.Items.Clear();
                        lstQuestion.SelectedIndex = -1;
                        Response.Redirect("HumanFinish.aspx", false);
                    }
                    else
                    {
                        for (int i=0; i<myData.Rows.Count; i++ )
                        {
                            lstQuestion.Items.Add(myData.Rows[i]["QuestionID"].ToString());
                        }
                        lstQuestion.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Get_List", ex.Message);
            }
            finally
            {
                DB = null;
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
            string strQuestionID = "";

            try
            {
                strQuestionID = lstQuestion.SelectedItem.Text;
                strSQL = "Select QuestionID, QTitle, IsOptional, QType "
                        + "From Question "
                        + "Where SurveyID=6 And PageNo>2 And ParentID=@ID And Deleted='N' "
                        + "Order By OrderNo";
                DB.AddSqlParameter("@ID", Convert.ToInt32(lstQuestion.SelectedItem.Value));
                myData = DB.GetDataTable(strSQL);
                if (DB.DBErrorMessage != "")
                {
                    ShareFunction.PutLog("Load_Question", DB.DBErrorMessage);
                    Message.MsgShow(this.Page, "系統發生異常。");
                }
                else
                {
                    if (myData==null)
                    {
                        Message.MsgShow_And_Redirect(this.Page, "系統資料尚未設定完成，敬請見諒。", "ComingSoon.aspx");
                    }
                    else
                    {
                        Repeater1.DataSource = myData;
                        Repeater1.DataBind();
                    }
                    if (lstQuestion.SelectedIndex==lstQuestion.Items.Count-1)
                    {
                        btnSubmit.Text = "問卷結束";
                    }
                    else
                    {
                        btnSubmit.Text = "下一頁";
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
            CheckBoxList rdbOption;
            Label lblQuestion;
            DataTable myData;

            try
            {
                if (!Check_Required())
                {
                    Message.MsgShow(this.Page, "您還有題目沒有選擇，請檢查。");
                    return;
                }

                intPaperID = Convert.ToInt64(ViewState["PaperID"]);

                foreach (RepeaterItem objItem in Repeater1.Items)
                {
                    rdbOption = (CheckBoxList)ShareFunction.FindControlEx(objItem, "rdbOption");
                    strQID = rdbOption.Attributes["QuestionID"].ToString();

                    lblQuestion = (Label)ShareFunction.FindControlEx(objItem, "lblQuestion");
                    strQTitle = lblQuestion.Text;

                    // 判斷是否已經填寫過
                    strSQL = "Select Count(*) From HumanData Where PaperID=@PID And QuestionID=@QID";
                    DB.AddSqlParameter("@PID", intPaperID);
                    DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                    intCount = Convert.ToInt32(DB.GetData(strSQL));

                    if (intCount > 0)
                    {
                        // 改為複選後，有資料需要先刪除再重新新增
                        strSQL = "Delete HumanData Where PaperID=@PID And QuestionID=@QID";
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                        DB.RunSQL(strSQL);
                        if (DB.DBErrorMessage != "")
                        {
                            ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage);
                            Message.MsgShow(this.Page, "系統發生異常，更新失敗。");
                            return;
                        }
                    }

                    // 檢查每個Checkbox
                    foreach (ListItem objCheck in rdbOption.Items)
                    {
                        if (objCheck.Selected)
                        {
                            strValue = objCheck.Value;
                            strText = objCheck.Text;

                            strSQL = "Insert Into HumanData "
                                    + "(PaperID, QuestionID, QTitle, SelectOption, SelectValue) "
                                    + "Values (@PID, @QID, @Title, @Text, @Value)";
                            DB.AddSqlParameter("@PID", intPaperID);
                            DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                            DB.AddSqlParameter("@Title", strQTitle);
                            DB.AddSqlParameter("@Text", strText);
                            DB.AddSqlParameter("@Value", strValue);
                            if (DB.RunSQL(strSQL) < 1)
                            {
                                ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage);
                            }
                        }
                    }
                }
                if (lstQuestion.SelectedIndex == lstQuestion.Items.Count - 1)
                {
                    // 問卷結束
                    Response.Redirect("HumanFinish.aspx", false);
                }
                else
                {
                    lstQuestion.SelectedIndex += 1;
                    Load_Question();
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("btnSubmit_Click", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            DataRowView myRow = (System.Data.DataRowView)e.Item.DataItem;
            DataTable myData;
            Panel myPanel;

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                switch (myRow.Row["QType"].ToString())
                {
                    case "1":
                        myPanel = (Panel)ShareFunction.FindControlEx(e.Item, "PanelTitle");
                        myPanel.Visible = true;
                        myPanel = (Panel)ShareFunction.FindControlEx(e.Item, "PanelQuestion");
                        myPanel.Visible = false;
                        break;
                    case "13":
                        CheckBoxList objRadio = (CheckBoxList)e.Item.FindControl("rdbOption");
                        myData = Get_Option(Convert.ToInt32(myRow.Row["QuestionID"]));
                        if (myData!=null)
                        {
                            for (int i=0; i<myData.Rows.Count; i++)
                            {
                                objRadio.Items.Add(new ListItem(myData.Rows[i]["SelectOption"].ToString(), myData.Rows[i]["SelectValue"].ToString()));
                                if (myData.Rows[i]["DefaultItem"].ToString()=="Y")
                                {
                                    objRadio.Items[i].Selected = true;
                                }
                                if (objRadio.Items[i].Value=="0")
                                {
                                    objRadio.Items[i].Attributes.Add("group", "Group_" + myRow.Row["QuestionID"].ToString() + "_0");
                                    objRadio.Items[i].Attributes.Add("onclick", "javascript:none_check(this);");
                                }
                                else
                                {
                                    objRadio.Items[i].Attributes.Add("group", "Group_" + myRow.Row["QuestionID"].ToString() + "_1");
                                    objRadio.Items[i].Attributes.Add("onclick", "javascript:other_check(this);");
                                }
                            }
                        }

                        //string strRule = objRadio.UniqueID + ":{ required:true }";
                        //if (litRule.Text != "")
                        //{
                        //    litRule.Text += "," + Environment.NewLine;
                        //}
                        //litRule.Text += strRule;

                        myPanel = (Panel)ShareFunction.FindControlEx(e.Item, "PanelTitle");
                        myPanel.Visible = false;
                        myPanel = (Panel)ShareFunction.FindControlEx(e.Item, "PanelQuestion");
                        myPanel.Visible = true;
                        break;
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
                if (DB.DBErrorMessage!="")
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

        private bool Check_Required()
        {
            CheckBoxList rdbOption;
            Label lblWarning;
            bool CheckResult = true;
            bool IsOK = true;

            foreach (RepeaterItem objItem in Repeater1.Items)
            {
                rdbOption = (CheckBoxList)ShareFunction.FindControlEx(objItem, "rdbOption");
                // 不知道為什麼會出現一個沒有選項的CheckboxList，所以需要先檢查是否有子項目
                if (rdbOption.Items.Count>0)
                {
                    lblWarning = (Label)ShareFunction.FindControlEx(objItem, "lblWarning");
                    IsOK = false;
                    foreach (ListItem objCheck in rdbOption.Items)
                    {
                        if (objCheck.Selected)
                        {
                            IsOK = true;
                            break;
                        }
                    }
                    if (!IsOK)
                    {
                        lblWarning.Visible = true;
                    }
                    else
                    {
                        lblWarning.Visible = false;
                    }

                    CheckResult = CheckResult && IsOK;
                }
            }
            return CheckResult;
        }
    }
}