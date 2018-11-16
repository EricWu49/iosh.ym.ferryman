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
    public partial class Resource : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack )
            {
                Bind_Body();
                if (Request.QueryString["id"]!=null)
                {
                    hidSetID.Value = Request.QueryString["id"];
                    Load_Resource(Request.QueryString["id"]);
                }
            }
            else
            {
                hidSetID.Value = "";
            }
        }

        private void Bind_Body()
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select Code, ParmName From SystemParameter Where ParmType='BODY' And Disabled='N' Order By SortID";
                myData = DB.GetDataTable(strSQL);
                ddlBody.DataSource = myData;
                ddlBody.DataTextField = "ParmName";
                ddlBody.DataValueField = "Code";
                ddlBody.DataBind();
                ddlBody.Items.Insert(0, new ListItem("請選擇...", "0"));
                ddlBody.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Bind_Body", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;
            string strBody, strKeyword, strJoin;

            try
            {
                if (ddlBody.SelectedIndex==0)
                {
                    strBody = "";
                }
                else
                {
                    strBody = ddlBody.SelectedValue;
                }

                strKeyword = txtKeyword.Text.Trim();

                if (strBody=="" && strKeyword == "")
                {
                    Message.MsgShow(this.Page, "請至少輸入一個選擇條件。");
                    return;
                }

                strSQL = "Select VideoName, Description, YoutubeID From BodyResource ";
                strJoin = "Where ";
                if (strBody!="")
                {
                    strSQL = strSQL + strJoin + "BodyCode=@Code ";
                    DB.AddSqlParameter("@Code", strBody);
                    strJoin = "And ";
                }

                if (strKeyword!="")
                {
                    strSQL = strSQL + strJoin + "(VideoName Like @Word Or Description Like @Word) ";
                    DB.AddSqlParameter("@Word", "%" + strKeyword + "%");
                }
                myData = DB.GetDataTable(strSQL);
                Repeater1.DataSource = myData;
                Repeater1.DataBind();
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("btnQuery_Click", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        private void Load_Resource(string setID)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;

            try
            {
                strSQL = "Select ResourceName as VideoName, Description, ResourcePath as YoutubeID " +
                         "From ResourceSetDetail T0 Join EvaluateResource T1 On T0.ResourceID = T1.ResourceID " +
                         "Where SetID = @ID";
                DB.AddSqlParameter("@ID", Convert.ToInt32(setID));
                myData = DB.GetDataTable(strSQL);
                if (DB.DBErrorMessage!="")
                {
                    ShareFunction.PutLog("Load_Resource", DB.DBErrorMessage);
                }
                Repeater1.DataSource = myData;
                Repeater1.DataBind();
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Load_Resource", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType==ListItemType.Header)
            {
                Label myLabel;
                myLabel = (Label)e.Item.FindControl("lblDescription");

                if (hidSetID.Value!="")
                {
                    DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
                    try
                    {
                        string strSQL = "";
                        string strDescription = "";
                        strSQL = "Select Description From ResourceSet Where SetID=@ID";
                        DB.AddSqlParameter("@ID", Convert.ToInt32(hidSetID.Value));
                        strDescription = DB.GetData(strSQL);
                        if (myLabel != null)
                        {
                            myLabel.Text = strDescription;
                            myLabel.Visible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShareFunction.PutLog("Repeater1_ItemDataBound", ex.Message);
                    }
                    finally
                    {
                        DB = null;
                    }
                }
                else
                {
                    if (myLabel!=null)
                    {
                        myLabel.Text = "";
                        myLabel.Visible = false;
                    }
                }
            }
        }
    }
}