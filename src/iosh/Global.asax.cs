using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Ferryman.Utility;

namespace iosh
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            string strUserSession = System.Guid.NewGuid().ToString();
            Session["SessionID"] = strUserSession;
            //CookieClass.ClearCookie(CookieClass.CookieDefine.CookieName);
            //CookieClass.WriteCookie(CookieClass.CookieDefine.CookieName, CookieClass.CookieDefine.sUserKey, strUserSession);
            //UserClass.DeleteCookie(UserClass.SurveyCookie.sCookieName);
            UserClass.WriteCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserKey, strUserSession);
            UserClass.WriteCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserID, "0");
            UserClass.WriteCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserAccount, "");
            UserClass.WriteCookie(UserClass.SurveyCookie.sCookieName, UserClass.SurveyCookie.sUserName, "");
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            Session["SessionID"] = null;
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}