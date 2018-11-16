using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Ferryman.Utility;
using System.Data;

namespace Admin.ajax
{
    /// <summary>
    /// GetStrategyDataList 的摘要描述
    /// </summary>
    public class GetStrategyDataList : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strJSON = "";

            // 取得前端傳入的查詢參數
            //string strSituationID = "";

            //if (context.Request["id"] != null)
            //{
            //    strSituationID = context.Request["id"];
            //}

            //if (strSituationID == "")
            //{
            //    Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
            //    objJSON.Add("total", 0);
            //    objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
            //    strJSON = JsonConvert.SerializeObject(objJSON);
            //}
            //else
            //{
            //    strJSON = GetData(strSituationID);
            //}
            strJSON = GetData();

            context.Response.ContentType = "text/plain";
            context.Response.Write(strJSON);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string GetData()
        {
            string strJSON = "";
            string strCount = "";
            DataTable myTable = null;

            try
            {
                StrategyClass clsStrategy = new StrategyClass();

                myTable = clsStrategy.GetStrategyList("");
                strCount = (myTable == null ? "0" : myTable.Rows.Count.ToString());

                if (myTable == null)
                {
                    if (clsStrategy.ErrorMessage != "")
                    {
                        ShareFunction.PutLog("GetStrategyDataList.GetData", clsStrategy.ErrorMessage);
                    }
                    return "[]";
                }
                else
                {
                    strJSON = JsonConvert.SerializeObject(myTable, Formatting.Indented);
                    return strJSON;
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("GetStrategyDataList.GetData", ex.Message);
                return "[]";
            }
        }
    }
}