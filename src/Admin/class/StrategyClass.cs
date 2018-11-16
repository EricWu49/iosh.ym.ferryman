using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ferryman.DATA;
using Newtonsoft.Json;
using Ferryman.Utility;
using System.Data;

namespace Admin
{

    public class StrategyClass
    {
        DBClass DB;
        private string _ErrorMessage = "";

        public StrategyClass()
        {
            DB = new DBClass(General.DataBaseType.MSSQL, "DB");
        }

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
        }

        public DataTable GetStrategyList(string FilterString)
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select strategyid, strategyname, strategytype, strategysource "
                       + "From Strategy  ";
                if (FilterString != "")
                {
                    strSQL = strSQL + "Where " + FilterString + " ";
                }
                strSQL = strSQL + "Order by StrategyID ";
                myData = DB.GetDataTable(strSQL);
                _ErrorMessage = DB.DBErrorMessage;
                return myData;
            }
            catch (Exception ex)
            {
                _ErrorMessage = ex.Message;
                return null;
            }
        }

        public DataTable GetHealthSuggest(string QuestionID, string SelectValue)
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                // 目前LineID只支援1
                strSQL = "Select T0.uniqueid, T1.strategyid, T1.strategyname, T1.strategytype, T1.strategysource "
                       + "From HealthSuggest T0 Join Strategy T1 On T0.StrategyID=T1.StrategyID "
                       + "Where T0.QuestionID=@ID And SelectValue=@Value And LineID=1  "
                       + "Order By T0.StrategyID";
                DB.AddSqlParameter("@ID", QuestionID);
                DB.AddSqlParameter("@Value", SelectValue);
                myData = DB.GetDataTable(strSQL);
                _ErrorMessage = DB.DBErrorMessage;
                return myData;
            }
            catch (Exception ex)
            {
                _ErrorMessage = ex.Message;
                return null;
            }
        }
    }
}