using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Runtime.InteropServices;

namespace ImageFlowUploader
{
    public class PRMGLoginSession
    {
        HttpWebRequest sessionRequest = null;// = (HttpWebRequest)WebRequest.Create("");
        HttpWebResponse sessionResponse = null;
        CookieContainer sessionCookies = new CookieContainer();
        public string username, password, secretQuestion_type, secretQuestion_question, secretQuestion_answer, applyId;
        public bool IsCompleteConnection { get; set; }
        public delegate void PRMGSessionLoggedIn(object s, EventArgs e);
        public event PRMGSessionLoggedIn PRMGSessionHasLoggedIn;

        public PRMGLoginSession() {
            this.IsCompleteConnection = false;
        }

        //Step 1
        public void Step1_FetchHomepage()
        {
            System.Diagnostics.Debug.WriteLine("Hit Step1_FetchHomepage");
            //  GET https://www.prmglending.net/ HTTP/1.1
            //Accept: text/html, application/xhtml+xml, *
            /*
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            Accept-Encoding: gzip, deflate
            Host: www.prmglending.net
            Connection: Keep-Alive
            */

            string responseHtml = this.GetMethod("https://www.prmglending.net/", "");
            if (!String.IsNullOrEmpty(responseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseHtml);
            }
                
        }

        //Step 2
        public void Step2_PostUsername()
        {
            System.Diagnostics.Debug.WriteLine("Hit Step2_PostUsername");

            //POST https://www.prmglending.net/acctLoginPro.aspx HTTP/1.1
            //Accept: text/html, application/xhtml+xml, *
            /*
            Referer: https://www.prmglending.net/
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            Content-Type: application/x-www-form-urlencoded
            Accept-Encoding: gzip, deflate
            Host: www.prmglending.net
            Content-Length: 42 --need to set
            Connection: Keep-Alive
            Cache-Control: no-cache
            Cookie: ASP.NET_SessionId=u4t2vtcpesad13cplhcczy20; sUrlExtCid=0

            UserName=alakra&Authentication=MULTIFACTOR
            */

            string postData = "UserName=" + Globals.OurPRMGSession.username + "&Authentication=MULTIFACTOR";

            string responseHtml = this.PostMethod("https://www.prmglending.net/acctLoginPro.aspx", "https://www.prmglending.net/", postData);
            if (!String.IsNullOrEmpty(responseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseHtml);
            }

        }

        //Step 3
        public void Step3_ChallengeQuestion()
        {
            //GET https://www.prmglending.net/secretQuestion.aspx HTTP/1.1
            //Accept: text/html, application/xhtml+xml, *
            /*
            Referer: https://www.prmglending.net/
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            Accept-Encoding: gzip, deflate
            Host: www.prmglending.net
            Connection: Keep-Alive
            Cache-Control: no-cache
            Cookie: ASP.NET_SessionId=u4t2vtcpesad13cplhcczy20; sUrlExtCid=0
            */
            System.Diagnostics.Debug.WriteLine("Hit Step3_ChallengeQuestion");


            string responseHtml = this.GetMethod("https://www.prmglending.net/secretQuestion.aspx", "https://www.prmglending.net/");


            if (!String.IsNullOrEmpty(responseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseHtml);

                //Find the question submit form

                //HtmlNode questionTypeNode = responseHtmlDoc.DocumentNode.SelectSingleNode("pqestionCode");

                foreach (HtmlNode node in responseHtmlDoc.DocumentNode.Descendants("input"))
                    if (node.Attributes["name"] != null && node.Attributes["name"].Value == "pqestionCode")                    
                        this.secretQuestion_type = node.Attributes["value"].Value;


                List<HtmlNode> paraNodes = responseHtmlDoc.DocumentNode.Descendants("p").ToList();



                

                //HtmlNode firstP = responseHtmlDoc.DocumentNode.SelectSingleNode("/form/p[1]");

                if (paraNodes.Count > 0)// (firstP != null)
                {
                    //secretQuestion_type = questionTypeNode.Attributes["value"].Value;
                    //secretQuestion_question = firstP.InnerText;
                    //secretQuestion_answer = "apples";

                    this.secretQuestion_question = paraNodes[0].InnerText.Replace(System.Environment.NewLine, "");

                }
            }


        }

