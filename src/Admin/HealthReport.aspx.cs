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
    public partial class HealthReport : System.Web.UI.Page
    {
        int _SurveyID = 4;
        DBClass DB;
        int intTotal = 0;
        string strDateFilter, strDate1, strDate2;

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
            Data_Prepare();
            Chart1();
            Total_Ratio();
            Chart2();
            Chart3();
            pnlReport.Visible = true;
            DB = null;
        }

        /// <summary>
        /// 計算選填此構面的總問卷數
        /// </summary>
        private void Data_Prepare()
        {
            string strSQL = "";
            string strData = "";

            try
            {
                // 日期條件設定
                strDateFilter = "";
                if (txtStartDate.Text.Trim() != "")
                {
                    // 有起始日期
                    if (txtEndDate.Text.Trim() != "")
                    {
                        // 有起訖日期
                        strDateFilter = "CreateTime Between @Date1 And @Date2 ";
                        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        // 沒有截止日期
                        strDateFilter = "CreateTime>=@Date1 ";
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
                        strDateFilter = "CreateTime<@Date2 ";
                        strDate1 = "";
                        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strDateFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                // 本項調查總問卷數
                if (strDateFilter == "")
                {
                    strSQL = "Select Count(Distinct PaperID) From Paper Where SurveyID=@SID And FinishTime Is Not Null And Invalid='N' ";
                }
                else
                {
                    strSQL = "Select Count(Distinct PaperID) From Paper Where SurveyID=@SID And FinishTime Is Not Null And Invalid='N' And " + strDateFilter;
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
                strData = DB.GetData(strSQL);
                if (DB.DBErrorMessage != "")
                {
                    ShareFunction.PutLog("Data_Prepare", DB.DBErrorMessage);
                    intTotal = 0;
                }
                else
                {
                    intTotal = Convert.ToInt32(strData);
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Data_Prepare", ex.Message);
                intTotal = 0;
            }
        }

        /// <summary>
        /// 產出確診病症的統計
        /// </summary>
        private void Chart1()
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strSQL = "";
            int i, intHeight;
            DataTable myData;
            int _DimensionID = 4;       // 確診傷病的構面

            try
            {
                if (strDateFilter == "")
                {
                    // 沒有日期條件
                    strSQL = "Select QTitle, PaperCount "
                            + "From "
                            + "    (Select D.QTitle, Count(D.PaperID) as PaperCount, RANK() Over (Order By Count(D.PaperID) Desc) as RowRank "
                            + "     From Paper M Join PaperDetail D On M.PaperID = D.PaperID "
                            + "     Where M.SurveyID = @SID And M.FinishTime Is Not Null And M.Invalid = 'N' And D.SelectValue='1' And D.QuestionID In "
                            + "           (Select QuestionID From QuestionDimension Where DimensionID=@DID) "
                            + "     Group By D.QTitle) A "
                            + "Where RowRank<=10"
                            + "Order By PaperCount Desc";
                }
                else
                {
                    // 有日期條件
                    strSQL = "Select QTitle, PaperCount "
                            + "From "
                            + "    (Select D.QTitle, Count(D.PaperID) as PaperCount, RANK() Over (Order By Count(D.PaperID) Desc) as RowRank "
                            + "     From Paper M Join PaperDetail D On M.PaperID = D.PaperID "
                            + "     Where M.SurveyID = @SID And M.FinishTime Is Not Null And M.Invalid = 'N' And D.SelectValue='1' And D.QuestionID In "
                            + "           (Select QuestionID From QuestionDimension Where DimensionID=@DID) And " + strDateFilter + " "
                            + "     Group By D.QTitle) A "
                            + "Where RowRank<=10"
                            + "Order By PaperCount Desc";
                }
                DB.AddSqlParameter("@SID", 2);
                DB.AddSqlParameter("@DID", _DimensionID);
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
                    strCategories = "";
                    strData = "";

                    for (i = 0; i < myData.Rows.Count; i++)
                    {
                        strCategories += "'" + myData.Rows[i]["QTitle"].ToString().Replace('　', ' ').Trim() + "',";
                        strData += (Math.Round(Convert.ToDouble(myData.Rows[i]["PaperCount"].ToString()) / intTotal, 4) * 100).ToString() + ",";
                    }
                    strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                    strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                    intHeight = 75 + myData.Rows.Count * 50;

                    strScript += "$(function () {" + Environment.NewLine;
                    strScript += "    $('#myChart_Bar_1').highcharts({" + Environment.NewLine;
                    strScript += "        chart: {" + Environment.NewLine;
                    strScript += "            type: 'bar'," + Environment.NewLine;
                    strScript += "            height: " + intHeight.ToString() + Environment.NewLine; 
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        title: {" + Environment.NewLine;
                    strScript += "            text: '所有人員確診傷病統計'" + Environment.NewLine;          // 圖表標題
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        subtitle: {" + Environment.NewLine;
                    strScript += "            text: '前十大確診傷病'" + Environment.NewLine;          // 圖表副標題
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

                    ClientScript.RegisterStartupScript(this.Page.GetType(), "chart1_script", strScript, true);
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "系統發生錯誤。");
                        ShareFunction.PutLog("Chart1", DB.DBErrorMessage);
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "沒有符合條件的資料。");
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Chart1", ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 計算人數與比率
        /// </summary>
        private void Total_Ratio()
        {
            string strSQL = "";
            //string strDate1, strDate2, strFilter;
            int intTotal, intBase;

            try
            {
                // 求工作適能指數為普通與弱的總數
                strSQL = "Select Count(*) From vw_WIndex_Paper_List Where ResultCode In (3,4) ";
                if (strDateFilter != "")
                {
                    strSQL += "And " + strDateFilter;

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

                if (strDateFilter == "")
                {
                    strSQL = "Select Count(*) From vw_Paper_Relation Where SurveyID=@SID "
                            + "And ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4))";
                }
                else
                {
                    strSQL = "Select Count(*) From vw_Paper_Relation Where SurveyID=@SID "
                            + "And ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4) And " + strDateFilter + ") "
                            + "And " + strDateFilter;
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
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Total_Ratio", ex.Message);
            }
        }

        /// <summary>
        /// 產出經常不舒服的項目統計
        /// </summary>
        private void Chart2()
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strSQL = "";
            int i, intHeight;
            DataTable myData;
            int _QuestionID = 243;

            try
            {
                if (strDateFilter == "")
                {
                    // 沒有日期條件
                    strSQL = "Select D.QTitle, Count(D.PaperID) as PaperCount "
                            + "From Paper M Join PaperDetail D On M.PaperID = D.PaperID "
                            + "Where M.SurveyID = @SID And M.FinishTime Is Not Null And M.Invalid = 'N' And D.LineID=1 And D.SelectValue='4' And D.QuestionID In "
                            + "      (Select QuestionID From Question Where SurveyID=@SID And ParentID=@QID) "
                            + "Group By D.QTitle "
                            + "Order By PaperCount Desc";
                }
                else
                {
                    // 有日期條件
                    strSQL = "Select D.QTitle, Count(D.PaperID) as PaperCount "
                            + "From Paper M Join PaperDetail D On M.PaperID = D.PaperID "
                            + "Where M.SurveyID = @SID And M.FinishTime Is Not Null And M.Invalid = 'N' And D.LineID=1 And D.SelectValue='4' And D.QuestionID In "
                            + "      (Select QuestionID From Question Where SurveyID=@SID And ParentID=@QID) And " + strDateFilter + " "
                            + "Group By D.QTitle "
                            + "Order By PaperCount Desc";
                }
                DB.AddSqlParameter("@SID", _SurveyID);
                DB.AddSqlParameter("@QID", _QuestionID);
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
                    strCategories = "";
                    strData = "";

                    for (i = 0; i < myData.Rows.Count; i++)
                    {
                        strCategories += "'" + myData.Rows[i]["QTitle"].ToString().Replace('　', ' ').Trim() + "',";
                        strData += (Math.Round(Convert.ToDouble(myData.Rows[i]["PaperCount"].ToString()) / intTotal, 4) * 100).ToString() + ",";
                    }
                    strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                    strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                    intHeight = 75 + myData.Rows.Count * 50;

                    strScript += "$(function () {" + Environment.NewLine;
                    strScript += "    $('#myChart_Bar_2').highcharts({" + Environment.NewLine;
                    strScript += "        chart: {" + Environment.NewLine;
                    strScript += "            type: 'bar'," + Environment.NewLine;
                    strScript += "            height: " + intHeight.ToString() + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        title: {" + Environment.NewLine;
                    strScript += "            text: '經常不舒服情形統計'" + Environment.NewLine;          // 圖表標題
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        subtitle: {" + Environment.NewLine;
                    strScript += "            text: '填寫人數：" + intTotal.ToString("n0") + "'" + Environment.NewLine;          // 圖表副標題
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

                    ClientScript.RegisterStartupScript(this.Page.GetType(), "chart2_script", strScript, true);
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "系統發生錯誤。");
                        ShareFunction.PutLog("Chart2", DB.DBErrorMessage);
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "沒有符合條件的資料。");
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Chart2", ex.Message);
            }
        }

        /// <summary>
        /// 產生與工作有關的不舒服情形統計
        /// </summary>
        private void Chart3()
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strSQL = "";
            int i, intHeight;
            DataTable myData;
            int _QuestionID = 243;

            try
            {
                if (strDateFilter == "")
                {
                    // 沒有日期條件
                    strSQL = "Select D1.QTitle, Count(D1.PaperID) as PaperCount "
                            + "From Paper M Join PaperDetail D1 On M.PaperID = D1.PaperID "
                            + "             Join PaperDetail D2 On D1.PaperID=D2.PaperID And D1.QuestionID = D2.QuestionID "
                            + "Where M.SurveyID = @SID And M.FinishTime Is Not Null And M.Invalid = 'N' And D1.LineID=1 And D1.SelectValue In ('3','4') And "
                            + "      D2.LineID=2 And D2.SelectValue='2' And "
                            + "      D1.QuestionID In (Select QuestionID From Question Where SurveyID=@SID And ParentID=@QID) "
                            + "Group By D1.QTitle "
                            + "Order By Count(D1.PaperID) Desc";
                }
                else
                {
                    // 有日期條件
                    strSQL = "Select D1.QTitle, Count(D1.PaperID) as PaperCount "
                            + "From Paper M Join PaperDetail D1 On M.PaperID = D1.PaperID "
                            + "             Join PaperDetail D2 On D1.PaperID=D2.PaperID And D1.QuestionID = D2.QuestionID "
                            + "Where M.SurveyID = @SID And M.FinishTime Is Not Null And M.Invalid = 'N' And D1.LineID=1 And D1.SelectValue In ('3','4') And "
                            + "      D2.LineID=2 And D2.SelectValue='2' And "
                            + "      D1.QuestionID In (Select QuestionID From Question Where SurveyID=@SID And ParentID=@QID) And " + strDateFilter + " "
                            + "Group By D1.QTitle "
                            + "Order By Count(D1.PaperID) Desc";
                }
                DB.AddSqlParameter("@SID", _SurveyID);
                DB.AddSqlParameter("@QID", _QuestionID);
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
                    strCategories = "";
                    strData = "";

                    for (i = 0; i < myData.Rows.Count; i++)
                    {
                        strCategories += "'" + myData.Rows[i]["QTitle"].ToString().Replace('　', ' ').Trim() + "',";
                        strData += (Math.Round(Convert.ToDouble(myData.Rows[i]["PaperCount"].ToString()) / intTotal, 4) * 100).ToString() + ",";
                    }
                    strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                    strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                    intHeight = 75 + myData.Rows.Count * 50;

                    strScript += "$(function () {" + Environment.NewLine;
                    strScript += "    $('#myChart_Bar_3').highcharts({" + Environment.NewLine;
                    strScript += "        chart: {" + Environment.NewLine;
                    strScript += "            type: 'bar'," + Environment.NewLine;
                    strScript += "            height: " + intHeight.ToString() + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        title: {" + Environment.NewLine;
                    strScript += "            text: '與工作有關的不舒服情形統計'" + Environment.NewLine;          // 圖表標題
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        subtitle: {" + Environment.NewLine;
                    strScript += "            text: '填寫人數：" + intTotal.ToString("n0") + "'" + Environment.NewLine;          // 圖表副標題
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

                    ClientScript.RegisterStartupScript(this.Page.GetType(), "chart3_script", strScript, true);
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "系統發生錯誤。");
                        ShareFunction.PutLog("Chart3", DB.DBErrorMessage);
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "沒有符合條件的資料。");
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Chart3", ex.Message);
            }
        }
    }
}