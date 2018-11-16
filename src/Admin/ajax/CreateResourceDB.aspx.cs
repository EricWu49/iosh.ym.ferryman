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
    /// <summary>
    /// 因為ResourceDB沒有識別ID當主鍵，暫不支援更新，要更新，需先刪除再新增
    /// </summary>
    public partial class CreateResourceDB : System.Web.UI.Page
    {
        private int _SituationID = 0;
        private int _StrategyID = 0;
        private string _Operation = "A";

        protected void Page_Load(object sender, EventArgs e)
        {
            bool IsDataOK = true;

            try
            {
                if (Request["situation"] != null)
                {
                    _SituationID = Convert.ToInt32(Request["situation"]);
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
                    if (_Operation=="A")
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
                strSQL = "Insert Into ResourceDB " +
                         "(SituationID, StrategyID) " +
                         "Values (@Situation, @StrategyID)";
                DB.AddSqlParameter("@Situation", _SituationID);
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

        //private bool Update_Data()
        //{
        //    DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
        //    string strSQL = "";
        //    string strData = "";

        //    try
        //    {
        //        strSQL = "Update ResourceDB " +
        //                 "Set(SurveyID, RuleID, DimensionID, MinValue, MaxValue, SituationID) " +
        //                 "Values (@Survey, @Rule, @Dimension, @Min, @Max, @Situation)";
        //        DB.AddSqlParameter("@Survey", _SurveyID);
        //        DB.AddSqlParameter("@Rule", _RuleID);
        //        DB.AddSqlParameter("@Dimension", _DimensionID);
        //        DB.AddSqlParameter("@Min", _MinValue);
        //        DB.AddSqlParameter("@Max", _MaxValue);
        //        DB.AddSqlParameter("@Situation", _SituationID);
        //        if (DB.RunSQL(strSQL) > 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            ShareFunction.PutLog("Insert_Data", DB.DBErrorMessage);
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ShareFunction.PutLog("Insert_Data", ex.Message);
        //        return false;
        //    }
        //    finally
        //    {
        //        DB = null;
        //    }
        //}
    }
}