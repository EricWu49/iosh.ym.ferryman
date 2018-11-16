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
    /// GetStrategy 的摘要描述
    /// </summary>
    public class GetStrategy : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strJSON = "";

            // 取得前端傳入的查詢參數
            string strSituationID = "";

            if (context.Request["id"] != null)
            {
                strSituationID = context.Request["id"];
            }

            if (strSituationID == "" )
            {
                Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                objJSON.Add("total", 0);
                objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                strJSON = JsonConvert.SerializeObject(objJSON);
            }
            else
            {
                strJSON = GetData(strSituationID);
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

        private string GetData(string SituationID)
        {
            string strJSON = "";
            string strCount = "";
            DataTable myTable = null;

            try
            {
                SurveyClass clsSurvey = new SurveyClass();

                myTable = clsSurvey.GetStrategy(SituationID);
                strCount = (myTable == null ? "0" : myTable.Rows.Count.ToString());

                if (myTable == null)
                {
                    if (clsSurvey.ErrorMessage!="")
                    {
                        ShareFunction.PutLog("GetStrategy.GetData", clsSurvey.ErrorMessage);
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
                ShareFunction.PutLog("GetStrategy.GetData", ex.Message);
                //Newtonsoft.Json.Linq.JObject objJSON = new Newtonsoft.Json.Linq.JObject();
                ////objJSON.Add("total", 0);
                //objJSON.Add("rows", new Newtonsoft.Json.Linq.JArray());
                //return JsonConvert.SerializeObject(objJSON);
                return "[]";
            }
        }
    }
}