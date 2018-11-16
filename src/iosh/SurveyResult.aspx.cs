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
    public partial class SurveyResult : System.Web.UI.Page
    {
        long _PaperID = 0;
        int _SurveyID = 0;
        DBClass DB;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _PaperID = Convert.ToInt64(Session["PaperID"]);
                _SurveyID = Convert.ToInt32(Session["SurveyID"]);
                Main_Process();
                ViewState.Add("PaperID", _PaperID);
            }
        }

        private void Main_Process()
        {
            try
            {
                DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                Index_Report();
                Radar_Report();
                Bar_Report();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 產生工作適能診斷結果
        /// </summary>
        private void Index_Report()
        {
            string strSQL = "";
            string strData, strConnector;
            Single numIndex = 0;
            List<ClassifyData> lstClassify;
            ClassifyData objClassify;
            DataTable myData, dtClassify;
            int intCode = 0;
            int i;

            try
            {
                strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                DB.AddSqlParameter("@PaperID", _PaperID);
                strData = DB.GetData(strSQL);
                lblIndex.Text = strData;

                // 取出要分析的構面
                numIndex = Convert.ToSingle(strData);

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

                    for (i = 0; i < lstClassify.Count; i++)
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
                    DB.AddSqlParameter("@Value", numIndex);
                    myData = DB.GetDataTable(strSQL);

                    if (myData != null)
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
                        if (DB.RunSQL(strSQL) < 1)
                        {
                            ShareFunction.PutLog("Index_Report", DB.DBErrorMessage);
                        }
                    }
                    else
                    {
                        ShareFunction.PutLog("Index_Report", DB.DBErrorMessage);
                        strData = "";
                    }

                    lblLevel.Text = strData;

                    if (lblLevel.Text == "弱" || lblLevel.Text == "普通")
                    {
                        pnlWarning.Visible = true;
                        pnlInfo.Visible = false;
                        pnlAction.Visible = true;
                    }
                    else
                    {
                        pnlWarning.Visible = false;
                        pnlInfo.Visible = true;
                        pnlAction.Visible = true;
                    }

                }       // if (dtClassify != null)
            }       // try
            catch (Exception ex)
            {
                ShareFunction.PutLog("Index_Report", ex.Message);
                Message.MsgShow(this.Page, "無法判斷工作適能指數。");
            }
        }

        /// <summary>
        /// 產生雷達圖表
        /// </summary>
        private void Radar_Report()
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select S.DimensionName, Case S.MaxPoint When 0 Then R.Point Else (R.Point/S.MaxPoint) * 100 End as Point "
                       + "From PaperResult R Join SurveyDimension S On R.DimensionID=S.DimensionID "
                       + "Where PaperID=@PaperID And S.DimensionID Between 2 And 8 "
                       + "Order By S.OrderNo";
                DB.AddSqlParameter("@PaperID", _PaperID);
                myData = DB.GetDataTable(strSQL);

                if (myData != null)
                {
                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        strCategories += "'" + myData.Rows[i]["DimensionName"].ToString() + "',";
                        strData += myData.Rows[i]["Point"].ToString() + ",";
                    }
                    strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                    strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                    strScript += "$(function () {" + Environment.NewLine;
                    strScript += "    $('#myChart_Radar').highcharts({" + Environment.NewLine;
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
                    strScript += "            enabled: false" + Environment.NewLine;
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
                else
                {
                    if (DB.DBErrorMessage!="")
                    {
                        ShareFunction.PutLog("Radar_Report", DB.DBErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Radar_Report", ex.Message );
            }
        }

        /// <summary>
        /// 產生長條圖表
        /// </summary>
        private void Bar_Report()
        {
            LiteralControl litChart = new LiteralControl();
            string strScript = "";
            string strCategories = "";
            string strData = "";
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select S.DimensionName, Case S.MaxPoint When 0 Then R.Point Else (R.Point/S.MaxPoint) * 100 End as Point "
                       + "From PaperResult R Join SurveyDimension S On R.DimensionID=S.DimensionID "
                       + "Where PaperID=@PaperID And S.DimensionID Between 2 And 13 "
                       + "Order By S.OrderNo";
                DB.AddSqlParameter("@PaperID", _PaperID);
                myData = DB.GetDataTable(strSQL);

                if (myData != null)
                {
                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        strCategories += "'" + myData.Rows[i]["DimensionName"].ToString() + "',";
                        strData += myData.Rows[i]["Point"].ToString() + ",";
                    }
                    strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                    strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                    strScript += "$(function () {" + Environment.NewLine;
                    strScript += "    $('#myChart_Bar').highcharts({" + Environment.NewLine;
                    strScript += "        chart: {" + Environment.NewLine;
                    strScript += "            type: 'bar'" + Environment.NewLine;
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
                    strScript += "        }]" + Environment.NewLine;
                    strScript += "    });" + Environment.NewLine;
                    strScript += "});" + Environment.NewLine;

                    ClientScript.RegisterStartupScript(this.Page.GetType(), "bar_script", strScript, true);
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Bar_Report", DB.DBErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Bar_Report", ex.Message);
            }
        }


        private struct ClassifyData
        {
            public int ClassifyID;
            public string Formula;
            public string DataType;
            public string ClassifyValue;
        }

        protected void Action_Click(object sender, EventArgs e)
        {
            Session["ParentPaper"] = ViewState["PaperID"].ToString();
            Session["PaperID"] = null;
            Response.Redirect("Survey.aspx?id=" + ((Button)sender).CommandArgument + "&paper=" + ViewState["PaperID"].ToString());
        }
    }
}