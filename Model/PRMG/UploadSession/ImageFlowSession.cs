using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Runtime.InteropServices;
using ProcessorsToolkit.ViewModel.PRMGUploadWindow;

namespace ProcessorsToolkit.Model.PRMG.UploadSession
{
    public class ImageFlowSession
    {
        //private HttpWebRequest _sessionRequest = null;// (HttpWebRequest)WebRequest.Create("");
        private HttpWebResponse _sessionResponse;
        public CookieCollection SessionCookies;// = new CookieContainer();
        public bool IsCompletedConnection { get; set; }
        //public delegate void ImgFlowSessionLoggedIn(object s, EventArgs e);
        //public event ImgFlowSessionLoggedIn ImgFlowSessionHasLoggedIn;
        //private readonly UploadWindowVM _parentVM;
        private string _redirectHeaderVal;


        public ImageFlowSession()//UploadWindowVM parentVM)
        {
            //_parentVM = parentVM;
            this.IsCompletedConnection = false;
            SessionCookies = new CookieCollection();
        }

        //Step 1 --We are skipping a couple steps in Fiddler
        public void Step1_FetchImageFlowLogin()
        {
            //GET https://imageflow.prmg.net/xLogonoem.aspx?ReturnUrl=%2fxsuite%2fxapps%2fxdoc%2faxProjectTool.aspx%3fxProjectId%3d1000%26xToolId%3dDOCUMENTUPLOAD%26sessiondata%3d519084212536074240%26xContainerKey%3d214269&xProjectId=1000&xToolId=DOCUMENTUPLOAD&sessiondata=519084212536074240&xContainerKey=214269 HTTP/1.1
            //Accept: text/html, application/xhtml+xml, */*
            /*Accept-Language: en-US
            User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            Accept-Encoding: gzip, deflate
            Connection: Keep-Alive
            Host: imageflow.prmg.net
            */
            System.Diagnostics.Debug.WriteLine("Hit Step1_FetchImageFlowLogin");
            /*
            string url = String.Concat("https://imageflow.prmg.net/xLogonoem.aspx?ReturnUrl=%2fxsuite%2fxapps%2fxdoc%2faxProjectTool.aspx%3fxProjectId%3d1000%26xToolId%3dDOCUMENTUPLOAD%26sessiondata%3d",
                UploadWindowVM.ImgFlowSessionKey, "%26xContainerKey%3d", UploadWindowVM.ImgFlowContainerKey, "&xProjectId=1000&xToolId=DOCUMENTUPLOAD&sessiondata=",
             UploadWindowVM.ImgFlowSessionKey, "&xContainerKey=", UploadWindowVM.ImgFlowContainerKey);
            */

            var destUrl =
                "/xsuite/xapps/xdoc/axProjectTool.aspx?xProjectId=1000&xToolId=FILECATALOG&sessiondata=" +
                UploadWindowVM.ImgFlowSessionKey; //  190597851099279360

            var url = "https://imageflow.prmg.net/xLogonoem.aspx?ReturnUrl="
                      + System.Web.HttpUtility.UrlEncode(destUrl) + "&xProjectId=1000&xToolId=FILECATALOG&sessiondata=" +
                      UploadWindowVM.ImgFlowSessionKey;

            //string responseHtml = HttpGet(url, "");
            var responseObj = ConnectionMethods.ImgFlowGet(url, SessionCookies);
            SessionCookies.Add(responseObj.ResponseCookies);

            if (!String.IsNullOrEmpty(responseObj.RedirectHeaderVal))
                ParseRedirectHeader(responseObj.RedirectHeaderVal);

            // var url2 = _redirectHeaderVal;
            // string responseHtml2 = HttpGet(_redirectHeaderVal, "");

            //var url3 = _redirectHeaderVal;
            //string responseHtm3 = HttpGet(_redirectHeaderVal, "");

            //var loanList = new AvailableLoansList();//_parentVM);
            //loanList.LoadList();

        }

