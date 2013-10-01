using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Runtime.InteropServices;

namespace ImageFlowUploader
{
    public class ImageFlowSession
    {
        HttpWebRequest sessionRequest = null;// (HttpWebRequest)WebRequest.Create("");
        HttpWebResponse sessionResponse = null;
        public CookieContainer sessionCookies = new CookieContainer();
        public bool IsCompletedConnection { get; set; }
        public delegate void ImgFlowSessionLoggedIn(object s, EventArgs e);
        public event ImgFlowSessionLoggedIn ImgFlowSessionHasLoggedIn;
        private string _redirectHeaderVal;

        public ImageFlowSession() {
            this.IsCompletedConnection = false;
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



      // OLD ->  https://imageflow.prmg.net/xsuite/xapps/xdoc/axProjectTool.aspx?xProjectId=1000&xToolId=DOCUMENTUPLOAD&sessiondata=22605791642007552&xContainerKey=239915

            //NEW -> Location: https://imageflow.prmg.net/xsuite/xapps/xdoc/axProjectTool.aspx?xProjectId=1000&xToolId=FILECATALOG&sessiondata=148101179066387712

            /*string url OLD = String.Concat("https://imageflow.prmg.net/xLogonoem.aspx?ReturnUrl=%2fxsuite%2fxapps%2fxdoc%2faxProjectTool.aspx%3fxProjectId%3d1000%26xToolId%3dDOCUMENTUPLOAD%26sessiondata%3d",
                Globals.sessionKey, "%26xContainerKey%3d", Globals.containerKey, "&xProjectId=1000&xToolId=DOCUMENTUPLOAD&sessiondata=",
             Globals.sessionKey, "&xContainerKey=", Globals.containerKey);
            */

            var destUrl =
                "/xsuite/xapps/xdoc/axProjectTool.aspx?xProjectId=1000&xToolId=FILECATALOG&sessiondata=" +
                Globals.sessionKey;//  190597851099279360

            var url = "https://imageflow.prmg.net/xLogonoem.aspx?ReturnUrl=" 
                + System.Web.HttpUtility.UrlEncode(destUrl) + "&xProjectId=1000&xToolId=FILECATALOG&sessiondata=" + Globals.sessionKey;


            /*
                "https://imageflow.prmg.net/xLogonoem.aspx?ReturnUrl=%2fxsuite%2fxapps%2fxdoc%2faxProjectTool.aspx%3fxProjectId%3d1000%26xToolId%3dFILECATALOG%26sessiondata%3d" +
                Globals.sessionKey + "&xProjectId=1000&xToolId=FILECATALOG&sessiondata=" + Globals.sessionKey; //148101179066387712
            */

                              //             /xLogonoem.aspx?ReturnUrl=%2fxsuite%2fxapps%2fxdoc%2faxProjectTool.aspx%3fxProjectId%3d1000%26xToolId%3dFILECATALOG%26sessiondata%3d148101179066387712&xProjectId=1000&xToolId=FILECATALOG&sessiondata=148101179066387712
            //string url2 = String.Format("https://imageflow.prmg.net/xLogonoem.aspx?ReturnUrl=https%3A%2F%2Fimageflow.prmg.net%2Fzapp%2FAvista%2FloanSearchCritiaFrame.aspx%3FclickAction%3D%26")

                

      //  https://imageflow.prmg.net/zapp/Avista/loanSearchCritiaFrame.aspx?clickAction=&

            string responseHtml = this.GetMethod(url, "");

            var url2 = _redirectHeaderVal;
            string responseHtml2 = GetMethod(_redirectHeaderVal, "");
        }

        //Step2 follow the redirect this page gives us to check connection
        public void Step2_FetchImgFlowPage()
        {
            


        }


        //Step 3
        public void Step3_WeShouldBeDoneByHere()
        {
            System.Diagnostics.Debug.WriteLine("Hit Step2_WeShouldBeDoneByHere");
            //TODO: We should do one last check before calling it finished
            this.IsCompletedConnection = true;
            //We should have a valid request cookie container
            System.Diagnostics.Debug.WriteLine("Completed connection!");

            var SessionCompletedLogin = ImgFlowSessionHasLoggedIn;
            if (SessionCompletedLogin != null)
                SessionCompletedLogin(this, null);
        }

        private string GetMethod(string targetUrl, string referer)
        {
            sessionRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
            sessionRequest.AllowAutoRedirect = false;
            sessionRequest.Method = "GET";
            sessionRequest.Accept = "text/html, application/xhtml+xml, */*";
            if (referer != "")
                sessionRequest.Referer = referer;
            sessionRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            sessionRequest.Host = "imageflow.prmg.net";
            sessionRequest.KeepAlive = true;
            sessionRequest.CookieContainer = sessionCookies;

            sessionResponse = null;
            string responseHtml = null;

            //try
            //{
                sessionResponse = (HttpWebResponse)sessionRequest.GetResponse();
                Stream responseStream = sessionResponse.GetResponseStream();
                responseHtml = new StreamReader(responseStream).ReadToEnd();
                if (sessionResponse.Headers["Location"] != null)
                    this.ParseRedirectHeader(sessionResponse.Headers["Location"]);
                if (responseHtml != null)
                    sessionCookies.Add(sessionResponse.Cookies);

           // }
           // catch { }

            return responseHtml;
        }

        private void ParseRedirectHeader(string locationHeader)
        {
            System.Diagnostics.Debug.WriteLine("Locaton header: " + locationHeader);
            //var redirectHeader = new Uri(locationHeader);
            _redirectHeaderVal = "https://imageflow.prmg.net" + locationHeader;
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
            sessionRequest.Host = "imageflow.prmg.net";
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

    }
}
