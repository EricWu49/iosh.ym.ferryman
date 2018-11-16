using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Ferryman.DATA;
using Ferryman.Utility;
using System.Data;
using System.Text;

namespace Admin
{
    public partial class Export : System.Web.UI.Page
    {
        string strSurvey = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack )
            {
                string strDate1 = "";
                string strDate2 = "";

                if (Request.QueryString["id"]==null)
                {
                    ShareFunction.Register_Close_Script(this.Page);
                }
                else
                {
                    strSurvey = Request.QueryString["id"];
                    if (Request.QueryString["date1"]!=null)
                    {
                        strDate1 = Request.QueryString["date1"];
                        strDate1 = Convert.ToDateTime(strDate1).ToString("yyyy/MM/dd 00:00:00");
                    }
                    if (Request.QueryString["date2"] != null)
                    {
                        strDate2 = Request.QueryString["date2"];
                        strDate2 = Convert.ToDateTime(strDate2).ToString("yyyy/MM/dd 23:59:59");
                    }
                    switch (strSurvey)
                    {
                        case "2":       // 工作適能
                            Export_WIndex_Data(strDate1, strDate2);
                            break;
                        case "3":       // 個人生活狀況
                            Export_Life_Data(strDate1, strDate2);
                            break;
                        case "4":       // 健康狀況
                            Export_Health_Data(strDate1, strDate2);
                            break;
                        case "5":       // 工作環境危害因子
                            Export_Place_Data(strDate1, strDate2);
                            break;
                        case "6":       // 人因危害因子
                            Export_Human_Data(strDate1, strDate2);
                            break;
                        case "7":       // 自覺肌肉骨骼症狀
                            Export_Body_Data(strDate1, strDate2);
                            break;
                        case "8":       // 工作壓力檢測
                            Export_Pressure_Data(strDate1, strDate2);
                            break;
                        case "9":       // 肌肉骨骼健康評估
                            Export_Body_Data_Ex(strDate1, strDate2);
                            break;
                        default:
                            ShareFunction.Register_Close_Script(this.Page);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 工作適能指數資料匯出
        /// </summary>
        /// <param name="strDate1"></param>
        /// <param name="strDate2"></param>
        private void Export_WIndex_Data(string strDate1, string strDate2)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable dtQuestion, dtPaper, myData;
            string strResult, strSplit, strData;
            string strFilter;
            int i, j;
            long intLastPaper;
            string[] aryPaper;
            DataRow[] myRow;

            try
            {
                // 取出題目
                strSQL = "Select QuestionID, QTitle, Row_Number() Over(Order By SurveyID, PageNo, OrderNo) as RNO "
                        + "From Question "
                        + "Where SurveyID In (0, 2) And Deleted='N' And CategoryID>0  "
                        + "Order By SurveyID, PageNo, OrderNo";
                dtQuestion = DB.GetDataTable(strSQL);

                if (dtQuestion == null)
                {
                    ShareFunction.PutLog("Export_WIndex_Data", "沒有資料：" + DB.DBErrorMessage);
                    return;
                }

                strFilter = "";
                if (strDate1 != "")
                {
                    // 有起始日期
                    if (strDate2 != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (strDate2 != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                strSQL = "Select P.PaperID, D.QuestionID, D.SelectOption "
                        + "From vw_WIndex_Paper_List P Join PaperDetail D On P.PaperID = D.PaperID "
                        + "     Join Question Q On D.QuestionID=Q.QuestionID "
                        + "Where Q.CategoryID>0 ";
                if (strFilter != "")
                {
                    strSQL += "And " + strFilter + " ";
                }
                strSQL += "Order By PaperID, QuestionID";

                if (strDate1 != "")
                {
                    DB.AddSqlParameter("@Date1", strDate1);
                }
                if (strDate2 != "")
                {
                    DB.AddSqlParameter("@Date2", strDate2);
                }

                dtPaper = DB.GetDataTable(strSQL);
                if (dtPaper == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "很抱歉，系統發生錯誤，請洽系統維護人員。");
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "您所查詢的日期區間沒有符合的資料。");
                    }
                    return;
                }

                strResult = "";
                strSplit = "";
                // 輸出標題列
                for (i = 0; i < dtQuestion.Rows.Count; i++)
                {
                    strResult += strSplit + dtQuestion.Rows[i]["QTitle"].ToString().Trim();
                    strSplit = ",";
                }
                //strResult += ",工作適能指數,工作適能等級,開始時間,完成時間" + Environment.NewLine;
                strResult += ",工作適能指數,工作適能等級,開始時間,完成時間,問卷編號" + Environment.NewLine;

                // 多五個用來儲存工作適能指數與等級、開始與完成時間、問卷編號
                aryPaper = new string[dtQuestion.Rows.Count + 5];

                // 輸出問卷內容
                intLastPaper = 0;
                strSplit = "";
                for (i = 0; i < dtPaper.Rows.Count; i++)
                {
                    if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString()) != intLastPaper)
                    {
                        if (intLastPaper != 0)
                        {
                            // 計算工作適能指數
                            strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                            DB.AddSqlParameter("@PaperID", intLastPaper);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count()-5] = strData;

                            // 取得工作適能等級及時間
                            strSQL = "Select CreateTime, FinishTime, ResultValue From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastPaper);
                            myData = DB.GetDataTable(strSQL);
                            if (myData!=null)
                            {
                                aryPaper[aryPaper.Count() - 4] = myData.Rows[0]["ResultValue"].ToString();
                                aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            }

                            // 儲存PaperID
                            aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                            // 將資料暫存陣列輸出
                            strSplit = "";
                            for (j = 0; j < aryPaper.Count(); j++)
                            {
                                strResult += strSplit + aryPaper[j];
                                strSplit = ",";
                            }
                            strResult += Environment.NewLine;
                        }

                        // 清空資料暫存陣列內容
                        for (j = 0; j < aryPaper.Count(); j++)
                        {
                            aryPaper[j] = "";
                        }
                    }       // if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString())!=intLastPaper)

                    intLastPaper = Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString());

                    // 搜尋問題資料表，找出欄位索引值
                    myRow = dtQuestion.Select("QuestionID=" + dtPaper.Rows[i]["QuestionID"].ToString());
                    if (myRow == null)
                    {
                        ShareFunction.PutLog("Export_WIndex_Data", "找不到問題編號" + dtPaper.Rows[i]["QuestionID"].ToString() + "的資料");
                    }
                    else
                    {
                        aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = dtPaper.Rows[i]["SelectOption"].ToString();
                    }
                }       // for i

