using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace iosh
{

    public static class UserClass
    {
        public struct SurveyCookie
        {
            public const string sCookieName = "iosh";
            public const string sSurvey = "Survey_ID";
            public const string sStartTime = "Start_Time";
            public const string sEndTime = "End_Time";
            // 保留登入者的連線代號，用來分辨匿名用戶是否是同一人使用
            public const string sMemberSession = "UserKey";
            // 保留登入者的會員編號
            public const string sUserID = "UserID";            // 使用者代碼
            public const string sUserKey = "UserKey";           // 使用者連線代碼
            public const string sUserAccount = "UserAccount";   // 使用者帳號
            public const string sUserName = "UserName";         // 使用者姓名
            public const string sMenuID = "MenuID";             // 啟用的選單ID            public string PaperID;
        }

        #region "Session處理"
        public static void Save_Session(string SessionKey, string SessionValue)
        {
            //objPage.Session[SessionKey] = SessionValue;
            HttpContext.Current.Session[SessionKey] = SessionValue;
        }

        public static string Get_Session(string SessionKey)
        {
            if ((HttpContext.Current.Session[SessionKey] == null))
            {
                return null;
            }
            else
            {
                return HttpContext.Current.Session[SessionKey].ToString();
            }
        }
        #endregion

        #region "Cookie處理"
        public static void WriteCookie(string strCookiesName, string strKey, string strValue)
        {
            HttpCookie MyCookie = default(HttpCookie);

            strValue = HttpUtility.UrlEncode(strValue);

            //MyCookie = objPage.Request.Cookies[strCookiesName];
            MyCookie = System.Web.HttpContext.Current.Request.Cookies[strCookiesName];
            if ((MyCookie != null))
            {
                if (MyCookie[strKey]==null)
                {
                    MyCookie.Values.Add(strKey, strValue);
                }
                else
                {
                    MyCookie[strKey] = strValue;
                }
                MyCookie.Expires = DateTime.Now.AddDays(1);
                //objPage.Response.AppendCookie(MyCookie);
                System.Web.HttpContext.Current.Response.AppendCookie(MyCookie);
            }
            else
            {
                MyCookie = new HttpCookie(strCookiesName);
                MyCookie.Values.Add(strKey, strValue);
                MyCookie.Expires = DateTime.Now.AddDays(1);
                //objPage.Response.AppendCookie(MyCookie);
                System.Web.HttpContext.Current.Response.AppendCookie(MyCookie);
            }
        }

        public static string ReadCookie(string strCookiesName, string strKey, string strDefault)
        {
            HttpCookie MyCookie = default(HttpCookie);
            //MyCookie = objPage.Request.Cookies[strCookiesName];
            MyCookie = System.Web.HttpContext.Current.Request.Cookies[strCookiesName];
            if ((MyCookie != null))
            {
                if (MyCookie[strKey] == null)
                {
                    return strDefault;
                }
                else
                {
                    return HttpUtility.UrlDecode(MyCookie[strKey]);
                }
            }
            else
            {
                return strDefault;
            }
        }

        public static void DeleteCookie(string strCookiesName)
        {
            HttpCookie MyCookie = default(HttpCookie);
            //MyCookie = objPage.Request.Cookies[strCookiesName];
            MyCookie = System.Web.HttpContext.Current.Request.Cookies[strCookiesName];
            if ((MyCookie != null))
            {
                MyCookie.Expires = DateTime.Now.AddDays(-1);
                //objPage.Response.SetCookie(MyCookie);
                System.Web.HttpContext.Current.Response.SetCookie(MyCookie);
            }
        }

        public static bool CookieIsEnabled()
        {
            WriteCookie("CookieTest", SurveyCookie.sStartTime, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            if (string.IsNullOrEmpty(ReadCookie("CookieTest", SurveyCookie.sStartTime, "")))
            {
                DeleteCookie("CookieTest");
                return false;
            }
            else
            {
                DeleteCookie("CookieTest");
                return true;
            }
        }
        #endregion
    }
}