using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ferryman.Utility;

namespace iosh
{
    public partial class Main : System.Web.UI.MasterPage
    {
        string Active_Menu
        {
            get
            {
                return hidMenuItem.Value;
            }
            set
            {
                hidMenuItem.Value = value;
            }
        }

        bool Show_Login
        {
            get
            {
                return UserPanel.Visible;
            }
            set
            {
                //lblUserName.Text = CookieClass.ReadCookie(CookieClass.CookieDefine.CookieName, CookieClass.CookieDefine.sUserName, "");
                lblUserName.Text = UserClass.ReadCookie(UserClass.SurveyCookie.sCookieName , UserClass.SurveyCookie.sUserName , "");
                if (lblUserName.Text.Trim() != "")
                {
                    UserPanel.Visible = true;
                }
                UserPanel.Visible = false;
            }
        }

        string UserName
        {
            get
            {
                return lblUserName.Text;
            }
            set
            {
                lblUserName.Text = value;
                if (lblUserName.Text.Trim() == "")
                {
                    UserPanel.Visible = false;
                }
                else
                {
                    UserPanel.Visible = true;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string strID = "";

            if (!IsPostBack)
            {
                UserPanel.Visible = false;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                string strPage = Request.Url.LocalPath;
                switch (strPage.ToLower())
                {
                    case "/default.aspx":       // 首頁
                        Session["NAV_INDEX"] = "nav_home";
                        break;
                    case "/resource.aspx":      // 衛教資源
                        Session["NAV_INDEX"] = "nav_resource";
                        break;
                    case "/survey.aspx":        /// 各種調查
                        strID = Request.Url.Query;
                        strID = strID.Replace("?id=", "");
                        Session["NAV_INDEX"]= "nav_waindex" + strID;
                        break;
                    case "/survey_body.aspx":        /// 自覺肌肉骨骼
                        strID = Request.Url.Query;
                        strID = strID.Replace("?id=", "");
                        Session["NAV_INDEX"] = "nav_waindex" + strID;
                        break;
                    case "/userverify.aspx":
                        Session["NAV_INDEX"] = "nav_home";
                        break;
                    default:
                        //hidMenuItem.Value = "nav_home";
                        //Session["NAV_INDEX"] = "nav_home";
                        break;
                }

                if (Session["NAV_INDEX"]==null)
                {
                    hidMenuItem.Value = "nav_home";
                }
                else
                {
                    hidMenuItem.Value = Session["NAV_INDEX"].ToString();
                }

                //CookieClass.WriteCookie(CookieClass.CookieDefine.CookieName, CookieClass.CookieDefine.sMenuID, hidMenuItem.Value);
                //lblUserName.Text = CookieClass.ReadCookie(CookieClass.CookieDefine.CookieName, CookieClass.CookieDefine.sUserName, "");
                UserClass.WriteCookie(UserClass.SurveyCookie.sCookieName , UserClass.SurveyCookie.sMenuID , hidMenuItem.Value);
                lblUserName.Text = UserClass.ReadCookie(UserClass.SurveyCookie.sCookieName , UserClass.SurveyCookie.sUserName , "");
                if (lblUserName.Text.Trim()!="")
                {
                    UserPanel.Visible = true; 
                }
            }
        }
    }
}