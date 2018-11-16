using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Ferryman.Utility
{
    public class MailClass
    {
        /// <summary>
        /// 寄信標題
        /// </summary>
        //static string mailTitle = ConfigurationManager.AppSettings["mailTitle"].Trim();

        /// <summary>
        /// 寄信人Email
        /// </summary>
        //static string sendMail = ConfigurationManager.AppSettings["sendMail"].Trim();
        
        /// <summary>
        /// 收信人Email(多筆用逗號隔開)
        /// </summary>
        //static string receiveMails = ConfigurationManager.AppSettings["receiveMails"].Trim();

        /// <summary>
        /// 寄信smtp server
        /// </summary>
        static string smtpServer = ConfigurationManager.AppSettings["SMTPServer"].Trim();

        /// <summary>
        /// 寄信smtp server的Port，預設25
        /// </summary>
        static int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"].Trim());

        /// <summary>
        /// 寄信帳號
        /// </summary>
        static string mailAccount = ConfigurationManager.AppSettings["MailAccount"].Trim();

        /// <summary>
        /// 寄信密碼
        /// </summary>
        static string mailPwd = ConfigurationManager.AppSettings["MailPassword"].Trim();

        static MailMessage mms;

        public MailClass()
        {
            mms = new System.Net.Mail.MailMessage();
        }

        public void MailFrom(string MailFrom)
        {
            mms.From = new MailAddress(MailFrom);
        }

        public void MailFrom(string MailFrom, string DisplayName)
        {
            mms.From = new MailAddress(MailFrom, DisplayName);
        }

        public void MailTo(string MailTo)
        {
            mms.To.Add(new MailAddress(MailTo));
        }

        public void MailTo(string MailTo, string DisplayName)
        {
            mms.To.Add(new MailAddress(MailTo, DisplayName));
        }

        public void MailBcc(string MailBcc)
        {
            mms.Bcc.Add(new MailAddress(MailBcc));
        }

        public void MailBcc(string MailBcc, string DisplayName)
        {
            mms.Bcc.Add(new MailAddress(MailBcc, DisplayName));
        }

        public bool Send(string MailSubject, string MailBody, bool isBodyHtml)
        {
            try
            {
                //信件主旨
                mms.Subject = MailSubject;
                //信件內容
                mms.Body = MailBody;
                //信件內容 是否採用Html格式
                mms.IsBodyHtml = isBodyHtml;

                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.Credentials = new NetworkCredential(mailAccount, mailPwd);//寄信帳密
                    client.Send(mms);//寄出一封信
                    client.Dispose();
                }//end using 

                return true;//成功
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("MailClass.Send", ex.Message);
                return false;//寄失敗
            }
        }
    }
}