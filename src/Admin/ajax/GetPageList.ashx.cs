using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Ferryman.DATA;
using Newtonsoft.Json;
using Ferryman.Utility;

namespace Admin
{
    /// <summary>
    /// GetPageList 的摘要描述
    /// </summary>
    public class GetPageList : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            DataTable myData = null;
            myData = GetData();
            string strJSON = "";
            if ((myData != null))
            {
                strJSON = JsonConvert.SerializeObject(myData, Formatting.Indented);
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

        private DataTable GetData()
        {
            DataTable myData = null;

            try
            {
                PageTypeClass clsPageType = new PageTypeClass();
                clsPageType.Disabled = "N";
                myData = clsPageType.Select("");
                if (clsPageType.ErrorMessage!="")
                {
                    ShareFunction.PutLog("GetPageList.GetData", clsPageType.ErrorMessage);
                }
                return myData;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("GetPageList.GetData", ex.Message);
                return null;
            }
        }
    }
}