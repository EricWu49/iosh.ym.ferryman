using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Ferryman.DATA;
using Ferryman.Utility;


namespace iosh
{

    public static class ShareCode
    {
        public static string GoBackt_Report(long ReportID)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;
            string strPage = "";
            string strData = "";

            try
            {
                strSQL = "Select ParentPaper From Paper Where PaperID=@ID";
                DB.AddSqlParameter("@ID", ReportID);
                strData = DB.GetData(strSQL);
                if (DB.DBErrorMessage!="")
                {
                    ShareFunction.PutLog("GoBackt_Report", DB.DBErrorMessage);
                    return "";
                }
                else
                {
                    if (strData=="0")
                    {
                        return "Default.aspx";
                    }
                    else
                    {
                        strSQL = "Select PaperID, SessionID, SurveyID From Paper Where PaperID=@ID";
                        DB.AddSqlParameter("@ID", Convert.ToInt64(strData));
                        myData = DB.GetDataTable(strSQL);

                        if (myData != null)
                        {
                            System.Web.HttpContext.Current.Session["SurveyID"] = myData.Rows[0]["SurveyID"].ToString();
                            System.Web.HttpContext.Current.Session["PaperID"] = myData.Rows[0]["PaperID"].ToString();
                            System.Web.HttpContext.Current.Session["SessionID"] = myData.Rows[0]["SessionID"].ToString();

                            strSQL = "Select P.PageFile "
                                    + "From Survey S Join PageType P On S.PageID=P.PageID "
                                    + "Where S.SurveyID=@SID";
                            DB.AddSqlParameter("@SID", Convert.ToInt32(System.Web.HttpContext.Current.Session["SurveyID"]));
                            strPage = DB.GetData(strSQL);

                            if (strPage != null)
                            {
                                return strPage;
                            }
                            else
                            {
                                if (DB.DBErrorMessage != "")
                                {
                                    ShareFunction.PutLog("GoBackt_Report", DB.DBErrorMessage);
                                }
                                return "";
                            }
                        }
                        else
                        {
                            if (DB.DBErrorMessage != "")
                            {
                                ShareFunction.PutLog("GoBackt_Report", DB.DBErrorMessage);
                            }
                            return "";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("GoBackt_Report", ex.Message );
                return "";
            }
            finally
            {
                DB = null;
            }
        }

        public static bool Close_Report(long PaperID)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strSN = "";

            try
            {
                // 產生報告序號
                strSN = ShareFunction.GetRandomString();
                //strSN = strSN + PaperID.ToString("00000");
                strSQL = "Update Paper Set FinishTime=GetDate(), PaperSN=@SN Where PaperID=@ID And FinishTime Is Null";
                DB.AddSqlParameter("@SN", strSN);
                DB.AddSqlParameter("@ID", PaperID);
                if (DB.RunSQL(strSQL)>0)
                {
                    return true;
                }
                else
                {
                    ShareFunction.PutLog("Clse_Report", DB.DBErrorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Clse_Report", ex.Message);
                return false;
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 取得調查頁面設定(SurveyPage)的下一個處理Page
        /// </summary>
        /// <param name="SurveyID">調查ID</param>
        /// <param name="PageNo">頁數</param>
        /// <returns></returns>
        public static string GetNextWebPage(int SurveyID, int PageNo)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strPage = "";

            try
            {
                strSQL = "Select PageFile From SurveyPage Where SurveyID=@ID And PageNo=@Page";
                DB.AddSqlParameter("@ID", SurveyID);
                DB.AddSqlParameter("@Page", PageNo);
                strPage = DB.GetData(strSQL);
                if (strPage==null)
                {
                    strPage = "";
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("GetNextWebPage", DB.DBErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("GetNextWebPage", ex.Message);
                strPage = "";
            }
            finally
            {
                DB = null;
            }

            return strPage;
        }

        public static string GetReportPage(int SurveyID)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strPage = "";

            try
            {
                strSQL = "Select P.PageFile " +
                         "From Survey S Join PageType P On S.PageID=P.PageID " +
                         "Where S.SurveyID=@SID And P.Disabled='N'";
                DB.AddSqlParameter("@SID", SurveyID);
                strPage = DB.GetData(strSQL);
                if (strPage == "")
                {
                    // 預設值
                    strPage = "SurveyFinish.aspx";
                    if (strPage == null)
                    {
                        strPage = "";
                        if (DB.DBErrorMessage != "")
                        {
                            ShareFunction.PutLog("GetReportPage", DB.DBErrorMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strPage = "SurveyFinish.aspx";
                ShareFunction.PutLog("GetReportPage", ex.Message);
            }
            finally
            {
                DB = null;
            }
            return strPage;
        }

        public static int GetSurveyID(long PaperID)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            int intID = 0;

            strSQL = "Select SurveyID From Paper Where PaperID=@PID";
            DB.AddSqlParameter("@PID", PaperID);
            try
            {
                intID = Convert.ToInt32(DB.GetData(strSQL));
            }
            catch (Exception ex)
            {
                intID = 0;
            }
            finally
            {
                DB = null;
            }
            return intID;
        }
    }
}