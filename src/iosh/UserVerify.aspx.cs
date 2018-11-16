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
    public partial class UserVerify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["email"]==null)
                {
                    Response.Redirect("Default.aspx", true);
                }
                else
                {
                    VerifyUser(Request.QueryString["email"].Trim())
;                }
            }
        }

        private void VerifyUser(string strEmail)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strScript = "";

            try
            {
                strSQL = "Update UserAccount Set Locked='N' Where Email=@Email And Locked='Y'";
                DB.AddSqlParameter("@Email", strEmail);
                if (Convert.ToInt32(DB.RunSQL(strSQL))>0)
                {
                    strScript = "<script type='text/javascript'>$(\"#info-ng\").css(\"display\",\"none\");</script>";
                }
                else
                {
                    strScript = "<script type='text/javascript'>$(\"#info-ok\").css(\"display\",\"none\");</script>";
                }
                Literal1.Text = strScript;
            }
            catch (Exception ex)
            {
                Message.MsgShow(this.Page, "帳號啟用失敗。");
                ShareFunction.PutLog("VerifyUser", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }
    }
}