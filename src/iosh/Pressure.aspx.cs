using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Ferryman.DATA;
using Ferryman.Utility;

namespace iosh
{
    public partial class Pressure : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PaperID"] != null)
                {
                    ViewState["PaperID"] = Session["PaperID"].ToString();
                    Check_Result();
                }
                else
                {
                    Response.Redirect("Default.aspx", false);
                }
            }
        }

        private void Check_Result()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;
            int intPressure, intSatisfy;

            try
            {
                strSQL = "Select DimensionID, SumOfValue "
                        + "From PaperResult "
                        + "Where PaperID=@ID";
                DB.AddSqlParameter("@ID", Convert.ToInt64(ViewState["PaperID"].ToString()));
                myData = DB.GetDataTable(strSQL);
                if (myData!=null)
                {
                    intPressure = 0;
                    intSatisfy = 0;
                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        
                        switch (Convert.ToInt32(myData.Rows[i]["DimensionID"].ToString()))
                        {
                            case 35:
                                intPressure=Convert.ToInt32(myData.Rows[i]["SumOfValue"]);
                                break;
                            case 36:
                                intSatisfy= Convert.ToInt32(myData.Rows[i]["SumOfValue"]);
                                break;
                        }
                    }   // for i

                    switch (intPressure)
                    {
                        case 1:
                        case 2:
                            litPressure.Text = "您的工作壓力不大，工作狀況並未產生明顯的壓力。";
                            break;
                        case 3:
                            litPressure.Text = "您有時有工作壓力，建議您參考「<a href='object/attachments/health/58.pdf' style='color: #0000ff' target='_blank'>職場心理衛生</a>」資料，也建議您進入「<a href='https://meeting.ilosh.gov.tw/overwork/pTest/pTest.aspx' style='color: #0000ff' target='_blank'>簡易工作壓力量表</a>」做進一步檢測。";
                            break;
                        case 4:
                        case 5:
                            litPressure.Text = "您工作壓力過大，建議您參考「<a href='object/attachments/health/58.pdf' style='color: #0000ff' target='_blank'>職場心理衛生</a>」資料，也建議您進入「<a href='https://meeting.ilosh.gov.tw/overwork/pTest/pTest.aspx' style='color: #0000ff' target='_blank'>簡易工作壓力量表</a>」做進一步檢測。";
                            break;
                    }
                    litPressure.Text = "<p style=\"font-size: 12pt; \">" + litPressure.Text + "</p>";

                    switch (intSatisfy)
                    {
                        case 1:
                        case 2:
                            litSatisfy.Text = "您對目前的工作感到滿意，請繼續保持身心健康，有助提升工作滿意度。";
                            break;
                        case 3:
                        case 4:
                        case 5:
                            litSatisfy.Text = "您對工作普通滿意或不滿意，影響工作滿意度包括身體、心理、社會因素。您可以使用系統提供之構面選項，評估健康相關之影響因素，也可進入「<a href='https://meeting.ilosh.gov.tw/overwork/index.aspx' target='_blank'>心理量表檢測系統平台</a>」，評估心理影響因素。";
                            break;
                    }
                    litSatisfy.Text = "<p style=\"font-size: 12pt; \">" + litSatisfy.Text + "</p>";
                }   // if (myData!=null)
                else
                {
                    if (DB.DBErrorMessage!="")
                    {
                        ShareFunction.PutLog("Check_Result", DB.DBErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Check_Result", ex.Message );
            }
            finally
            {
                DB = null;
            }
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            if (ViewState["PaperID"] != null)
            {
                string strPage = "";
                strPage = ShareCode.GoBackt_Report(Convert.ToInt64(ViewState["PaperID"].ToString()));
                Response.Redirect(strPage, false);
            }
            else
            {
                Response.Redirect("Default.aspx", false);
            }
        }
    }
}