        //Step 2
        public void Step2_WeShouldBeDoneByHere()
        {
            System.Diagnostics.Debug.WriteLine("Hit Step2_WeShouldBeDoneByHere");
            //TODO: We should do one last check before calling it finished
            IsCompletedConnection = true;
            //We should have a valid request cookie container
            System.Diagnostics.Debug.WriteLine("Completed connection!");
            /*
            var SessionCompletedLogin = ImgFlowSessionHasLoggedIn;
            if (SessionCompletedLogin != null)
                SessionCompletedLogin(this, null);
            */
            //_parentVM.OnSuccessfulImgFlowLogin();

        }

        private string HttpGet(string targetUrl, string referer = "")
        {
            if (String.IsNullOrEmpty(targetUrl))
            {
                System.Diagnostics.Debug.WriteLine("targetUrl is null or empty!");
                return "Null or empty URL supplied";
            }
            var sessionRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
            //_sessionRequest.Proxy = null;
            sessionRequest.AllowAutoRedirect = false;
            sessionRequest.Method = "GET";
            sessionRequest.Accept = "text/html, application/xhtml+xml, */*";
            if (referer != "")
                sessionRequest.Referer = referer;
            sessionRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            sessionRequest.Host = "imageflow.prmg.net";
            sessionRequest.KeepAlive = true;
            sessionRequest.CookieContainer = new CookieContainer();
            sessionRequest.CookieContainer.Add(SessionCookies);

            _sessionResponse = null;
            string responseHtml = null;

            try
            {
                _sessionResponse = (HttpWebResponse)sessionRequest.GetResponse();

                if (_sessionResponse.Cookies != null && _sessionResponse.Cookies.Count != 0)
                    SessionCookies.Add(_sessionResponse.Cookies);


                if (_sessionResponse.Headers["Location"] != null)
                    ParseRedirectHeader(_sessionResponse.Headers["Location"]);
                var responseStream = _sessionResponse.GetResponseStream();
                if (responseStream != null) 
                    responseHtml = new StreamReader(responseStream).ReadToEnd();
                //if (responseHtml != null )
                  //  SessionCookies.Add(_sessionResponse.Cookies);

            }
            catch (Exception exception)
            { }

           

            return responseHtml;
        }

        private string HttpPost(string targetUrl, string refer, string postData)
        {
            var sessionRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
            //_sessionRequest.Proxy = null;
            sessionRequest.AllowAutoRedirect = false;
            sessionRequest.Method = "POST";
            sessionRequest.Accept = "text/html, application/xhtml+xml, */*";
            sessionRequest.Referer = refer;
            sessionRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            sessionRequest.ContentType = "application/x-www-form-urlencoded";
            sessionRequest.Host = "imageflow.prmg.net";
            sessionRequest.KeepAlive = true;
            sessionRequest.CookieContainer = new CookieContainer();
            sessionRequest.CookieContainer.Add(SessionCookies);

            ASCIIEncoding enconding = new ASCIIEncoding();
            byte[] byte1 = enconding.GetBytes(postData);
            sessionRequest.ContentLength = byte1.Length;
            sessionRequest.GetRequestStream().Write(byte1, 0, byte1.Length);
            _sessionResponse = null;
            string responseHtml = null;
            try
            {
                _sessionResponse = (HttpWebResponse)sessionRequest.GetResponse();
                if (_sessionResponse.Cookies != null && _sessionResponse.Cookies.Count != 0)
                    SessionCookies.Add(_sessionResponse.Cookies);
                Stream responseStream = _sessionResponse.GetResponseStream();
                responseHtml = new StreamReader(responseStream).ReadToEnd();
                //if (responseHtml != null)
                //    SessionCookies.Add(_sessionResponse.Cookies);
            }
            catch { }

            return responseHtml;
        }

        private void ParseRedirectHeader(string locationHeader)
        {
            System.Diagnostics.Debug.WriteLine("Locaton header: " + locationHeader);
            //var redirectHeader = new Uri(locationHeader);
            if (locationHeader.StartsWith("/"))
                _redirectHeaderVal = "https://imageflow.prmg.net" + locationHeader;

            else
                _redirectHeaderVal = locationHeader;
        }
    }
}
