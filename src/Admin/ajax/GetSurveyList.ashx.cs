using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Ferryman.Utility;
using System.Data;

namespace Admin
{
    /// <summary>
    /// GetSurveyList 的摘要描述
    /// </summary>
    public class GetSurveyList : IHttpHandler
    {
        int _PageIndex = 1;
        int _PageSize = 10;

        public void ProcessRequest(HttpContext context)
        {
            // 取得前端回傳的資訊
            int intParse = 0;

            // 目前所在頁碼
            if ((context.Request["page"] != null))
            {
                if (int.TryParse(context.Request["page"], out intParse))
                {
                    _PageIndex = int.Parse(context.Request["page"]);
                }
            }
            // 每頁包含資料筆數
            if ((context.Request["rows"] != null))
            {
                if (int.TryParse(context.Request["rows"], out intParse))
                {
                    _PageSize = int.Parse(context.Request["rows"]);
                }

            }

            string strJSON = "";

            // 取得前端傳入的查詢參數
            //string strEmpID = "";

            //if (context.Request["empid"] != null)
            //{
            //    //strEmpID = context.Request["empid"];
            //}

            //strEmpID = context.Session["EmpID"].ToString();

            // 取得資料
            strJSON = GetData();

            //ShareFunction.PutLog("JSON", strJSON);

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
                SurveyClass clsSurvey = new SurveyClass();

                myTable = clsSurvey.GetSurveyList("");
                strCount = (myTable == null ? "0" : myTable.Rows.Count.ToString());

                if (myTable == null)
                {
                    ShareFunction.PutLog("GetPaperList.GetData", clsSurvey.ErrorMessage);
                    return "";
                }
                else
                {
                    strJSON = JsonConvert.SerializeObject(myTable, Formatting.Indented);
                    Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                    Newtonsoft.Json.Linq.JArray objJArray = default(Newtonsoft.Json.Linq.JArray);

                    if (strJSON == "null")
                    {
                        objJSON.Add("total", Convert.ToInt32(strCount));
                        objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                        return JsonConvert.SerializeObject(objJSON);
                    }
                    else
                    {
                        objJArray = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(strJSON);
                        objJSON.Add("total", Convert.ToInt32(strCount));
                        objJSON.Add("rows", objJArray);
                        return JsonConvert.SerializeObject(objJSON);
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("GetPaperList.GetData", ex.Message);
                Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                objJSON.Add("total", 0);
                objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                return JsonConvert.SerializeObject(objJSON);
            }
        }
    }
}