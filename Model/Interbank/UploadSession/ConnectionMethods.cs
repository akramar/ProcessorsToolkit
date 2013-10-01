using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ProcessorsToolkit.Model.Interbank.UploadSession
{
    class ConnectionMethods : ConnectionMethodsBase
    {
        // GET https://portal.interbankwholesale.com/IBPortal/Login.aspx HTTP/1.1
        // Host: portal.interbankwholesale.com
        // Connection: keep-alive
        // Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
        // User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1652.0 Safari/537.36
        // Accept-Encoding: gzip,deflate,sdch
        // Accept-Language: en-US,en;q=0.8

        private const string Host = "portal.interbankwholesale.com";

        static protected internal GetResponse IBWGet(string targetUrl, CookieCollection cookies, string referer = "")
        {
            return Get(targetUrl, Host, cookies, referer);
        }

        protected internal static PostResponse IBWPost(string targetUrl, CookieCollection cookies, string postData, string referer = "")
        {
            return Post(targetUrl, Host, cookies, postData, referer);
        }

        protected internal static HeadResponse IBWHead(string targetUrl, CookieCollection cookies, string referer = "")
        {
            return Head(targetUrl, Host, cookies, referer);
        }


    }
}
