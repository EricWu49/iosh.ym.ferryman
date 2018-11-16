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
    public partial class ReportTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack )
            {
                DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                string strSQL = "";
                DataTable myData;

                strSQL = "Select PaperID From Paper Where SurveyID=2 And FinishTime Is Not Null";
                myData = DB.GetDataTable(strSQL);
                ddlReport.DataSource = myData;
                ddlReport.DataTextField="PaperID";
                ddlReport.DataValueField = "PaperID";
                ddlReport.DataBind();
                if (myData==null)
                {
                    ddlReport.SelectedIndex = -1;
                }
                else
                {
                    ddlReport.SelectedIndex = ddlReport.Items.Count - 1;
                }

                strSQL = "Select PaperID From Paper Where SurveyID=9 And FinishTime Is Not Null";
                myData = DB.GetDataTable(strSQL);
                ddlBody.DataSource = myData;
                ddlBody.DataTextField = "PaperID";
                ddlBody.DataValueField = "PaperID";
                ddlBody.DataBind();
                if (myData == null)
                {
                    ddlBody.SelectedIndex = -1;
                }
                else
                {
                    ddlBody.SelectedIndex = ddlBody.Items.Count - 1;
                }
            }
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            if (ddlReport.SelectedIndex > -1)
            {
                DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                string strSQL = "";
                DataTable myData;
                string strPage = "";

                strSQL = "Select PaperID, SessionID, SurveyID From Paper Where PaperID=@ID";
                DB.AddSqlParameter("@ID", Convert.ToInt64(ddlReport.SelectedValue));
                myData = DB.GetDataTable(strSQL);

                if (myData != null)
                {
                    Session["SurveyID"] = myData.Rows[0]["SurveyID"].ToString();
                    Session["PaperID"] = myData.Rows[0]["PaperID"].ToString();
                    Session["SessionID"] = myData.Rows[0]["SessionID"].ToString();

                    strSQL = "Select P.PageFile "
                            + "From Survey S Join PageType P On S.PageID=P.PageID "
                            + "Where S.SurveyID=@SID";
                    DB.AddSqlParameter("@SID", Convert.ToInt32(Session["SurveyID"]));
                    strPage = DB.GetData(strSQL);

                    if (strPage != null)
                    {
                        Response.Redirect(strPage, false);
                    }
                }
                DB = null;
            }
        }

        protected void btnBody_Click(object sender, EventArgs e)
        {
            if (ddlReport.SelectedIndex > -1)
            {
                DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                string strSQL = "";
                DataTable myData;
                string strPage = "";

                strSQL = "Select PaperID, SessionID, SurveyID From Paper Where PaperID=@ID";
                DB.AddSqlParameter("@ID", Convert.ToInt64(ddlBody.SelectedValue));
                myData = DB.GetDataTable(strSQL);

                if (myData != null)
                {
                    Session["SurveyID"] = myData.Rows[0]["SurveyID"].ToString();
                    Session["PaperID"] = myData.Rows[0]["PaperID"].ToString();
                    Session["SessionID"] = myData.Rows[0]["SessionID"].ToString();

                    strSQL = "Select P.PageFile "
                            + "From Survey S Join PageType P On S.PageID=P.PageID "
                            + "Where S.SurveyID=@SID";
                    DB.AddSqlParameter("@SID", Convert.ToInt32(Session["SurveyID"]));
                    strPage = DB.GetData(strSQL);

                    if (strPage != null)
                    {
                        Response.Redirect(strPage, false);
                    }
                }
                DB = null;
            }
        }
    }
}