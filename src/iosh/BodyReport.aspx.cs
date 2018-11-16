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
    public partial class BodyReport : System.Web.UI.Page
    {
        int _SurveyID = 10;
        DBClass DB;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PaperID"] == null)
                {
                    Response.Redirect("Default.aspx", false);
                }
                else
                {
                    ViewState.Add("PaperID", Session["PaperID"]);
                    ShareCode.Close_Report(Convert.ToInt64(Session["PaperID"]));
                    _SurveyID = ShareCode.GetSurveyID(Convert.ToInt64(Session["PaperID"]));
                    ViewState.Add("SurveyID", _SurveyID);
                    DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                    Load_Result();
                    DB = null;
                }
            }
        }

        private void Load_Result()
        {
            string strSQL = "";
            DataTable dt1;

            try
            {
                //strSQL = "Select T0.QuestionID, T0.QTitle, T2.ResourceCode, T0.SelectValue as PainValue, IsNull(T1.SelectValue,2)-2 as EffectValue, "
                //       + "        Case When IsNull(T1.SelectValue, 2) - 2 > 0 Then 3 Else "
                //       + "             Case When T0.SelectValue < 2 Then 1 When T0.SelectValue < 4 Then 2 Else 3 End End as PainLevel "
                //       + "From "
                //       + "  (Select QuestionID, QTitle, SelectValue "
                //       + "  From BodyData "
                //       + "  Where PaperID = @ID And QuestionID In "
                //       + "      (Select QuestionID From Question Where SurveyID = @SID And PageNo = 2)) T0 "
                //       + "  Left Join "
                //       + "    (Select Q.ParentID, B.SelectValue "
                //       + "    From BodyData B Join Question Q On B.QuestionID = Q.QuestionID "
                //       + "    Where B.PaperID = @ID And B.QuestionID In "
                //       + "            (Select QuestionID From Question Where SurveyID = @SID And PageNo = 3)) T1 On T0.QuestionID = T1.ParentID "
                //       + "  Join BodySuggest T2 On T0.QuestionID = T2.QuestionID";
                strSQL = "Select T0.QuestionID, T0.QTitle, T2.ResourceCode, T0.SelectValue as PainValue, IsNull(T1.SelectValue,2)-2 as EffectValue, "
                       + "        Case When IsNull(T1.SelectValue, 2) - 2 > 0 Then 3 Else "
                       + "             Case When T0.SelectValue < 2 Then 1 When T0.SelectValue < 4 Then 2 Else 3 End End as PainLevel, T0.SelectValue as PainValue "
                       + "From "
                       + "  (Select QuestionID, QTitle, SelectValue "
                       + "  From BodyData "
                       + "  Where PaperID = @ID And LineID=1 And QuestionID In "
                       + "      (Select QuestionID From Question Where SurveyID = @SID And PageNo = 2)) T0 "
                       + "  Left Join "
                       + "    (Select Q.ParentID, B.SelectValue "
                       + "    From BodyData B Join Question Q On B.QuestionID = Q.QuestionID "
                       + "    Where B.PaperID = @ID And LineID=1 And B.QuestionID In "
                       + "            (Select QuestionID From Question Where SurveyID = @SID And PageNo = 3)) T1 On T0.QuestionID = T1.ParentID "
                       + "  Join BodySuggest T2 On T0.QuestionID = T2.QuestionID";
                DB.AddSqlParameter("@ID", Convert.ToInt64(ViewState["PaperID"]));
                DB.AddSqlParameter("@SID", _SurveyID);
                dt1 = DB.GetDataTable(strSQL);

                if (dt1 == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Load_Result", DB.DBErrorMessage);
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
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Result", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        private void Show_Result(DataTable dt)
        {
            string strSQL = "";
            //DataTable myData;
            string strCode = "";
            Literal litElement;
            int i;
            string strLevel = "";
            string strRemind = "";
            string strVideo = "";       // 衛教影片Script
            string strPainValue = "";
            string strPainName = "";
            string strEffect = "";
            string strRisk = "";
            int intRiskIndex, intRiskResult;
            string strPass = "";
            string strSuggest = "";
            string strMotion = "";
            DataTable myData, dtResource, dtSuggest;
            DataColumn myField;
            DataRow myRow;
            DataRow[] drResources;
            int intCount = 0;
            string strPaperSN = "";

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
                    //if (strEffect == "0")
                    //{
                    //    strEffect = "否";
                    //}
                    //else
                    //{
                    //    strEffect = "是";
                    //}

                    // 判斷是否有做基礎動作篩檢，目前只有一題，所以後續處理暫不考慮多題的情形
                    strSQL = "Select SelectValue From BodyData " +
                             "Where PaperID=@PID And QuestionID In (Select QuestionID From Question Where SurveyID=@SID And PageNo=4)";
                    DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["PaperID"]));
                    DB.AddSqlParameter("@SID", _SurveyID);
                    strMotion = DB.GetData(strSQL);
                    strRemind = "";

                    if (strMotion=="1")
                    {
                        // 有做基礎動作篩檢
                        myRow = myData.NewRow();
                        myRow["Position"] = dt.Rows[i]["QTitle"];

                        // 計算未通過檢測的項目數
                        strSQL = "Select Count(B.UniqueID) "
                               + "From BodyData B Join BodyMotion M On B.QuestionID = M.QuestionID "
                               + "Where B.PaperID = @PID And B.QuestionID In (Select QuestionID From Question Where SurveyID = @SID And PageNo > 4) And "
                               + "      M.MotionCode In (Select MotionCode From BodyCheckAction Where PositionID = @Code) And Left(B.SelectValue,1)= '0'";
                        DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["PaperID"]));
                        DB.AddSqlParameter("@SID", _SurveyID);
                        DB.AddSqlParameter("@Code", strCode);
                        intCount = Convert.ToInt32(DB.GetData(strSQL));
                        switch (strLevel)
                        {
                            case "1":       // 低度疼痛
                                strPainName = "低度疼痛";
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
                                        strRisk = "中度風險";       // 2018/11/12 由高度風險修正為中度風險
                                        //strSuggest = "建議由物理治療師進行Re-check";
                                        intRiskIndex = 3;           // 2018/11/12 由4修正為3
                                    }
                                }
                                break;
                            case "2":       // 中度疼痛
                                strPainName = "中度疼痛";
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
                                        strRisk = "低度風險";       // 2018/11/12 由中度風險修正為低度風險
                                        //strSuggest = "建議由物理治療師進行Re-check";
                                        intRiskIndex = 2;           // 2018/11/12 由3修正為2
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
                                        strRisk = "中度風險";           // 2018/11/12 由高度風險修正為中度風險
                                        //strSuggest = "建議由物理治療師進行Re-check";
                                        intRiskIndex = 3;       // 2018/11/12 由4修正為3
                                    }
                                }
                                break;
                            case "3":       // 高度疼痛
                                strPainName = "高度疼痛";
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
                                        strRisk = "中度風險";       // 2018/11/12 由高度風險修正為中度風險
                                        //strSuggest = "建議由物理治療師進行Re-check";
                                        intRiskIndex = 3;           // 2018/11/12 由4修正為3
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
                                        strRisk = "中度風險";       // 2018/11/12 由高度風險修正為中度風險
                                        //strSuggest = "建議由物理治療師進行Re-check";
                                        intRiskIndex = 3;           // 2018/11/12 由4修正為3
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
                                strPainName = "低度疼痛";
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
                                strPainName = "中度疼痛";
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
                                strPainName = "高度疼痛";
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
                    // 挑出衛教人員所設定的影片
                    strSQL = "Select A.Code, A.VideoName, A.Description, A.YoutubeID " +
                             "From BodyResource A Join PaperResource B On A.Code=B.ResourceCode " +
                             "Where B.PaperID=@PID And B.PositionCode = @Code " +
                             "Order By A.Code";
                    DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["PaperID"]));
                    DB.AddSqlParameter("@Code", strCode);
                    dtSuggest = DB.GetDataTable(strSQL);

                    // 挑出各部位的所有衛教影片
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

                    // 若衛教人員有設定資料，將重複的影片剔除
                    if (dtSuggest != null)
                    {
                        for (int j=0; j< dtSuggest.Rows.Count; j++)
                        {
                            drResources= dtResource.Select("Code='" + dtSuggest.Rows[j]["Code"].ToString() + "'");
                            if (drResources.Length>0)
                            {
                                dtResource.Rows.Remove(drResources[0]);
                            }
                        }
                    }

                    if (dtResource!=null)
                    {
                        strVideo = "   <div class=\"row noprint\">" + Environment.NewLine
                                + "      <div class=\"col-xs-12 col-sm-6 col-md-4 col-lg-3\">" + Environment.NewLine
                                + "         <img src = \"object/images/" + strCode + ".jpg\" alt=\"" + dtResource.Rows[0]["Description"].ToString() + "部位圖\" />" + Environment.NewLine
                                + "      </div>" + Environment.NewLine
                                + "      <div class=\"col-xs-12 col-sm-6 col-md-8 col-lg-9\">" + Environment.NewLine
                                + ResourceList(dtSuggest, dtResource, intRiskIndex)
                                + "      </div>" + Environment.NewLine
                                + "   </div>" + Environment.NewLine;
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
                                    + strVideo
                                    + "</div>" + Environment.NewLine;

                    Panel1.Controls.Add(litElement);

                    // 判斷目前這個部位的風險等級是否比綜合判斷的風險等級高
                    if (intRiskIndex>intRiskResult)
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
                if (myData.Rows.Count>0)
                {
                    string strScript = "";
                    Ferryman.Utility.Chart myChart = new Ferryman.Utility.Chart();
                    myChart.MinValue = 0;
                    myChart.MaxValue = 4;
                    strScript = myChart.RadarChart("myChart_Radar", myData, "風險等級", "Position", "RiskValue");
                    ClientScript.RegisterStartupScript(this.Page.GetType(), "radar_script", strScript, true);
                    //strScript = myChart.BarChart("myChart_Bar", myData, "風險等級", "Position", "RiskValue");
                    //ClientScript.RegisterStartupScript(this.Page.GetType(), "bar_script", strScript, true);
                    RiskReport(myData);
                }
                else
                {
                    pnlChart.Visible = false;
                }

                // 輸出報告序號
                strSQL = "Select PaperSN From Paper Where PaperID=@ID";
                DB.AddSqlParameter("@ID", Convert.ToInt32(ViewState["PaperID"]));
                strPaperSN = DB.GetData(strSQL);
                if (strPaperSN=="")
                {
                    //strPaperSN = "本份評估報告不適用序號";
                    pnlPaperSN.Visible = false;
                }
                else
                {
                    strPaperSN = strPaperSN + Convert.ToInt32(ViewState["PaperID"]).ToString("00000");
                    pnlPaperSN.Visible = true;
                }
                lblPaperSN.Text = strPaperSN;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Show_Result", ex.Message);
            }
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            if (ViewState["PaperID"] != null)
            {
                string strPage = "";
                strPage = ShareCode.GoBackt_Report(Convert.ToInt64(ViewState["PaperID"].ToString()));
                Response.Redirect(strPage, false);
            }
            else
            {
                Response.Redirect("Default.aspx", false);
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

        private string ResourceList(DataTable dtSuggest, DataTable myData, int RiskIndex)
        {
            string strReturn = "";
            string strStyle = "";
            int i = 0;

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

            if (dtSuggest!=null)
            {
                strReturn += "<div class=\"col-xs-12 col-sm-12 col-md-12 col-lg-12 alert alert-info\">" + Environment.NewLine;
                strReturn += "推薦給您的衛教影片：" + Environment.NewLine;
                strReturn += "   </div>" + Environment.NewLine;
                for (i = 0; i < dtSuggest.Rows.Count; i++)
                {
                    strReturn += "   <div class=\"col-xs-12 col-sm-12 col-md-12 col-lg-6\" style=\"height:50px;\">" + Environment.NewLine;
                    strReturn += "         <a class=\"btn " + strStyle + "\" onclick = \"GetVedio('" + dtSuggest.Rows[i]["YoutubeID"].ToString() + "')\" href = \"#exampleModal\" data-toggle = \"modal\" role=\"button\" >" + Environment.NewLine;
                    strReturn += "            [影片] : " + dtSuggest.Rows[i]["Description"].ToString() + Environment.NewLine;
                    strReturn += "         </a>" + Environment.NewLine;
                    strReturn += "   </div>" + Environment.NewLine;
                }
            }

            if (myData!=null)
            {
                strReturn += "<div class=\"col-xs-12 col-sm-12 col-md-12 col-lg-12 alert alert-info\">" + Environment.NewLine;
                strReturn += "相關的衛教影片：" + Environment.NewLine;
                strReturn += "   </div>" + Environment.NewLine;
                if (myData.Rows.Count>0)
                {
                    for (i = 0; i < myData.Rows.Count; i++)
                    {
                        strReturn += "   <div class=\"col-xs-12 col-sm-12 col-md-12 col-lg-6\" style=\"height:50px;\">" + Environment.NewLine;
                        strReturn += "         <a class=\"btn " + strStyle + "\" onclick = \"GetVedio('" + myData.Rows[i]["YoutubeID"].ToString() + "')\" href = \"#exampleModal\" data-toggle = \"modal\" role=\"button\" >" + Environment.NewLine;
                        strReturn += "            [影片] : " + myData.Rows[i]["Description"].ToString() + Environment.NewLine;
                        strReturn += "         </a>" + Environment.NewLine;
                        strReturn += "   </div>" + Environment.NewLine;
                    }
                }
            }
            strReturn = "<div class=\"row\">" + strReturn + "</div>";
            return strReturn;
        }
    }
}