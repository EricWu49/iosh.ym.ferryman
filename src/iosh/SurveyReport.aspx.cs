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
    public partial class SurveyReport : System.Web.UI.Page
    {
        int _ReportID = 0;
        long _PaperID = 0;
        int _SurveyID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] == null)
                {
                    Panel1.Controls.Add(new LiteralControl("<span>未知的報表</span>"));
                }
                else
                {
                    if (Request.QueryString["id"]!="")
                    {
                        _ReportID = Convert.ToInt32(Request.QueryString["id"]);
                        _PaperID = Convert.ToInt64(Session["PaperID"]);
                        _SurveyID = Convert.ToInt32(Session["SurveyID"]);
                        Main_Process();
                    }
                }
            }
        }

        private void Main_Process()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select ChartID, ChartTitle, ChartType, ChartWidth, ChartHeight From ReportChart Where ReportID=@ID Order By OrderNo";
                DB.AddSqlParameter("@ID", _ReportID);
                myData = DB.GetDataTable(strSQL);
                if (myData == null)
                {
                    Panel1.Controls.Add(new LiteralControl("<span>未定義的報表</span>"));
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Main_Process", DB.DBErrorMessage);
                    }
                }
                else
                {
                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        switch (Convert.ToInt32(myData.Rows[i]["ChartType"].ToString()))
                        {
                            case 1:         // 文字
                                Text_Report(DB, myData.Rows[i]);
                                break;
                            case 2:         // 分析
                                Result_Report(DB, myData.Rows[i]);
                                break;
                            case 11:        // 長條圖
                                Bar_Report(DB, myData.Rows[i]);
                                break;
                            case 14:        // 雷達圖
                                Radar_Report(DB, myData.Rows[i]);
                                break;
                            case 99:        // 網頁元素
                                Element_Report(DB, myData.Rows[i]);
                                break;
                        }
                        Panel1.Controls.Add(new LiteralControl("<br/>"));
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Main_Process", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        private void Text_Report(DBClass DB, DataRow myRow)
        {
            Label myLabel;
            DataTable dtChart;
            string strSQL = "";

            try
            {
                myLabel = new Label();
                myLabel.Text = myRow["ChartTitle"].ToString();
                Panel1.Controls.Add(myLabel);

                strSQL = "Select PropertyName, PropertyValue, ResultName From ChartDetail Where ChartID=@ID";
                DB.AddSqlParameter("@ID", Convert.ToInt32(myRow["ChartID"].ToString()));
                dtChart = DB.GetDataTable(strSQL);
                if (dtChart != null)
                {
                    myLabel = new Label();
                    for (int j = 0; j < dtChart.Rows.Count; j++)
                    {
                        switch (dtChart.Rows[j]["PropertyName"].ToString())
                        {
                            case "VALUE":
                                strSQL = dtChart.Rows[j]["PropertyValue"].ToString();
                                DB.AddSqlParameter("@PaperID", Convert.ToInt64(Session["PaperID"]));
                                myLabel.Text = DB.GetData(strSQL);
                                if (dtChart.Rows[j]["ResultName"].ToString() != "")
                                {
                                    if (myLabel.CssClass == "")
                                    {
                                        myLabel.CssClass = dtChart.Rows[j]["ResultName"].ToString().ToLower();
                                    }
                                    else
                                    {
                                        myLabel.CssClass += " " + dtChart.Rows[j]["ResultName"].ToString().ToLower();
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    Panel1.Controls.Add(myLabel);
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Text_Report(" + myRow["ChartID"].ToString() + ")", DB.DBErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Text_Report", ex.Message);
            }
        }

        private void Result_Report(DBClass DB, DataRow myRow)
        {
            Label myLabel;
            DataTable dtChart, dtClassify, myData;
            string strSQL = "";
            Single numPoint = 0;
            List<ClassifyData> lstClassify;
            ClassifyData objClassify;
            int i, j;
            string strData = "";
            int intCode = 0;
            string strConnector = "";

            try
            {
                myLabel = new Label();
                myLabel.Text = myRow["ChartTitle"].ToString();
                Panel1.Controls.Add(myLabel);

                strSQL = "Select PropertyName, PropertyValue, ResultName From ChartDetail Where ChartID=@ID";
                DB.AddSqlParameter("@ID", Convert.ToInt32(myRow["ChartID"].ToString()));
                dtChart = DB.GetDataTable(strSQL);
                if (dtChart != null)
                {
                    myLabel = new Label();
                    for (j = 0; j < dtChart.Rows.Count; j++)
                    {
                        switch (dtChart.Rows[j]["PropertyName"].ToString())
                        {
                            case "VALUE":
                                // 計算要分析的得分
                                strSQL = dtChart.Rows[j]["PropertyValue"].ToString();
                                DB.AddSqlParameter("@PaperID", Convert.ToInt64(Session["PaperID"]));
                                numPoint = Convert.ToSingle(DB.GetData(strSQL));

                                // 取出要分析的構面
                                strSQL = "Select ClassifyID, Formula, DataType From Classify Where SurveyID=@ID";
                                DB.AddSqlParameter("@ID", _SurveyID);
                                dtClassify = DB.GetDataTable(strSQL);
                                if (dtClassify != null)
                                {
                                    // 保存分析構面資料公式
                                    lstClassify = new List<ClassifyData>();
                                    for (i = 0; i < dtClassify.Rows.Count; i++)
                                    {
                                        objClassify = new ClassifyData();
                                        objClassify.ClassifyID = Convert.ToInt32(dtClassify.Rows[i]["ClassifyID"]);
                                        objClassify.Formula = dtClassify.Rows[i]["Formula"].ToString();
                                        objClassify.DataType = dtClassify.Rows[i]["DataType"].ToString();

                                        // 取得分析值
                                        strSQL = dtClassify.Rows[i]["Formula"].ToString();
                                        DB.AddSqlParameter("@PaperID", _PaperID);
                                        strData = DB.GetData(strSQL);
                                        if (dtClassify.Rows[i]["DataType"].ToString() == "R")
                                        {
                                            strSQL = "Select ClassID From ClassifyDetail Where ClassifyID=@ID And @Value Between MinValue And MaxValue";
                                            DB.AddSqlParameter("@ID", Convert.ToInt32(dtClassify.Rows[i]["ClassifyID"]));
                                            DB.AddSqlParameter("@Value", Convert.ToSingle(strData));
                                            strData = DB.GetData(strSQL);
                                            objClassify.ClassifyValue = strData;
                                        }
                                        else
                                        {
                                            objClassify.ClassifyValue = strData;
                                        }

                                        // 儲存
                                        lstClassify.Add(objClassify);
                                    }

                                    strSQL = "select GroupID, COUNT(*) "
                                            + "From ResultCondition "
                                            + "Where SurveyID=" + _SurveyID.ToString() + " ";
                                        
                                    strConnector = "And ";
                                    for (i=0; i<lstClassify.Count; i++)
                                    {
                                        strSQL += strConnector + "(ClassifyID=" + lstClassify[i].ClassifyID.ToString() + " And ResultValue=N'" + lstClassify[i].ClassifyValue + "') ";
                                        strConnector = "Or ";
                                    }
                                    strSQL += "Group By GroupID "
                                             + "Having Count(*)=" + lstClassify.Count.ToString();
                                    myData = DB.GetDataTable(strSQL);
                                    strData = myData.Rows[0]["GroupID"].ToString();

                                    // 增加ResultCode，以便將結果寫入Paper使用    2016/11/20
                                    strSQL = "Select ResultValue, ResultCode From ResultRule " +
                                             "Where SurveyID=@SID And GroupID=@GID And @Value Between MinValue And MaxValue ";
                                    DB.AddSqlParameter("@SID", _SurveyID);
                                    DB.AddSqlParameter("@GID", Convert.ToInt32(strData));
                                    DB.AddSqlParameter("@Value", numPoint);
                                    myData = DB.GetDataTable(strSQL);
                                    if (myData!=null)
                                    {
                                        strData = myData.Rows[0]["ResultValue"].ToString();
                                        intCode = Convert.ToInt32(myData.Rows[0]["ResultCode"].ToString());

                                        strSQL = "Update Paper "
                                               + "Set ResultCode=@Code, "
                                               + "    ResultValue=@Value "
                                               + "Where PaperID=@ID";
                                        DB.AddSqlParameter("@Code", intCode);
                                        DB.AddSqlParameter("@Value", strData);
                                        DB.AddSqlParameter("@ID", _PaperID);
                                        if (DB.RunSQL(strSQL)<1)
                                        {
                                            ShareFunction.PutLog("Result_Report", DB.DBErrorMessage);
                                        }
                                    }
                                    else
                                    {
                                        ShareFunction.PutLog("Result_Report", DB.DBErrorMessage);
                                        strData = "";
                                    }
                                    myLabel.Text = strData;
                                    if (dtChart.Rows[j]["ResultName"].ToString() != "")
                                    {
                                        if (myLabel.CssClass == "")
                                        {
                                            myLabel.CssClass = dtChart.Rows[j]["ResultName"].ToString().ToLower();
                                        }
                                        else
                                        {
                                            myLabel.CssClass += " " + dtChart.Rows[j]["ResultName"].ToString().ToLower();
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    Panel1.Controls.Add(myLabel);

                    // 先寫死，未來再設計
                    if (strData=="弱" || strData == "普通")
                    {
                        Literal litHint = new Literal();
                        litHint.Text = "<div class='alert alert-warning' role='alert'>您的工作適能等級評定結果偏低，建議您可以點選「個人構面分析」做進一步的診斷。</div>";
                        Panel1.Controls.Add(litHint);
                    }
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Result_Report(" + myRow["ChartID"].ToString() + ")", DB.DBErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Result_Report", ex.Message);
            }
        }

        private void Radar_Report(DBClass DB, DataRow myRow)
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strChartID = "";

            strChartID = myRow["ChartID"].ToString();
            //litChart.Text = "<canvas id=\"myChart\" width=\"400\" height=\"400\"></canvas>";
            litChart.Text = "<div id=\"myChart_" + strChartID + "\" style=\"min-width: 400px; max-width: 600px; height: 300px; margin: 0 auto\"></div>";
            Panel1.Controls.Add(litChart);

            Radar_Data(DB, Convert.ToInt32(strChartID), out strCategories, out strData);

            //strScript += "<script type=\"text/javascript\">";
            //strScript += "var ctx = $(\"#myChart\");";
            //strScript += "var data=" + Radar_Data(DB) + ";";
            //strScript += "var myRadarChart = new Chart(ctx, {";
            //strScript += "    type: 'radar',";
            //strScript += "    data: data,";
            //strScript += "    options: {";
            //strScript += "            scale: {";
            //strScript += "                reverse: true,";
            //strScript += "                ticks: {";
            //strScript += "                   beginAtZero: true";
            //strScript += "               }";
            //strScript += "           }";
            //strScript += "    }";
            //strScript += "});";
            //strScript += "</script>";
            strScript += "$(function () {" + Environment.NewLine;
            strScript += "    $('#myChart_" + strChartID + "').highcharts({" + Environment.NewLine;
            strScript += "        chart: {" + Environment.NewLine;
            strScript += "            polar: true," + Environment.NewLine;
            strScript += "            type: 'area'" + Environment.NewLine;      // line: 內部沒有填滿, area: 內部填滿
            strScript += "        }," + Environment.NewLine;
            strScript += "        title: {" + Environment.NewLine;
            strScript += "            text: ''" + Environment.NewLine;          // 圖表標題
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
            strScript += "            gridLineInterpolation: 'polygon'," + Environment.NewLine;
            strScript += "            lineWidth: 0," + Environment.NewLine;
            strScript += "            min: 0," + Environment.NewLine;     // 最小值
            strScript += "            max: 100" + Environment.NewLine;    // 最大值
            strScript += "        }," + Environment.NewLine;
            strScript += "        legend: {" + Environment.NewLine;         // 圖例設定
            strScript += "            enabled: false" + Environment.NewLine ;
            strScript += "        }," + Environment.NewLine;
            strScript += "        series: [{" + Environment.NewLine;        // 數值設定
            strScript += "            name: '能力指數'," + Environment.NewLine;
            strScript += "            data: " + strData + "," + Environment.NewLine;
            strScript += "            pointPlacement: 'on'" + Environment.NewLine;
            strScript += "        }]" + Environment.NewLine;
            strScript += "    });" + Environment.NewLine;
            strScript += "});" + Environment.NewLine;

            ClientScript.RegisterStartupScript(this.Page.GetType(), "radar_script", strScript, true);
        }

        private void Bar_Report(DBClass DB, DataRow myRow)
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strChartID = "";

            strChartID = myRow["ChartID"].ToString();
            litChart.Text = "<div id=\"myChart_" + strChartID + "\" style=\"min-width: 400px; max-width: 600px; height: 300px; margin: 0 auto\"></div>";
            Panel1.Controls.Add(litChart);

            Radar_Data(DB, Convert.ToInt32(strChartID), out strCategories, out strData);

            strScript += "$(function () {" + Environment.NewLine;
            strScript += "    $('#myChart_" + strChartID + "').highcharts({" + Environment.NewLine;
            strScript += "        chart: {" + Environment.NewLine;
            strScript += "            type: 'bar'" + Environment.NewLine;      // line: 內部沒有填滿, area: 內部填滿
            //strScript += "            style: {'fontSize': '12pt'}" + Environment.NewLine;
            strScript += "        }," + Environment.NewLine;
            strScript += "        title: {" + Environment.NewLine;
            strScript += "            text: ''" + Environment.NewLine;          // 圖表標題
            strScript += "        }," + Environment.NewLine;
            strScript += "        xAxis: {" + Environment.NewLine;
            strScript += "            categories: " + strCategories + "," + Environment.NewLine;
            strScript += "            tickmarkPlacement: 'on'," + Environment.NewLine;
            strScript += "            lineWidth: 0," + Environment.NewLine;
            strScript += "            labels: {" + Environment.NewLine ;
            strScript += "                style: {'fontSize':'12pt'}" + Environment.NewLine;
            strScript += "            }" + Environment.NewLine;
            strScript += "        }," + Environment.NewLine;
            strScript += "        yAxis: {" + Environment.NewLine;
            //strScript += "            gridLineInterpolation: 'polygon'," + Environment.NewLine;
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
            strScript += "        series: [{" + Environment.NewLine;        // 數值設定
            strScript += "            name: '能力指數'," + Environment.NewLine;
            strScript += "            data: " + strData + "," + Environment.NewLine;
            //strScript += "            pointPlacement: 'on'" + Environment.NewLine;
            strScript += "        }]" + Environment.NewLine;
            strScript += "    });" + Environment.NewLine;
            strScript += "});" + Environment.NewLine;

            ClientScript.RegisterStartupScript(this.Page.GetType(), "radar_script", strScript, true);
        }

        private void Element_Report(DBClass DB, DataRow myRow)
        {
            string strSQL = "";
            DataTable dtChart;
            Literal myLiteral;
            string strData = "";

            try
            {
                strSQL = "Select PropertyName, PropertyValue, ResultName From ChartDetail Where ChartID=@ID";
                DB.AddSqlParameter("@ID", Convert.ToInt32(myRow["ChartID"].ToString()));
                dtChart = DB.GetDataTable(strSQL);
                if (dtChart != null)
                {
                    for (int j = 0; j < dtChart.Rows.Count; j++)
                    {
                        switch (dtChart.Rows[j]["PropertyName"].ToString())
                        {
                            case "ELEMENT":
                                strData = dtChart.Rows[j]["PropertyValue"].ToString();
                                DB.AddSqlParameter("@PaperID", Convert.ToInt64(Session["PaperID"]));
                                myLiteral = new Literal();
                                myLiteral.Text = HttpUtility.HtmlDecode(strData);
                                Panel1.Controls.Add(myLiteral);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Element_Report(" + myRow["ChartID"].ToString() + ")", DB.DBErrorMessage);
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void Radar_Data(DBClass DB, int intChartID, out string strCategories, out string strData)
        {
            //string strData = "";
            string strSQL = "";
            //string strLabel = "";
            //string strReturn = "";
            DataTable myData;

            strCategories = "";
            strData = "";

            try
            {
                strSQL = "Select PropertyValue From ChartDetail Where ChartID=@ID And PropertyName='DATA'";
                DB.AddSqlParameter("@ID", intChartID);
                strSQL = DB.GetData(strSQL);

                if (strSQL != null)
                {
                    DB.AddSqlParameter("@PaperID", Convert.ToInt64(Session["PaperID"]));
                    myData = DB.GetDataTable(strSQL);
                    if (myData != null)
                    {
                        for (int i = 0; i < myData.Rows.Count; i++)
                        {
                            //strLabel += "'" + myData.Rows[i]["ParmName"].ToString() + "',";
                            strCategories += "'" + myData.Rows[i]["DimensionName"].ToString() + "',";
                            strData += myData.Rows[i]["Point"].ToString() + ",";
                        }
                        //strLabel = "lables:[" + strLabel.Substring(0, strLabel.Length - 1) + "]";
                        //strData = "data:[" + strData.Substring(0, strData.Length - 1) + "]";
                        strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                        strData = "[" + strData.Substring(0, strData.Length - 1) + "]";
                        //strReturn = "{" + strLabel + ", datasets:[{" + strData + "}]}" ;
                    }
                }
                else
                {
                    ShareFunction.PutLog("Radar_Data", DB.DBErrorMessage );
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Radar_Data", ex.Message);
            }

            //return strReturn;
        }

        private struct ClassifyData
        {
            public int ClassifyID;
            public string Formula;
            public string DataType;
            public string ClassifyValue;
        }
    }
}