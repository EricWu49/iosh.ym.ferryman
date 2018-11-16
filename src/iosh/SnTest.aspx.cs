using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iosh
{
    public partial class SnTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            ListBox1.Items.Clear();
            for (int i=0; i<10; i++)
            {
                Random Counter = new Random(Guid.NewGuid().GetHashCode());
                
                ListBox1.Items.Add(Counter.Next().ToString());
            }
        }
    }
}