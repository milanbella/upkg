using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Vmachal
{
    namespace ServerApi
    {
        public class Config
        {
            public static Config config = new Config("http://localhost:3000/api");

            public readonly string API_URL;

            Config(string apiUrl)
            {
                this.API_URL = apiUrl;
            }

        }

        public class HttpGetJsonCall
        {
            private static readonly string CLASS_NAME = "HttpGetJsonCall";
            private string url;
            private UnityWebRequest wr;
            private NameValueCollection query;

            public string resultJsonStr;

            HttpGetJsonCall(string url)
            {
                this.wr = new UnityWebRequest();
                this.url = url;
                this.wr.method = UnityWebRequest.kHttpVerbGET;
                this.wr.downloadHandler = new DownloadHandlerBuffer();

                this.wr.useHttpContinue = false;
                this.wr.redirectLimit = 0;  // disable redirects
                this.wr.timeout = 60;       // don't make this small, web requests do take some time

                this.query = HttpUtility.ParseQueryString(string.Empty);
            }

            void AddQueryParam(string key, string val)
            {
                this.query[key] = val;
            }

            IEnumerator Call()
            {
                string METHOD = "Call()";
                string url;
                if (this.query.Count > 0)
                {
                    url = $"{this.url}?{this.query.ToString()}";
                }
                else
                {
                    url = $"{this.url}";
                }
                this.wr.url = url;

                yield return this.wr.SendWebRequest();

                if (this.wr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"{HttpGetJsonCall.CLASS_NAME}:{METHOD}: error: {this.wr.error}, url: {url}, status: {this.wr.responseCode}");
                    throw new Exception($"error: {this.wr.error}, url: {url}, status: {this.wr.responseCode}");
                }
                else
                {
                    string contenType = this.wr.GetResponseHeader("Content-Type");
                    if (!contenType.Contains("json"))
                    {
                        Debug.LogError($"{HttpGetJsonCall.CLASS_NAME}:{METHOD}: error: response content type is not json, url: {url}");
                        throw new Exception($"error: response content type is not json, url: {url}");

                    }
                    this.resultJsonStr = this.wr.downloadHandler.text;
                }

            }

        }

        public class HttpPostJsonCall
        {
            private static readonly string CLASS_NAME = "HttpPostJsonCall";
            private string url;
            private UnityWebRequest wr;
            private NameValueCollection query;

            public string resultJsonStr;

            public HttpPostJsonCall(string url, string requestJsonStr)
            {
                this.wr = new UnityWebRequest();
                this.url = url;
                this.wr.method = UnityWebRequest.kHttpVerbPOST;

                Byte[] payload = Encoding.UTF8.GetBytes(requestJsonStr);
                UploadHandler uploadHandler = new UploadHandlerRaw(payload);
                uploadHandler.contentType = "application/json";
                this.wr.uploadHandler = uploadHandler;
                this.wr.SetRequestHeader("Content-Type", "application/json");

                this.wr.downloadHandler = new DownloadHandlerBuffer();

                this.wr.useHttpContinue = false;
                this.wr.redirectLimit = 0;  // disable redirects
                this.wr.timeout = 60;       // don't make this small, web requests do take some time

                this.query = HttpUtility.ParseQueryString(string.Empty);
            }

            public void AddQueryParam(string key, string val)
            {
                this.query[key] = val;
            }

            public IEnumerator Call()
            {
                string METHOD = "Call()";
                string url;
                if (this.query.Count > 0)
                {
                    url = $"{this.url}?{this.query.ToString()}";
                }
                else
                {
                    url = $"{this.url}";
                }
                this.wr.url = url;

                yield return this.wr.SendWebRequest();

                if (this.wr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"{HttpPostJsonCall.CLASS_NAME}:{METHOD}: error: {this.wr.error}, url: {url}, status: {this.wr.responseCode}");
                    throw new Exception($"error: {this.wr.error}, url: {url}, status: {this.wr.responseCode}");
                }
                else
                {
                    string contenType = this.wr.GetResponseHeader("Content-Type");
                    if (!contenType.Contains("json"))
                    {
                        Debug.LogError($"{HttpPostJsonCall.CLASS_NAME}:{METHOD}: error: response content type is not json, url: {url}");
                        throw new Exception($"error: response content type is not json, url: {url}");

                    }
                    this.resultJsonStr = this.wr.downloadHandler.text;
                }

            }

        }

        public class UserApi
        {

            public class UserVerifyPasswordAndIssueJwt
            {
                public readonly static string CLASS_NAME = "UserVerifyPasswordAndIssueJwt";

                [Serializable]
                public class Request
                {
                    public string applicationName;
                    public string deviceId;
                    public string userName;
                    public string password;
                }

                [Serializable]
                public class Response
                {
                    public string username;
                    public string email;
                    public string jwt;
                }
                public readonly Request request;
                public Response response;

                public UserVerifyPasswordAndIssueJwt(Request request)
                {
                    this.request = request;
                }

                public IEnumerator Call()
                {
                    string requestJsonStr = JsonUtility.ToJson(request);
                    HttpPostJsonCall http = new HttpPostJsonCall($"{Config.config.API_URL}/user/UserVerifyPasswordAndIssueJwt", requestJsonStr);

                    yield return http.Call();

                    this.response = JsonUtility.FromJson<Response>(http.resultJsonStr);

                }

            }

        }

    }

}
