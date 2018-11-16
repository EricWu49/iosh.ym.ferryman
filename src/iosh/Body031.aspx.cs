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
    public partial class Body031 : System.Web.UI.Page
    {
        int _SurveyID = 7;      // 自覺肌肉骨骼症狀問卷代碼
        //int _PageNo = 4;        // 目前頁碼
        List<string> lstBody, lstResult;
        List<string> lstQueue, lstAnswer;
        int _QuestionID = 0;
        int _BodyIndex = 0;

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
                    if (Session["SessionID"] != null)
                    {
                        Session["SessionID"] = UserClass.ReadCookie( UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserKey, "");
                    }

                    ViewState.Add("SessionID", Session["SessionID"]);

                    lstBody = new List<string>();
                    lstQueue = new List<string>();
                    lstAnswer = new List<string>();
                    lstResult = new List<string>();

                    // 判斷疼痛最高的兩個部位
                    PaintPoint();

                    if (lstBody.Count > 0)
                    {
                        //ViewState.Add("PageNo", _PageNo);
                        _QuestionID = Convert.ToInt32(lstBody[0]);
                        Load_Question(true);
                        ViewState.Add("lstBody", lstBody);          // 保存要診斷的身體部位
                        ViewState.Add("lstQueue", lstQueue);        // 保存診斷問題紀錄
                        ViewState.Add("lstAnswer", lstAnswer);      // 保存診斷回答紀錄
                        ViewState.Add("lstResult", lstResult);      // 保存診斷部位的診斷結果代碼
                        ViewState.Add("QuestionID", _QuestionID);   // 紀錄目前處理的問題編號
                        ViewState.Add("BodyIndex", _BodyIndex);     // 目前處理的身體部位索引值
                    }
                }
            }
            else
            {
                //_PageNo = Convert.ToInt32(ViewState["PageNo"]);
                lstBody = (List<string>)ViewState["lstBody"];
                lstQueue = (List<string>)ViewState["lstQueue"];
                lstAnswer = (List<string>)ViewState["lstAnswer"];
                lstResult = (List<string>)ViewState["lstResult"];
                _QuestionID = Convert.ToInt32(ViewState["QuestionID"]);
                _BodyIndex = Convert.ToInt32(ViewState["BodyIndex"]);
            }
            
        }

        /// <summary>
        /// 計算疼痛指數，取出前兩大部位
        /// </summary>
        private void PaintPoint()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;
            int intPoint = 0;       // 疼痛指數
            int i = 0;              // 索引
            int intCount = 0;       // 計算已經取得幾個部位

            try
            {
                strSQL = "Select TA.QuestionID, TA.SelectValue, TB.SelectValue, TB.SelectValue+TA.SelectValue*2 as PaintPoint " +
                         "From " +
                            "(Select T0.QuestionID, T1.ParentID, T0.SelectValue " +
                            "From BodyData T0 Join Question T1 On T0.QuestionID = T1.QuestionID And T1.PageNo = 3 " +
                            "Where T0.PaperID = @Paper) TA " +
                            "JOIN " +
                            "(Select T2.QuestionID, T2.SelectValue " +
                            "From BodyData T2 Join Question T3 On T2.QuestionID = T3.QuestionID And T3.PageNo = 2 " +
                            "Where T2.PaperID = @Paper) TB On TA.ParentID = TB.QuestionID " +
                         "Order By TB.SelectValue + TA.SelectValue * 2 desc";
                DB.AddSqlParameter("@Paper", Convert.ToInt64(ViewState["PaperID"]));
                myData = DB.GetDataTable(strSQL);

                if (myData!=null)
                {
                    lstBody.Add(myData.Rows[0]["QuestionID"].ToString());
                    intPoint = Convert.ToInt32(myData.Rows[0]["PaintPoint"]);
                    intCount = 1;

                    if (myData.Rows.Count>1)
                    {
                        i = 1;
                        while(i < myData.Rows.Count && intCount<3)
                        {
                            if (Convert.ToInt32(myData.Rows[i]["PaintPoint"])==intPoint)
                            {
                                lstBody.Add(myData.Rows[i]["QuestionID"].ToString());
                            }
                            else
                            {
                                if (intCount<2)
                                {
                                    lstBody.Add(myData.Rows[i]["QuestionID"].ToString());
                                    intPoint = Convert.ToInt32(myData.Rows[i]["PaintPoint"]);
                                }
                                intCount = intCount + 1;
                            }
                            i = i + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message.ShowError(this.Page, "PaintPoint", ex.Message, "系統執行異常。");
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 下一頁按鈕事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strValue = "";
            //string strQID = "";
            long intPaperID = 0;
            //string strText;
            //int intCount = 0;
            //string strQTitle = "";
            RadioButton rdbAnswer;
            //Label lblQuestion;
            //int intErrorCount = 0;
            //string strPage = "";
            bool HasSelected = false;
            DataTable myData;

            try
            {
                intPaperID = Convert.ToInt64(ViewState["PaperID"]);
                if (intPaperID == 0)
                {
                    // PaperID=0在問卷第二頁是異常狀態，保存資料也會造成判讀有問題
                    Message.MsgShow(this.Page, "資料異常，請重新操作。");
                    return;
               }

                //intErrorCount = 0;
                // 處理問卷回答資料
                foreach (RepeaterItem objItem in Repeater1.Items)
                {
                    rdbAnswer = (RadioButton)ShareFunction.FindControlEx(objItem, "rdbAnswer");
                    if (rdbAnswer.Checked)
                    {
                        lstQueue.Add(lblQID.Text);
                        strValue = rdbAnswer.Attributes["OptionID"].ToString();
                        lstAnswer.Add(strValue);
                        HasSelected = true;
                        break;
                    }
                }

                if (!HasSelected)
                {
                    Message.MsgShow(this.Page, "請選擇最符合您的情況的選項。");
                    return;
                }

                // 依據選擇的OptionID，找出要跳到哪一題，再執行Load_Question
                strSQL = "Select SelectValue, JumpQID From OptionData Where OptionID=@ID ";
                DB.AddSqlParameter("@ID", Convert.ToInt32(strValue));
                myData = DB.GetDataTable(strSQL);
                if (myData==null)
                {
                    Message.ShowError(this.Page, "btnSubmit_Click", DB.DBErrorMessage, "系統發生異常。");
                    return;
                }

                if (myData.Rows[0]["JumpQID"].ToString() == "0")
                {
                    // 表示已經是最後一題
                    lstResult.Add(myData.Rows[0]["SelectValue"].ToString());

                    //lstBody.Remove(lstBody[0]);
                    //if (lstBody.Count==0)
                    if (_BodyIndex == (lstBody.Count-1))
                    {
                            // 結束所有問題了，可以儲存了
                            Save_Answer();
                    }
                    else
                    {
                        _BodyIndex = _BodyIndex + 1;
                        // 還有其他部位要詢問
                        _QuestionID = Convert.ToInt32(lstBody[_BodyIndex]);
                        Load_Question(true);
                        ViewState["BodyIndex"] = _BodyIndex;
                        ViewState["QuestionID"] = _QuestionID;
                    }
                }
                else
                {
                    // 表示還有下一題
                    _QuestionID = Convert.ToInt32(myData.Rows[0]["JumpQID"].ToString());
                    Load_Question(false);
                    ViewState["QuestionID"] = _QuestionID;
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

        /// <summary>
        /// Repeater資料繫結事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // 解決Repeater中的Radio Button無法視為同一個群組的問題，須搭配javascript使用
                RadioButton rdo = (RadioButton)e.Item.FindControl("rdbAnswer");
                string script = "setExclusiveRadioButton('Repeater1.*answer-group',this)";
                rdo.Attributes.Add("onclick", script);
            }
        }

        /// <summary>
        /// 載入問題
        /// </summary>
        /// <param name="EntryQuestion">是否是各部位開始的問題</param>
        private void Load_Question(bool EntryQuestion)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;
            int intQuestionID = 0;

            try
            {
                if (EntryQuestion)
                {
                    strSQL = "Select QuestionID, QTitle From Question Where SurveyID=@Survey And ParentID=@Question And Deleted='N' And CategoryID=4 ";
                }
                else
                {
                    strSQL = "Select QuestionID, QTitle From Question Where SurveyID=@Survey And QuestionID=@Question And Deleted='N' And CategoryID=4 ";
                }
                DB.AddSqlParameter("@Survey", _SurveyID);
                DB.AddSqlParameter("@Question", _QuestionID);
                myData = DB.GetDataTable(strSQL);

                if (myData == null)
                {
                    if (DB.DBErrorMessage!="")
                    {
                        Message.ShowError(this.Page, "Load_Question", DB.DBErrorMessage, "問卷設定存取錯誤");
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "問卷設定異常");
                    }
                    return;
                }
                else
                {
                    intQuestionID = Convert.ToInt32(myData.Rows[0]["QuestionID"]);
                    lblQuestion.Text = myData.Rows[0]["QTitle"].ToString();
                    lblQID.Text = intQuestionID.ToString();

                    strSQL = "Select OptionID, SelectOption, SelectValue From OptionData Where QuestionID=@QID And Disabled='N' Order By SortID";
                    DB.AddSqlParameter("@QID", intQuestionID);
                    myData = DB.GetDataTable(strSQL);

                    if (myData==null)
                    {
                        if (DB.DBErrorMessage != "")
                        {
                            Message.ShowError(this.Page, "Load_Question", DB.DBErrorMessage, "選項設定存取錯誤");
                        }
                        else
                        {
                            Message.MsgShow(this.Page, "選項設定異常");
                        }
                        return;
                    }
                    else
                    {
                        Repeater1.DataSource = myData;
                        Repeater1.DataBind();
                    }
                }   // if else end
            }
            catch (Exception ex)
            {
                Message.ShowError(this.Page, "Load_Question", ex.Message, "系統執行錯誤");
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 儲存
        /// </summary>
        private void Save_Answer()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            int intCount = 0;
            int intPaperID = Convert.ToInt32(ViewState["PaperID"]);
            int intQID, intAID;
            int intErrorCount = 0;

            intQID = 0;
            intAID = 0;

            for (int i=0; i<lstQueue.Count; i++)
            {
                try
                {
                    intQID = Convert.ToInt32(lstQueue[i]);
                    intAID = Convert.ToInt32(lstAnswer[i]);

                    // 判斷是否已經填寫過
                    strSQL = "Select Count(*) From BodyData Where PaperID=@PID And QuestionID=@QID";
                    DB.AddSqlParameter("@PID", intPaperID);
                    DB.AddSqlParameter("@QID", intQID);
                    intCount = Convert.ToInt32(DB.GetData(strSQL));

                    if (intCount > 0)
                    {
                        // 更新
                        strSQL = "Update BodyData "
                                + "Set SelectOption=B.SelectOption, "
                                + "    SelectValue=B.SelectValue "
                                + "From BodyData A, OptionData B "
                                + "Where A.PaperID=@PID And A.QuestionID=@QID And B.OptionID=@AID";
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", intQID);
                        DB.AddSqlParameter("@AID", intAID);
                    }
                    else
                    {
                        // 新增
                        strSQL = "Insert Into BodyData "
                                + "(PaperID, QuestionID, QTitle, SelectOption, SelectValue) "
                                + "Select @PID, @QID, A.QTitle, B.SelectOption, B.SelectValue "
                                + "From Question A ,OptionData B "
                                + "Where A.QuestionID=@QID And B.OptionID=@AID ";
                        DB.AddSqlParameter("@PID", intPaperID);
                        DB.AddSqlParameter("@QID", intQID);
                        DB.AddSqlParameter("@AID", intAID);
                    }

                    if (DB.RunSQL(strSQL) < 1)
                    {
                        intErrorCount = intErrorCount + 1;
                        ShareFunction.PutLog("Save_Answer", DB.DBErrorMessage + "；BodyData(" + intPaperID.ToString() + "," + intQID.ToString() + "," + intAID.ToString());
                    }
                }
                catch (Exception ex)
                {
                    intErrorCount = intErrorCount + 1;
                    ShareFunction.PutLog("Save_Answer", "PaperID=" + intPaperID.ToString() + ",QuestionID=" + intQID.ToString() + ",OptionID=" + intAID.ToString() + "儲存失敗，" + ex.Message);
                }
            }

            for (int i = 0; i < lstBody.Count; i++)
            {
                try
                {
                    strSQL = "Select Count(*) From BodyResult Where PaperID=@PID And QuestionID=@QID";
                    DB.AddSqlParameter("@PID", intPaperID);
                    DB.AddSqlParameter("@QID", Convert.ToInt32(lstBody[i]));
                    if (DB.GetData(strSQL) == "1")
                    {
                        // 已經存在，Update
                        strSQL = "Update BodyResult " +
                                 "Set EvaluateCode=@Code " +
                                 "Where PaperID=@PID And QuestionID=@QID";
                    }
                    else
                    {
                        // 不存在，Insert
                        strSQL = "Insert BodyResult " +
                                 "(PaperID, QuestionID, EvaluateCode) " +
                                 "Values (@PID, @QID, @Code)";
                    }
                    DB.AddSqlParameter("@PID", intPaperID);
                    DB.AddSqlParameter("@QID", Convert.ToInt32(lstBody[i]));
                    DB.AddSqlParameter("@Code", lstResult[i]);

                    if (DB.RunSQL(strSQL)<1)
                    {
                        intErrorCount = intErrorCount + 1;
                        ShareFunction.PutLog("Save_Answer(Result)", DB.DBErrorMessage + "；BodyData(" + intPaperID.ToString() + "," + lstBody[i] + "," + lstResult[i] + ")");
                    }
                }
                catch (Exception ex1)
                {
                    intErrorCount = intErrorCount + 1;
                    ShareFunction.PutLog("Save_Answer", "PaperID=" + intPaperID.ToString() + ",QuestionID=" + intQID.ToString() + ",OptionID=" + intAID.ToString() + "儲存失敗，" + ex1.Message);
                }
            }

            if (intErrorCount>0)
            {
                Message.MsgShow(this.Page, "資料儲存過程有遇到錯誤，無法提供建議報告。");
            }
            else
            {
                Response.Redirect("BodySuggest.aspx", false);
            }

            DB = null;
            Session["PaperID"] = ViewState["PaperID"];
            Session["SessionID"] = ViewState["SessionID"];
        }
    }
}