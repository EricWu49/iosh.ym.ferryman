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
    public class SurveyClass
    {
        DBClass DB;
        private string _ErrorMessage = "";

        public SurveyClass()
        {
            DB = new DBClass(General.DataBaseType.MSSQL, "DB");
        }

        public string ErrorMessage {
            get { return _ErrorMessage; }
        }

        public DataTable GetSurveyList(string FilterString)
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select S.SurveyID, S.SurveyName, S.Instruction, S.Closed, Case S.Closed When 'Y' Then N'是' Else N'否' End as ClosedText,  IsNull(P.PageType, N'未設定') as PageType, S.Remark, P.PageID "
                       + "From Survey S Left Join PageType P On S.PageID=P.PageID ";
                if (FilterString != "")
                {
                    strSQL = strSQL + "Where " + FilterString + " ";
                }
                strSQL = strSQL + "Order by SurveyID Desc ";
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

        /// <summary>
        /// 取得診斷維度
        /// </summary>
        /// <param name="FilterString"></param>
        /// <returns></returns>
        public DataTable GetSurveyDimension(string FilterString)
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select DimensionID, DimensionName + '(' + Cast(MaxPoint as varchar(7)) + ')' as DimensionName "
                       + "From SurveyDimension " 
                       + "Where Disabled='N' ";
                if (FilterString != "")
                {
                    strSQL = strSQL + "And " + FilterString + " ";
                }
                strSQL = strSQL + "Order by OrderNo ";
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

        public DataTable GetSituationRule(string SurveyID, string DimensionID)
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select T0.ruleid, T0.minvalue, T0.maxvalue, T0.situationid, T1.situationname, T1.suggest "
                       + "From SituationRule T0 Join Situation T1 On T0.SituationID=T1.SituationID "
                       + "Where T0.SurveyID=@Survey And T0.DimensionID=@Dimension " 
                       + "Order By T0.MinValue, T0.RuleID";
                DB.AddSqlParameter("@Survey", SurveyID);
                DB.AddSqlParameter("@Dimension", DimensionID);
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

        public DataTable GetStrategy(string SituationID)
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select T0.situationid, T1.strategyid, T1.strategyname, T1.strategytype, T1.strategysource "
                       + "From ResourceDB T0 Join Strategy T1 On T0.StrategyID=T1.StrategyID "
                       + "Where T0.SituationID=@ID  "
                       + "Order By T0.StrategyID";
                DB.AddSqlParameter("@ID", SituationID);
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

        public DataTable GetSituationList()
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select situationid, situationname, suggest "
                       + "From Situation ";
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

        public DataTable GetQuestionList(string SurveyID)
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select QuestionID, QTitle "
                       + "From Question "
                       + "Where Deleted='N' And QType>10 And SurveyID=@ID "
                       + "Order By OrderNo";
                DB.AddSqlParameter("@ID", SurveyID);
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

        public DataTable GetOptionList(string QuestionID)
        {
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select SelectOption, SelectValue "
                       + "From OptionData "
                       + "Where Disabled='N' And ColumnNo=1 And QuestionID=@ID "
                       + "Order By SortID";
                DB.AddSqlParameter("@ID", QuestionID);
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