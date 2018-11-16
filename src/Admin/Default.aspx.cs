using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ferryman.Cryptology;
using Ferryman.DATA;
using System.Data;
using Ferryman.Utility;

namespace Admin
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            DataTable myData;
            string strPassword = "";
            Ferryman.Cryptology.MD5Class clsMD5 = new MD5Class();

            try
            {
                strSQL = "Select UserPassword, UserName From AdminUser Where UserAccount=@User And Disabled='0'";
                DB.AddSqlParameter("@User", txtAccount.Text.Trim());
                myData = DB.GetDataTable(strSQL);
                if (myData==null)
                {
                    Message.MsgShow(this.Page, "帳號錯誤。");
                    if (DB.DBErrorMessage=="")
                    {
                        ShareFunction.PutLog("btnLogin_Click", "帳號" + txtAccount.Text.Trim() + "登入失敗。");
                    }
                    else
                    {
                        ShareFunction.PutLog("btnLogin_Click", "帳號" + txtAccount.Text.Trim() + "登入錯誤：" + DB.DBErrorMessage);
                    }
                }
                else
                {
                    strPassword = clsMD5.GetMD5Password(txtPassword.Text.Trim());
                    if (myData.Rows[0]["UserPassword"].ToString().ToLower() == strPassword.ToLower())
                    {
                        Session["IsLogin"] = true;
                        Session["UserName"]=myData.Rows[0]["UserName"].ToString();
                        Response.Redirect("Admin.aspx", false);
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "密碼錯誤。");
                        Session["IsLogin"] = null;
                        Session["UserName"] = null;
                        ShareFunction.PutLog("btnLogin_Click", "帳號" + txtAccount.Text.Trim() + "密碼錯誤。");
                    }
                }
            }
            catch (Exception ex)
            {
                Message.MsgShow(this.Page, "登入過程發生異常，請稍後再試。");
                ShareFunction.PutLog("btnLogin_Click", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }
    }
}