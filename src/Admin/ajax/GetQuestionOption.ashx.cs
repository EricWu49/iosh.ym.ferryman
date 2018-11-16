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
    /// GetQuestionOption 的摘要描述
    /// </summary>
    public class GetQuestionOption : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strJSON = "";

            // 取得前端傳入的查詢參數
            string strQuestionID = "";

            if (context.Request["id"] != null)
            {
                // 取得資料
                strQuestionID = context.Request["id"];
                strJSON = GetData(strQuestionID);
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
        private string GetData(string strQuestionID)
        {
            string strJSON = "";
            string strCount = "";
            DataTable myTable = null;

            try
            {
                SurveyClass clsSurvey = new SurveyClass();

                myTable = clsSurvey.GetOptionList(strQuestionID);
                strCount = (myTable == null ? "0" : myTable.Rows.Count.ToString());

                if (myTable == null)
                {
                    ShareFunction.PutLog("GetQuestionOption.GetData", clsSurvey.ErrorMessage);
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
                ShareFunction.PutLog("GetQuestionOption.GetData", ex.Message);
                return "[]";
            }
        }
    }
}