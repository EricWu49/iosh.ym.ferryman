using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Ferryman.DATA;
using Ferryman.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Admin.ajax
{
    /// <summary>
    /// InsertPaperResource 的摘要描述
    /// </summary>
    public class InsertPaperResource : IHttpHandler
    {
        //public class ElementItem
        //{
        //    public string ElementID { get; set; }
        //    public string ElementValue { get; set; }
        //}

        //public class FormElement
        //{
        //    List<ElementItem> FormItem;
        //}

        public void ProcessRequest(HttpContext context)
        {
            string strID = "";
            string strCode = "";
            string strData = "";
            string strResult = "";
            
            try
            {
                strID = context.Request["id"];
                strCode = context.Request["code"];
                strData = context.Request["formdata"];
                ShareFunction.PutLog("ProcessRequest", strData);
                strResult = InsertData(strID, strCode, strData);
            }
            catch (Exception ex)
            {
                strResult = ex.Message;
                ShareFunction.PutLog("ProcessRequest", ex.Message);
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(strResult);
        }

        private string InsertData(string PaperID, string Code, string strData)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            Dictionary<string, string> FormElements;

            try
            {
                FormElements = JsonConvert.DeserializeObject<Dictionary<string, string>>(strData);
                
                // 先刪除舊的資料
                strSQL = "Delete PaperResource Where PaperID=@PID And PositionCode=@PCode";
                DB.AddCommand(strSQL);

                foreach (KeyValuePair<string, string> FormItem in FormElements)
                {
                    if (FormItem.Key!="")
                    {
                        strSQL = "Insert Into PaperResource " +
                                 "Values (@PID, @PCode, '" + FormItem.Key.Replace("video-", "") + "')";
                        DB.AddCommand(strSQL);
                    }
                }
                DB.AddSqlParameter("@PID", Convert.ToInt64(PaperID));
                DB.AddSqlParameter("@PCode", Code);

                if (DB.DoTransaction())
                {
                    return "";
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("InsertData", DB.DBErrorMessage);
                        return "儲存發生錯誤。";
                    }
                    else
                    {
                        return "儲存失敗。";
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("InsertData", ex.Message);
                return ex.Message;
            }
            finally
            {
                DB = null;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}