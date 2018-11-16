using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ferryman.DATA;
using Ferryman.Utility;
using System.Data;

namespace Admin
{
    public partial class CreateHealthSuggest : System.Web.UI.Page
    {
        private int _QuestionID = 0;
        private string _SelectValue = "";
        private int _StrategyID = 0;
        private string _Operation = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            bool IsDataOK = true;

            try
            {
                if (Request["question"] != null)
                {
                    _QuestionID = Convert.ToInt32(Request["question"]);
                }
                else
                {
                    IsDataOK = false;
                }

                if (Request["value"] != null)
                {
                    _SelectValue = Request["value"];
                }
                else
                {
                    IsDataOK = false;
                }

                if (Request["strategy"] != null)
                {
                    _StrategyID = Convert.ToInt32(Request["strategy"]);
                }
                else
                {
                    IsDataOK = false;
                }

                if (Request["operation"] != null)
                {
                    _Operation = Request["operation"];
                }
                else
                {
                    IsDataOK = false;
                }

                if (IsDataOK)
                {
                    if (_Operation == "A")
                    {
                        if (Insert_Data() == true)
                        {
                            Response.Write("");
                        }
                        else
                        {
                            Response.Write("新增失敗");
                        }
                    }
                    //else
                    //{
                    //    if (Update_Data() == true)
                    //    {
                    //        Response.Write("");
                    //    }
                    //    else
                    //    {
                    //        Response.Write("修改失敗");
                    //    }
                    //}
                }
                else
                {
                    Response.Write("資料不足");
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Page_Load", ex.Message);
                Response.Write("儲存錯誤");
            }
        }

        private bool Insert_Data()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strData = "";

            try
            {
                strSQL = "Insert Into HealthSuggest " +
                         "(QuestionID, LineID, SelectValue, StrategyID) " +
                         "Values (@Question, 1, @Value, @StrategyID)";
                DB.AddSqlParameter("@Question", _QuestionID);
                DB.AddSqlParameter("@Value", _SelectValue);
                DB.AddSqlParameter("@StrategyID", _StrategyID);
                if (DB.RunSQL(strSQL) > 0)
                {
                    return true;
                }
                else
                {
                    ShareFunction.PutLog("Insert_Data", DB.DBErrorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Insert_Data", ex.Message);
                return false;
            }
            finally
            {
                DB = null;
            }
        }
    }
}