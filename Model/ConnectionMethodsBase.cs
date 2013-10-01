using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace ProcessorsToolkit.Model
{
    internal class ConnectionMethodsBase
    {
        //private CookieCollection _sessionCookies;// = new CookieContainer();

        //public string Username { get; set; }
        //public string Password { get; set; }
        //public bool IsCompleteConnection { get; set; }

        public class GetResponse
        {
            public GetResponse() { ResponseCookies = new CookieCollection(); }
            public string ResponseHtml { get; set; }
            public CookieCollection ResponseCookies { get; set; }
            public string RedirectHeaderVal { get; set; }
        }

        //private string PRMGGet(string targetUrl, string referer = "")
        protected static GetResponse Get(string targetUrl, string host, CookieCollection cookies, string referer = "")
        {
            /*
            First add this: using System.IO.Compression;

            Next add the following to the request headers:
            webRequest.Headers.Add(System.Net.HttpRequestHeader.AcceptEncoding, "gzip");

            After creating a response add this code to get the response stream:
            StreamReader str;
            //The following is to check if the server sending the response supports Gzip
            if (webResponse.Headers.Get("Content-Encoding") != null &&
            webResponse.Headers.Get("Content-Encoding").ToLower() == "gzip")
            {
            str = new StreamReader(new GZipStream(webResponse.GetResponseStream(), CompressionMode.Decompress));
            }
            else
            {
            str = new StreamReader(webResponse.GetResponseStream());
            }
            */
            //TODO: stick in using()
            var sessionRequest = (HttpWebRequest) WebRequest.Create(targetUrl);
            //_sessionRequest.Proxy = null; //Skipping search for proxy saves a lot of time but maybe less durable
            sessionRequest.AllowAutoRedirect = false;
            sessionRequest.Method = "GET";
            sessionRequest.Accept = "text/html, application/xhtml+xml, */*";
            if (referer != String.Empty)
                sessionRequest.Referer = referer;
            sessionRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            sessionRequest.Host = host; //"www.prmglending.net";
            sessionRequest.KeepAlive = true;
            sessionRequest.CookieContainer = new CookieContainer();
            sessionRequest.CookieContainer.Add(cookies);
            sessionRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            //string responseHtml = null;
            var responseObj = new GetResponse();

            using (var sessionResponse = (HttpWebResponse) sessionRequest.GetResponse())
            {
                try
                {
                    if (sessionResponse.Headers["Location"] != null)
                        responseObj.RedirectHeaderVal = sessionResponse.Headers["Location"];
                    

                    if (sessionResponse.Cookies != null && sessionResponse.Cookies.Count != 0)
                        responseObj.ResponseCookies.Add(sessionResponse.Cookies);

                    var responseStream = sessionResponse.GetResponseStream();
                    if (responseStream != null)
                        responseObj.ResponseHtml = new StreamReader(responseStream).ReadToEnd();

                    //if (responseHtml != null)
                    //   _sessionCookies.Add(sessionResponse.Cookies);

                }
                catch (Exception ex)
                {
                }
            }
            return responseObj;
        }

        public class PostResponse
        {
            public PostResponse() { ResponseCookies = new CookieCollection(); }
            public string ResponseHtml { get; set; }
            public CookieCollection ResponseCookies { get; set; }
            public string RedirectHeaderVal { get; set; }
        }

        public static PostResponse Post(string targetUrl,  string host, CookieCollection cookies, string postData, string referer)
        {
            var sessionRequest = (HttpWebRequest) WebRequest.Create(targetUrl);


            //sessionRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
            //_sessionRequest.Proxy = null;
            sessionRequest.AllowAutoRedirect = false;
            sessionRequest.Method = "POST";
            sessionRequest.Accept = "text/html, application/xhtml+xml, */*";
            sessionRequest.Referer = referer;
            sessionRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            sessionRequest.ContentType = "application/x-www-form-urlencoded";
            sessionRequest.Host = host; // "www.prmglending.net";
            sessionRequest.KeepAlive = true;
            sessionRequest.CookieContainer = new CookieContainer();
            sessionRequest.CookieContainer.Add(cookies);

            var enconding = new ASCIIEncoding();
            var postDataBytes = enconding.GetBytes(postData);
            sessionRequest.ContentLength = postDataBytes.Length;

            sessionRequest.GetRequestStream().Write(postDataBytes, 0, postDataBytes.Length);

            //Refer to the other class we use for posting? Imageflow connector?
            /*using (var writer = new StreamWriter(sessionRequest.GetRequestStream(), Encoding.ASCII))
            {
                writer.Write(postData);
            }*/

            //string responseHtml = null;
            var responseObj = new PostResponse();

            using (var sessionResponse = (HttpWebResponse) sessionRequest.GetResponse())
            {
                try
                {
                    if (sessionResponse.Headers["Location"] != null)
                        responseObj.RedirectHeaderVal = sessionResponse.Headers["Location"];

                    if (sessionResponse.Cookies != null && sessionResponse.Cookies.Count != 0)
                        responseObj.ResponseCookies.Add(sessionResponse.Cookies);

                    var responseStream = sessionResponse.GetResponseStream();
                    if (responseStream != null)
                        responseObj.ResponseHtml = new StreamReader(responseStream).ReadToEnd();
                }
                catch (Exception ex)
                {
                }
            }
            return responseObj;
        }

        public class HeadResponse
        {
            public HeadResponse(){ResponseCookies = new CookieCollection();}
            public CookieCollection ResponseCookies { get; set; }
            public string RedirectHeaderVal { get; set; }
        }

        public static HeadResponse Head(string targetUrl, string host, CookieCollection cookies, string referer)
        {
            var sessionRequest = (HttpWebRequest) WebRequest.Create(targetUrl);

            //_sessionRequest.Proxy = null;
            sessionRequest.AllowAutoRedirect = false;
            sessionRequest.Method = "HEAD";
            sessionRequest.Accept = "text/html, application/xhtml+xml, */*";
            if (referer != String.Empty)
                sessionRequest.Referer = referer;
            sessionRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            sessionRequest.Host = host; // "www.prmglending.net";
            sessionRequest.KeepAlive = true;
            sessionRequest.CookieContainer = new CookieContainer();
            sessionRequest.CookieContainer.Add(cookies);
            sessionRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            var responseObj = new HeadResponse();

            using (var sessionResponse = (HttpWebResponse) sessionRequest.GetResponse())
            {
                try
                {
                    if (sessionResponse.Headers["Location"] != null)
                        responseObj.RedirectHeaderVal = sessionResponse.Headers["Location"];
                    //ParseRedirectHeader(sessionResponse.Headers["Location"]);

                    if (sessionResponse.Cookies != null && sessionResponse.Cookies.Count != 0)
                        responseObj.ResponseCookies.Add(sessionResponse.Cookies);

                    // if (sessionResponse.Cookies != null)
                    //    _sessionCookies.Add(sessionResponse.Cookies);
                }
                catch (Exception ex)
                {
                }
            }
            return responseObj;
        }
    }
}
