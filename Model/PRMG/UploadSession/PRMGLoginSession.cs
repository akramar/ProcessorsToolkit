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
using ProcessorsToolkit.Model.Common.UploadSession;

namespace ProcessorsToolkit.Model.PRMG.UploadSession
{
    public class PRMGLoginSession
    {
        //private HttpWebRequest sessionRequest;// = (HttpWebRequest)WebRequest.Create("");
        //private HttpWebResponse _sessionResponse;
        private readonly CookieCollection _sessionCookies;// = new CookieContainer();
        //private UploadWindowVM _parentVM;
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecretQuestionType { get; set; }
        public string SecretQuestionQuestion { get; set; }
        public string SecretQuestionAnswer { get; set; }
        public string ApplyId { get; set; }
        public string CurrentUserId { get; set; }
        public bool IsCompleteConnection { get; set; }
        public string ImgFlowSessionKey { get; set; }
        //public delegate void PRMGSessionLoggedIn(object s, EventArgs e);
        //public event PRMGSessionLoggedIn PRMGSessionHasLoggedIn;

        public PRMGLoginSession()//UploadWindowVM parentVM)//string username, string password)
        {
            //_parentVM = parentVM;
            //Username = username;
            //Password = password;
            IsCompleteConnection = false;
            _sessionCookies = new CookieCollection();
        }

        //Step 1 -- We need to do this to get an ASP session cookie
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

            //Set-Cookie: sUrlExtCid=0; path=/; secure; HttpOnly
            // except maybe already? Cookie: ASP.NET_SessionId=rwrptpciq3gb23xom2b05oxh; sUrlExtCid=0
            //never mind, we still need to fetch a session cookie. Maybe skip parsing homepage?
            //http://stackoverflow.com/questions/4507941/c-sharp-webclient-openread-url
            //http://stackoverflow.com/questions/6237734/how-to-request-only-the-http-header-with-c

            var responseObj = ConnectionMethods.PRMGHead("https://www.prmglending.net/", _sessionCookies);
            _sessionCookies.Add(responseObj.ResponseCookies);


            /*var responseHtml = PRMGGet("https://www.prmglending.net/", "");
            if (!String.IsNullOrEmpty(responseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseHtml);
            }*/

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
            Content-Length: 42 //need to set
            Connection: Keep-Alive
            Cache-Control: no-cache
            Cookie: ASP.NET_SessionId=u4t2vtcpesad13cplhcczy20; sUrlExtCid=0

            UserName=myUser&Authentication=MULTIFACTOR
            */

            var postData = "UserName=" + Username + "&Authentication=MULTIFACTOR";

            //var responseHtml = PRMGPost("https://www.prmglending.net/acctLoginPro.aspx", "https://www.prmglending.net/", postData);
            var responseObj = ConnectionMethods.PRMGPost("https://www.prmglending.net/acctLoginPro.aspx",
                                                         _sessionCookies, postData, "https://www.prmglending.net/");
            _sessionCookies.Add(responseObj.ResponseCookies);

