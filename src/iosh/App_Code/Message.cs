using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace Ferryman.Utility
{
    /// <summary>
    ///  類別名稱 : Message
    ///  類別用途 : 將後端程式要顯示的訊息傳遞到前端
    ///  改版記錄 : 2011/6/8
    ///  判斷如果使用者是簡體語系，自動將訊息內容將繁體字轉為簡體字
    /// </summary>

    public class Message
    {

        public static void MsgShow(System.Web.UI.Page refPage, string MessageString)
        {
            string strMessage = null;
            string strScript = "";

            //MessageString = Regex.Replace(MessageString, "'", "\'")
            strMessage = Convert_Message(MessageString);
            strScript = "<script language='javascript'>";
            strScript = strScript + "window.alert('" + strMessage + "');";
            strScript = strScript + "</script>";
            refPage.ClientScript.RegisterClientScriptBlock(refPage.GetType(), "Message", strScript, false);
        }

        public static void MsgShow(System.Web.UI.Page refPage, string MessageString, string[] ParmArray)
        {
            string strMessage = null;
            string strScript = "";

            //MessageString = Regex.Replace(MessageString, "'", "\'")
            if (ParmArray.GetLength(0) > 0)
            {
                for (int i = 0; i <= ParmArray.GetUpperBound(0); i++)
                {
                    MessageString = MessageString.Replace("<@" + (i + 1).ToString() + "/>", ParmArray[i]);
                }
            }
            strMessage = Convert_Message(MessageString);

            strScript = "<script language='javascript'>";
            strScript = strScript + "window.alert('" + strMessage + "');";
            strScript = strScript + "</script>";
            refPage.ClientScript.RegisterClientScriptBlock(refPage.GetType(), "Error", strScript, false);
        }

        public static void ShowError(Page refPage, string FunctionName, string ErrorCode)
        {
            string strMessage = null;
            string MessageString = null;

            ErrorCode = Convert_Message(ErrorCode);

            MessageString = "系統執行過程發生錯誤。" + "\\n" + "副程式：" + FunctionName + "\\n" + "錯誤訊息：" + ErrorCode;
            strMessage = "<script language='javascript'>";
            strMessage = strMessage + "window.alert('" + MessageString + "');";
            strMessage = strMessage + "</script>";
            refPage.ClientScript.RegisterClientScriptBlock(refPage.GetType(), "Error", strMessage, false);

            ShareFunction.PutLog(FunctionName, ErrorCode);
        }

        public static void ShowError(Page refPage, string FunctionName, string ErrorCode, string ErrorMessage)
        {
            string strMessage = null;
            string MessageString = null;

            ErrorCode = Convert_Message(ErrorCode);

            MessageString = "系統執行過程發生錯誤。" + "\\n" + "副程式：" + FunctionName + "\\n" + "錯誤訊息：" + ErrorCode;
            strMessage = "<script language='javascript'>";
            strMessage = strMessage + "window.alert('" + ErrorMessage + "');";
            strMessage = strMessage + "</script>";
            refPage.ClientScript.RegisterClientScriptBlock(refPage.GetType(), "Error", strMessage, false);

            ShareFunction.PutLog(FunctionName, ErrorCode);
        }

        private static string Convert_Message(string strMessage)
        {
            //Return System.Web.HttpUtility.HtmlEncode(strMessage.Replace(vbCrLf, "\n"))
            strMessage = Regex.Replace(strMessage, "'", "\\'");

            return strMessage.Replace(VB.sCR(), "\\n");
        }

        public static void MsgShow_And_Redirect(System.Web.UI.Page refPage, string MessageString, string URL, bool Parent = false)
        {
            string strMessage = null;
            string strScript = "";

            //MessageString = Regex.Replace(MessageString, "'", "\'")
            strMessage = Convert_Message(MessageString);

            strScript = "<script language='javascript'>" + VB.sCR();
            strScript = strScript + "window.alert('" + strMessage + "');" + VB.sCR();
            if (Parent)
            {
                strScript = strScript + "window.parent.location='" + URL + "';" + VB.sCR();
            }
            else
            {
                strScript = strScript + "window.location='" + URL + "';" + VB.sCR();
            }
            strScript = strScript + "</script>" + VB.sCR();
            refPage.ClientScript.RegisterClientScriptBlock(refPage.GetType(), "Error", strScript, false);
        }

        public static string MessageReplace(string MessageString, string[] ParmArray)
        {
            if (ParmArray.GetLength(0) > 0)
            {
                for (int i = 0; i <= ParmArray.GetUpperBound(0); i++)
                {
                    MessageString = MessageString.Replace("<@" + (i + 1).ToString() + "/>", ParmArray[i]);
                }
            }
            return MessageString;
        }
    }
}