        //Step 4
        public void Step4_PostSecretAnswer()
        {
            //POST https://www.prmglending.net/secretQuestion.aspx HTTP/1.1
            //Accept: text/html, application/xhtml+xml, *
            /*
            Referer: https://www.prmglending.net/secretQuestion.aspx
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            Content-Type: application/x-www-form-urlencoded
            Accept-Encoding: gzip, deflate
            Host: www.prmglending.net
            Content-Length: 62
            Connection: Keep-Alive
            Cache-Control: no-cache
            Cookie: ASP.NET_SessionId=u4t2vtcpesad13cplhcczy20; sUrlExtCid=0
            
            IsPostback=1&pqestionCode=hometownnewspaper&pAnswerCont=apples
            */
            System.Diagnostics.Debug.WriteLine("Hit Step4_PostSecretAnswer");

            string postData = "IsPostback=1&pqestionCode=" + this.secretQuestion_type + "&pAnswerCont=" + this.secretQuestion_answer; 
            string responseHtml = this.PostMethod("https://www.prmglending.net/secretQuestion.aspx", 
                "https://www.prmglending.net/secretQuestion.aspx", postData);

            if (!String.IsNullOrEmpty(responseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseHtml);
            }
        }

        //Step 5
        public void Step5_GetPasswordLoginPage()
        {
            //GET https://www.prmglending.net/secureLogin.aspx HTTP/1.1
            //Accept: text/html, application/xhtml+xml, */* 
            /*Referer: https://www.prmglending.net/secretQuestion.aspx
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            Accept-Encoding: gzip, deflate
            Host: www.prmglending.net
            Connection: Keep-Alive
            Cache-Control: no-cache
            Cookie: ASP.NET_SessionId=u4t2vtcpesad13cplhcczy20; sUrlExtCid=0
            */
            System.Diagnostics.Debug.WriteLine("Hit Step5_GetPasswordLoginPage");

            string responseHtml = this.GetMethod("https://www.prmglending.net/secureLogin.aspx", "https://www.prmglending.net/secretQuestion.aspx");

            if (!String.IsNullOrEmpty(responseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseHtml);
            }
        }

        //Step 6
        public void Step6_PostLoginPassword()
        {
            //POST https://www.prmglending.net/1003loginPro.aspx HTTP/1.1
            //Accept: text/html, application/xhtml+xml, */*
            /*Referer: https://www.prmglending.net/secureLogin.aspx
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            Content-Type: application/x-www-form-urlencoded
            Accept-Encoding: gzip, deflate
            Host: www.prmglending.net
            Content-Length: 33
            Connection: Keep-Alive
            Cache-Control: no-cache
            Cookie: ASP.NET_SessionId=u4t2vtcpesad13cplhcczy20; sUrlExtCid=0

            password=1WE85kbu%21&goNext=Login
            */
            System.Diagnostics.Debug.WriteLine("Hit Step6_PostLoginPassword");

            //this.password = "1WE85kbu!";
            string postData = "password=" + System.Uri.EscapeDataString(Globals.OurPRMGSession.password) + "&goNext=Login";

            string responseHtml = this.PostMethod("https://www.prmglending.net/1003loginPro.aspx", 
                "https://www.prmglending.net/secureLogin.aspx", postData);

        }

        //Step 7
        public void Step7_GetTheNavigator()
        {
            //GET https://www.prmglending.net/Pipeline/Nav.aspx?NavItemExpanded=Links,PipelineSearch HTTP/1.1
            //Accept: text/html, application/xhtml+xml, */*
            /*Referer: https://www.prmglending.net/Pipeline/Default.aspx?Body=QuickLinks
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            Accept-Encoding: gzip, deflate
            Host: www.prmglending.net
            Connection: Keep-Alive
            Cookie: ASP.NET_SessionId=u4t2vtcpesad13cplhcczy20; sUrlExtCid=56
            */
            System.Diagnostics.Debug.WriteLine("Hit Step7_GetTheNavigator");

            string responseHtml = this.GetMethod("https://www.prmglending.net/Pipeline/Nav.aspx",
                "https://www.prmglending.net/Pipeline/Default.aspx?Body=QuickLinks");

            if (!String.IsNullOrEmpty(responseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseHtml);

                var inputElements = responseHtmlDoc.DocumentNode.Descendants("input");

                foreach (var input in inputElements)
                {
                    if (input.Attributes["name"] != null && input.Attributes["name"].Value == "applyId")
                    {
                        System.Diagnostics.Debug.WriteLine("applyId is " + input.Attributes["value"].Value);
                        //Globals.currentApplyId = input.Attributes["value"].Value;
                        this.applyId = input.Attributes["value"].Value;
                    }

                    if (input.Attributes["name"] != null && input.Attributes["name"].Value == "userId")
                    {
                        System.Diagnostics.Debug.WriteLine("userId is " + input.Attributes["value"].Value);
                        Globals.currentUserId = input.Attributes["value"].Value;
                    }
                }

                if (!String.IsNullOrEmpty(Globals.currentUserId) && !String.IsNullOrEmpty(this.applyId))
                    this.IsCompleteConnection = true;
            }

        }

