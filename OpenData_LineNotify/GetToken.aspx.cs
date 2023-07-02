using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OpenData_LineNotify
{
    public partial class GetToken : System.Web.UI.Page
    {
        private string _clientid = "請改成自己的";//請改成自己的
        private string _redirecturi = "https://localhost:44371/GetToken.aspx";//port號請改成自己的
        private string _clientsecret = "請改成自己的";//請改成自己的
        protected void Page_Load(object sender, EventArgs e)
        {
            string userCode = Request.QueryString["code"];
            if (userCode != null)
            {

                //string msg = GetTokenFromCode(userCode).ToString();
                //lblMessage.Text = msg;
                string token = goGetToken(userCode).ToString();
                var URL = "https://localhost:44371/SendMessage.aspx?";//port號請改成自己的
                URL += "token=" + token;
                Response.Redirect(URL);
            }

        }
        #region Code取得Token

        private string goGetToken(string Code)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            string Url = "https://notify-bot.line.me/oauth/token";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Method = "POST";
            request.KeepAlive = true; //是否保持連線
            request.ContentType = "application/x-www-form-urlencoded";
            string posturi = "";
            posturi += "grant_type=authorization_code";
            posturi += "&code=" + Code; //Authorize code
            posturi += "&redirect_uri=" + _redirecturi;
            posturi += "&client_id=" + _clientid;
            posturi += "&client_secret=" + _clientsecret;

            byte[] bytes = Encoding.UTF8.GetBytes(posturi);//轉byte[]

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }



            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();//回傳JSON
            responseString = "[" + responseString + "]";
            response.Close();

            var token = JsonConvert.DeserializeObject<JArray>(responseString)[0]["access_token"].ToString();
            return token;
        }


        #endregion
    }
}