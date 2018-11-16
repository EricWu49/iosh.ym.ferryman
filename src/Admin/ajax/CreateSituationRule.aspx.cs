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
    public partial class CreateSituationRule : System.Web.UI.Page
    {
        private int _RueID = 0;
        private int _SurveyID = 0;
        private int _DimensionID = 0;
        private int _RuleID = 0;
        private float _MinValue = 0;
        private float _MaxValue = 0;
        private int _SituationID = 0;
             
        /// <summary>
        /// 診斷規則維護
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            bool IsDataOK = true;

            try
            {
                if (Request["id"] != null)
                {
                    _RueID = Convert.ToInt32(Request["id"]);
                }
                else
                {
                    IsDataOK = false;
                }

                if (Request["survey"] != null)
                {
                    _SurveyID = Convert.ToInt32(Request["survey"]);
                }
                else
                {
                    IsDataOK = false;
                }

                if (Request["dimension"] != null)
                {
                    _DimensionID = Convert.ToInt32(Request["dimension"]);
                }
                else
                {
                    IsDataOK = false;
                }

                if (Request["min"] != null)
                {
                    _MinValue = Convert.ToSingle(Request["min"]);
                }
                else
                {
                    IsDataOK = false;
                }

                if (Request["max"] != null)
                {
                    _MaxValue = Convert.ToSingle(Request["max"]);
                }
                else
                {
                    IsDataOK = false;
                }

                if (Request["situation"] != null)
                {
                    _SituationID = Convert.ToInt32(Request["situation"]);
                }
                else
                {
                    IsDataOK = false;
                }

                if (IsDataOK)
                {
                    if (_RuleID == 0)
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
                    else
                    {
                        if (Update_Data() == true)
                        {
                            Response.Write("");
                        }
                        else
                        {
                            Response.Write("新增失敗");
                        }
                    }
                }
                else
                {
                    Response.Write("資料不足");
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Page_Load", ex.Message);
                Response.Write("新增錯誤");
            }
        }

        /// <summary>
        /// 新增診斷規則
        /// </summary>
        /// <returns></returns>
        private bool Insert_Data()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strData = "";

            try
            {
                strSQL = "Select Max(RuleID) From SituationRule Where SurveyID=@Survey";
                DB.AddSqlParameter("@Survey", _SurveyID);
                strData = DB.GetData(strSQL);
                if (strData==null)
                {
                    _RuleID = 1;
                }
                else
                {
                    _RuleID = Convert.ToInt32(strData) + 1;
                }

                strSQL = "Insert Into SituationRule " +
                         "(SurveyID, RuleID, DimensionID, MinValue, MaxValue, SituationID) " +
                         "Values (@Survey, @Rule, @Dimension, @Min, @Max, @Situation)";
                DB.AddSqlParameter("@Survey", _SurveyID);
                DB.AddSqlParameter("@Rule", _RuleID);
                DB.AddSqlParameter("@Dimension", _DimensionID);
                DB.AddSqlParameter("@Min", _MinValue);
                DB.AddSqlParameter("@Max", _MaxValue);
                DB.AddSqlParameter("@Situation", _SituationID);
                if (DB.RunSQL(strSQL)>0)
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

        /// <summary>
        /// 更新診斷規則
        /// </summary>
        /// <returns></returns>
        private bool Update_Data()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strData = "";

            try
            {
                strSQL = "Update SituationRule " +
                         "Set MinValue=@Min, MaxValue=@Max, SituationID=@Situation " +
                         "Where SurveyID=@Survey And RuleID=@Rule";
                DB.AddSqlParameter("@Survey", _SurveyID);
                DB.AddSqlParameter("@Rule", _RuleID);
                DB.AddSqlParameter("@Min", _MinValue);
                DB.AddSqlParameter("@Max", _MaxValue);
                DB.AddSqlParameter("@Situation", _SituationID);
                if (DB.RunSQL(strSQL) > 0)
                {
                    return true;
                }
                else
                {
                    ShareFunction.PutLog("Update_Data", DB.DBErrorMessage);
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