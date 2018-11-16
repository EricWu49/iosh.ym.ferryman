using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ferryman.Utility
{
    public class CookieClass
    {
        public struct CookieDefine
        {
            public const string CookieName = "iosh";
            public const string sUserID = "UserID";             // 使用者代碼
            public const string sUserKey = "UserKey";           // 使用者連線代碼
            public const string sUserAccount = "UserAccount";   // 使用者帳號
            public const string sUserName = "UserName";         // 使用者姓名
            public const string sMenuID = "MenuID";             // 啟用的選單ID
            //public const string sUserRole = "UserRole";
        }

        public static void WriteCookie(string strCookiesName, string strKey, string strValue)
        {
            HttpCookie MyCookie;
            MyCookie = System.Web.HttpContext.Current.Request.Cookies[strCookiesName];
            if (!(MyCookie == null))
            {
                MyCookie[strKey] = strValue;
                MyCookie.Expires = DateTime.Now.AddDays(1);
                HttpContext.Current.Response.AppendCookie(MyCookie);
            }
            else
            {
                MyCookie = new HttpCookie(strCookiesName);
                MyCookie.Values.Add(strKey, strValue);
                MyCookie.Expires = DateTime.Now.AddDays(1);
                HttpContext.Current.Response.AppendCookie(MyCookie);
            }
        }

        public static string ReadCookie(string strCookiesName, string strKey, string strDefault)
        {
            HttpCookie MyCookie;
            MyCookie = HttpContext.Current.Request.Cookies[strCookiesName];
            if (!(MyCookie == null))
                if (MyCookie[strKey]==null)
                {
                    return strDefault;
                }
                else
                {
                    return MyCookie[strKey];
                }
            else
                return strDefault;
        }

        public static void SetExpire(string strCookiesName, int ExpireDays)
        {
            HttpCookie MyCookie;
            MyCookie = HttpContext.Current.Request.Cookies[strCookiesName];
            if (!(MyCookie == null))
            {
                MyCookie.Expires = DateTime.Now.AddDays(ExpireDays);
                HttpContext.Current.Response.AppendCookie(MyCookie);
            }
            else
            {
                MyCookie = new HttpCookie(strCookiesName);
                MyCookie.Expires = DateTime.Now.AddDays(ExpireDays);
                HttpContext.Current.Response.AppendCookie(MyCookie);
            }
            return;
        }

        public static void ClearCookie(string strCookiesName)
        {
            HttpCookie MyCookie;
            MyCookie = HttpContext.Current.Request.Cookies[strCookiesName];
            if (!(MyCookie == null))
            {
                MyCookie.Expires = DateTime.Now.AddDays(30);
                HttpContext.Current.Response.AppendCookie(MyCookie);
            }
        }
    }
}