using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Configuration;
using System.IO;
using System.Text.RegularExpressions;//導入命名空間(正則表達式)

namespace Ferryman.Utility
{
    public static class ShareFunction
    {
        /// <summary>
        /// 記錄程式執行過程的紀錄
        /// </summary>
        /// <param name="FunctionName">執行的程序名稱</param>
        /// <param name="strValue">紀錄的訊息文字</param>
        public static void PutLog(string FunctionName, string strValue)
        {
            string strLogFileName = null;
            string strLogDirectory = null;
            string IP = null;
            string LOG_PAGE_NAME = null;

            try
            {
                if ((System.Web.HttpContext.Current == null))
                {
                    IP = "0.0.0.0";
                    LOG_PAGE_NAME = "N/A";
                    strLogDirectory = "C:\\Log";
                }
                else
                {
                    IP = System.Web.HttpContext.Current.Request.UserHostAddress;
                    LOG_PAGE_NAME = System.Web.HttpContext.Current.Request.FilePath;

                    strLogDirectory = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\LOG";
                }

                if (!System.IO.Directory.Exists(strLogDirectory))
                {
                    System.IO.Directory.CreateDirectory(strLogDirectory);
                }

                strLogFileName = strLogDirectory + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                strValue = DateTime.Now.ToString("HH:mm:ss") + VB.sTAB() + IP + VB.sTAB() + LOG_PAGE_NAME + VB.sTAB() + FunctionName + VB.sTAB() + strValue;

                if (System.IO.File.Exists(strLogFileName))
                {
                    System.IO.StreamWriter sr = System.IO.File.AppendText(strLogFileName);
                    sr.WriteLine(strValue);
                    sr.Close();
                }
                else
                {
                    System.IO.StreamWriter sr = System.IO.File.CreateText(strLogFileName);
                    sr.WriteLine(strValue);
                    sr.Close();
                }

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 取得Web.Config的設定值
        /// </summary>
        /// <param name="KeyName">設定鍵值</param>
        /// <returns>設定的參數值</returns>
        public static string GetConfig(string KeyName)
        {
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

            if ((config.AppSettings.Settings[KeyName] == null))
            {
                return null;
            }
            else
            {
                return config.AppSettings.Settings[KeyName].Value;
            }
        }

        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream StringToStream(string src)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(src);
            return new MemoryStream(byteArray);
        }

        public static void Prevent_GoBack()
        {
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
            System.Web.HttpContext.Current.Response.Expires = 0;
            System.Web.HttpContext.Current.Response.CacheControl = "no-cache";
        }

        public static object Register_Close_Script(Page myPage, string WindowName = "closewin", string MessageString = "")
        {
            string strScript = "";
            if (!string.IsNullOrEmpty(MessageString))
            {
                strScript = "<script type=\"text/javascript\">" +   Environment.NewLine  + "window.alert('" + MessageString + "');" +   Environment.NewLine + "window.opener=null;   " +   Environment.NewLine + "window.open('','_self');   " + "window.close(); " + "</script>";
            }
            else
            {
                strScript = "<script type=\"text/javascript\">" +   Environment.NewLine + "window.opener=null;   " +   Environment.NewLine + "window.open('','_self');   " + "window.close(); " + "</script>";
            }
            if (!myPage.ClientScript.IsClientScriptBlockRegistered(WindowName))
            {
                myPage.ClientScript.RegisterClientScriptBlock(myPage.GetType(), WindowName, strScript);
            }
            return true;
        }

        public static object Register_Close_And_Redirect_Script(Page myPage, string ParentLocation, string MessageString = "")
        {
            string strScript = "";
            if (string.IsNullOrEmpty(MessageString))
            {
                strScript = "<script type=\"text/javascript\">" +   Environment.NewLine + "window.parent.location='" + ParentLocation + "';" +   Environment.NewLine + "window.opener=null;   " +   Environment.NewLine + "window.open('','_self');   " + "window.close(); " + "</script>";
            }
            else
            {
                strScript = "<script type=\"text/javascript\">" +   Environment.NewLine + "window.alert('" + MessageString + "');" +   Environment.NewLine + "window.parent.location='" + ParentLocation + "';" +   Environment.NewLine + "window.opener=null;   " +   Environment.NewLine + "window.open('','_self');   " + "window.close(); " + "</script>";
            }
            if (!myPage.ClientScript.IsClientScriptBlockRegistered("self_close"))
            {
                myPage.ClientScript.RegisterClientScriptBlock(myPage.GetType(), "self_close", strScript);
            }
            return true;
        }

        public static object Register_Close_And_Reload_Script(Page myPage, string MessageString = "")
        {
            string strScript = "";
            if (string.IsNullOrEmpty(MessageString))
            {
                strScript = "<script type=\"text/javascript\">" +   Environment.NewLine + "window.opener.location.reload();" +   Environment.NewLine + "window.opener=null;   " +   Environment.NewLine + "window.open('','_self');   " + "window.close(); " + "</script>";
            }
            else
            {
                strScript = "<script type=\"text/javascript\">" +   Environment.NewLine + "window.alert('" + MessageString + "');" +   Environment.NewLine + "window.opener.location.reload();" +   Environment.NewLine + "window.opener=null;   " +   Environment.NewLine + "window.open('','_self');   " + "window.close(); " + "</script>";
            }
            if (!myPage.ClientScript.IsClientScriptBlockRegistered("self_close"))
            {
                myPage.ClientScript.RegisterClientScriptBlock(myPage.GetType(), "self_close", strScript);
            }
            return true;
        }

        public static void Register_Redirect_Script(Page myPage, string URLPath, bool Parent = false)
        {
            string strScript = "";

            strScript = "<script language='javascript'>" +   Environment.NewLine;
            if (Parent)
            {
                strScript = strScript + "window.parent.location='" + URLPath + "';" +   Environment.NewLine;
            }
            else
            {
                strScript = strScript + "window.location='" + URLPath + "';" +   Environment.NewLine;
            }
            strScript = strScript + "</script>" +   Environment.NewLine;
            myPage.ClientScript.RegisterClientScriptBlock(myPage.GetType(), "Redirect", strScript, false);
        }

        public static void Register_ReLoad_Script(Page myPage, bool Parent = false)
        {
            string strScript = "";

            strScript = "<script language='javascript'>" +   Environment.NewLine;
            if (Parent)
            {
                strScript = strScript + "window.opener.location.reload();" +   Environment.NewLine;
            }
            else
            {
                strScript = strScript + "window.location.reload();" +   Environment.NewLine;
            }
            strScript = strScript + "</script>" +   Environment.NewLine;
            myPage.ClientScript.RegisterClientScriptBlock(myPage.GetType(), "Reload", strScript, false);
        }

        public static System.Web.UI.Control FindControlEx(System.Web.UI.Control Parent, string ID)
        {
            System.Web.UI.Control oCtrl = null;
            // 先使用 FindControl 去尋找指定的子控制項
            oCtrl = Parent.FindControl(ID);

            // 若尋找不到則往下層遞迴方式去尋找
            if ((oCtrl == null))
            {
                foreach (System.Web.UI.Control oChildCtrl in Parent.Controls)     // 以遞迴方式呼叫原函式
                {
                    oCtrl = FindControlEx(oChildCtrl, ID);      // 若有尋找到指定控制項則離開迴圈
                    if (!(oCtrl == null))
                    {
                        break;
                    }
                }
            }

            return oCtrl;
        }

        public static string GetNowTimeValue()
        {
            Int64 timeid;
            DateTime myDate = DateTime.UtcNow;
            timeid = Convert.ToInt64(myDate.Ticks - new TimeSpan(Convert.ToDateTime("1970/1/1").Ticks).TotalMilliseconds);
            return timeid.ToString();
        }

        //定義一個函數,作用:判斷strNumber是否為數字,是數字返回True,不是數字返回False
        public static bool IsNumeric(String strNumber)
        {
            if (strNumber.Trim()=="")
            {
                return false;
            }
            else
            {
                Regex NumberPattern = new Regex("[^0-9.-]");
                return !NumberPattern.IsMatch(strNumber);
            }
        }

        public static void Register_Open_Script(Page myPage, string URLPath)
        {
            string strScript = "";

            strScript = "<script language='javascript'>" + Environment.NewLine; 
            strScript = strScript + "window.open('" + URLPath + "');" +  Environment.NewLine;
            strScript = strScript + "</script>" +  Environment.NewLine;
            myPage.ClientScript.RegisterClientScriptBlock(myPage.GetType(), "winopen", strScript, false);
        }
    }

    public static class VB
    {
        public static string sTAB()
        {
            return Convert.ToChar(9).ToString();
        }

        public static string sCR()
        {
            return Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString();
        }
    }
}