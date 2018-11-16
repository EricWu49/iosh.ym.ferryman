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
    /// GetSituaionList 的摘要描述
    /// </summary>
    public class GetSituationList : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strJSON = "";

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
            DataTable myTable = null;

            try
            {
                SurveyClass clsSurvey = new SurveyClass();

                myTable = clsSurvey.GetSituationList();

                if (myTable == null)
                {
                    ShareFunction.PutLog("GetSituationList.GetData", clsSurvey.ErrorMessage);
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
                ShareFunction.PutLog("GetSituationList.GetData", ex.Message);
                return "[]";
            }
        }
    }
}