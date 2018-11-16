using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ferryman.Utility;

namespace Admin
{
    public partial class DeleteResourceDB : System.Web.UI.Page
    {
        int _SituationID = 0;
        int _StrategyID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["situation"] != null)
            {
                _SituationID = Convert.ToInt32(Request["situation"]);
                if (Request["strategy"] != null)
                {
                    _StrategyID = Convert.ToInt32(Request["strategy"]);
                    Remove_Data();
                }
                else
                {
                    Response.Write("資料不足。");
                }
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
                string strFilter = "";
                strFilter = "SituationID=" + _SituationID.ToString() + " And StrategyID=" + _StrategyID.ToString();
                if (DeleteData.Delete("ResourceDB", strFilter) == true)
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