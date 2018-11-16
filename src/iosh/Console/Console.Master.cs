using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iosh.Console
{
    public partial class Console : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["IsLogin"] == null)
                {
                    // 還未登入
                    lblLoginUser.Visible = false;

                    if (Request.Path.ToLower().IndexOf("default.aspx") > 0)
                    {
                        // 目前在登入頁面，隱藏登入後才可以顯示的選單
                        string strScript = "";
                        strScript = "$('.login-menu').css('display', 'none');";
                        if (!this.Page.ClientScript.IsClientScriptBlockRegistered("console_script"))
                        {
                            this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "console_script", strScript, true);
                        }
                    }
                    else
                    {
                        if (Request.Path.ToLower().IndexOf("newcompany.aspx") > 0)
                        {
                            // 新公司註冊
                            string strScript = "";
                            strScript = "$('.login-menu').css('display', 'none');";
                            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("console_script"))
                            {
                                this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "console_script", strScript, true);
                            }
                        }
                        else
                        {
                            // 目前不是登入頁面，轉址到登入頁面
                            Response.Redirect("Default.aspx");
                        }
                    }
                    //pnlMenu.Visible = false;
                    //lblLoginUser.Visible = false;
                }
                else
                {
                    // 已經登入
                    //pnlMenu.Visible = true;
                    lblLoginUser.Visible = true;
                    lblLoginUser.Text = Session["UserName"].ToString();
                }


                //if (Request.Path.ToLower().IndexOf("default.aspx") > 0)
                //{
                //    pnlMenu.Visible = false;
                //    lblLoginUser.Visible = false;
                //}
                //else
                //{
                //    if (Session["IsLogin"] == null)
                //    {
                //        Response.Redirect("Default.aspx");
                //        //pnlMenu.Visible = false;
                //        //lblLoginUser.Visible = false;
                //    }
                //    else
                //    {
                //        pnlMenu.Visible = true;
                //        lblLoginUser.Visible = true;
                //        lblLoginUser.Text = Session["UserName"].ToString();
                //    }
                //}
            }
        }
    }
}