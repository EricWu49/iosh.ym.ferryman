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
    /// GetHealthSuggest 的摘要描述
    /// </summary>
    public class GetHealthSuggest : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strJSON = "";

            // 取得前端傳入的查詢參數
            string strQuestionID = "";
            string strSelectValue = "";

            if (context.Request["id"] != null)
            {
                strQuestionID = context.Request["id"];
            }

            if (context.Request["value"] != null)
            {
                strSelectValue = context.Request["value"];
            }

            if (strQuestionID == "" || strSelectValue == "")
            {
                Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                objJSON.Add("total", 0);
                objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                strJSON = JsonConvert.SerializeObject(objJSON);
            }
            else
            {
                strJSON = GetData(strQuestionID, strSelectValue);
            }

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

        private string GetData(string QuestionID, string SelectValue)
        {
            string strJSON = "";
            string strCount = "";
            DataTable myTable = null;

            try
            {
                StrategyClass clsStrategy = new StrategyClass();

                myTable = clsStrategy.GetHealthSuggest(QuestionID, SelectValue);
                strCount = (myTable == null ? "0" : myTable.Rows.Count.ToString());

                if (myTable == null)
                {
                    if (clsStrategy.ErrorMessage != "")
                    {
                        ShareFunction.PutLog("GetHealthSuggest.GetData", clsStrategy.ErrorMessage);
                    }
                    return "[]";
                }
                else
                {
                    strJSON = JsonConvert.SerializeObject(myTable, Formatting.Indented);
                    return strJSON;
                    //Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                    //Newtonsoft.Json.Linq.JArray objJArray = default(Newtonsoft.Json.Linq.JArray);

                    //if (strJSON == "null")
                    //{
                    //    //objJSON.Add("total", Convert.ToInt32(strCount));
                    //    objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                    //    return JsonConvert.SerializeObject(objJSON);
                    //}
                    //else
                    //{
                    //    objJArray = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(strJSON);
                    //    //objJSON.Add("total", Convert.ToInt32(strCount));
                    //    objJSON.Add("rows", objJArray);
                    //    return JsonConvert.SerializeObject(objJSON);
                    //}
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("GetHealthSuggest.GetData", ex.Message);
                //Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                ////objJSON.Add("total", 0);
                //objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                //return JsonConvert.SerializeObject(objJSON);
                return "[]";
            }
        }
    }
}