            if (!String.IsNullOrEmpty(responseObj.ResponseHtml))
            {
                var responseHtmlDoc = new HtmlDocument();
                responseHtmlDoc.LoadHtml(responseObj.ResponseHtml);
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

            //var responseHtml = PRMGGet("https://www.prmglending.net/secretQuestion.aspx", "https://www.prmglending.net/");
            var responseObj = ConnectionMethods.PRMGGet("https://www.prmglending.net/secretQuestion.aspx", _sessionCookies);
            _sessionCookies.Add(responseObj.ResponseCookies);

            if (!String.IsNullOrEmpty(responseObj.ResponseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseObj.ResponseHtml);

                //Find the question submit form

                //HtmlNode questionTypeNode = responseHtmlDoc.DocumentNode.SelectSingleNode("pqestionCode"); //qestion not a typo?

                foreach (HtmlNode node in responseHtmlDoc.DocumentNode.Descendants("input"))
                    if (node.Attributes["name"] != null && node.Attributes["name"].Value == "pqestionCode")                    
                        SecretQuestionType = node.Attributes["value"].Value;

                var paraNodes = responseHtmlDoc.DocumentNode.Descendants("p").ToList();


                //HtmlNode firstP = responseHtmlDoc.DocumentNode.SelectSingleNode("/form/p[1]");

                if (paraNodes.Count > 0)// (firstP != null)
                {
                    //secretQuestion_type = questionTypeNode.Attributes["value"].Value;
                    //secretQuestion_question = firstP.InnerText;
                    //secretQuestion_answer = "the answer";

                    SecretQuestionQuestion = paraNodes[0].InnerText.Replace(Environment.NewLine, "");
                    //_parentVM.OnHaveSecretQuestion();
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


            //IsPostback=1&pqestionCode=cityborn&pAnswerCont=apples&mylogin=Continue&rememberComputer=1

            var postData = "IsPostback=1&pqestionCode=" + SecretQuestionType + "&pAnswerCont=" + SecretQuestionAnswer; 
           // var responseHtml = PRMGPost("https://www.prmglending.net/secretQuestion.aspx", 
            //    "https://www.prmglending.net/secretQuestion.aspx", postData);

            var responseObj = ConnectionMethods.PRMGPost("https://www.prmglending.net/secretQuestion.aspx",
                                                         _sessionCookies, postData,
                                                         "https://www.prmglending.net/secretQuestion.aspx");
            _sessionCookies.Add(responseObj.ResponseCookies);

            if (!String.IsNullOrEmpty(responseObj.ResponseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseObj.ResponseHtml);
            }
        }

        //Step 5 !! This can be skipped altogether, no need to download
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

            //var responseHtml = PRMGGet("https://www.prmglending.net/secureLogin.aspx", 
             //   "https://www.prmglending.net/secretQuestion.aspx");
            var responseObj = ConnectionMethods.PRMGGet("https://www.prmglending.net/secureLogin.aspx", _sessionCookies, "https://www.prmglending.net/secretQuestion.aspx");
            _sessionCookies.Add(responseObj.ResponseCookies);

            if (!String.IsNullOrEmpty(responseObj.ResponseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseObj.ResponseHtml);
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

            //this.password = "$$$!";
            var postData = "password=" + Uri.EscapeDataString(Password) + "&goNext=Login";

            var responseObj = ConnectionMethods.PRMGPost("https://www.prmglending.net/1003loginPro.aspx",
                                                         _sessionCookies, postData,
                                                         "https://www.prmglending.net/secureLogin.aspx");
            _sessionCookies.Add(responseObj.ResponseCookies);
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

            var responseObj = ConnectionMethods.PRMGGet("https://www.prmglending.net/Pipeline/Nav.aspx", _sessionCookies,
                "https://www.prmglending.net/Pipeline/Default.aspx?Body=QuickLinks");

            _sessionCookies.Add(responseObj.ResponseCookies);


            if (!String.IsNullOrEmpty(responseObj.ResponseHtml) )
            {
                var responseHtmlDoc = new HtmlDocument();
                responseHtmlDoc.LoadHtml(responseObj.ResponseHtml);

                var inputElements = responseHtmlDoc.DocumentNode.Descendants("input");

                foreach (var input in inputElements)
                {
                    if (input.Attributes["name"] != null && input.Attributes["name"].Value == "applyId")
                    {
                        System.Diagnostics.Debug.WriteLine("applyId is " + input.Attributes["value"].Value);
                        //Globals.currentApplyId = input.Attributes["value"].Value;
                        ApplyId = input.Attributes["value"].Value;
                    }

                    if (input.Attributes["name"] != null && input.Attributes["name"].Value == "userId")
                    {
                        System.Diagnostics.Debug.WriteLine("userId is " + input.Attributes["value"].Value);
                        CurrentUserId = input.Attributes["value"].Value;
                        //Globals.currentUserId = input.Attributes["value"].Value;
                    }
                }

                if (!String.IsNullOrEmpty(CurrentUserId) && !String.IsNullOrEmpty(ApplyId))
                    IsCompleteConnection = true;
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


            //this causes probles if applyID's loan is not allowing uploads yet. Use FILECATALOG below
            // var responseHtml = HttpGet(("https://www.prmglending.net/services/ImageFlowLaunch.aspx?ToolId=DOCUMENTUPLOAD&applyId=" 
            //     + ApplyId + "&userId=" + CurrentUserId), "");
            var responseObj = ConnectionMethods.PRMGGet("https://www.prmglending.net/services/ImageFlowLaunch.aspx?xProjectId=1000&ToolId=FILECATALOG",
                    _sessionCookies);
            _sessionCookies.Add(responseObj.ResponseCookies);

            if (!String.IsNullOrEmpty(responseObj.RedirectHeaderVal))
                ParseRedirectHeader(responseObj.RedirectHeaderVal);
            //_parentVM.OnSuccessfulPRMGLogin();
        }

        public CookieCollection Step9_InitiateDocuTrac()
        {

            var dtCookies = new CookieCollection();

            var destUrl = "/xsuite/xapps/xdoc/axProjectTool.aspx?xProjectId=1000&xToolId=FILECATALOG&sessiondata=" + ImgFlowSessionKey;

            var url = "https://imageflow.prmg.net/xLogonoem.aspx?ReturnUrl=" + HttpUtility.UrlEncode(destUrl)
                      + "&xProjectId=1000&xToolId=FILECATALOG&sessiondata=" + ImgFlowSessionKey;

            var responseObj = ConnectionMethods.ImgFlowGet(url, dtCookies);
            dtCookies.Add(responseObj.ResponseCookies);

            return dtCookies;

        }

        public void LogOutSession()
        {
            System.Diagnostics.Debug.WriteLine("Logging out session");
            var responseObj = ConnectionMethods.PRMGGet("https://www.prmglending.net/services/Logout.aspx",
                                                        _sessionCookies);
            _sessionCookies.Add(responseObj.ResponseCookies);
        }


        private void ParseRedirectHeader(string locationHeader)
        {
            //Used when catching redirect from PRMG to Imageflow

            if (locationHeader.StartsWith("/"))
                return;

            var redirectHeader = new Uri(locationHeader);
            var sessionData = HttpUtility.ParseQueryString(redirectHeader.Query).Get("sessiondata");
            //var containerKey = HttpUtility.ParseQueryString(redirectHeader.Query).Get("xContainerKey");

            ImgFlowSessionKey = sessionData;
        }
    }
}
