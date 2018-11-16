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
    public partial class BodyA : System.Web.UI.Page
    {
        int _SurveyID = 10;      // (新)肌肉骨骼症狀評估問卷代碼

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 暫存調查編號
                if (Session["SurveyID"]!=null)
                {
                    _SurveyID = Convert.ToInt32(Session["SurveyID"]);
                }

                ViewState.Add("SurveyID", _SurveyID);
                Session["SurveyID"] = _SurveyID;

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
                strSQL = "Select A.QuestionID, A.QTitle, A.IsOptional, B.AttrValue "
                        + "From Question A Join QuestionAttribute B On A.QuestionID=B.QuestionID And B.AttrName='addition' "
                        + "Where A.SurveyID=@ID And A.PageNo=2 And A.Deleted='N' "
                        + "Order By A.OrderNo";
                DB.AddSqlParameter("@ID", _SurveyID);
                myData = DB.GetDataTable(strSQL);
                if (DB.DBErrorMessage != "")
                {
                    ShareFunction.PutLog("Load_Question", DB.DBErrorMessage);
                }
                Repeater1.DataSource = myData;
                Repeater1.DataBind();
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
        /// 問卷送出(待修改)
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
            RadioButtonList rdbOption, rdbPosition;
            Panel MyPanel;
            Label lblQuestion;
            int intErrorCount = 0;
            string strPage = "";

            try
            {
                intPaperID = Convert.ToInt64(ViewState["PaperID"]);
                intErrorCount = 0;
                foreach (RepeaterItem objItem in Repeater1.Items)
                {
                    rdbOption = (RadioButtonList)ShareFunction.FindControlEx(objItem, "rdbOption");
                    MyPanel = (Panel)ShareFunction.FindControlEx(objItem, "Panel1");
                    rdbPosition = (RadioButtonList)ShareFunction.FindControlEx(objItem, "rdbPosition");
                    strQID = rdbOption.Attributes["QuestionID"].ToString();
                    strValue = rdbOption.SelectedValue;
                    strText = rdbOption.SelectedItem.Text;

                    lblQuestion = (Label)ShareFunction.FindControlEx(objItem, "lblQuestion");
                    strQTitle = lblQuestion.Text;

                    // 判斷是否已經填寫過
                    strSQL = "Select Count(*) From BodyData Where PaperID=@PID And QuestionID=@QID And LineID=1";
                    DB.AddSqlParameter("@PID", intPaperID);
                    DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                    intCount = Convert.ToInt32(DB.GetData(strSQL));

                    if (intCount > 0)
                    {
                        // 更新
                        strSQL = "Update BodyData "
                                + "Set SelectOption=@Text, "
                                + "    SelectValue=@Value "
                                + "Where PaperID=@PID And QuestionID=@QID And LineID=1";
                        DB.AddSqlParameter("@Text", strText);
                        DB.AddSqlParameter("@Value", strValue);
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                    }
                    else
                    {
                        // 新增
                        strSQL = "Insert Into BodyData "
                                + "(PaperID, QuestionID, QTitle, LineID, SelectOption, SelectValue) "
                                + "Values (@PID, @QID, @Title, 1, @Text, @Value)";
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                        DB.AddSqlParameter("@Title", strQTitle);
                        DB.AddSqlParameter("@Text", strText);
                        DB.AddSqlParameter("@Value", strValue);
                    }
                    if (DB.RunSQL(strSQL) < 1)
                    {
                        //Message.MsgShow(this.Page, "資料儲存失敗。");
                        intErrorCount = intErrorCount + 1;
                        ShareFunction.PutLog("btnSubmit_Click", DB.DBErrorMessage + "BodyData(" + intPaperID.ToString() + "," + strQID + ","+strValue);
                    }
                    else
                    {
                        if (MyPanel.Visible )
                        {
                            // 有左右區別的部位
                            if (strValue!="0")
                            {
                                // 有疼痛
                                // 判斷是否已經填寫過
                                strSQL = "Select Count(*) From BodyData Where PaperID=@PID And QuestionID=@QID And LineID=2";
                                DB.AddSqlParameter("@PID", intPaperID);
                                DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                                intCount = Convert.ToInt32(DB.GetData(strSQL));

                                if (intCount > 0)
                                {
                                    // 更新
                                    strSQL = "Update BodyData "
                                            + "Set SelectOption=@Text, "
                                            + "    SelectValue=@Value "
                                            + "Where PaperID=@PID And QuestionID=@QID And LineID=2";
                                    DB.AddSqlParameter("@Text", strText);
                                    DB.AddSqlParameter("@Value", strValue);
                                    DB.AddSqlParameter("@PID", intPaperID);
                                    DB.AddSqlParameter("@QID", Convert.ToInt64(strQID));
                                }
                                else
                                {
                                    // 新增
                                    strSQL = "Insert Into BodyData "
                                            + "(PaperID, QuestionID, QTitle, LineID, SelectOption, SelectValue) "
                                            + "Values (@PID, @QID, @Title, 2, @Text, @Value)";
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
                        }
                    }
                }

                if (intErrorCount>0)
                {
                    Message.MsgShow_And_Redirect(this.Page, "部分回答無法儲存，資料可能會有誤差。", strPage);
                }
                else
                {
                    Response.Redirect("BodyB.aspx", false);
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
                RadioButtonList objRadio = (RadioButtonList)e.Item.FindControl("rdbOption");
                string strRule = objRadio.UniqueID + ":{ required:true }";
                if (litRule.Text != "")
                {
                    litRule.Text += "," + Environment.NewLine;
                }
                litRule.Text += strRule;

                Panel myPanel = (Panel)e.Item.FindControl("Panel1");
                if (myPanel!=null)
                {
                    myPanel.Visible = Convert.ToBoolean(myRow["AttrValue"].ToString());
                }

                if (myPanel.Visible)
                {
                    foreach (ListItem myItem in objRadio.Items)
                    {
                        if (myItem.Value == "0")
                        {
                            myItem.Attributes.Add("onclick", "javascript: showit(" + myRow["QuestionID"].ToString() + ",false);");
                        }
                        else
                        {
                            myItem.Attributes.Add("onclick", "javascript: showit(" + myRow["QuestionID"].ToString() + ",true);");
                        }
                    }
                }
            }
        }
    }
}