        //Step 8
        public void Step8_GetImageFlowLaunchData()
        {
            //GET https://www.prmglending.net/services/ImageFlowLaunch.aspx?ToolId=DOCUMENTUPLOAD&applyId=214269&userId=7343 HTTP/1.1
            //Accept: text/html, application/xhtml+xml, */*
            /*Accept-Language: en-US
            User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            Accept-Encoding: gzip, deflate
            Host: www.prmglending.net
            Connection: Keep-Alive
            Cookie: ASP.NET_SessionId=u4t2vtcpesad13cplhcczy20; sUrlExtCid=56
            */
            System.Diagnostics.Debug.WriteLine("Hit Step8_GetImageFlowLaunchData");


           // string responseHtml = this.GetMethod(("https://www.prmglending.net/services/ImageFlowLaunch.aspx?ToolId=DOCUMENTUPLOAD&applyId="
             //   + this.applyId + "&userId=" + Globals.currentUserId), "");

            var responseHtml = GetMethod("https://www.prmglending.net/services/ImageFlowLaunch.aspx?xProjectId=1000&ToolId=FILECATALOG", "");

            var SessionCompletedLogin = PRMGSessionHasLoggedIn;
            if (SessionCompletedLogin != null)
                SessionCompletedLogin(this, null);
        }

        private string GetMethod(string targetUrl, string referer)
        {/*
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

            sessionRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
            sessionRequest.AllowAutoRedirect = false;
            sessionRequest.Method = "GET";
            sessionRequest.Accept = "text/html, application/xhtml+xml, */*";
            if (referer != "")
                sessionRequest.Referer = referer;
            sessionRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            sessionRequest.Host = "www.prmglending.net";
            sessionRequest.KeepAlive = true;
            sessionRequest.CookieContainer = sessionCookies;
            sessionRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            sessionResponse = null;
            string responseHtml = null;

            //put session response into a using
            try
            {
                sessionResponse = (HttpWebResponse)sessionRequest.GetResponse();
                if (sessionResponse.Headers["Location"] != null)
                    this.ParseRedirectHeader(sessionResponse.Headers["Location"]);
                Stream responseStream = sessionResponse.GetResponseStream();
                responseHtml = new StreamReader(responseStream).ReadToEnd();
                if (responseHtml != null)
                    sessionCookies.Add(sessionResponse.Cookies);

            }
            catch { }

            return responseHtml;
        }

        private string PostMethod(string targetUrl, string refer, string postData)
        {
            sessionRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
            sessionRequest.AllowAutoRedirect = false;
            sessionRequest.Method = "POST";
            sessionRequest.Accept = "text/html, application/xhtml+xml, */*";
            sessionRequest.Referer = refer;
            sessionRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            sessionRequest.ContentType = "application/x-www-form-urlencoded";
            sessionRequest.Host = "www.prmglending.net";
            sessionRequest.KeepAlive = true;
            sessionRequest.CookieContainer = sessionCookies;
                        
            ASCIIEncoding enconding = new ASCIIEncoding();
            byte[] byte1 = enconding.GetBytes(postData);
            sessionRequest.ContentLength = byte1.Length;
            sessionRequest.GetRequestStream().Write(byte1, 0, byte1.Length);
            sessionResponse = null;
            string responseHtml = null;
            try
            {
                sessionResponse = (HttpWebResponse)sessionRequest.GetResponse();
                Stream responseStream = sessionResponse.GetResponseStream();
                responseHtml = new StreamReader(responseStream).ReadToEnd();
                if (responseHtml != null)
                    sessionCookies.Add(sessionResponse.Cookies);
            }
            catch { }

            return responseHtml;
        }

        private void ParseRedirectHeader(string locationHeader)
        {
            //Ideally we want:
            // Location: https://imageflow.prmg.net/xsuite/xapps/xdoc/axProjectTool.aspx?xProjectId=1000&xToolId=FILECATALOG&sessiondata=148101179066387712
            var redirectHeader = new Uri(locationHeader);


            var sessionData = System.Web.HttpUtility.ParseQueryString(redirectHeader.Query).Get("sessiondata");
            var containerKey = HttpUtility.ParseQueryString(redirectHeader.Query).Get("xContainerKey");

            Globals.sessionKey = sessionData;
            /*
            if (locationHeader.Contains("sessiondata") && locationHeader.Contains("xContainerKey"))
            {
                Globals.sessionKey = locationHeader.Substring(
                    locationHeader.IndexOf("sessiondata=") + 12,
                    (locationHeader.IndexOf("&xContainerKey=") - locationHeader.IndexOf("sessiondata=")) - 12);

                Globals.containerKey = locationHeader.Substring(locationHeader.IndexOf("&xContainerKey=") + 15, 6);
            }
            else if ( if (locationHeader.Contains("sessiondata"))*/
        }
    }
}
