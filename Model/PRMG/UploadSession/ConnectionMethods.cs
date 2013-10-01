using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ProcessorsToolkit.Model.PRMG.UploadSession
{
    class ConnectionMethods : ConnectionMethodsBase
    {
        private const string PRMGHost = "www.prmglending.net";
        private const string ImgFlowHost = "imageflow.prmg.net";

        static protected internal GetResponse PRMGGet(string targetUrl, CookieCollection cookies, string referer = "")
        {
            return Get(targetUrl, PRMGHost, cookies, referer);
        }

        protected internal static PostResponse PRMGPost(string targetUrl, CookieCollection cookies, string postData, string referer = "")
        {
            return Post(targetUrl, PRMGHost, cookies, postData, referer);
        }

        protected internal static HeadResponse PRMGHead(string targetUrl, CookieCollection cookies, string referer = "")
        {
            return Head(targetUrl, PRMGHost, cookies, referer);
        }

        protected internal static GetResponse ImgFlowGet(string targetUrl, CookieCollection cookies, string referer = "")
        {
            return Get(targetUrl, ImgFlowHost, cookies, referer);
        }

        protected internal static PostResponse ImgFlowPost(string targetUrl, CookieCollection cookies, string postData, string referer = "")
        {
            return Post(targetUrl, ImgFlowHost, cookies, postData, referer);
        }
    }
}
