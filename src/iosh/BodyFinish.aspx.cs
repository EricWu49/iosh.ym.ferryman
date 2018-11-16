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
    public partial class BodyFinish : System.Web.UI.Page
    {
        int _SurveyID = 7;
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
                strSQL = "Select D.QuestionID, D.QTitle, S.ResourceCode, D.SelectValue "
                        + "From BodyData D Join BodySuggest S On D.QuestionID=S.QuestionID "
                        + "Where D.PaperID = @ID And D.QuestionID In(Select QuestionID "
                        + "                                      From Question "
                        + "                                      Where SurveyID=@SID And PageNo=2) "
                        + "       And Cast(D.SelectValue as int)>0";
                DB.AddSqlParameter("@ID", Convert.ToInt64(ViewState["PaperID"]));
                DB.AddSqlParameter("@SID", _SurveyID);
                dt1 = DB.GetDataTable(strSQL);

                if (dt1 == null)
                {
                    if (DB.DBErrorMessage!="")
                    {
                        ShareFunction.PutLog("Load_Result", DB.DBErrorMessage);
                        Message.MsgShow(this.Page, "系統發生錯誤。");
                    }
                    else
                    {
                        // 所有痠痛等級都回答"0"
                        //Message.MsgShow(this.Page, "恭喜您，您的肌肉骨骼沒有痠痛問題。");
                        PlaceHolder1.Visible = false;
                        PlaceHolder2.Visible = true;
                    }
                }
                else
                {
                    Repeater1.DataSource = dt1;
                    Repeater1.DataBind();

                    Get_Resource(dt1);
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

        private void Get_Resource(DataTable dt)
        {
            string strSQL = "";
            DataTable myData;
            string strCode = "";
            Literal litElement;
            int i, j;
            string strPosition = "";
            string strRemind, strEffect = "";

            try
            {
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    litElement = new Literal();

                    strCode = dt.Rows[i]["ResourceCode"].ToString();
                    strSQL = "Select Description, YoutubeID From BodyResource Where Code=@Code";
                    DB.AddSqlParameter("@Code", strCode);
                    myData = DB.GetDataTable(strSQL);
                    if (myData != null)
                    {
                        // 2016/12/17 新增需求，判斷疼痛等級及影響工作情形，給予不同的提醒文字
                        strSQL = "Select D.SelectValue "
                                + "From BodyData D Join Question Q On D.QuestionID=Q.QuestionID "
                                + "Where PaperID=@PID And Q.ParentID=@QID";
                        DB.AddSqlParameter("@PID", Convert.ToInt64(ViewState["PaperID"]));
                        DB.AddSqlParameter("@QID", Convert.ToInt32(dt.Rows[i]["QuestionID"].ToString()));
                        strEffect = DB.GetData(strSQL);
                        if (Convert.ToInt32(dt.Rows[i]["SelectValue"].ToString())>2 && Convert.ToInt32(strEffect)>3 )
                        {
                            // 疼痛等級大於等於3，且有請假
                            strRemind = "<div class=\"alert alert-danger\" role=\"alert\">疼痛較嚴重且影響工作，建議應進一步尋求醫療處置，但仍可參考下列影片執行自我健康促進</div>";
                        }
                        else
                        {
                            // 疼痛等級小於3，或沒有請假
                            strRemind = "<div class=\"alert alert-info\" role=\"alert\">請參考下列影片執行自我健康促進</div>";
                        }
                        litElement.Text = "<div id=\"tab-" + strCode + "\">" + Environment.NewLine
                                        + strRemind + Environment.NewLine
                                        + "   <div class=\"row\">" + Environment.NewLine
                                        + "      <div class=\"col-xs-12 col-sm-6 col-md-6 col-lg-6\">" + Environment.NewLine
                                        + "         <img src = \"object/images/" + strCode + ".jpg\" alt=\"" + myData.Rows[0]["Description"].ToString() + "部位圖\" />" + Environment.NewLine
                                        + "      </div>" + Environment.NewLine
                                        + "      <div class=\"col-xs-12 col-sm-6 col-md-6 col-lg-6\">" + Environment.NewLine
                                        + "         <iframe width = \"400\" height=\"225\" src=\"https://www.youtube.com/embed/" + myData.Rows[0]["YoutubeID"].ToString() + "?rel=0\" frameborder=\"0\" allowfullscreen></iframe><br/>" + Environment.NewLine
                                        + "         <span>" + myData.Rows[0]["Description"].ToString() + "衛教影片 </span>" + Environment.NewLine
                                        + "      </div>" + Environment.NewLine
                                        + "   </div>" + Environment.NewLine;
                        strSQL = "Select A.PositionCode, A.Description, C.Code, C.Description as VideoName, C.YoutubeID "
                               + "From BodyPosition A Join BodyRule B On A.PositionCode = B.PositionCode "
                               + "                    Join BodyResource C On B.ResourceCode = C.Code "
                               + "Where A.ParentCode = @Code "
                               + "Order By A.PositionCode, B.RuleID";
                        DB.AddSqlParameter("@Code", strCode);
                        myData = DB.GetDataTable(strSQL);
                        if (myData==null)
                        {
                            if (DB.DBErrorMessage!="")
                            {
                                ShareFunction.PutLog("Get_Resource", DB.DBErrorMessage);
                                continue;
                            }
                            else
                            {
                                // 沒有Level 2資料
                                strSQL = "Select B.PositionCode, C.Code, C.Description, C.YoutubeID "
                                        + "From BodyRule B Join BodyResource C On B.ResourceCode=C.Code "
                                        + "Where B.PositionCode=@Code";
                                DB.AddSqlParameter("@Code", strCode);
                                myData = DB.GetDataTable(strSQL);
                                if (myData !=null)
                                {
                                    litElement.Text += "<div class=\"row\">" + Environment.NewLine;
                                    for (j = 0; j < myData.Rows.Count; j++)
                                    {
                                        litElement.Text += "   <div class=\"col-xs-12 col-sm-12 col-md-6 col-lg-4 page-header\">" + Environment.NewLine;
                                        litElement.Text += "         <a class=\"btn btn-warning\" onclick = \"GetVedio('" + myData.Rows[j]["YoutubeID"].ToString() + "')\" href = \"#exampleModal\" data-toggle = \"modal\" role=\"button\" >" + Environment.NewLine;
                                        litElement.Text += "            [影片] : " + myData.Rows[j]["Description"].ToString() + Environment.NewLine;
                                        litElement.Text += "         </a>" + Environment.NewLine;
                                        litElement.Text += "   </div>" + Environment.NewLine;
                                    }
                                    litElement.Text += "</div>" + Environment.NewLine;
                                }
                                else
                                {
                                    ShareFunction.PutLog("Get_Resource", DB.DBErrorMessage);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            strPosition = "";
                            litElement.Text += "<h3>先參考示意圖找出您痠痛的類型，再觀看我們所提供的衛教影片</h3>" + Environment.NewLine;

                            for (j=0; j<myData.Rows.Count; j++)
                            {
                                if (strPosition != myData.Rows[j]["PositionCode"].ToString())
                                {
                                    if (strPosition!="")
                                    {
                                        litElement.Text += "</div>" + Environment.NewLine;
                                    }
                                    litElement.Text += "<div class=\"row\">" + Environment.NewLine;
                                    litElement.Text += "   <div class=\"col-xs-12 col-sm-12 col-md-12 col-lg-12 page-header\">" + Environment.NewLine;
                                    litElement.Text += "      <div class=\"bg-primary\">" + myData.Rows[j]["Description"].ToString() + "</div>" + Environment.NewLine;
                                    litElement.Text += "   </div>" + Environment.NewLine;
                                    litElement.Text += "   <div class=\"col-xs-12 col-sm-12 col-md-12 col-lg-12\">" + Environment.NewLine;
                                    litElement.Text += "      <img src = \"object/images/" + myData.Rows[j]["PositionCode"].ToString() + ".jpg\" alt=\"" + myData.Rows[j]["Description"].ToString() + "示意圖\" />" + Environment.NewLine;
                                    litElement.Text += "   </div>" + Environment.NewLine;
                                    litElement.Text += "</div>" + Environment.NewLine;

                                    litElement.Text += "<div class=\"row\">" + Environment.NewLine;

                                    strPosition = myData.Rows[j]["PositionCode"].ToString();
                                }
                                litElement.Text += "   <div class=\"col-xs-12 col-sm-12 col-md-6 col-lg-4 page-header\">" + Environment.NewLine;
                                litElement.Text += "         <a class=\"btn btn-warning\" onclick = \"GetVedio('" + myData.Rows[j]["YoutubeID"].ToString() + "')\" href = \"#exampleModal\" data-toggle = \"modal\" role=\"button\" >" + Environment.NewLine;
                                litElement.Text += "            [影片] : " + myData.Rows[j]["VideoName"].ToString() + Environment.NewLine;
                                litElement.Text += "         </a>" + Environment.NewLine;
                                litElement.Text += "   </div>" + Environment.NewLine;
                            }
                            litElement.Text += "</div>" + Environment.NewLine;
                        }
                        litElement.Text += "</div>" + Environment.NewLine;
                        Panel1.Controls.Add(litElement);
                    }
                    else
                    {
                        if (DB.DBErrorMessage!="")
                        {
                            ShareFunction.PutLog("Get_Resource", DB.DBErrorMessage);
                        }
                        litElement.Text = "<div id=\"tab-" + strCode + "\">" + Environment.NewLine
                                        + "<p>沒有資料。</p>" + Environment.NewLine
                                        + "</div>";
                    }
                    Panel1.Controls.Add(litElement);
                }
            }
            catch( Exception ex)
            {
                ShareFunction.PutLog("Get_Resource", ex.Message);
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