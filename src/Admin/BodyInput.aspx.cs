using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Ferryman.DATA;
using Ferryman.Utility;

namespace Admin
{
    public partial class BodyInput : System.Web.UI.Page
    {
        int _SurveyID = 9;
        int _PaperID = 0;
        string strSQL = "";
        DBClass DB = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState.Add("PaperID", "");
                pnlReport.Visible = false;
            }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            string strPaperSN = "";

            if (txtReportSN.Text.Trim() == "")
            {
                Ferryman.Utility.Message.MsgShow(this.Page, "請輸入評估報告序號。");
                pnlReport.Visible = false;
            }
            else
            {
                if (txtReportSN.Text.Length > 10)
                {
                    strPaperSN = txtReportSN.Text.Substring(0, 10);
                }
                else
                {
                    strPaperSN = txtReportSN.Text;
                }
                DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                Get_Report(strPaperSN);
                if (_PaperID < 0)
                {
                    Message.MsgShow(this.Page, "評估報告序號不存在，請重新輸入。");
                }
                else
                {
                    if (_PaperID == 0)
                    {
                        Message.MsgShow(this.Page, "該份報告尚未完成基礎動作控制篩檢。");
                    }
                    else
                    {
                        Load_Report();
                    }
                }
                DB = null;
            }
        }

        /// <summary>
        /// 由評估報告序號取得報告ID
        /// </summary>
        /// <param name="strReportSN">評估報告序號</param>
        private void Get_Report(string strReportSN)
        {
            DataTable myData = null;
            string strReturn = "";

            try
            {
                strSQL = "Select PaperID, SessionID, SurveyID From Paper Where PaperSN=@SN";
                DB.AddSqlParameter("@SN", strReportSN);
                myData = DB.GetDataTable(strSQL);

                if (myData != null)
                {
                    ViewState["PaperID"] = myData.Rows[0]["PaperID"].ToString();

                    strSQL = "Select Count(*) From BodyData " +
                             "Where PaperID=@PID And QuestionID In (Select QuestionID From Question Where SurveyID=@SID And PageNo=5 And Deleted='N')";
                    DB.AddSqlParameter("@PID", Convert.ToInt32(ViewState["PaperID"]));
                    DB.AddSqlParameter("@SID", _SurveyID);
                    strReturn = DB.GetData(strSQL);
                    if (Convert.ToInt32(strReturn) > 0)
                    {
                        // 此份報告已經全數完成
                        _PaperID = Convert.ToInt32(ViewState["PaperID"]);
                    }
                    else
                    {
                        // 此份報告沒有做過基礎動作控制篩檢
                        _PaperID = 0;
                    }
                }
                else
                {
                    _PaperID = -1;
                    if (DB.DBErrorMessage!="")
                    {
                        ShareFunction.PutLog("Get_Report", DB.DBErrorMessage);
                    }
                }
                hidPaperID.Value = _PaperID.ToString();
            }
            catch (Exception ex)
            {
                _PaperID = -1;
                ShareFunction.PutLog("Get_Report", ex.Message);
            }
        }

        /// <summary>
        /// 取得報告內容
        /// </summary>
        private void Load_Report()
        {
            string strSQL = "";
            DataTable dt1;

            try
            {
                strSQL = "Select T0.QuestionID, T0.QTitle, T2.ResourceCode, T0.SelectValue as PainValue, IsNull(T1.SelectValue,2)-2 as EffectValue, "
                       + "        Case When IsNull(T1.SelectValue, 2) - 2 > 0 Then 3 Else "
                       + "             Case When T0.SelectValue < 2 Then 1 When T0.SelectValue < 4 Then 2 Else 3 End End as PainLevel, T0.SelectValue as PainValue "
                       + "From "
                       + "  (Select QuestionID, QTitle, SelectValue "
                       + "  From BodyData "
                       + "  Where PaperID = @ID And QuestionID In "
                       + "      (Select QuestionID From Question Where SurveyID = @SID And PageNo = 2)) T0 "
                       + "  Left Join "
                       + "    (Select Q.ParentID, B.SelectValue "
                       + "    From BodyData B Join Question Q On B.QuestionID = Q.QuestionID "
                       + "    Where B.PaperID = @ID And B.QuestionID In "
                       + "            (Select QuestionID From Question Where SurveyID = @SID And PageNo = 3)) T1 On T0.QuestionID = T1.ParentID "
                       + "  Join BodySuggest T2 On T0.QuestionID = T2.QuestionID";
                DB.AddSqlParameter("@ID", _PaperID);
                DB.AddSqlParameter("@SID", _SurveyID);
                dt1 = DB.GetDataTable(strSQL);

                if (dt1 == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Load_Report", DB.DBErrorMessage);
                        Message.MsgShow(this.Page, "系統發生錯誤。");
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "系統找不到您的報告。");
                    }
                }
                else
                {
                    Repeater1.DataSource = dt1;
                    Repeater1.DataBind();

                    Show_Result(dt1);
                    pnlReport.Visible = true;
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Report", ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 顯示評估結果與衛教影片
        /// </summary>
        /// <param name="dt">報告資料</param>
        private void Show_Result(DataTable dt)
        {
            string strSQL = "";
            string strCode = "";
            Literal litElement;
            int i;
            string strLevel = "";
            string strRemind = "";
            string strVideo = "";       // 衛教影片Script
            string strPainValue = "";
            string strEffect = "";
            string strRisk = "";
            int intRiskIndex, intRiskResult;
            string strPass = "";
            string strSuggest = "";
            string strMotion = "";
            DataTable myData, dtResource;
            DataColumn myField;
            DataRow myRow;
            int intCount = 0;

            try
            {
                // 產生圖表所需要的資料表
                myData = new DataTable();
                myField = new DataColumn("Position", System.Type.GetType("System.String"));
                myData.Columns.Add(myField);
                myField = new DataColumn("RiskValue", System.Type.GetType("System.Int32"));
                myData.Columns.Add(myField);
                myField = new DataColumn("RiskName", System.Type.GetType("System.String"));
                myData.Columns.Add(myField);

                intRiskResult = 0;      // 各部位綜合比較最高風險等級
                intRiskIndex = 0;       // 單一部位的風險等級
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    litElement = new Literal();

                    strCode = dt.Rows[i]["ResourceCode"].ToString();
                    strLevel = dt.Rows[i]["PainLevel"].ToString();
                    strPainValue = dt.Rows[i]["PainValue"].ToString();
                    strEffect = dt.Rows[i]["EffectValue"].ToString();

                    // 判斷是否有做基礎動作篩檢，目前只有一題，所以後續處理暫不考慮多題的情形
                    strSQL = "Select SelectValue From BodyData " +
                             "Where PaperID=@PID And QuestionID In (Select QuestionID From Question Where SurveyID=@SID And PageNo=4)";
                    DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["PaperID"]));
                    DB.AddSqlParameter("@SID", _SurveyID);
                    strMotion = DB.GetData(strSQL);
                    strRemind = "";

                    if (strMotion == "1")
                    {
                        // 有做基礎動作篩檢
                        myRow = myData.NewRow();
                        myRow["Position"] = dt.Rows[i]["QTitle"];

                        // 計算未通過檢測的項目數
                        strSQL = "Select Count(B.UniqueID) "
                               + "From BodyData B Join BodyMotion M On B.QuestionID = M.QuestionID "
                               + "Where B.PaperID = @PID And B.QuestionID In (Select QuestionID From Question Where SurveyID = @SID And PageNo > 4) And "
                               + "      M.MotionCode In (Select MotionCode From BodyCheckAction Where PositionID = @Code) And Left(B.SelectValue,1)= '0'";
                        DB.AddSqlParameter("@PID", _PaperID);
                        DB.AddSqlParameter("@SID", _SurveyID);
                        DB.AddSqlParameter("@Code", strCode);
                        intCount = Convert.ToInt32(DB.GetData(strSQL));
                        switch (strLevel)
                        {
                            case "1":       // 低度疼痛
                                if (strEffect == "0")
                                {
                                    // 沒有影響工作表現
                                    strEffect = "否";
                                    if (intCount > 0)
                                    {
                                        // 至少一個項目檢測未通過
                                        strPass = "未通過篩檢";
                                        strRisk = "低度風險";
                                        //strSuggest = "無";
                                        intRiskIndex = 2;
                                    }
                                    else
                                    {
                                        // 通過檢測
                                        strPass = "通過篩檢";
                                        strRisk = "極低度/暫無風險";
                                        //strSuggest = "無";
                                        intRiskIndex = 1;
                                    }
                                }
                                else
                                {
                                    // 影響工作表現
                                    strEffect = "是";
                                    if (intCount > 0)
                                    {
                                        // 至少一個項目檢測未通過
                                        strPass = "未通過篩檢";
                                        strRisk = "高度風險";
                                        //strSuggest = "建議由物理治療師進行進階動作控制篩檢";
                                        intRiskIndex = 4;
                                    }
                                    else
                                    {
                                        // 通過檢測
                                        strPass = "通過篩檢";
                                        strRisk = "高度風險";
                                        //strSuggest = "建議由物理治療師進行Re-check";
                                        intRiskIndex = 4;
                                    }
                                }
                                break;
                            case "2":       // 中度疼痛
                                if (strEffect == "0")
                                {
                                    // 沒有影響工作表現
                                    strEffect = "否";
                                    if (intCount > 0)
                                    {
                                        // 至少一個項目檢測未通過
                                        strPass = "未通過篩檢";
                                        strRisk = "中度風險";
                                        //strSuggest = "建議由物理治療師進行進階動作控制篩檢";
                                        intRiskIndex = 3;
                                    }
                                    else
                                    {
                                        // 通過檢測
                                        strPass = "通過篩檢";
                                        //strRisk = "PT Recheck";
                                        strRisk = "中度風險";
                                        //strSuggest = "建議由物理治療師進行Re-check";
                                        intRiskIndex = 3;       // 待確認等級
                                    }
                                }
                                else
                                {
                                    // 影響工作表現
                                    strEffect = "是";
                                    if (intCount > 0)
                                    {
                                        // 至少一個項目檢測未通過
                                        strPass = "未通過篩檢";
                                        strRisk = "高度風險";
                                        //strSuggest = "建議由物理治療師進行進階動作控制篩檢";
                                        intRiskIndex = 4;
                                    }
                                    else
                                    {
                                        // 通過檢測
                                        strPass = "通過篩檢";
                                        strRisk = "高度風險";
                                        //strSuggest = "建議由物理治療師進行Re-check";
                                        intRiskIndex = 4;       // 待確認等級
                                    }
                                }
                                break;
                            case "3":       // 高度疼痛
                                if (strEffect == "0")
                                {
                                    // 沒有影響工作表現
                                    strEffect = "否";
                                    if (intCount > 0)
                                    {
                                        // 至少一個項目檢測未通過
                                        strPass = "未通過篩檢";
                                        strRisk = "高度風險";
                                        //strSuggest = "建議由物理治療師進行進階動作控制篩檢";
                                        intRiskIndex = 4;
                                    }
                                    else
                                    {
                                        // 通過檢測
                                        strPass = "通過篩檢";
                                        //strRisk = "PT Recheck";
                                        strRisk = "高度風險";
                                        //strSuggest = "建議由物理治療師進行Re-check";
                                        intRiskIndex = 4;
                                    }
                                }
                                else
                                {
                                    // 影響工作表現
                                    strEffect = "是";
                                    if (intCount > 0)
                                    {
                                        // 至少一個項目檢測未通過
                                        strPass = "未通過篩檢";
                                        strRisk = "高度風險";
                                        //strSuggest = "建議由物理治療師進行進階動作控制篩檢";
                                        intRiskIndex = 4;
                                    }
                                    else
                                    {
                                        // 通過檢測
                                        strPass = "通過篩檢";
                                        strRisk = "高度風險";
                                        //strSuggest = "建議由物理治療師進行Re-check";
                                        intRiskIndex = 4;
                                    }
                                }
                                break;
                        }

                        myRow["RiskValue"] = intRiskIndex;
                        myRow["RiskName"] = strRisk;
                        myData.Rows.Add(myRow);
                    }
                    else
                    {
                        // 沒有做基礎動作篩檢
                        switch (strLevel)
                        {
                            case "1":       // 低度疼痛
                                if (strEffect == "0")
                                {
                                    // 沒有影響工作表現
                                    strEffect = "否";
                                    strPass = "尚未檢測";
                                    strRisk = "極低度/暫無風險";
                                    //strSuggest = "若肌肉骨骼症狀加重，建議尋求物理治療師/職業醫護人員進行動作控制篩檢";
                                    intRiskIndex = 5;
                                }
                                else
                                {
                                    // 影響工作表現
                                    strEffect = "是";
                                    strPass = "尚未檢測";
                                    strRisk = "存在風險";
                                    //strSuggest = "建議您尋求物理治療師/職業醫護人員進行動作控制篩檢";
                                    intRiskIndex = 6;
                                }
                                break;
                            case "2":       // 中度疼痛
                                if (strEffect == "0")
                                {
                                    // 沒有影響工作表現
                                    strEffect = "否";
                                    strPass = "尚未檢測";
                                    strRisk = "存在風險";
                                    //strSuggest = "建議您尋求物理治療師/職業醫護人員進行動作控制篩檢";
                                    intRiskIndex = 6;
                                }
                                else
                                {
                                    // 影響工作表現
                                    strEffect = "是";
                                    strPass = "尚未檢測";
                                    strRisk = "存在風險";
                                    //strSuggest = "建議您尋求物理治療師/職業醫護人員進行動作控制篩檢";
                                    intRiskIndex = 6;
                                }
                                break;
                            case "3":       // 高度疼痛
                                if (strEffect == "0")
                                {
                                    // 沒有影響工作表現
                                    strEffect = "否";
                                    strPass = "尚未檢測";
                                    strRisk = "存在風險";
                                    //strSuggest = "建議您尋求物理治療師/職業醫護人員進行動作控制篩檢";
                                    intRiskIndex = 6;
                                }
                                else
                                {
                                    // 影響工作表現
                                    strEffect = "是";
                                    strPass = "尚未檢測";
                                    strRisk = "存在風險";
                                    //strSuggest = "建議您尋求物理治療師/職業醫護人員進行動作控制篩檢";
                                    intRiskIndex = 6;
                                }
                                break;
                        }
                    }

                    // 依據風險等級，設定評估建議內容  2017/9/17需求
                    switch (intRiskIndex)
                    {
                        case 1:     // 極低度
                            strSuggest = "恭喜您目前沒有肌肉骨骼症狀，但是還是要繼續維持良好的運動習慣。<br/>" +
                                         "我們提供給您以下的辦公室運動影片，可以搭配進行，維持良好的肌肉骨骼健康。<br/>" +
                                         "若肌肉骨骼症狀加重，建議尋求物理治療師 / 職業醫護人員進行協助。";
                            break;
                        case 2:     // 低度
                            strSuggest = "建議根據您肌肉骨骼不適之部位，觀看我們所提供之肌肉骨骼自我運動衛教影片。<br/>" +
                                         "若肌肉骨骼症狀加重，建議尋求物理治療師 / 職業醫護人員進行協助。";
                            break;
                        case 3:     // 中度
                            strSuggest = "建議尋找物理治療師進行協助，由物理治療師進行進階動作控制篩檢。<br/>" +
                                         "在此之前，可以建議根據您肌肉骨骼不適之部位，觀看我們所提供之肌肉骨骼自我運動衛教影片。";
                            break;
                        case 4:     // 高度
                            strSuggest = "建議尋找物理治療師進行協助，建議由物理治療師進行進階動作控制篩檢，並且遵循復工指引逐漸重回職場。<br/>" +
                                         "在此之前，可以建議根據您肌肉骨骼不適之部位，觀看我們所提供之肌肉骨骼自我運動衛教影片。";
                            break;
                        case 5:     // 暫無風險
                            strSuggest = "恭喜您目前沒有肌肉骨骼症狀，但是還是要繼續維持良好的運動習慣。<br/>" +
                                         "我們提供給您以下的辦公室運動影片，可以搭配進行，維持良好的肌肉骨骼健康。<br/>" +
                                         "若肌肉骨骼症狀加重，建議尋求物理治療師 / 職業醫護人員進行協助。";
                            break;
                        case 6:     // 存在風險
                            strSuggest = "建議您尋求物理治療師/職業醫護人員進行基礎動作篩檢測試。<br/>" +
                                         "在此之前建議根據您肌肉骨骼不適之部位，觀看我們所提供之肌肉骨骼自我運動衛教影片。";
                            break;
                    }

                    // 依據部位，產出衛教影片連結
                    //strSQL = "Select Description, YoutubeID From BodyResource Where Code=@Code";
                    if (intRiskIndex == 1 || intRiskIndex == 5)
                    {
                        // 極低度風險
                        strSQL = "Select A.Code, A.VideoName, A.Description, A.YoutubeID " +
                                 "From BodyResource A Join BodyRule B On A.Code=B.ResourceCode " +
                                 "Where B.PositionCode = @Code " +
                                 "Order By B.RuleID";
                        DB.AddSqlParameter("@Code", "00");
                    }
                    else
                    {
                        strSQL = "Select Distinct A.Code, A.VideoName, A.Description, A.YoutubeID " +
                                 "From BodyResource A Left Join BodyRule B On A.Code=B.ResourceCode " +
                                 "Where A.Code=@Code OR B.PositionCode=@Code OR " +
                                        "B.PositionCode In (Select PositionCode From BodyPosition Where ParentCode=@Code) " +
                                 "Order By A.Code";
                        DB.AddSqlParameter("@Code", strCode);
                    }
                    dtResource = DB.GetDataTable(strSQL);
                    if (dtResource != null)
                    {
                        //strVideo = "   <div>" + Environment.NewLine
                        //         + "      <div>" + Environment.NewLine
                        //        + ResourceList(dtResource, intRiskIndex)
                        //        + "      </div>" + Environment.NewLine
                        //        + "   </div>" + Environment.NewLine;
                        strVideo = "<div id='resource-" + strCode + "' style='margin-top: 20px;'>" + Environment.NewLine
                                + "可以選擇的衛教影片有：<br/>" + Environment.NewLine
                                + ResourceList(dtResource, intRiskIndex)
                                + "   </div>" + Environment.NewLine
                                + "<div style='clear: both;'></div>";
                    }
                    else
                    {
                        strVideo = "";
                    }

                    strRemind = "<div>疼痛分數(0-5)：" + strPainValue + "<br/>是否影響工作表現：" + strEffect + "<br/>" +
                                "基礎動作篩檢測試：" + strPass + "<br/>風險值：" + strRisk + "<br/>" +
                                "評估建議：<br/><div style='padding-left: 40px;'>" + strSuggest + "</div></div>";
                    litElement.Text = "<div id=\"tab-" + strCode + "\">" + Environment.NewLine
                                    + strRemind + Environment.NewLine
                                    + strVideo + Environment.NewLine
                                    + "<input type=\"button\" id=\"submit-" + strCode + "\" value=\"儲存\" style=\"width: 120px;\" onclick=\"javascript:submitForm('" + strCode + "');\" />" + Environment.NewLine
                                    + "</div>" + Environment.NewLine;

                    Panel1.Controls.Add(litElement);

                    // 判斷目前這個部位的風險等級是否比綜合判斷的風險等級高
                    if (intRiskIndex > intRiskResult)
                    {
                        // 這各部位的風險等級較高，將綜合判斷的風險等級設定為這各部位的風險等級
                        intRiskResult = intRiskIndex;
                    }
                }
                hidRiskID.Value = intRiskResult.ToString();
                switch (intRiskResult)
                {
                    case 1:
                        lblIndex.Text = "極低度";
                        break;
                    case 2:
                        lblIndex.Text = "低度";
                        break;
                    case 3:
                        lblIndex.Text = "中度";
                        break;
                    case 4:
                        lblIndex.Text = "高度";
                        break;
                    case 5:
                        lblIndex.Text = "極低度/暫無風險";
                        break;
                    case 6:
                        lblIndex.Text = "存在風險";
                        break;
                }

                // 如果有做基礎動作檢測，myData會有資料，需要產生圖表，否則不需要產生
                if (myData.Rows.Count > 0)
                {
                    string strScript = "";
                    Ferryman.Utility.Chart myChart = new Ferryman.Utility.Chart();
                    myChart.MinValue = 0;
                    myChart.MaxValue = 4;
                    strScript = myChart.RadarChart("myChart_Radar", myData, "風險等級", "Position", "RiskValue");
                    ClientScript.RegisterStartupScript(this.Page.GetType(), "radar_script", strScript, true);
                    RiskReport(myData);
                }
                else
                {
                }

                // 輸出報告序號
                lblPaperSN.Text = txtReportSN.Text ;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Show_Result", ex.Message);
            }
        }

        private void RiskReport(DataTable myData)
        {
            string strContent = "";

            for (int i = 0; i < myData.Rows.Count; i++)
            {
                strContent += "<li class='level" + myData.Rows[i]["RiskValue"].ToString() + "'>" +
                                myData.Rows[i]["Position"].ToString() + "：" + myData.Rows[i]["RiskName"].ToString() +
                             "</li>" + Environment.NewLine;
            }
            strContent = "<ul>" + Environment.NewLine + strContent + "</ul>";
            litResultList.Text = strContent;
        }

        private string ResourceList(DataTable myData, int RiskIndex)
        {
            string strReturn = "";
            string strStyle = "";

            // 設定資源按鈕樣式
            switch (RiskIndex)
            {
                case 1:
                case 5:
                    strStyle = "btn-info";
                    break;
                case 2:
                    strStyle = "btn-success";
                    break;
                case 3:
                    strStyle = "btn-warning";
                    break;
                case 4:
                case 6:
                    strStyle = "btn-danger";
                    break;
            }

            for (int i = 0; i < myData.Rows.Count; i++)
            {
                strReturn += "   <span style=\"width: 33%; display: block; float: left; \">" + Environment.NewLine;
                strReturn += "         <input type='checkbox' id='video-" + myData.Rows[i]["Code"].ToString() + "' name='video-" + myData.Rows[i]["Code"].ToString() + "' /> ";
                strReturn += myData.Rows[i]["Description"].ToString() + Environment.NewLine;
                strReturn += "   </span>" + Environment.NewLine;
            }
            //strReturn = "<div class=\"row\">" + strReturn + "</div>";
            return strReturn;
        }

    }
}