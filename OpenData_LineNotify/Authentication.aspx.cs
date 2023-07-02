using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OpenData_LineNotify
{
    public partial class Authentication : System.Web.UI.Page
    {
        private string _clientid = "請改成自己的";//請改成自己的
        private string _redirecturi = "https://localhost:44371/GetToken.aspx";//port號請改成自己的
        protected void Page_Load(object sender, EventArgs e)
        {
            var URL = "https://notify-bot.line.me/oauth/authorize?";
            URL += "response_type=code";
            URL += "&client_id=" + _clientid;
            URL += "&redirect_uri=" + _redirecturi;
            URL += "&scope=notify";
            URL += "&state=1234";
            Response.Redirect(URL);

        }
    }
}