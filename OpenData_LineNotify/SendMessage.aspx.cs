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
    public partial class SendMessage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string token = Request.QueryString["token"];
            string message = getopdatapm25();
            Message(token, message);

        }
        private string getopdatapm25()
        {
            string opendataurl = "https://data.epa.gov.tw/api/v2/aqx_p_02?api_key=e8dd42e6-9b8b-43f8-991e-b3dee723a52d&limit=1000&sort=datacreationdate%20desc&format=JSON";
            HttpWebRequest opendatarequest = (HttpWebRequest)WebRequest.Create(opendataurl);
            opendatarequest.Method = "GET";
            // opendatarequest.KeepAlive = true; //是否保持連線
            opendatarequest.ContentType = "application/json; charset=utf-8";
            var opendataresponse = (HttpWebResponse)opendatarequest.GetResponse();
            var responseString = new StreamReader(opendataresponse.GetResponseStream()).ReadToEnd();//回傳JSON
            opendataresponse.Close();
            Root root = new Root();
            Root descJson = JsonConvert.DeserializeObject<Root>(responseString);//反序列化
            List<Record> records = new List<Record>();
            records = descJson.records;


            string message = "";
            foreach (var data in records)
            {
                string concentration = "";
                if (data.county == "新北市" && data.site == "三重")
                {
                    if (Convert.ToInt16(data.pm25) <= 35)
                    {
                        concentration = "(濃度較低)";
                    }
                    else if (Convert.ToInt16(data.pm25) <= 53)
                    {
                        concentration = "(濃度中)";
                    }
                    else
                    {
                        concentration = "(濃度高)";
                    };
                    message += data.county.ToString() + "," + data.site.ToString() + "\nPM2.5濃度為:" + data.pm25.ToString() + concentration + "\n資料時間:" + data.datacreationdate;
                }
            }

            return message;
        }
        private void Message(string Token, string message)
        {
            string Url = "https://notify-api.line.me/api/notify";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.KeepAlive = true; //是否保持連線
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Set("Authorization", "Bearer " + Token);
            string content = "message=\n";
            content += message;
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            response.Close();



        }
        public class Extras
        {
            public string api_key { get; set; }
        }

        public class Field
        {
            public string id { get; set; }
            public string type { get; set; }
            public Info info { get; set; }
        }

        public class Info
        {
            public string label { get; set; }
        }

        public class Links
        {
            public string start { get; set; }
            public string next { get; set; }
        }

        public class Record
        {
            public string site { get; set; }
            public string county { get; set; }
            public string pm25 { get; set; }
            public string datacreationdate { get; set; }
            public string itemunit { get; set; }
        }

        public class Root
        {
            public List<Field> fields { get; set; }
            public string resource_id { get; set; }
            public Extras __extras { get; set; }
            public bool include_total { get; set; }
            public string total { get; set; }
            public string resource_format { get; set; }
            public string limit { get; set; }
            public string offset { get; set; }
            public Links _links { get; set; }
            public List<Record> records { get; set; }
        }
    }
}
