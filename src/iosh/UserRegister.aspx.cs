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
    public partial class UserRegister : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            DBClass DB = new DBClass(General.DataBaseType.MSSQL, "DB");
            string strSQL = "";
            string strAccount = "";
            string strPassword = "";
            string strName = "";
            //string strResult = "";

            try
            {
                strAccount = txtAccount.Text.Trim();
                strPassword = txtPassword.Text.Trim();
                strName = txtName.Text.Trim();

                strSQL = "Select Count(*) From UserAccount Where UserAccount=@Account Or Email=@Account";
                DB.AddSqlParameter("@Account", strAccount);
                if (Convert.ToInt32(DB.GetData(strSQL))>0)
                {
                    Message.MsgShow(this.Page, "該Email已經有註冊過了。");
                }
                else
                {
                    Ferryman.Cryptology.MD5Class md5 = new Ferryman.Cryptology.MD5Class();

                    strSQL = "Insert UserAccount " +
                             "(UserAccount, UserName, CompanyID, UserPass, Email, Phone, Locked, Disabled, AdminFlag, NurseFlag) " +
                             "Values (@Account, @Name, 0, @Pass, @Account, '', 'Y', 'N', 'N', 'N')";
                    DB.AddSqlParameter("@Account", strAccount);
                    DB.AddSqlParameter("@Name", strName);
                    DB.AddSqlParameter("@Pass", md5.GetMD5Password(strPassword));
                    if (DB.InsertOneRecord(strSQL)>0)
                    {
                        DB = null;
                        if (SendVerifyMail(strAccount, strName))
                        {
                            Message.MsgShow_And_Redirect(this.Page, "系統已經發送一封帳號啟用的通知信到您填寫的Email信箱，請點擊信中的「帳號啟用」連結來啟動您的帳號。", "Defaulta.aspx");
                            Response.Redirect("Default.aspx", true);
                        }
                        else
                        {
                            Message.MsgShow_And_Redirect(this.Page, "發送註冊通知信失敗，請通知系統管理員。", "Default.aspx");
                        }
                    }
                    else
                    {
                        Message.MsgShow(this.Page, "用戶註冊失敗，請通知系統管理員。");
                        ShareFunction.PutLog("btnLogin_Click", DB.DBErrorMessage);
                    }
                }
                DB = null;
            }
            catch (Exception ex)
            {
                Message.MsgShow(this.Page, "用戶註冊過程發生錯誤，請通知系統管理員。");
                ShareFunction.PutLog("btnLogin_Click", ex.Message);
            }
        }

        private bool SendVerifyMail(string UserAccount, string UserName)
        {
            string strSubject = "";
            string strBody = "";
            MailClass myMail = new Ferryman.Utility.MailClass();

            try
            {
                strSubject = "工作適能指數評估平台會員註冊驗證通知信";
                strBody = "<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\" />" +
                          "<title>工作適能指數評估平台會員註冊驗證通知信</title></head>" +
                          "<body>" + UserName + " 先生/小姐 您好，<br/>" +
                          "感謝您於工作適能指數評估平台註冊會員，請點擊下方的【帳號啟用】連結，來啟用您所註冊的帳號。<br/>" +
                          "您的帳號必須要啟用後，才能登入本系統。<br>" +
                          "<a href='" + Request.Url.Scheme + "://" + Request.Url.Authority + "/UserVerify.aspx?email=" + UserAccount + "'>帳號啟用</a><br/><br/>" +
                          "<b>本郵件由系統自動寄出，請勿直接回信。</body></html>";
                myMail.MailFrom("noreply@ioshweb.com", "工作適能指數評估平台");
                myMail.MailTo(UserAccount, UserName);
                return myMail.Send(strSubject, strBody, true);
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("SendVerifyMail", ex.Message);
                return false;
            }
        }
    }
}