                // 計算工作適能指數
                strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                DB.AddSqlParameter("@PaperID", intLastPaper);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 5] = strData;

                // 取得工作適能等級及時間
                strSQL = "Select CreateTime, FinishTime, ResultValue From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastPaper);
                myData = DB.GetDataTable(strSQL);
                if (myData != null)
                {
                    aryPaper[aryPaper.Count() - 4] = myData.Rows[0]["ResultValue"].ToString();
                    aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                }

                // 儲存PaperID
                aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                // 將資料暫存陣列輸出
                strSplit = "";
                for (j = 0; j < aryPaper.Count(); j++)
                {
                    strResult += strSplit + aryPaper[j];
                    strSplit = ",";
                }
                strResult += Environment.NewLine;

                Data_Output(strResult);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Export_WIndex_Data", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 個人生活狀況資料匯出
        /// </summary>
        /// <param name="strDate1"></param>
        /// <param name="strDate2"></param>
        private void Export_Life_Data(string strDate1, string strDate2)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable dtQuestion, dtPaper, myData;
            string strResult, strSplit, strData;
            string strFilter;
            int i, j;
            long intLastPaper, intLastParent;
            string[] aryPaper;
            DataRow[] myRow;

            try
            {
                // 取出題目
                strSQL = "Select QuestionID, QTitle, Row_Number() Over(Order By SurveyID, PageNo, OrderNo) as RNO "
                        + "From Question "
                        + "Where SurveyID In (0, 3) And Deleted='N' And CategoryID>0  "
                        + "Order By SurveyID, PageNo, OrderNo";
                dtQuestion = DB.GetDataTable(strSQL);

                if (dtQuestion == null)
                {
                    ShareFunction.PutLog("Export_Life_Data", "沒有資料：" + DB.DBErrorMessage);
                    return;
                }

                strFilter = "";
                if (strDate1 != "")
                {
                    // 有起始日期
                    if (strDate2 != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (strDate2 != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                strSQL = "Select P.PaperID, D.QuestionID, D.SelectOption, P.ParentPaper "
                        + "From vw_Paper_Relation P Join PaperDetail D On P.PaperID = D.PaperID "
                        + "     Join Question Q On D.QuestionID=Q.QuestionID "
                        + "Where P.SurveyID=3 And Q.CategoryID>0 ";
                if (strFilter != "")
                {
                    strSQL += "And " + strFilter + " ";
                }
                strSQL += "Order By PaperID, QuestionID";

                if (strDate1 != "")
                {
                    DB.AddSqlParameter("@Date1", strDate1);
                }
                if (strDate2 != "")
                {
                    DB.AddSqlParameter("@Date2", strDate2);
                }

                dtPaper = DB.GetDataTable(strSQL);
                if (dtPaper == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "很抱歉，系統發生錯誤，請洽系統維護人員。");
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "您所查詢的日期區間沒有符合的資料。");
                    }
                    return;
                }

                strResult = "";
                strSplit = "";
                // 輸出標題列
                for (i = 0; i < dtQuestion.Rows.Count; i++)
                {
                    strResult += strSplit + dtQuestion.Rows[i]["QTitle"].ToString().Trim();
                    strSplit = ",";
                }
                strResult += ",工作適能指數,工作適能等級,開始時間,完成時間,問卷編號" + Environment.NewLine;

                // 多四個用來儲存工作適能指數與等級、開始與完成時間、問卷編號            
                aryPaper = new string[dtQuestion.Rows.Count + 5];

                // 輸出問卷內容
                intLastPaper = 0;
                intLastParent = 0;
                strSplit = "";
                for (i = 0; i < dtPaper.Rows.Count; i++)
                {
                    if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString()) != intLastPaper)
                    {
                        if (intLastPaper != 0)
                        {
                            // 計算工作適能指數
                            strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 5] = strData;

                            // 取得工作適能等級
                            strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 4] = strData;

                            // 取得填寫時間
                            strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastPaper);
                            myData = DB.GetDataTable(strSQL);
                            if (myData != null)
                            {
                                aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            }

                            // 儲存PaperID
                            aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                            // 將資料暫存陣列輸出
                            strSplit = "";
                            for (j = 0; j < aryPaper.Count(); j++)
                            {
                                strResult += strSplit + aryPaper[j];
                                strSplit = ",";
                            }
                            strResult += Environment.NewLine;
                        }

                        // 清空資料暫存陣列內容
                        for (j = 0; j < aryPaper.Count(); j++)
                        {
                            aryPaper[j] = "";
                        }
                    }       // if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString())!=intLastPaper)

                    intLastPaper = Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString());
                    intLastParent = Convert.ToInt64(dtPaper.Rows[i]["ParentPaper"].ToString());

                    // 搜尋問題資料表，找出欄位索引值
                    myRow = dtQuestion.Select("QuestionID=" + dtPaper.Rows[i]["QuestionID"].ToString());
                    if (myRow == null)
                    {
                        ShareFunction.PutLog("Export_Life_Data", "找不到問題編號" + dtPaper.Rows[i]["QuestionID"].ToString() + "的資料");
                    }
                    else
                    {
                        aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = dtPaper.Rows[i]["SelectOption"].ToString();
                    }
                }       // for i

                // 計算工作適能指數
                strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 5] = strData;

                // 取得工作適能等級
                strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 4] = strData;

                // 取得填寫時間
                strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastPaper);
                myData = DB.GetDataTable(strSQL);
                if (myData != null)
                {
                    aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                }

                // 儲存PaperID
                aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                // 將資料暫存陣列輸出
                strSplit = "";
                for (j = 0; j < aryPaper.Count(); j++)
                {
                    strResult += strSplit + aryPaper[j];
                    strSplit = ",";
                }
                strResult += Environment.NewLine;

                Data_Output(strResult);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Export_Life_Data", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 工作環境危害因子資料匯出
        /// </summary>
        /// <param name="strDate1"></param>
        /// <param name="strDate2"></param>
        private void Export_Place_Data(string strDate1, string strDate2)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable dtQuestion, dtPaper, myData;
            string strResult, strSplit, strData;
            string strFilter;
            int i, j;
            long intLastPaper, intLastParent;
            string[] aryPaper;
            DataRow[] myRow;

            try
            {
                // 取出題目
                strSQL = "Select QuestionID, OptionID, QTitle, ROW_NUMBER() Over (Order By OrderNo) as RNO "
                        + "From "
                        + "(Select QuestionID, OptionID, SelectOption as QTitle, SortID as OrderNo From OptionData Where QuestionID=186 And Disabled='N' "
                        + "Union "
                        + "Select QuestionID, 0 as OptionID, QTitle, OrderNo + 100 as OrderNo From Question Where SurveyID = 5 And Deleted = 'N' And QType In(11, 12)) A ";
                dtQuestion = DB.GetDataTable(strSQL);

                if (dtQuestion == null)
                {
                    ShareFunction.PutLog("Export_Place_Data", "沒有資料：" + DB.DBErrorMessage);
                    return;
                }

                strFilter = "";
                if (strDate1 != "")
                {
                    // 有起始日期
                    if (strDate2 != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (strDate2 != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                strSQL = "Select P.PaperID, D.QuestionID, D.SelectOption, D.SelectValue, P.ParentPaper "
                        + "From vw_Paper_Relation P Join PlaceData D On P.PaperID = D.PaperID "
                        + "     Join Question Q On D.QuestionID=Q.QuestionID "
                        + "Where P.SurveyID=5 And Q.CategoryID>0 ";
                if (strFilter != "")
                {
                    strSQL += "And " + strFilter + " ";
                }
                strSQL += "Order By P.PaperID, D.QuestionID";

                if (strDate1 != "")
                {
                    DB.AddSqlParameter("@Date1", strDate1);
                }
                if (strDate2 != "")
                {
                    DB.AddSqlParameter("@Date2", strDate2);
                }

                dtPaper = DB.GetDataTable(strSQL);
                if (dtPaper == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "很抱歉，系統發生錯誤，請洽系統維護人員。");
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "您所查詢的日期區間沒有符合的資料。");
                    }
                    return;
                }

                strResult = "";
                strSplit = "";
                // 輸出標題列
                for (i = 0; i < dtQuestion.Rows.Count; i++)
                {
                    strResult += strSplit + dtQuestion.Rows[i]["QTitle"].ToString().Trim();
                    strSplit = ",";
                }
                strResult += ",工作適能指數,工作適能等級,開始時間,完成時間,問卷編號" + Environment.NewLine;

                // 多四個用來儲存工作適能指數與等級、開始與完成時間、問卷編號            
                aryPaper = new string[dtQuestion.Rows.Count + 5];

                // 輸出問卷內容
                intLastPaper = 0;
                intLastParent = 0;
                strSplit = "";
                for (i = 0; i < dtPaper.Rows.Count; i++)
                {
                    if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString()) != intLastPaper)
                    {
                        if (intLastPaper != 0)
                        {
                            // 計算工作適能指數
                            strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 5] = strData;

                            // 取得工作適能等級
                            strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 4] = strData;

                            // 取得填寫時間
                            strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastPaper);
                            myData = DB.GetDataTable(strSQL);
                            if (myData != null)
                            {
                                aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            }

                            // 儲存PaperID
                            aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                            // 將資料暫存陣列輸出
                            strSplit = "";
                            for (j = 0; j < aryPaper.Count(); j++)
                            {
                                strResult += strSplit + aryPaper[j];
                                strSplit = ",";
                            }
                            strResult += Environment.NewLine;
                        }

                        // 清空資料暫存陣列內容
                        for (j = 0; j < aryPaper.Count(); j++)
                        {
                            aryPaper[j] = "";
                        }
                    }       // if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString())!=intLastPaper)

                    intLastPaper = Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString());
                    intLastParent = Convert.ToInt64(dtPaper.Rows[i]["ParentPaper"].ToString());

                    // 搜尋問題資料表，找出欄位索引值
                    if (dtPaper.Rows[i]["QuestionID"].ToString()=="186")
                    {
                        // 多選題，所以要增加判斷選項ID
                        myRow = dtQuestion.Select("QuestionID=" + dtPaper.Rows[i]["QuestionID"].ToString() + " And OptionID=" + dtPaper.Rows[i]["SelectValue"].ToString());
                        strData = "1";
                    }
                    else
                    {
                        myRow = dtQuestion.Select("QuestionID=" + dtPaper.Rows[i]["QuestionID"].ToString());
                        strData = dtPaper.Rows[i]["SelectOption"].ToString();
                    }
                    if (myRow == null)
                    {
                        ShareFunction.PutLog("Export_Life_Data", "找不到問題編號" + dtPaper.Rows[i]["QuestionID"].ToString() + "的資料");
                    }
                    else
                    {
                        //aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = dtPaper.Rows[i]["SelectOption"].ToString();
                        aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = strData;
                    }
                }       // for i

                // 計算工作適能指數
                strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 5] = strData;

                // 取得工作適能等級
                strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 4] = strData;

                // 取得填寫時間
                strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastPaper);
                myData = DB.GetDataTable(strSQL);
                if (myData != null)
                {
                    aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                }

                // 儲存PaperID
                aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                // 將資料暫存陣列輸出
                strSplit = "";
                for (j = 0; j < aryPaper.Count(); j++)
                {
                    strResult += strSplit + aryPaper[j];
                    strSplit = ",";
                }
                strResult += Environment.NewLine;

                Data_Output(strResult);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Export_Place_Data", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 健康狀況資料匯出
        /// </summary>
        /// <param name="strDate1"></param>
        /// <param name="strDate2"></param>
        private void Export_Health_Data(string strDate1, string strDate2)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable dtQuestion, dtPaper, myData;
            string strResult, strSplit, strData;
            string strFilter;
            int i, j;
            long intLastPaper, intLastParent;
            string[] aryPaper;
            DataRow[] myRow;

            try
            {
                // 取出題目
                strSQL = "Select QuestionID, LineID, QTitle, ROW_NUMBER() Over (Order By OrderNo, LineID) as RNO "
                        + "From "
                        + "(Select QuestionID,1 as LineID, QTitle, OrderNo From Question Where SurveyID = 4 And Deleted = 'N' And CategoryID=2 "
                        + "Union "
                        + "Select QuestionID,2 as LineID, N'與工作可能有關' as QTitle, OrderNo From Question Where SurveyID = 4 And Deleted = 'N' And CategoryID=2) A ";
                dtQuestion = DB.GetDataTable(strSQL);

                if (dtQuestion == null)
                {
                    ShareFunction.PutLog("Export_Health_Data", "沒有資料：" + DB.DBErrorMessage);
                    return;
                }

                strFilter = "";
                if (strDate1 != "")
                {
                    // 有起始日期
                    if (strDate2 != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (strDate2 != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                strSQL = "Select P.PaperID, D.QuestionID, D.LineID, D.SelectOption, D.SelectValue, P.ParentPaper "
                        + "From vw_Paper_Relation P Join PaperDetail D On P.PaperID = D.PaperID  "
                        + "     Join Question Q On D.QuestionID=Q.QuestionID "
                        + "Where P.SurveyID=4 And Q.CategoryID=2 ";
                if (strFilter != "")
                {
                    strSQL += "And " + strFilter + " ";
                }
                strSQL += "Order By P.PaperID, D.QuestionID";

                if (strDate1 != "")
                {
                    DB.AddSqlParameter("@Date1", strDate1);
                }
                if (strDate2 != "")
                {
                    DB.AddSqlParameter("@Date2", strDate2);
                }

                dtPaper = DB.GetDataTable(strSQL);
                if (dtPaper == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "很抱歉，系統發生錯誤，請洽系統維護人員。");
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "您所查詢的日期區間沒有符合的資料。");
                    }
                    return;
                }

                strResult = "";
                strSplit = "";
                // 輸出標題列
                for (i = 0; i < dtQuestion.Rows.Count; i++)
                {
                    strResult += strSplit + dtQuestion.Rows[i]["QTitle"].ToString().Trim();
                    strSplit = ",";
                }
                strResult += ",工作適能指數,工作適能等級,開始時間,完成時間,問卷編號" + Environment.NewLine;

                // 多四個用來儲存工作適能指數與等級、開始與完成時間、問卷編號            
                aryPaper = new string[dtQuestion.Rows.Count + 5];

                // 輸出問卷內容
                intLastPaper = 0;
                intLastParent = 0;
                strSplit = "";
                for (i = 0; i < dtPaper.Rows.Count; i++)
                {
                    if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString()) != intLastPaper)
                    {
                        if (intLastPaper != 0)
                        {
                            // 計算工作適能指數
                            strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 5] = strData;

                            // 取得工作適能等級
                            strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 4] = strData;

                            // 取得填寫時間
                            strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastPaper);
                            myData = DB.GetDataTable(strSQL);
                            if (myData != null)
                            {
                                aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            }

                            // 儲存PaperID
                            aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                            // 將資料暫存陣列輸出
                            strSplit = "";
                            for (j = 0; j < aryPaper.Count(); j++)
                            {
                                strResult += strSplit + aryPaper[j];
                                strSplit = ",";
                            }
                            strResult += Environment.NewLine;
                        }

                        // 清空資料暫存陣列內容
                        for (j = 0; j < aryPaper.Count(); j++)
                        {
                            aryPaper[j] = "";
                        }
                    }       // if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString())!=intLastPaper)

                    intLastPaper = Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString());
                    intLastParent = Convert.ToInt64(dtPaper.Rows[i]["ParentPaper"].ToString());

                    // 搜尋問題資料表，找出欄位索引值
                    myRow = dtQuestion.Select("QuestionID=" + dtPaper.Rows[i]["QuestionID"].ToString() + " And LineID=" + dtPaper.Rows[i]["LineID"].ToString());

                    if (dtPaper.Rows[i]["LineID"].ToString() == "2")
                    {
                        // 多選題，所以要增加判斷選項ID
                        strData = (Convert.ToInt32(dtPaper.Rows[i]["LineID"].ToString())-1).ToString();
                    }
                    else
                    {
                        strData = dtPaper.Rows[i]["SelectOption"].ToString();
                    }
                    if (myRow == null)
                    {
                        ShareFunction.PutLog("Export_Health_Data", "找不到問題編號" + dtPaper.Rows[i]["QuestionID"].ToString() + "的資料");
                    }
                    else
                    {
                        //aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = dtPaper.Rows[i]["SelectOption"].ToString();
                        aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = strData;
                    }
                }       // for i

                // 計算工作適能指數
                strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 5] = strData;

                // 取得工作適能等級
                strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 4] = strData;

                // 取得填寫時間
                strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastPaper);
                myData = DB.GetDataTable(strSQL);
                if (myData != null)
                {
                    aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                }

                // 儲存PaperID
                aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                // 將資料暫存陣列輸出
                strSplit = "";
                for (j = 0; j < aryPaper.Count(); j++)
                {
                    strResult += strSplit + aryPaper[j];
                    strSplit = ",";
                }
                strResult += Environment.NewLine;

                Data_Output(strResult);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Export_Health_Data", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 人因危害因子資料匯出
        /// </summary>
        /// <param name="strDate1"></param>
        /// <param name="strDate2"></param>
        private void Export_Human_Data(string strDate1, string strDate2)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable dtQuestion, dtPaper, myData;
            string strResult, strSplit, strData;
            string strFilter;
            int i, j;
            long intLastPaper, intLastParent;
            string[] aryPaper;
            DataRow[] myRow;

            try
            {
                // 取出題目
                strSQL = "Select QuestionID, SelectValue, QTitle, ROW_NUMBER() Over (Order By QuestionID, SelectValue) as RNO "
                        + "From "
                        + "(Select QuestionID, QTitle, '1' as SelectValue, OrderNo "
                        + " From Question "
                        + " Where SurveyID = 6 And Deleted = 'N' And CategoryID=2 And IsSubQuestion='N' "
                        + "Union "
                        + "Select QuestionID, SelectOption as QTitle, SelectValue, SortID as OrderNo "
                        + "From OptionData "
                        + "Where QuestionID In (Select QuestionID "
                        + "                     From Question "
                        + "                     Where SurveyID=6 And Deleted='N' And CategoryID=2 And IsSubQuestion='Y') And SelectValue<>'0') A ";
                dtQuestion = DB.GetDataTable(strSQL);

                if (dtQuestion == null)
                {
                    ShareFunction.PutLog("Export_Human_Data", "沒有資料：" + DB.DBErrorMessage);
                    return;
                }

                strFilter = "";
                if (strDate1 != "")
                {
                    // 有起始日期
                    if (strDate2 != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (strDate2 != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                strSQL = "Select P.PaperID, D.QuestionID, D.QTitle, D.SelectOption, D.SelectValue, P.ParentPaper  "
                        + "From vw_Paper_Relation P Join HumanData D On P.PaperID = D.PaperID "
                        + "     Join Question Q On D.QuestionID=Q.QuestionID "
                        + "Where P.SurveyID=6 And Q.CategoryID=2 And D.SelectValue<>'0' ";
                if (strFilter != "")
                {
                    strSQL += "And " + strFilter + " ";
                }
                strSQL += "Order By P.PaperID, D.QuestionID";

                if (strDate1 != "")
                {
                    DB.AddSqlParameter("@Date1", strDate1);
                }
                if (strDate2 != "")
                {
                    DB.AddSqlParameter("@Date2", strDate2);
                }

                dtPaper = DB.GetDataTable(strSQL);
                if (dtPaper == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "很抱歉，系統發生錯誤，請洽系統維護人員。");
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "您所查詢的日期區間沒有符合的資料。");
                    }
                    return;
                }

                strResult = "";
                strSplit = "";
                // 輸出標題列
                for (i = 0; i < dtQuestion.Rows.Count; i++)
                {
                    strResult += strSplit + dtQuestion.Rows[i]["QTitle"].ToString().Trim();
                    strSplit = ",";
                }
                strResult += ",工作適能指數,工作適能等級,開始時間,完成時間,問卷編號" + Environment.NewLine;

                // 多四個用來儲存工作適能指數與等級、開始與完成時間、問卷編號            
                aryPaper = new string[dtQuestion.Rows.Count + 5];

                // 輸出問卷內容
                intLastPaper = 0;
                intLastParent = 0;
                strSplit = "";
                for (i = 0; i < dtPaper.Rows.Count; i++)
                {
                    if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString()) != intLastPaper)
                    {
                        if (intLastPaper != 0)
                        {
                            // 計算工作適能指數
                            strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 5] = strData;

                            // 取得工作適能等級
                            strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 4] = strData;

                            // 取得填寫時間
                            strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastPaper);
                            myData = DB.GetDataTable(strSQL);
                            if (myData != null)
                            {
                                aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            }

                            // 儲存PaperID
                            aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                            // 將資料暫存陣列輸出
                            strSplit = "";
                            for (j = 0; j < aryPaper.Count(); j++)
                            {
                                strResult += strSplit + aryPaper[j];
                                strSplit = ",";
                            }
                            strResult += Environment.NewLine;
                        }

                        // 清空資料暫存陣列內容
                        for (j = 0; j < aryPaper.Count(); j++)
                        {
                            aryPaper[j] = "";
                        }
                    }       // if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString())!=intLastPaper)

                    intLastPaper = Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString());
                    intLastParent = Convert.ToInt64(dtPaper.Rows[i]["ParentPaper"].ToString());

                    // 搜尋問題資料表，找出欄位索引值
                    myRow = dtQuestion.Select("QuestionID=" + dtPaper.Rows[i]["QuestionID"].ToString() + " And SelectValue='" + dtPaper.Rows[i]["SelectValue"].ToString() + "'");

                    // 一律儲存1
                    //if (dtPaper.Rows[i]["LineID"].ToString() == "2")
                    //{
                    //    // 多選題，所以要增加判斷選項ID
                    //    strData = (Convert.ToInt32(dtPaper.Rows[i]["LineID"].ToString()) - 1).ToString();
                    //}
                    //else
                    //{
                    //    strData = dtPaper.Rows[i]["SelectOption"].ToString();
                    //}
                    if (myRow == null)
                    {
                        ShareFunction.PutLog("Export_Human_Data", "找不到問題編號" + dtPaper.Rows[i]["QuestionID"].ToString() + "的資料");
                    }
                    else
                    {
                        aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = "1";
                    }
                }       // for i

                // 計算工作適能指數
                strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 5] = strData;

                // 取得工作適能等級
                strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 4] = strData;

                // 取得填寫時間
                strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastPaper);
                myData = DB.GetDataTable(strSQL);
                if (myData != null)
                {
                    aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                }

                // 儲存PaperID
                aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                // 將資料暫存陣列輸出
                strSplit = "";
                for (j = 0; j < aryPaper.Count(); j++)
                {
                    strResult += strSplit + aryPaper[j];
                    strSplit = ",";
                }
                strResult += Environment.NewLine;

                Data_Output(strResult);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Export_Humnan_Data", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 肌肉骨骼症狀資料匯出
        /// </summary>
        /// <param name="strDate1"></param>
        /// <param name="strDate2"></param>
        private void Export_Body_Data(string strDate1, string strDate2)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable dtQuestion, dtPaper, myData;
            string strResult, strSplit, strData;
            string strFilter;
            int i, j;
            long intLastPaper, intLastParent;
            string[] aryPaper;
            DataRow[] myRow;

            try
            {
                // 取出題目
                strSQL = "Select QuestionID, QTitle, ROW_NUMBER() Over (Order By PageNo, OrderNo) as RNO "
                        + "From Question "
                        + "Where SurveyID=7 And Deleted='N' "
                        + "Order By PageNo, OrderNo ";
                dtQuestion = DB.GetDataTable(strSQL);

                if (dtQuestion == null)
                {
                    ShareFunction.PutLog("Export_Body_Data", "沒有資料：" + DB.DBErrorMessage);
                    return;
                }

                strFilter = "";
                if (strDate1 != "")
                {
                    // 有起始日期
                    if (strDate2 != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (strDate2 != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                strSQL = "Select P.PaperID, D.QuestionID, D.QTitle, D.SelectOption, D.SelectValue, P.ParentPaper  "
                        + "From vw_Paper_Relation P Join BodyData D On P.PaperID = D.PaperID "
                        + "     Join Question Q On D.QuestionID=Q.QuestionID "
                        + "Where P.SurveyID=7 And Q.CategoryID=2 And Q.Deleted='N' ";
                if (strFilter != "")
                {
                    strSQL += "And " + strFilter + " ";
                }
                strSQL += "Order By P.PaperID, D.QuestionID";

                if (strDate1 != "")
                {
                    DB.AddSqlParameter("@Date1", strDate1);
                }
                if (strDate2 != "")
                {
                    DB.AddSqlParameter("@Date2", strDate2);
                }

                dtPaper = DB.GetDataTable(strSQL);
                if (dtPaper == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "很抱歉，系統發生錯誤，請洽系統維護人員。");
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "您所查詢的日期區間沒有符合的資料。");
                    }
                    return;
                }

                strResult = "";
                strSplit = "";
                // 輸出標題列
                for (i = 0; i < dtQuestion.Rows.Count; i++)
                {
                    strResult += strSplit + dtQuestion.Rows[i]["QTitle"].ToString().Trim();
                    strSplit = ",";
                }
                strResult += ",工作適能指數,工作適能等級,開始時間,完成時間,問卷編號" + Environment.NewLine;

                // 多四個用來儲存工作適能指數與等級、開始與完成時間、問卷編號            
                aryPaper = new string[dtQuestion.Rows.Count + 5];

                // 輸出問卷內容
                intLastPaper = 0;
                intLastParent = 0;
                strSplit = "";
                for (i = 0; i < dtPaper.Rows.Count; i++)
                {
                    if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString()) != intLastPaper)
                    {
                        if (intLastPaper != 0)
                        {
                            // 計算工作適能指數
                            strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 5] = strData;

                            // 取得工作適能等級
                            strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 4] = strData;

                            // 取得填寫時間
                            strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastPaper);
                            myData = DB.GetDataTable(strSQL);
                            if (myData != null)
                            {
                                aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            }

                            // 儲存PaperID
                            aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                            // 將資料暫存陣列輸出
                            strSplit = "";
                            for (j = 0; j < aryPaper.Count(); j++)
                            {
                                strResult += strSplit + aryPaper[j];
                                strSplit = ",";
                            }
                            strResult += Environment.NewLine;
                        }

                        // 清空資料暫存陣列內容
                        for (j = 0; j < aryPaper.Count(); j++)
                        {
                            aryPaper[j] = "";
                        }
                    }       // if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString())!=intLastPaper)

                    intLastPaper = Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString());
                    intLastParent = Convert.ToInt64(dtPaper.Rows[i]["ParentPaper"].ToString());

                    // 搜尋問題資料表，找出欄位索引值
                    myRow = dtQuestion.Select("QuestionID=" + dtPaper.Rows[i]["QuestionID"].ToString());

                    if (myRow == null)
                    {
                        ShareFunction.PutLog("Export_Body_Data", "找不到問題編號" + dtPaper.Rows[i]["QuestionID"].ToString() + "的資料");
                    }
                    else
                    {
                        aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = dtPaper.Rows[i]["SelectValue"].ToString();
                    }
                }       // for i

                // 計算工作適能指數
                strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 5] = strData;

                // 取得工作適能等級
                strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 4] = strData;

                // 取得填寫時間
                strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastPaper);
                myData = DB.GetDataTable(strSQL);
                if (myData != null)
                {
                    aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                }

                // 儲存PaperID
                aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                // 將資料暫存陣列輸出
                strSplit = "";
                for (j = 0; j < aryPaper.Count(); j++)
                {
                    strResult += strSplit + aryPaper[j];
                    strSplit = ",";
                }
                strResult += Environment.NewLine;

                Data_Output(strResult);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Export_Body_Data", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        /// <summary>
        /// 工作壓力檢測資料匯出
        /// </summary>
        /// <param name="strDate1"></param>
        /// <param name="strDate2"></param>
        private void Export_Pressure_Data(string strDate1, string strDate2)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable dtQuestion, dtPaper, myData;
            string strResult, strSplit, strData;
            string strFilter;
            int i, j;
            long intLastPaper, intLastParent;
            string[] aryPaper;
            DataRow[] myRow;

            try
            {
                // 取出題目
                strSQL = "Select QuestionID, QTitle, ROW_NUMBER() Over (Order By OrderNo) as RNO "
                        + "From Question "
                        + "Where SurveyID=8 And Deleted='N' "
                        + "Order By OrderNo ";
                dtQuestion = DB.GetDataTable(strSQL);

                if (dtQuestion == null)
                {
                    ShareFunction.PutLog("Export_Pressure_Data", "沒有資料：" + DB.DBErrorMessage);
                    return;
                }

                strFilter = "";
                if (strDate1 != "")
                {
                    // 有起始日期
                    if (strDate2 != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (strDate2 != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                strSQL = "Select P.PaperID, D.QuestionID, D.QTitle, D.SelectOption, D.SelectValue, P.ParentPaper  "
                        + "From vw_Paper_Relation P Join PaperDetail D On P.PaperID = D.PaperID "
                        + "     Join Question Q On D.QuestionID=Q.QuestionID "
                        + "Where P.SurveyID=8 And Q.CategoryID=2 And Q.Deleted='N' ";
                if (strFilter != "")
                {
                    strSQL += "And " + strFilter + " ";
                }
                strSQL += "Order By P.PaperID, D.QuestionID";

                if (strDate1 != "")
                {
                    DB.AddSqlParameter("@Date1", strDate1);
                }
                if (strDate2 != "")
                {
                    DB.AddSqlParameter("@Date2", strDate2);
                }

                dtPaper = DB.GetDataTable(strSQL);
                if (dtPaper == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "很抱歉，系統發生錯誤，請洽系統維護人員。");
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "您所查詢的日期區間沒有符合的資料。");
                    }
                    return;
                }

                strResult = "";
                strSplit = "";
                // 輸出標題列
                for (i = 0; i < dtQuestion.Rows.Count; i++)
                {
                    strResult += strSplit + dtQuestion.Rows[i]["QTitle"].ToString().Trim();
                    strSplit = ",";
                }
                strResult += ",工作適能指數,工作適能等級,開始時間,完成時間,問卷編號" + Environment.NewLine;

                // 多四個用來儲存工作適能指數與等級、開始與完成時間、問卷編號            
                aryPaper = new string[dtQuestion.Rows.Count + 5];

                // 輸出問卷內容
                intLastPaper = 0;
                intLastParent = 0;
                strSplit = "";
                for (i = 0; i < dtPaper.Rows.Count; i++)
                {
                    if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString()) != intLastPaper)
                    {
                        if (intLastPaper != 0)
                        {
                            // 計算工作適能指數
                            strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 5] = strData;

                            // 取得工作適能等級
                            strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 4] = strData;

                            // 取得填寫時間
                            strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastPaper);
                            myData = DB.GetDataTable(strSQL);
                            if (myData != null)
                            {
                                aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            }

                            // 儲存PaperID
                            aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                            // 將資料暫存陣列輸出
                            strSplit = "";
                            for (j = 0; j < aryPaper.Count(); j++)
                            {
                                strResult += strSplit + aryPaper[j];
                                strSplit = ",";
                            }
                            strResult += Environment.NewLine;
                        }

                        // 清空資料暫存陣列內容
                        for (j = 0; j < aryPaper.Count(); j++)
                        {
                            aryPaper[j] = "";
                        }
                    }       // if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString())!=intLastPaper)

                    intLastPaper = Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString());
                    intLastParent = Convert.ToInt64(dtPaper.Rows[i]["ParentPaper"].ToString());

                    // 搜尋問題資料表，找出欄位索引值
                    myRow = dtQuestion.Select("QuestionID=" + dtPaper.Rows[i]["QuestionID"].ToString());

                    if (myRow == null)
                    {
                        ShareFunction.PutLog("Export_Pressure_Data", "找不到問題編號" + dtPaper.Rows[i]["QuestionID"].ToString() + "的資料");
                    }
                    else
                    {
                        aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = dtPaper.Rows[i]["SelectOption"].ToString();
                    }
                }       // for i

                // 計算工作適能指數
                strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 5] = strData;

                // 取得工作適能等級
                strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 4] = strData;

                // 取得填寫時間
                strSQL = "Select CreateTime, FinishTime From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastPaper);
                myData = DB.GetDataTable(strSQL);
                if (myData != null)
                {
                    aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    aryPaper[aryPaper.Count() - 2] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                }

                // 儲存PaperID
                aryPaper[aryPaper.Count() - 1] = intLastPaper.ToString();

                // 將資料暫存陣列輸出
                strSplit = "";
                for (j = 0; j < aryPaper.Count(); j++)
                {
                    strResult += strSplit + aryPaper[j];
                    strSplit = ",";
                }
                strResult += Environment.NewLine;

                Data_Output(strResult);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Export_Pressure_Data", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        private void Export_Body_Data_Ex(string strDate1, string strDate2)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable dtQuestion, dtPaper, myData;
            string strResult, strSplit, strData;
            string strFilter;
            int i, j;
            long intLastPaper, intLastParent;
            string[] aryPaper;
            DataRow[] myRow;

            try
            {
                // 取出題目
                //strSQL = "Select QuestionID, QTitle, ROW_NUMBER() Over (Order By PageNo, OrderNo) as RNO "
                //        + "From Question "
                //        + "Where SurveyID=9 And Deleted='N' "
                //        + "Order By PageNo, OrderNo ";
                //  增加基本資料  2017/11/5
                strSQL = "Select QuestionID, QTitle, ROW_NUMBER() Over (Order By SurveyID, PageNo, OrderNo) as RNO "
                        + "From Question "
                        + "Where SurveyID In (0, 9) And Deleted='N' And QType>1 "
                        + "Order By SurveyID, PageNo, OrderNo ";
                dtQuestion = DB.GetDataTable(strSQL);

                if (dtQuestion == null)
                {
                    ShareFunction.PutLog("Export_Body_Data_Ex", "沒有資料：" + DB.DBErrorMessage);
                    return;
                }

                strFilter = "";
                if (strDate1 != "")
                {
                    // 有起始日期
                    if (strDate2 != "")
                    {
                        // 有起訖日期
                        strFilter = "CreateTime Between @Date1 And @Date2 ";
                    }
                    else
                    {
                        // 沒有截止日期
                        strFilter = "CreateTime>=@Date1 ";
                    }
                }
                else
                {
                    // 沒有起始日期
                    if (strDate2 != "")
                    {
                        // 有截止日期
                        strFilter = "CreateTime<@Date2 ";
                    }
                    else
                    {
                        // 沒有輸入日期條件
                        strFilter = "";
                        strDate1 = "";
                        strDate2 = "";
                    }
                }

                //strSQL = "Select P.PaperID, D.QuestionID, D.QTitle, D.SelectOption, D.SelectValue, P.ParentPaper  "
                //        + "From vw_Paper_Relation P Join BodyData D On P.PaperID = D.PaperID "
                //        + "     Join Question Q On D.QuestionID=Q.QuestionID "
                //        + "Where P.SurveyID=9 And Q.CategoryID=2 And Q.Deleted='N' ";
                strSQL = "Select P.PaperID, D.QuestionID, D.QTitle, D.SelectOption, D.SelectValue, P.ParentPaper "
                        + "From vw_Paper_Relation P Join PaperDetail D On P.PaperID = D.PaperID "
                        + "     Join Question Q On D.QuestionID=Q.QuestionID "
                        + "Where P.SurveyID=9 And Q.CategoryID=1 And Q.Deleted='N' "
                        + "Union "
                        + "Select P.PaperID, D.QuestionID, D.QTitle, D.SelectOption, D.SelectValue, P.ParentPaper "
                        + "From vw_Paper_Relation P Join BodyData D On P.PaperID = D.PaperID "
                        + "     Join Question Q On D.QuestionID=Q.QuestionID "
                        + "Where P.SurveyID=9 And Q.CategoryID=2 And Q.Deleted='N' ";
                if (strFilter != "")
                {
                    strSQL += "And " + strFilter + " ";
                }
                strSQL += "Order By P.PaperID, D.QuestionID";

                if (strDate1 != "")
                {
                    DB.AddSqlParameter("@Date1", strDate1);
                }
                if (strDate2 != "")
                {
                    DB.AddSqlParameter("@Date2", strDate2);
                }

                dtPaper = DB.GetDataTable(strSQL);
                if (dtPaper == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        Message.MsgShow(this.Page, "很抱歉，系統發生錯誤，請洽系統維護人員。");
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "您所查詢的日期區間沒有符合的資料。");
                    }
                    return;
                }

                strResult = "";
                strSplit = "";
                // 輸出標題列
                for (i = 0; i < dtQuestion.Rows.Count; i++)
                {
                    strResult += strSplit + dtQuestion.Rows[i]["QTitle"].ToString().Trim();
                    strSplit = ",";
                }
                strResult += ",工作適能指數,工作適能等級,開始時間,完成時間,問卷編號,評估報告序號" + Environment.NewLine;

                // 多六個用來儲存工作適能指數與等級、開始與完成時間、問卷編號、評估報告序號            
                aryPaper = new string[dtQuestion.Rows.Count + 6];

                // 輸出問卷內容
                intLastPaper = 0;
                intLastParent = 0;
                strSplit = "";
                for (i = 0; i < dtPaper.Rows.Count; i++)
                {
                    if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString()) != intLastPaper)
                    {
                        if (intLastPaper != 0)
                        {
                            // 計算工作適能指數
                            strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 6] = strData;

                            // 取得工作適能等級
                            strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                            DB.AddSqlParameter("@PaperID", intLastParent);
                            strData = DB.GetData(strSQL);
                            aryPaper[aryPaper.Count() - 5] = strData;

                            // 取得填寫時間
                            strSQL = "Select CreateTime, FinishTime, PaperSN From Paper Where PaperID=@PaperID And FinishTime Is Not NULL";
                            DB.AddSqlParameter("@PaperID", intLastPaper);
                            myData = DB.GetDataTable(strSQL);
                            if (myData != null)
                            {
                                aryPaper[aryPaper.Count() - 4] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                if (myData.Rows[0]["PaperSN"].ToString()!="")
                                {
                                    aryPaper[aryPaper.Count() - 1] = "'" + myData.Rows[0]["PaperSN"].ToString() + intLastPaper.ToString("00000");
                                }
                            }

                            // 儲存PaperID
                            aryPaper[aryPaper.Count() - 2] = intLastPaper.ToString();

                            // 將資料暫存陣列輸出
                            strSplit = "";
                            for (j = 0; j < aryPaper.Count(); j++)
                            {
                                strResult += strSplit + aryPaper[j];
                                strSplit = ",";
                            }
                            strResult += Environment.NewLine;
                        }

                        // 清空資料暫存陣列內容
                        for (j = 0; j < aryPaper.Count(); j++)
                        {
                            aryPaper[j] = "";
                        }
                    }       // if (Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString())!=intLastPaper)

                    intLastPaper = Convert.ToInt64(dtPaper.Rows[i]["PaperID"].ToString());
                    intLastParent = Convert.ToInt64(dtPaper.Rows[i]["ParentPaper"].ToString());

                    // 搜尋問題資料表，找出欄位索引值
                    myRow = dtQuestion.Select("QuestionID=" + dtPaper.Rows[i]["QuestionID"].ToString());

                    if (myRow.Length == 0)
                    {
                        ShareFunction.PutLog("Export_Body_Data_Ex", "找不到問題編號" + dtPaper.Rows[i]["QuestionID"].ToString() + "的資料");
                    }
                    else
                    {
                        //aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = dtPaper.Rows[i]["SelectValue"].ToString();
                        aryPaper[Convert.ToInt32(myRow[0]["RNO"].ToString()) - 1] = dtPaper.Rows[i]["SelectOption"].ToString();
                    }
                }       // for i

                // 計算工作適能指數
                strSQL = "Select Sum(Point) From PaperResult Where PaperID=@PaperID And DimensionID Between 2 And 8";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 6] = strData;

                // 取得工作適能等級
                strSQL = "Select ResultValue From Paper Where PaperID=@PaperID";
                DB.AddSqlParameter("@PaperID", intLastParent);
                strData = DB.GetData(strSQL);
                aryPaper[aryPaper.Count() - 5] = strData;

                // 取得填寫時間
                strSQL = "Select CreateTime, FinishTime, PaperSN From Paper Where PaperID=@PaperID And FinishTime Is Not NULL";
                DB.AddSqlParameter("@PaperID", intLastPaper);
                myData = DB.GetDataTable(strSQL);
                if (myData != null)
                {
                    aryPaper[aryPaper.Count() - 4] = Convert.ToDateTime(myData.Rows[0]["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    aryPaper[aryPaper.Count() - 3] = Convert.ToDateTime(myData.Rows[0]["FinishTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    if (myData.Rows[0]["PaperSN"].ToString() != "")
                    {
                        aryPaper[aryPaper.Count() - 1] = "'" + myData.Rows[0]["PaperSN"].ToString() + intLastPaper.ToString("00000");
                    }
                }

                // 儲存PaperID
                aryPaper[aryPaper.Count() - 2] = intLastPaper.ToString();

                // 將資料暫存陣列輸出
                strSplit = "";
                for (j = 0; j < aryPaper.Count(); j++)
                {
                    strResult += strSplit + aryPaper[j];
                    strSplit = ",";
                }
                strResult += Environment.NewLine;

                Data_Output(strResult);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Export_Body_Data_Ex", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        private void Data_Output(string strData)
        {
            Response.ClearHeaders();
            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";
            Response.Buffer = true;
            Response.Expires = 0;
            switch (strSurvey)
            {
                case "2":
                    Response.AppendHeader("Content-Disposition", "attachment;filename='" + HttpUtility.UrlEncode("工作適能指數資料", System.Text.Encoding.UTF8) + ".csv'");
                    break;
                case "3":
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("個人生活狀況資料", System.Text.Encoding.UTF8) + ".csv");
                    break;
                case "4":
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("健康狀況資料", System.Text.Encoding.UTF8) + ".csv");
                    break;
                case "5":
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("工作環境危害因子資料", System.Text.Encoding.UTF8) + ".csv");
                    break;
                case "6":
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("人因危害因子資料", System.Text.Encoding.UTF8) + ".csv");
                    break;
                case "7":
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("自覺肌肉骨骼症狀資料", System.Text.Encoding.UTF8) + ".csv");
                    break;
                case "8":
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("工作壓力檢測資料", System.Text.Encoding.UTF8) + ".csv");
                    break;
                case "9":
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("肌肉骨骼健康評估", System.Text.Encoding.UTF8) + ".csv");
                    break;
            }
            //Response.Write("<meta https-equiv=Content-Type content=application/vnd.ms-excel;charset=UTF-8>");
            //Response.Charset = "UTF-8";
            //Response.ContentEncoding = System.Text.Encoding.GetEncoding(65001);
            //Response.Write("<meta https-equiv=Content-Type content=application/vnd.ms-excel;charset=BIG5>");
            Response.Charset = "BIG5";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding(950);
            //UTF8Encoding encoder = new UTF8Encoding();
            //byte[] bytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(strData)); 
            //strData = Encoding.UTF8.GetString(bytes);
            Response.Write(strData);
            Response.End();
        }
    }
}