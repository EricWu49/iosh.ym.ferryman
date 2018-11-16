﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ferryman.Utility;

namespace Admin
{
    public partial class DeleteSituationRule : System.Web.UI.Page
    {
        int _UniqueID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["id"] != null)
            {
                _UniqueID = Convert.ToInt32(Request["id"]);
                Remove_Data();
            }
            else
            {
                Response.Write("資料不足。");
            }
        }

        private void Remove_Data()
        {
            try
            {
                if (DeleteData.DeleteByID("SituationRule", "RuleID", _UniqueID) == true)
                {
                    Response.Write("");
                }
                else
                {
                    if (DeleteData.ErrorMessage != "")
                    {
                        ShareFunction.PutLog("Remove_Data", DeleteData.ErrorMessage);
                        Response.Write("移除失敗。");
                    }
                    else
                    {
                        Response.Write("查無資料");
                    }
                }
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Remove_Data", ex.Message);
                Response.Write("移除錯誤。");
            }
        }
    }
}