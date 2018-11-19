using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Ferryman.Utility;
using Ferryman.DATA;

namespace iosh
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //string strUserID = CookieClass.ReadCookie(CookieClass.CookieDefine.CookieName, CookieClass.CookieDefine.sUserID, "0");
                string strUserID = UserClass.ReadCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserID, "0");
                if (strUserID!="0")
                {
                    Panel_Login.Visible = false;
                }
            }
        }

        /// <summary>
        /// 註冊用戶登入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strPassword = "";
            DataTable myData;
            Ferryman.Cryptology.MD5Class md5 = new Ferryman.Cryptology.MD5Class();

            try
            {
                if (txtAccount.Text.Trim() == "" || txtPassword.Text.Trim()=="")
                {
                    Message.MsgShow(this.Page, "請輸入帳號及密碼。");
                    return;
                }

                strPassword = md5.GetMD5Password(txtPassword.Text.Trim());

                strSQL = "Select UserID, UserName, UserPass, Locked, Disabled From UserAccount Where UserAccount=@User";
                DB.AddSqlParameter("@User", txtAccount.Text.Trim());
                myData = DB.GetDataTable(strSQL);
                if (myData==null)
                {
                    Message.MsgShow(this.Page, "帳號不存在!");
                    return;
                }
                else
                {
                    if (myData.Rows[0]["Locked"].ToString()=="Y")
                    {
                        Message.MsgShow(this.Page, "您的帳號已經被鎖定，暫時無法登入。");
                        return;
                    }
                    if (myData.Rows[0]["Disabled"].ToString() == "Y")
                    {
                        Message.MsgShow(this.Page, "您的帳號已失效，無法登入。");
                        return;
                    }
                    if (myData.Rows[0]["UserPass"].ToString() == strPassword)
                    {
                        UserClass.WriteCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserAccount, txtAccount.Text.Trim());
                        UserClass.WriteCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserID, myData.Rows[0]["UserID"].ToString());
                        UserClass.WriteCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserName, myData.Rows[0]["UserName"].ToString());
                        Session.Add("UserAccount", txtAccount.Text.Trim());
                        Session.Add("UserID", myData.Rows[0]["UserID"].ToString());
                        Session.Add("UserName", myData.Rows[0]["UserName"].ToString());
                        Panel_Login.Visible = false;
                        litScript.Text = "<script type='text/javascript'>$('#master-container').css('display', 'none');</script>";
                        Message.MsgShow(this.Page, "登入成功");

                        Panel myPanel = (Panel)ShareFunction.FindControlEx(Master, "UserPanel");
                        Label myLabel = (Label)ShareFunction.FindControlEx(Master, "lblUserName");

                        if (myLabel!=null)
                        {
                            myLabel.Text = UserClass.ReadCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserName, "");
                        }
                        if (myPanel!=null)
                        {
                            myPanel.Visible = true;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "您的密碼錯誤，請重新輸入。");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Message.MsgShow(this.Page, "登入發生錯誤。");
                ShareFunction.PutLog("btnLogin_Click", ex.Message);
            }
            finally
            {
                DB = null;
            }
        }

    }
}