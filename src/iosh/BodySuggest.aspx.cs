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
    public partial class BodySuggest : System.Web.UI.Page
    {
        DBClass DB;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PaperID"] == null)
                {
                    Response.Redirect("Default.aspx", false);
                }
                else
                {
                    ViewState.Add("PaperID", Session["PaperID"]);
                    ShareCode.Close_Report(Convert.ToInt64(Session["PaperID"]));
                    DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                    Load_Result();
                    DB = null;
                }
            }
        }

        private void Load_Result()
        {
            string strSQL = "";
            DataTable dt1;

            try
            {
                strSQL = "Select T1.QTitle, T0.EvaluateCode "
                        + "From BodyResult T0 Join Question T1 On T0.QuestionID=T1.QuestionID "
                        + "Where T0.PaperID = @ID";
                DB.AddSqlParameter("@ID", Convert.ToInt64(ViewState["PaperID"]));
                dt1 = DB.GetDataTable(strSQL);

                if (dt1 == null)
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Load_Result", DB.DBErrorMessage);
                        Message.MsgShow(this.Page, "系統發生錯誤。");
                    }
                }
                else
                {
                    Repeater1.DataSource = dt1;
                    Repeater1.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Result", ex.Message);
            }
            finally
            {
                //DB = null;
            }
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            DataRowView myDataItems;
            string strCode;
            PlaceHolder myHolder;

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                myDataItems = (DataRowView)e.Item.DataItem;
                strCode = myDataItems["EvaluateCode"].ToString();

                //Reference the Controls.
                myHolder = (PlaceHolder)(e.Item.FindControl("PlaceResult"));

                Get_Resource(myHolder, strCode);
            }
        }

        private void Get_Resource(PlaceHolder pPlaceHolder, string pEvaluateCode)
        {
            string strSQL = "";
            DataTable myData;
            LiteralControl litResource;

            try
            {
                // 診斷結果與建議
                strSQL = "Select EvaluateResult, EvaluateSuggest " +
                         "From BodyEvaluate " +
                         "Where EvaluateCode=@Code";
                DB.AddSqlParameter("@Code", pEvaluateCode);
                myData = DB.GetDataTable(strSQL);
                if (myData != null)
                {
                    litResource = new LiteralControl();
                    litResource.Text = "<h4 style=\"color:#0022ff\">自評結果：" + myData.Rows[0]["EvaluateResult"].ToString() + "</h4>";
                    if (myData.Rows[0]["EvaluateSuggest"].ToString().Trim() != "")
                    {
                        litResource.Text += "<p><span style=\"color:#0022ff\">相關建議：</span>" + myData.Rows[0]["EvaluateSuggest"].ToString() + "</p>";
                    }
                    pPlaceHolder.Controls.Add(litResource);
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Get_Resource", DB.DBErrorMessage);
                    }
                }

                // 衛教資源
                strSQL = "Select R2.ResourceID, R2.ResourceName, R2.ExternalFlag, R2.[Description], R2.ResourceType, R2.ResourcePath " +
                         "From EvaluateRule R1 Join EvaluateResource R2 On R1.ResourceID=R2.ResourceID " +
                         "Where R1.EvaluateCode=@Code " +
                         "Order By R1.OrderNo ";
                DB.AddSqlParameter("@Code", pEvaluateCode);
                myData = DB.GetDataTable(strSQL);
                if (myData != null)
                {
                    litResource = new LiteralControl();
                    litResource.Text = "";
                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        switch (myData.Rows[i]["ResourceType"].ToString())
                        {
                            case "VIDEO":
                                litResource.Text += "<div class=\"col-xs-12 col-sm-6 col-md-4 col-lg-3\">" + Environment.NewLine;
                                litResource.Text += "    <div class=\"card bg-info\" style=\"height: 100 %; margin-bottom: 5px; \">" + Environment.NewLine;
                                litResource.Text += "        <div class=\"card-body\">" + Environment.NewLine;
                                litResource.Text += "            <div class=\"card-title text-center\">" + Environment.NewLine;
                                litResource.Text += "                <a class=\"btn\" onclick=\"GetVideo('" + myData.Rows[i]["ResourcePath"].ToString() + "');\" href=\"#exampleModal\" data-toggle=\"modal\" role=\"button\">" + Environment.NewLine;
                                litResource.Text += "                    <img src=\"https://i.ytimg.com/vi/" + myData.Rows[i]["ResourcePath"].ToString() + "/default.jpg\" alt=\"" + myData.Rows[i]["ResourceName"].ToString() + "\" />" + Environment.NewLine;
                                litResource.Text += "                </a>" + Environment.NewLine;
                                litResource.Text += "            </div>" + Environment.NewLine;
                                litResource.Text += "            <p class=\"card-text card-flex\">" + Environment.NewLine;
                                litResource.Text += "                " + myData.Rows[i]["Description"].ToString() + Environment.NewLine;
                                litResource.Text += "            </p>" + Environment.NewLine;
                                litResource.Text += "        </div>" + Environment.NewLine;
                                litResource.Text += "    </div>" + Environment.NewLine;
                                litResource.Text += "</div>" + Environment.NewLine;
                                break;
                            case "IMAGE":
                                break;
                            case "FILE":
                                break;
                        }
                    }
                    litResource.Text = "<div class=\"row row-flex\">" + Environment.NewLine +
                                        litResource.Text +
                                        "</div>";
                    pPlaceHolder.Controls.Add(litResource);
                }
                else
                {
                    if (DB.DBErrorMessage != "")
                    {
                        ShareFunction.PutLog("Get_Resource", DB.DBErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Message.ShowError(this.Page, "Get_Resource", ex.Message, "產生衛教資源發生錯誤。");
            }
        }
    }
}