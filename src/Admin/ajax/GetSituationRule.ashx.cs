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
    /// GetSituationRule 的摘要描述
    /// </summary>
    public class GetSituationRule : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strJSON = "";

            // 取得前端傳入的查詢參數
            string strSurveyID = "";
            string strDimensionID = "";

            if (context.Request["survey"] != null)
            {
                // 取得資料
                strSurveyID = context.Request["survey"];
            }

            if (context.Request["dimension"] != null)
            {
                // 取得資料
                strDimensionID = context.Request["dimension"];
            }

            if (strSurveyID=="" || strDimensionID=="")
            {
                Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                objJSON.Add("total", 0);
                objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                strJSON = JsonConvert.SerializeObject(objJSON);
            }
            else
            {
                strJSON = GetData(strSurveyID, strDimensionID);
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

        /// <summary>
        /// 取得診斷結果判斷條件
        /// </summary>
        /// <param name="SurveyID">問卷編號</param>
        /// <param name="DimensionID">維度編號</param>
        /// <returns></returns>
        private string GetData(string SurveyID, string DimensionID)
        {
            string strJSON = "";
            string strCount = "";
            DataTable myTable = null;

            try
            {
                SurveyClass clsSurvey = new SurveyClass();

                myTable = clsSurvey.GetSituationRule(SurveyID, DimensionID);
                strCount = (myTable == null ? "0" : myTable.Rows.Count.ToString());

                if (myTable == null)
                {
                    ShareFunction.PutLog("GetSurveyRule.GetData", clsSurvey.ErrorMessage);
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
                ShareFunction.PutLog("GetSurveyRule.GetData", ex.Message);
                //Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                ////objJSON.Add("total", 0);
                //objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                //return JsonConvert.SerializeObject(objJSON);
                return "[]";
            }
        }
    }
}