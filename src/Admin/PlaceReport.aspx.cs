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
    public partial class PlaceReport : System.Web.UI.Page
    {
        int _SurveyID = 5;
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
            Total_Ratio();
            Chart1();
            Chart2();
            OtherItem();
            Chart3();
            Chart4();
            DB = null;
        }

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
                if (strDateFilter=="")
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
        /// 計算人數與比率
        /// </summary>
        private void Total_Ratio()
        {
            string strSQL = "";
            //string strDate1, strDate2, strFilter;
            int intTotal, intBase;

            try
            {
                //if (txtStartDate.Text.Trim() != "")
                //{
                //    // 有起始日期
                //    if (txtEndDate.Text.Trim() != "")
                //    {
                //        // 有起訖日期
                //        strFilter = "CreateTime Between @Date1 And @Date2 ";
                //        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                //        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                //    }
                //    else
                //    {
                //        // 沒有截止日期
                //        strFilter = "CreateTime>=@Date1 ";
                //        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                //        strDate2 = "";
                //    }
                //}
                //else
                //{
                //    // 沒有起始日期
                //    if (txtEndDate.Text.Trim() != "")
                //    {
                //        // 有截止日期
                //        strFilter = "CreateTime<@Date2 ";
                //        strDate1 = "";
                //        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                //    }
                //    else
                //    {
                //        // 沒有輸入日期條件
                //        strFilter = "";
                //        strDate1 = "";
                //        strDate2 = "";
                //    }
                //}

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
                pnlTotalRatio.Visible = true;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Total_Ratio", ex.Message);
            }
        }

        /// <summary>
        /// 危險工作環境百分比(全體)
        /// </summary>
        private void Chart1()
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strSQL = "";
            //string strDate1, strDate2, strFilter;
            //int intTotal = 0;
            int i;
            DataTable myData;
            int _QuestionID = 186;       // 寫死在程式碼中，若Question題目有變，則須修改這裡

            try
            {
                //strFilter = "";
                //if (txtStartDate.Text.Trim() != "")
                //{
                //    // 有起始日期
                //    if (txtEndDate.Text.Trim() != "")
                //    {
                //        // 有起訖日期
                //        strFilter = "CreateTime Between @Date1 And @Date2 ";
                //        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                //        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                //    }
                //    else
                //    {
                //        // 沒有截止日期
                //        strFilter = "CreateTime>=@Date1 ";
                //        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                //        strDate2 = "";
                //    }
                //}
                //else
                //{
                //    // 沒有起始日期
                //    if (txtEndDate.Text.Trim() != "")
                //    {
                //        // 有截止日期
                //        strFilter = "CreateTime<@Date2 ";
                //        strDate1 = "";
                //        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                //    }
                //    else
                //    {
                //        // 沒有輸入日期條件
                //        strFilter = "";
                //        strDate1 = "";
                //        strDate2 = "";
                //    }
                //}

                if (strDateFilter == "")
                {
                    // 沒有日期條件
                    strSQL = "Select D.SelectValue, D.SelectOption, Count(D.PaperID) as PaperCount "
                            + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                            + "Where M.SurveyID = @SID And M.FinishTime Is Not Null And M.Invalid = 'N' And D.QuestionID=@QID "
                            + "Group By D.SelectValue, D.SelectOption "
                            + "Order By Count(D.PaperID) Desc";
                    // 以下為僅統計工作適能等級為普通與弱的資料
                    //strSQL = "Select D.SelectValue, D.SelectOption, Count(D.PaperID) as PaperCount "
                    //        + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                    //        + "Where M.SurveyID = @SID And M.ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4)) "
                    //        + "      And M.FinishTime Is Not Null And M.Invalid = 'N' And D.QuestionID=@QID "
                    //        + "Group By D.SelectValue, D.SelectOption "
                    //        + "Order By Count(D.PaperID) Desc";
                }
                else
                {
                    // 有日期條件
                    strSQL = "Select D.SelectValue, D.SelectOption, Count(D.PaperID) as PaperCount "
                            + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                            + "Where M.SurveyID = @SID And M.FinishTime Is Not Null And M.Invalid = 'N' And D.QuestionID=@QID And " + strDateFilter + " "
                            + "Group By D.SelectValue, D.SelectOption "
                            + "Order By Count(D.PaperID) Desc";
                    // 以下為僅統計工作適能等級為普通與弱的資料
                    //strSQL = "Select D.SelectValue, D.SelectOption, Count(D.PaperID) as PaperCount "
                    //        + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                    //        + "Where M.SurveyID = @SID And M.ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4)) "
                    //        + "      And M.FinishTime Is Not Null And M.Invalid = 'N' And D.QuestionID=@QID And " + strDateFilter
                    //        + "Group By D.SelectValue, D.SelectOption "
                    //        + "Order By Count(D.PaperID) Desc";
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
                    // 計算總數
                    //intTotal = Convert.ToInt32(ViewState["BaseCount"]);

                    strCategories = "";
                    strData = "";

                    for (i = 0; i < myData.Rows.Count; i++)
                    {
                        strCategories += "'" + myData.Rows[i]["SelectOption"].ToString() + "',";
                        strData += (Math.Round(Convert.ToDouble(myData.Rows[i]["PaperCount"].ToString()) / intTotal, 4) * 100).ToString() + ",";
                    }
                    strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                    strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                    strScript += "$(function () {" + Environment.NewLine;
                    strScript += "    $('#myChart_Bar_1').highcharts({" + Environment.NewLine;
                    strScript += "        chart: {" + Environment.NewLine;
                    strScript += "            type: 'bar'" + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        title: {" + Environment.NewLine;
                    strScript += "            text: '工作環境可能遭遇的危險統計'" + Environment.NewLine;          // 圖表標題
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        subtitle: {" + Environment.NewLine;
                    strScript += "            text: '全體'" + Environment.NewLine;          // 圖表副標題
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
        /// 危險工作環境百分比(僅單選部分)
        /// </summary>
        private void Chart2()
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strSQL="";
            //string strDate1, strDate2, strFilter;
            //int intTotal = 0;
            int i, intHeight;
            DataTable myData;
            int _QuestionID = 186;       // 寫死在程式碼中，若Question題目有變，則須修改這裡

            try
            {
                //strFilter = "";
                //if (txtStartDate.Text.Trim() != "")
                //{
                //    // 有起始日期
                //    if (txtEndDate.Text.Trim() != "")
                //    {
                //        // 有起訖日期
                //        strFilter = "CreateTime Between @Date1 And @Date2 ";
                //        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                //        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                //    }
                //    else
                //    {
                //        // 沒有截止日期
                //        strFilter = "CreateTime>=@Date1 ";
                //        strDate1 = Convert.ToDateTime(txtStartDate.Text.Trim()).ToString("yyyy/MM/dd");
                //        strDate2 = "";
                //    }
                //}
                //else
                //{
                //    // 沒有起始日期
                //    if (txtEndDate.Text.Trim() != "")
                //    {
                //        // 有截止日期
                //        strFilter = "CreateTime<@Date2 ";
                //        strDate1 = "";
                //        strDate2 = Convert.ToDateTime(txtEndDate.Text.Trim()).AddDays(1).ToString("yyyy/MM/dd");
                //    }
                //    else
                //    {
                //        // 沒有輸入日期條件
                //        strFilter = "";
                //        strDate1 = "";
                //        strDate2 = "";
                //    }
                //}

                if (strDateFilter == "")
                {
                    // 沒有日期條件
                    strSQL = "Select D.SelectValue, D.SelectOption, Count(D.PaperID) as PaperCount "
                            + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                            + "             Join (Select PaperID From PlaceData Where QuestionID=@QID Group By PaperID Having COUNT(UniqueID)=1) A On D.PaperID=A.PaperID "
                            + "Where M.SurveyID = @SID And M.FinishTime Is Not Null And M.Invalid = 'N' "
                            + "       And D.QuestionID=@QID "
                            + "Group By D.SelectValue, D.SelectOption "
                            + "Order By Count(D.PaperID) Desc";

                    // 以下為僅統計工作適能等級為普通與弱的資料
                    //strSQL = "Select D.SelectValue, D.SelectOption, Count(D.PaperID) as PaperCount "
                    //        + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                    //        + "             Join (Select PaperID From PlaceData Where QuestionID=@QID Group By PaperID Having COUNT(UniqueID)=1) A On D.PaperID=A.PaperID "
                    //        + "Where M.SurveyID = @SID And M.ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4)) "
                    //        + "      And M.FinishTime Is Not Null And M.Invalid = 'N' And D.QuestionID=@QID "
                    //        + "Group By D.SelectValue, D.SelectOption "
                    //        + "Order By Count(D.PaperID) Desc";
                }
                else
                {
                    // 有日期條件
                    strSQL = "Select D.SelectValue, D.SelectOption, Count(D.PaperID) as PaperCount "
                            + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                            + "             Join (Select PaperID From PlaceData Where QuestionID=@QID Group By PaperID Having COUNT(UniqueID)=1) A On D.PaperID=A.PaperID "
                            + "Where M.SurveyID = @SID And M.FinishTime Is Not Null And M.Invalid = 'N' "
                            + "       And D.QuestionID=@QID And " + strDateFilter + " "
                            + "Group By D.SelectValue, D.SelectOption "
                            + "Order By Count(D.PaperID) Desc";
                    // 以下為僅統計工作適能等級為普通與弱的資料
                    //strSQL = "Select D.SelectValue, D.SelectOption, Count(D.PaperID) as PaperCount "
                    //        + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                    //        + "             Join (Select PaperID From PlaceData Where QuestionID=@QID Group By PaperID Having COUNT(UniqueID)=1) A On D.PaperID=A.PaperID "
                    //        + "Where M.SurveyID = @SID And M.ParentPaper In (Select PaperID From vw_WIndex_Paper_List Where ResultCode In(3, 4)) "
                    //        + "      And M.FinishTime Is Not Null And M.Invalid = 'N' And D.QuestionID=@QID And " + strDateFilter + " "
                    //        + "Group By D.SelectValue, D.SelectOption "
                    //        + "Order By Count(D.PaperID) Desc";
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
                    // 計算總數
                    //intTotal = Convert.ToInt32(DB.GetData(strSQL));

                    // 設定圖表高度
                    intHeight = 75 + myData.Rows.Count * 50;

                    strCategories = "";
                    strData = "";

                    for (i = 0; i < myData.Rows.Count; i++)
                    {
                        strCategories += "'" + myData.Rows[i]["SelectOption"].ToString() + "',";
                        strData += (Math.Round(Convert.ToDouble(myData.Rows[i]["PaperCount"].ToString()) / intTotal, 4) * 100).ToString() + ",";
                    }
                    strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                    strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                    strScript += "$(function () {" + Environment.NewLine;
                    strScript += "    $('#myChart_Bar_2').highcharts({" + Environment.NewLine;
                    strScript += "        chart: {" + Environment.NewLine;
                    strScript += "            type: 'bar'," + Environment.NewLine;
                    strScript += "            height: " + intHeight.ToString() + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        title: {" + Environment.NewLine;
                    strScript += "            text: '工作環境可能遭遇的危險統計'" + Environment.NewLine;          // 圖表標題
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        subtitle: {" + Environment.NewLine;
                    strScript += "            text: '單選'" + Environment.NewLine;          // 圖表副標題
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
                        //Message.MsgShow(this.Page, "沒有符合條件的資料。");
                    }
                }

            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Chart2", ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 危險工作環境其他項目
        /// </summary>
        private void OtherItem()
        {
            string strSQL = "";
            DataTable myData;
            int _QuestionID = 187;
            int _OptionID = 605;

            try
            {
                strSQL = "Select SelectOption, Count(PaperID) as PCount "
                        + "From PlaceData "
                        + "Where QuestionID=@QID And SelectValue=@OID "
                        + "Group By SelectOption "
                        + "Order By Count(PaperID)";
                DB.AddSqlParameter("@QID", _QuestionID);
                DB.AddSqlParameter("@OID", _OptionID);
                myData = DB.GetDataTable(strSQL);
                rptOther.DataSource = myData;
                rptOther.DataBind();
                if (myData == null)
                {
                    pnlEmpty.Visible = true;
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("OtherItem", DB.DBErrorMessage);
                    }
                }
                else
                {
                    pnlEmpty.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("OtherItem", ex.Message);
            }
        }

        /// <summary>
        /// 經常有的工作環境百分比
        /// </summary>
        private void Chart3()
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strSQL = "";
            //string strDate1, strDate2, strFilter;
            //int intTotal = 0;
            int i, intHeight;
            DataTable myData;
            int _QuestionID = 187;       // 寫死在程式碼中，若Question題目有變，則須修改這裡

            try
            {
                if (strDateFilter == "")
                {
                    // 沒有日期條件
                    strSQL = "Select D.QTitle, COUNT(D.PaperID) as PaperCount "
                            + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                            + "Where M.SurveyID = @SID And D.QuestionID In (Select QuestionID From Question Where ParentID=@QID) "
                            + "      And M.FinishTime Is Not Null And M.Invalid = 'N' And D.SelectValue='3' "
                            + "Group By D.QTitle "
                            + "Order By Count(D.PaperID) Desc";
                }
                else
                {
                    // 有日期條件
                    strSQL = "Select D.QTitle, COUNT(D.PaperID) as PaperCount "
                            + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                            + "Where M.SurveyID = @SID And D.QuestionID In (Select QuestionID From Question Where ParentID=@QID) "
                            + "      And M.FinishTime Is Not Null And M.Invalid = 'N' And D.SelectValue='3' And " + strDateFilter + " "
                            + "Group By D.QTitle "
                            + "Order By Count(D.PaperID) Desc";
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
                    // 計算總數
                    //intTotal = Convert.ToInt32(DB.GetData(strSQL));

                    // 設定圖表高度
                    intHeight = 75 + myData.Rows.Count * 50;

                    strCategories = "";
                    strData = "";

                    for (i = 0; i < myData.Rows.Count; i++)
                    {
                        strCategories += "'" + myData.Rows[i]["QTitle"].ToString() + "',";
                        strData += (Math.Round(Convert.ToDouble(myData.Rows[i]["PaperCount"].ToString()) / intTotal, 4) * 100).ToString() + ",";
                    }
                    strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                    strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                    strScript += "$(function () {" + Environment.NewLine;
                    strScript += "    $('#myChart_Bar_3').highcharts({" + Environment.NewLine;
                    strScript += "        chart: {" + Environment.NewLine;
                    strScript += "            type: 'bar'," + Environment.NewLine;
                    strScript += "            height: " + intHeight.ToString() + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        title: {" + Environment.NewLine;
                    strScript += "            text: '工作環境項目統計'" + Environment.NewLine;          // 圖表標題
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        subtitle: {" + Environment.NewLine;
                    strScript += "            text: '經常有'" + Environment.NewLine;          // 圖表副標題
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
                        //Message.MsgShow(this.Page, "沒有符合條件的資料。");
                    }
                }

            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Chart3", ex.Message);
            }
        }

        /// <summary>
        /// 經常有或偶爾有的工作環境百分比
        /// </summary>
        private void Chart4()
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strSQL = "";
            //string strDate1, strDate2, strFilter;
            //int intTotal = 0;
            int i, intHeight;
            DataTable myData;
            int _QuestionID = 187;       // 寫死在程式碼中，若Question題目有變，則須修改這裡

            try
            {
                if (strDateFilter == "")
                {
                    // 沒有日期條件
                    strSQL = "Select D.QTitle, COUNT(D.PaperID) as PaperCount "
                            + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                            + "Where M.SurveyID = @SID And D.QuestionID In (Select QuestionID From Question Where ParentID=@QID) "
                            + "      And M.FinishTime Is Not Null And M.Invalid = 'N' And D.SelectValue In ('2', '3') "
                            + "Group By D.QTitle "
                            + "Order By Count(D.PaperID) Desc";
                }
                else
                {
                    // 有日期條件
                    strSQL = "Select D.QTitle, COUNT(D.PaperID) as PaperCount "
                            + "From Paper M Join PlaceData D On M.PaperID = D.PaperID "
                            + "Where M.SurveyID = @SID And D.QuestionID In (Select QuestionID From Question Where ParentID=@QID) "
                            + "      And M.FinishTime Is Not Null And M.Invalid = 'N' And D.SelectValue In ('2','3') And " + strDateFilter + " "
                            + "Group By D.QTitle "
                            + "Order By Count(D.PaperID) Desc";
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
                    // 計算總數
                    //intTotal = Convert.ToInt32(DB.GetData(strSQL));

                    // 設定圖表高度
                    intHeight = 75 + myData.Rows.Count * 50;

                    strCategories = "";
                    strData = "";

                    for (i = 0; i < myData.Rows.Count; i++)
                    {
                        strCategories += "'" + myData.Rows[i]["QTitle"].ToString() + "',";
                        strData += (Math.Round(Convert.ToDouble(myData.Rows[i]["PaperCount"].ToString()) / intTotal, 4) * 100).ToString() + ",";
                    }
                    strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                    strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                    strScript += "$(function () {" + Environment.NewLine;
                    strScript += "    $('#myChart_Bar_4').highcharts({" + Environment.NewLine;
                    strScript += "        chart: {" + Environment.NewLine;
                    strScript += "            type: 'bar'," + Environment.NewLine;
                    strScript += "            height: " + intHeight.ToString() + Environment.NewLine;
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        title: {" + Environment.NewLine;
                    strScript += "            text: '工作環境項目統計'" + Environment.NewLine;          // 圖表標題
                    strScript += "        }," + Environment.NewLine;
                    strScript += "        subtitle: {" + Environment.NewLine;
                    strScript += "            text: '經常有與偶爾有'" + Environment.NewLine;          // 圖表副標題
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

                    ClientScript.RegisterStartupScript(this.Page.GetType(), "chart4_script", strScript, true);
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "系統發生錯誤。");
                        ShareFunction.PutLog("Chart4", DB.DBErrorMessage);
                    }
                    else
                    {
                        //Message.MsgShow(this.Page, "沒有符合條件的資料。");
                    }
                }

            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Chart4", ex.Message);
            }
        }
    }
}