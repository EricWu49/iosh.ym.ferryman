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
    public partial class HumanReport : System.Web.UI.Page
    {
        int _SurveyID = 6;
        DBClass DB;
        List<long> lstQuestion;
        List<string> lstQTitle;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState.Add("BaseCount", 0);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            Total_Ratio();
            Level2_Chart();
            Generate_Chart();
            DB = null;
        }

        private void Total_Ratio()
        {
            string strSQL = "";
            string strDate1, strDate2, strFilter;
            int intTotal, intBase;

            try
            {
                if (txtStartDate.Text.Trim() != "")
                {
                    // 有起始日期
                    if (txtEndDate.Text.Trim() != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                        strDate2 = "";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (txtEndDate.Text.Trim() != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                        strDate1 = "";
                        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                // 求工作適能指數為普通與弱的總數
                strSQL = "Select Count(*) From vw_WIndex_Paper_List Where ResultCode In (3,4) ";
                if (strFilter != "")
                {
                    strSQL += "And " + strFilter;

                    if (strDate1 != "")
                    {
                        DB.AddSqlParameter("@Date1", strDate1);
                    }
                    if (strDate2 != "")
                    {
                        DB.AddSqlParameter("@Date2", strDate2);
                    }
                }

                intTotal = Convert.ToInt32(DB.GetData(strSQL));
                lblTotalCount.Text = intTotal.ToString("n0");

                if (strFilter == "")
                {
                    strSQL = "Select Count(*) From vw_Paper_Relation Where SurveyID=@SID "
                            + "And ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4))";
                }
                else
                {
                    strSQL = "Select Count(*) From vw_Paper_Relation Where SurveyID=@SID "
                            + "And ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4) And " + strFilter + ") "
                            + "And " + strFilter;
                    if (strDate1 != "")
                    {
                        DB.AddSqlParameter("@Date1", strDate1);
                    }
                    if (strDate2 != "")
                    {
                        DB.AddSqlParameter("@Date2", strDate2);
                    }
                }
                DB.AddSqlParameter("@SID", _SurveyID);
                intBase = Convert.ToInt32(DB.GetData(strSQL));
                lblThisCount.Text = intBase.ToString("n0");
                lblRatio.Text = (Math.Round(Convert.ToDouble(intBase) / intTotal, 4)).ToString("p2");
                ViewState["BaseCount"] = intBase;
                pnlTotalRatio.Visible = true;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Total_Ratio", ex.Message);
            }
        }

        private void Level2_Chart()
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strFilter = "";
            string strData = "";
            string strSQL = "";
            string strDate1, strDate2, strCategories;
            int intTotal = 0;
            int i;
            DataTable myData;

            try
            {
                strFilter = "";
                if (txtStartDate.Text.Trim() != "")
                {
                    // 有起始日期
                    if (txtEndDate.Text.Trim() != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                        strDate2 = "";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (txtEndDate.Text.Trim() != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                        strDate1 = "";
                        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                if (strFilter == "")
                {
                    // 沒有日期條件
                    strSQL = "Select D.QuestionID, D.QTitle, Sum(Case When D.SelectValue='1' Then 1 Else 0 End) as PaperCount "
                            + "From Paper M Join HumanData D On M.PaperID = D.PaperID "
                            + "		Join vw_Human_Question Q On D.QuestionID=Q.QuestionID "
                            + "Where M.SurveyID = @SID And M.ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4)) "
                            + "      And M.FinishTime Is Not Null And M.Invalid = 'N' And Q.ParentID=0 "
                            + "Group By D.QuestionID, D.QTitle "
                            + "Order By Sum(Case When D.SelectValue='1' Then 1 Else 0 End) Desc";
                }
                else
                {
                    // 有日期條件
                    strSQL = "Select D.QuestionID, D.QTitle, Sum(Case When D.SelectValue='1' Then 1 Else 0 End) as PaperCount "
                            + "From Paper M Join HumanData D On M.PaperID = D.PaperID "
                            + "		Join vw_Human_Question Q On D.QuestionID=Q.QuestionID "
                            + "Where M.SurveyID = @SID And M.ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4) And " + strFilter + ") "
                            + "      And M.FinishTime Is Not Null And M.Invalid = 'N' And Q.ParentID=0 And " + strFilter
                            + "Group By D.QuestionID, D.QTitle "
                            + "Order By COUNT(D.PaperID) Desc";
                }
                DB.AddSqlParameter("@SID", _SurveyID);
                if (strDate1 != "")
                {
                    DB.AddSqlParameter("@Date1", strDate1);
                }
                if (strDate2 != "")
                {
                    DB.AddSqlParameter("@Date2", strDate2);
                }
                myData = DB.GetDataTable(strSQL);

                if (myData != null)
                {
                    // 計算總數
                    intTotal = Convert.ToInt32(ViewState["BaseCount"]);

                    strCategories = "";
                    strData = "";

                    lstQuestion = new List<long>();
                    lstQTitle = new List<string>();

                    for (i = 0; i < myData.Rows.Count; i++)
                    {
                        strCategories += "'" + myData.Rows[i]["QTitle"].ToString() + "',";
                        strData += (Math.Round(Convert.ToDouble(myData.Rows[i]["PaperCount"].ToString()) / intTotal, 4) * 100).ToString() + ",";
                        lstQuestion.Add(Convert.ToInt64(myData.Rows[i]["QuestionID"].ToString()));
                        lstQTitle.Add(myData.Rows[i]["QTitle"].ToString());
                    }
                    strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                    strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                    strScript += "$(function () {" + Environment.NewLine;
                    strScript += "    $('#myChart_Bar').highcharts({" + Environment.NewLine;
                    strScript += "        chart: {" + Environment.NewLine;
                    strScript += "            type: 'bar'" + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        title: {" + Environment.NewLine;
                    strScript += "            text: '人因工程統計報告'" + Environment.NewLine;          // 圖表標題
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        xAxis: {" + Environment.NewLine;
                    strScript += "            categories: " + strCategories + "," + Environment.NewLine;
                    strScript += "            tickmarkPlacement: 'on'," + Environment.NewLine;
                    strScript += "            lineWidth: 0," + Environment.NewLine;
                    strScript += "            labels: {" + Environment.NewLine;
                    strScript += "                style: {'fontSize':'12pt'}" + Environment.NewLine;
                    strScript += "            }" + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        yAxis: {" + Environment.NewLine;
                    strScript += "            lineWidth: 0," + Environment.NewLine;
                    strScript += "            min: 0," + Environment.NewLine;     // 最小值
                    strScript += "            max: 100," + Environment.NewLine;    // 最大值
                    strScript += "            title: {" + Environment.NewLine;
                    strScript += "                text: ''" + Environment.NewLine;
                    strScript += "            }," + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        legend: {" + Environment.NewLine;         // 圖例設定
                    strScript += "            enabled: false" + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        plotOptions: {" + Environment.NewLine;
                    strScript += "            series: {" + Environment.NewLine;
                    strScript += "                dataLabels: {" + Environment.NewLine;
                    strScript += "                    enabled: true," + Environment.NewLine;
                    strScript += "                    format: '{y} %'," + Environment.NewLine;
                    strScript += "                    align: 'right'," + Environment.NewLine;
                    strScript += "                    color: '#FFFFFF'," + Environment.NewLine;
                    strScript += "                    x: -10" + Environment.NewLine;
                    strScript += "                }," + Environment.NewLine;
                    strScript += "                pointPadding: 0.1," + Environment.NewLine;
                    strScript += "                groupPadding: 0" + Environment.NewLine;
                    strScript += "            }" + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        exporting: {" + Environment.NewLine;
                    strScript += "            enabled: true" + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        series: [{" + Environment.NewLine;        // 數值設定
                    strScript += "            name: '百分比'," + Environment.NewLine;
                    strScript += "            data: " + strData + "," + Environment.NewLine;
                    strScript += "        }]" + Environment.NewLine;
                    strScript += "    });" + Environment.NewLine;
                    strScript += "});" + Environment.NewLine;

                    ClientScript.RegisterStartupScript(this.Page.GetType(), "level2_bar_script", strScript, true);
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "系統發生錯誤。");
                        ShareFunction.PutLog("Level2_Chart", DB.DBErrorMessage);
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "沒有符合條件的資料。");
                    }
                }

            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Level2_Chart", ex.Message);
            }
            finally
            {
            }
        }

        private void Generate_Chart()
        {
            LiteralControl litChart;
            string strScript = "";
            string strFilter = "";
            string strData = "";
            string strSQL, strSQL1;
            string strDate1, strDate2, strCategories;
            int intTotal, intHeight;
            int i, j;
            DataTable myData;

            try
            {
                strFilter = "";
                if (txtStartDate.Text.Trim() != "")
                {
                    // 有起始日期
                    if (txtEndDate.Text.Trim() != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                        strDate2 = "";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (txtEndDate.Text.Trim() != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                        strDate1 = "";
                        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                for (j = 0; j < lstQuestion.Count(); j++)
                {
                    if (strFilter == "")
                    {
                        // 沒有日期條件
                        strSQL1 = "Select Sum(Case When D.SelectValue='0' Then 0 Else 1 End) as PaperCount "
                                + "From vw_Paper_Relation M Join HumanData D On M.PaperID = D.PaperID "
                                + "		Join vw_Human_Question Q On D.QuestionID=Q.QuestionID "
                                + "Where M.SurveyID = @SID And M.ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4)) "
                                + "      And D.QuestionID=@QID ";
                        strSQL = "Select Q.QuestionID, Q.QTitle, Count(Distinct D.PaperID) as PaperCount "
                                + "From vw_Paper_Relation M Join HumanData D On M.PaperID = D.PaperID "
                                + "		Right Join vw_Human_Question Q On D.QuestionID=Q.QuestionID "
                                + "Where M.SurveyID = @SID And M.ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4)) "
                                + "      And Q.ParentID=@QID And D.SelectValue<>'0' "
                                + "Group By Q.QuestionID, Q.QTitle "
                                + "Order By Count(Distinct D.PaperID) Desc";
                    }
                    else
                    {
                        // 有日期條件
                        strSQL1 = "Select Sum(Case When D.SelectValue='0' Then 0 Else 1 End) as PaperCount "
                                + "From vw_Paper_Relation M Join HumanData D On M.PaperID = D.PaperID "
                                + "		Join vw_Human_Question Q On D.QuestionID=Q.QuestionID "
                                + "Where M.SurveyID = @SID And M.ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4) And " + strFilter + ") "
                                + "      And D.QuestionID=@QID And " + strFilter;

                        strSQL = "Select D.QuestionID, D.QTitle, Count(Distinct D.PaperID) as PaperCount "
                                + "From vw_Paper_Relation M Join HumanData D On M.PaperID = D.PaperID "
                                + "		Join vw_Human_Question Q On D.QuestionID=Q.QuestionID "
                                + "Where M.SurveyID = @SID And M.ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4) And " + strFilter + ") "
                                + "      And Q.ParentID=@QID And D.SelectValue<>'0' And " + strFilter
                                + "Group By D.QuestionID, D.QTitle "
                                + "Order By Count(Distinct D.PaperID) Desc";
                    }       // if (strFilter == "")

                    // 計算總數
                    DB.AddSqlParameter("@SID", _SurveyID);
                    if (strDate1 != "")
                    {
                        DB.AddSqlParameter("@Date1", strDate1);
                    }
                    if (strDate2 != "")
                    {
                        DB.AddSqlParameter("@Date2", strDate2);
                    }
                    DB.AddSqlParameter("@QID", lstQuestion[j]);
                    strData = DB.GetData(strSQL1);
                    intTotal = Convert.ToInt32(strData);

                    // 取得各小類的統計
                    DB.AddSqlParameter("@SID", _SurveyID);
                    if (strDate1 != "")
                    {
                        DB.AddSqlParameter("@Date1", strDate1);
                    }
                    if (strDate2 != "")
                    {
                        DB.AddSqlParameter("@Date2", strDate2);
                    }
                    DB.AddSqlParameter("@QID", lstQuestion[j]);
                    myData = DB.GetDataTable(strSQL);

                    if (myData != null)
                    {
                        strCategories = "";
                        strData = "";

                        for (i = 0; i < myData.Rows.Count; i++)
                        {
                            strCategories += "'" + myData.Rows[i]["QTitle"].ToString() + "',";
                            strData += (Math.Round(Convert.ToDouble(myData.Rows[i]["PaperCount"].ToString()) / intTotal, 4) * 100).ToString() + ",";
                        }
                        strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                        strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                        // 設定圖表高度
                        intHeight = 100 + 50 * myData.Rows.Count;

                        strScript = "<div id=\"myChart_Bar_" + lstQuestion[j].ToString() + "\" style=\"width: 100 %; height: " + intHeight.ToString() + "px; margin: 10px; border: solid 1px;\"></div>" + Environment.NewLine;
                        strScript += "<script type=\"text/javascript\">" + Environment.NewLine;
                        strScript += "$(function () {" + Environment.NewLine;
                        strScript += "    $('#myChart_Bar_" + lstQuestion[j].ToString() + "').highcharts({" + Environment.NewLine;
                        strScript += "        chart: {" + Environment.NewLine;
                        strScript += "            type: 'bar'" + Environment.NewLine;
                        strScript += "        }," + Environment.NewLine;
                        strScript += "        title: {" + Environment.NewLine;
                        strScript += "            text: '" + lstQTitle[j] + "'" + Environment.NewLine;          // 圖表標題
                        strScript += "        }," + Environment.NewLine;
                        strScript += "        xAxis: {" + Environment.NewLine;
                        strScript += "            categories: " + strCategories + "," + Environment.NewLine;
                        strScript += "            tickmarkPlacement: 'on'," + Environment.NewLine;
                        strScript += "            lineWidth: 0," + Environment.NewLine;
                        strScript += "            labels: {" + Environment.NewLine;
                        strScript += "                style: {'fontSize':'12pt'}" + Environment.NewLine;
                        strScript += "            }" + Environment.NewLine;
                        strScript += "        }," + Environment.NewLine;
                        strScript += "        yAxis: {" + Environment.NewLine;
                        strScript += "            lineWidth: 0," + Environment.NewLine;
                        strScript += "            min: 0," + Environment.NewLine;     // 最小值
                        strScript += "            max: 100," + Environment.NewLine;    // 最大值
                        strScript += "            title: {" + Environment.NewLine;
                        strScript += "                text: ''" + Environment.NewLine;
                        strScript += "            }," + Environment.NewLine;
                        strScript += "        }," + Environment.NewLine;
                        strScript += "        legend: {" + Environment.NewLine;         // 圖例設定
                        strScript += "            enabled: false" + Environment.NewLine;
                        strScript += "        }," + Environment.NewLine;
                        strScript += "        plotOptions: {" + Environment.NewLine;
                        strScript += "            series: {" + Environment.NewLine;
                        strScript += "                dataLabels: {" + Environment.NewLine;
                        strScript += "                    enabled: true," + Environment.NewLine;
                        strScript += "                    format: '{y} %'," + Environment.NewLine;
                        strScript += "                    align: 'right'," + Environment.NewLine;
                        strScript += "                    color: '#FFFFFF'," + Environment.NewLine;
                        strScript += "                    x: -10" + Environment.NewLine;
                        strScript += "                }," + Environment.NewLine;
                        strScript += "                pointPadding: 0.1," + Environment.NewLine;
                        strScript += "                groupPadding: 0" + Environment.NewLine;
                        strScript += "            }" + Environment.NewLine;
                        strScript += "        }," + Environment.NewLine;
                        strScript += "        exporting: {" + Environment.NewLine;
                        strScript += "            enabled: true" + Environment.NewLine;
                        strScript += "        }," + Environment.NewLine;
                        strScript += "        series: [{" + Environment.NewLine;        // 數值設定
                        strScript += "            name: '百分比'," + Environment.NewLine;
                        strScript += "            data: " + strData + "," + Environment.NewLine;
                        strScript += "        }]" + Environment.NewLine;
                        strScript += "    });" + Environment.NewLine;
                        strScript += "});" + Environment.NewLine;
                        strScript += "</script>" + Environment.NewLine;

                        litChart = new LiteralControl(strScript);
                        phdChart.Controls.Add(litChart);
                    }   // if (myData != null)
                    else
                    {
                        if (DB.DBErrorMessage != "")
                        {
                            Message.MsgShow(this.Page, "系統發生錯誤。");
                            ShareFunction.PutLog("Generate_Chart", DB.DBErrorMessage);
                        }
                        else
                        {
                            //Message.MsgShow(this.Page, "沒有符合條件的資料。");
                        }
                    }   // if (myData != null) else
                }   // for j
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Generate_Chart", ex.Message);
            }
            finally
            {
            }
        }
    }
}