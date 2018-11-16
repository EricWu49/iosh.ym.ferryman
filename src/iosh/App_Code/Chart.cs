using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Ferryman.Utility
{
    /// <summary>
    /// 繪製Highcharts圖表
    /// 需搭配highcharts套件使用
    /// </summary>
    public class Chart
    {
        public string ReportTitle = "";
        public float MinValue = 0;
        public float MaxValue = 100;

        public string RadarChart(string ChartContainerID, DataTable myData, string SeriesName, string CategoryField, string ValueField)
        {
            string strScript = "";
            string strCategories = "";
            string strData = "";

            try
            {
                for (int i = 0; i < myData.Rows.Count; i++)
                {
                    strCategories += "'" + myData.Rows[i][CategoryField].ToString() + "',"; 
                    strData += myData.Rows[i][ValueField].ToString() + ",";
                }
                strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                strScript += "$(function () {" + Environment.NewLine;
                strScript += "    $('#" + ChartContainerID + "').highcharts({" + Environment.NewLine;
                strScript += "        chart: {" + Environment.NewLine;
                strScript += "            polar: true," + Environment.NewLine;
                strScript += "            type: 'area'" + Environment.NewLine;      // line: 內部沒有填滿, area: 內部填滿
                strScript += "        }," + Environment.NewLine;
                strScript += "        title: {" + Environment.NewLine;
                strScript += "            text: '" + ReportTitle + "'" + Environment.NewLine;          // 圖表標題
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
                strScript += "            min: " + MinValue.ToString() + "," + Environment.NewLine;     // 最小值
                strScript += "            max: " + MaxValue.ToString() + Environment.NewLine;    // 最大值
                strScript += "        }," + Environment.NewLine;
                strScript += "        legend: {" + Environment.NewLine;         // 圖例設定
                strScript += "            enabled: false" + Environment.NewLine;
                strScript += "        }," + Environment.NewLine;
                strScript += "        series: [{" + Environment.NewLine;        // 數值設定
                strScript += "            name: '" + SeriesName + "'," + Environment.NewLine;
                strScript += "            data: " + strData + "," + Environment.NewLine;
                strScript += "            pointPlacement: 'on'" + Environment.NewLine;
                strScript += "        }]" + Environment.NewLine;
                strScript += "    });" + Environment.NewLine;
                strScript += "});" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("RadarChart", ex.Message);
                strScript = "";
            }
            return strScript;
        }

        public string BarChart(string ChartContainerID, DataTable myData, string SeriesName, string CategoryField, string ValueField)
        {
            string strScript = "";
            string strCategories = "";
            string strData = "";

            try
            {
                for (int i = 0; i < myData.Rows.Count; i++)
                {
                    strCategories += "'" + myData.Rows[i][CategoryField].ToString() + "',";
                    strData += myData.Rows[i][ValueField].ToString() + ",";
                }
                strCategories = "[" + strCategories.Substring(0, strCategories.Length - 1) + "]";
                strData = "[" + strData.Substring(0, strData.Length - 1) + "]";

                strScript += "$(function () {" + Environment.NewLine;
                strScript += "    $('#" + ChartContainerID + "').highcharts({" + Environment.NewLine;
                strScript += "        chart: {" + Environment.NewLine;
                strScript += "            type: 'bar'" + Environment.NewLine;
                strScript += "        }," + Environment.NewLine;
                strScript += "        title: {" + Environment.NewLine;
                strScript += "            text: '" + ReportTitle + "'" + Environment.NewLine;          // 圖表標題
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
                strScript += "            min: " + MinValue.ToString() + "," + Environment.NewLine;     // 最小值
                strScript += "            max: " + MaxValue.ToString() + "," + Environment.NewLine;    // 最大值
                strScript += "            title: {" + Environment.NewLine;
                strScript += "                text: ''" + Environment.NewLine;
                strScript += "            }," + Environment.NewLine;
                strScript += "        }," + Environment.NewLine;
                strScript += "        legend: {" + Environment.NewLine;         // 圖例設定
                strScript += "            enabled: false" + Environment.NewLine;
                strScript += "        }," + Environment.NewLine;
                strScript += "        series: [{" + Environment.NewLine;        // 數值設定
                strScript += "            name: '" + SeriesName  + "'," + Environment.NewLine;
                strScript += "            data: " + strData + "," + Environment.NewLine;
                strScript += "        }]" + Environment.NewLine;
                strScript += "    });" + Environment.NewLine;
                strScript += "});" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("RadarChart", ex.Message);
                strScript = "";
            }
            return strScript;
        }
    }
}