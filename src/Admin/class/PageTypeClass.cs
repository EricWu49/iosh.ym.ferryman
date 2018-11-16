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
    public class PageTypeClass
    {
        DBClass DB;
        private string _ErrorMessage = "";

        private int _PageID;
        private string _PageType;
        private string _PageFile;
        private string _Disabled;

        public int PageID
        {
            get
            {
                return _PageID;
            }
            set
            {
                _PageID = value;
            }
        }

        public string PageType
        {
            get
            {
                return _PageType;
            }
            set
            {
                _PageType = value;
            }
        }

        public string PageFile
        {
            get
            {
                return _PageFile;
            }
            set
            {
                _PageFile = value;
            }
        }

        public string Disabled
        {
            get
            {
                return _Disabled;
            }
            set
            {
                _Disabled = value;
            }
        }

        public PageTypeClass()
        {
            DB = new DBClass(General.DataBaseType.MSSQL, "DB");
        }

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
        }

        public DataTable Select(string OrderBy)
        {
            string strSQL = "";
            DataTable myData;
            string strSelect = "";
            string strWhere = "";
            string strAnd = "";

            try
            {
                if (_PageID!=null)
                {
                    strWhere = strWhere + strAnd + "PageID=@PageID ";
                    DB.AddSqlParameter("@PageID", _PageID);
                    strAnd = "And ";
                }

                if (_PageType != null)
                {
                    strWhere = strWhere + strAnd + "PageType Like '%' + @PageType + '%' ";
                    DB.AddSqlParameter("@PageType", _PageType);
                    strAnd = "And ";
                }

                if (_PageFile != null)
                {
                    strWhere = strWhere + strAnd + "PageFile Like '%' + @PageFile + '%' ";
                    DB.AddSqlParameter("@PageType", _PageType);
                    strAnd = "And ";
                }

                if (_Disabled != null)
                {
                    strWhere = strWhere + strAnd + "Disabled=@Disabled ";
                    DB.AddSqlParameter("@Disabled", _Disabled);
                    strAnd = "And ";
                }

                strSelect = "Select PageID, PageType, PageFile, Disabled From PageType ";
                if (strWhere != "")
                {
                    strSQL = strSelect + "Where " + strWhere + " ";
                }

                if (OrderBy!="")
                {
                    strSQL = strSQL + "Order By " + OrderBy;
                }

                myData = DB.GetDataTable(strSQL);

                if (DB.DBErrorMessage!="")
                {
                    _ErrorMessage = DB.DBErrorMessage;
                }
                return myData;
            }
            catch (Exception ex)
            {
                _ErrorMessage = ex.Message;
                return null;
            }
        }

        //public DataTable GetPageTypeList(string FilterString = "")
        //{
        //    string strSQL = "";
        //    DataTable myData;

        //    try
        //    {
        //        strSQL = "Select S.SurveyID, S.SurveyName, S.Instruction, S.Closed, Case S.Closed When 'Y' Then N'是' Else N'否' End as ClosedText,  IsNull(P.PageType, N'未設定') as PageType "
        //               + "From Survey S Left Join PageType P On S.PageID=P.PageID ";
        //        if (FilterString != "")
        //        {
        //            strSQL = strSQL + "Where " + FilterString + " ";
        //        }
        //        strSQL = strSQL + "Order by SurveyID Desc ";
        //        myData = DB.GetDataTable(strSQL);
        //        _ErrorMessage = DB.DBErrorMessage;
        //        return myData;
        //    }
        //    catch (Exception ex)
        //    {
        //        _ErrorMessage = ex.Message;
        //        return null;
        //    }
        //}
    }
}