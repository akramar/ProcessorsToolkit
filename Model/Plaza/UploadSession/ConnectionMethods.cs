using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ProcessorsToolkit.Model.Plaza.UploadSession
{
    class ConnectionMethods : ConnectionMethodsBase
    {
        private const string LoanScoreHeader = "au.loan-score.com";
        private const string DocuTracHeader = "docutrac.plazahomemortgage.com:8080";

        static protected internal GetResponse LoanScoreGet(string targetUrl, CookieCollection cookies, string referer = "")
        {
            return Get(targetUrl, LoanScoreHeader, cookies, referer);
        }

        protected internal static PostResponse LoanScorePost(string targetUrl, CookieCollection cookies, string postData, string referer = "")
        {
            return Post(targetUrl, LoanScoreHeader, cookies, postData, referer);
        }

        protected internal static HeadResponse LoanScoreHead(string targetUrl, CookieCollection cookies, string referer = "")
        {
            return Head(targetUrl, LoanScoreHeader, cookies, referer);
        }

        static protected internal GetResponse DocuTracGet(string targetUrl, CookieCollection cookies, string referer = "")
        {
            return Get(targetUrl, DocuTracHeader, cookies, referer);
        }

        protected internal static PostResponse DocuTracPost(string targetUrl, CookieCollection cookies, string postData, string referer = "")
        {
            return Post(targetUrl, DocuTracHeader, cookies, postData, referer);
        }

        protected internal static HeadResponse DocuTracHead(string targetUrl, CookieCollection cookies, string referer = "")
        {
            return Head(targetUrl, DocuTracHeader, cookies, referer);
        }
    }
}
