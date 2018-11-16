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
    /// GetSurveyDimension 的摘要描述
    /// </summary>
    public class GetSurveyDimension : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strJSON = "";

            // 取得前端傳入的查詢參數
            string strSurveyID = "";

            if (context.Request["id"] != null)
            {
                // 取得資料
                strSurveyID = context.Request["id"];
                strJSON = GetData(strSurveyID);
            }
            else
            {
                Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                objJSON.Add("total", 0);
                objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                strJSON = JsonConvert.SerializeObject(objJSON);
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
        /// 取得資料
        /// </summary>
        /// <param name="SurveyID"></param>
        /// <returns></returns>
        private string GetData(string SurveyID)
        {
            string strJSON = "";
            string strCount = "";
            DataTable myTable = null;

            try
            {
                SurveyClass clsSurvey = new SurveyClass();

                myTable = clsSurvey.GetSurveyDimension("SurveyID=" + SurveyID);
                strCount = (myTable == null ? "0" : myTable.Rows.Count.ToString());

                if (myTable == null)
                {
                    ShareFunction.PutLog("GetSurveyDimension.GetData", clsSurvey.ErrorMessage);
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
                ShareFunction.PutLog("GetSurveyDimension.GetData", ex.Message);
                //Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                ////objJSON.Add("total", 0);
                //objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                //return JsonConvert.SerializeObject(objJSON);
                return "[]";
            }
        }
    }
}