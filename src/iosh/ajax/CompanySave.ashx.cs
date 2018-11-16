using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iosh.ajax
{
    /// <summary>
    /// CompanySave 的摘要描述
    /// </summary>
    public class CompanySave : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class CompanyClass
    {
        int companyid = 0;
        string companysn = "";
        string companyname = "";
        string taxid = "";
        string disabled = "N";
    }

    public class UserClass
    {

    }
}