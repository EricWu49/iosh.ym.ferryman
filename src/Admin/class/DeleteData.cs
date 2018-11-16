using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ferryman.DATA;
using Ferryman.Utility;
using System.Data;

namespace Admin
{
    public static class DeleteData
    {
        private static string _Error = "";

        public static string ErrorMessage
        {
            get { return _Error; }
        }

        public static bool DeleteByKey(string TableName, string KeyField, string KeyValue)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";

            try
            {
                strSQL = "Delete " + TableName + " Where " + KeyField + "='" + KeyValue + "'";
                if (DB.RunSQL(strSQL)>0)
                {
                    return true;
                }
                else
                {
                    if (DB.DBErrorMessage!="")
                    {
                        _Error = DB.DBErrorMessage;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                _Error = ex.Message;
                return false;
            }
        }

        public static bool DeleteByID(string TableName, string KeyField, int KeyValue)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";

            try
            {
                strSQL = "Delete " + TableName + " Where " + KeyField + "=" + KeyValue.ToString();
                if (DB.RunSQL(strSQL) > 0)
                {
                    return true;
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        _Error = DB.DBErrorMessage;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                _Error = ex.Message;
                return false;
            }
        }

        public static bool Delete(string TableName, string FilterString)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";

            try
            {
                strSQL = "Delete " + TableName + " Where " + FilterString;
                if (DB.RunSQL(strSQL) > 0)
                {
                    return true;
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        _Error = DB.DBErrorMessage;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                _Error = ex.Message;
                return false;
            }
        }
    }
}