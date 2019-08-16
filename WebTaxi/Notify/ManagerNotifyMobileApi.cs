using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;

namespace WebTaxi.Notify
{
    public class ManagerNotifyMobileApi
    {
        private WebRequest tRequest = null;

        public ManagerNotifyMobileApi()
        {
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAlF2bG04:APA91bH3MERG96ODxo3MkgOal8zfT6S9NfIIwsQY-XNfa0K9lI_RqwfV6l9zbh1vzM019Z_cMIhQuqKCqe0v913I9QleeuCQZwdcJWKoe4g8WO_8ijdyqZqj1dO4w8vIj8FZgNCkU6MN"));
            tRequest.Headers.Add(string.Format("Sender: id={0}", "637225605966"));
            tRequest.ContentType = "application/json";
        }

        private void InitReqvest()
        {
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAlF2bG04:APA91bH3MERG96ODxo3MkgOal8zfT6S9NfIIwsQY-XNfa0K9lI_RqwfV6l9zbh1vzM019Z_cMIhQuqKCqe0v913I9QleeuCQZwdcJWKoe4g8WO_8ijdyqZqj1dO4w8vIj8FZgNCkU6MN"));
            tRequest.Headers.Add(string.Format("Sender: id={0}", "637225605966"));
            tRequest.ContentType = "application/json";
        }

        public void SendNotyfyStatusPickup(string tokenShope, string click_action, string body, string title)
        {
            if (tokenShope != null && tokenShope != "")
            {
                var payload = new
                {
                    to = tokenShope,
                    content_available = true,
                    notification = new
                    {
                        click_action = click_action,
                        body = body,
                        title = title,
                    },
                };
                string postbody = JsonConvert.SerializeObject(payload).ToString();
                byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    string sResponseFromServer = tReader.ReadToEnd();
                                }
                        }
                    }
                }
                InitReqvest();
            }
        }
    }
}
