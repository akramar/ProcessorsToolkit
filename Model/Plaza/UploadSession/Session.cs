using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using ProcessorsToolkit.Model.Common.UploadSession;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.Model.Plaza.UploadSession
{
    public class Session
    {

        public CookieCollection SessionCookies;
        public string CompanyId { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool IsCompleteConnection { get; set; }
        public string xEUVal { get; set; }
        public string xEPVal { get; set; }
        public string ViewState { get; set; }
        public string EventValidation { get; set; }

        public Session()
        {
            SessionCookies = new CookieCollection();
        }

        //***** Internally, Plaza uses DocuTrac (now under Ellie Mae), so link 4 may be able to view current loans if we have a container number
        // https://imageflow.prmg.net:443/xsuite/xapps/xdoc/webservice/xWsProcess.aspx
        // https://imageflow.prmg.net            /xsuite/xapps/xdoc/docUpload/default.aspx?xProjectId=1000&xToolId=DOCUMENTUPLOAD&sessiondata=2930647566935531520&xContainerKey=215743
        // https://docutrac.plazahomemortgage.com/xsuite/xapps/xdoc/containerViewer/default.aspx?xProjectId=1000&xToolId=CONTAINERVIEWER&xContainerKey=08%3bPQ
        // [4] https://docutrac.plazahomemortgage.com/xsuite/xapps/xdoc/docUpload/default.aspx?xProjectId=1000&xToolId=CONTAINERVIEWER&xContainerKey=08%3bPQ
        //

        public void Step1_GetHomepage()
        {
            /* Request: GET https://au.loan-score.com/ HTTP/1.1
            Accept: text/html, application/xhtml+xml, * /*
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko
            Accept-Encoding: gzip, deflate
            Host: au.loan-score.com
            DNT: 1
            Connection: Keep-Alive
            */

            /* Response: HTTP/1.1 200 OK
            Date: Sun, 24 Nov 2013 04:42:27 GMT
            Server: Microsoft-IIS/6.0
            X-Powered-By: ASP.NET
            Pragma: no-cache
            Content-Length: 3757
            Content-Type: text/html
            Expires: Sun, 24 Nov 2013 04:42:27 GMT
            Set-Cookie: SessionFarm%5FGUID=%7B506D0195%2D06BD%2D485B%2DA3F1%2D6EDBD78DCA12%7D; path=/
            Set-Cookie: ASPSESSIONIDQSCRCQQC=CHDLLAMAJCLBMICHLMANKEFO; path=/
            Cache-control: no-cache
            */

            Debug.WriteLine("Hit Step1_GetHompage");
            var responseObj = ConnectionMethods.LoanScoreGet("https://au.loan-score.com/", SessionCookies);
            SessionCookies.Add(responseObj.ResponseCookies);

        }

        public void Step2_PostLogin()
        {
            /* Request: POST https://au.loan-score.com/LSLoginPQ.asp HTTP/1.1
            Accept: text/html, application/xhtml+xml, * /*
            Referer: https://au.loan-score.com/relogin.asp?MID=3
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko
            Content-Type: application/x-www-form-urlencoded
            Accept-Encoding: gzip, deflate
            Host: au.loan-score.com
            Content-Length: 72
            DNT: 1
            Connection: Keep-Alive
            Cache-Control: no-cache
            Cookie: SessionFarm%5FGUID=%7B506D0195%2D06BD%2D485B%2DA3F1%2D6EDBD78DCA12%7D; ASPSESSIONIDQSCRCQQC=CHDLLAMAJCLBMICHLMANKEFO; LSau=BID=

            CompanyID=167301&B2=++Log-in++&LoginName=167301&LoginPass=LrCQOI61z5V%21
            */

            /* Response: HTTP/1.1 302 Object moved
            Date: Sun, 24 Nov 2013 04:52:52 GMT
            Server: Microsoft-IIS/6.0
            X-Powered-By: ASP.NET
            Pragma: no-cache
            Location: listpipeline3.asp
            Content-Length: 138
            Content-Type: text/html
            Expires: Sun, 24 Nov 2013 04:52:52 GMT
            Set-Cookie: LSau=BID=208528&User=167301&ID=167301; expires=Mon, 23-Dec-2013 08:00:00 GMT; path=/
            Cache-control: no-cache

            <head><title>Object moved</title></head>
            <body><h1>Object Moved</h1>This object may be found <a HREF="listpipeline3.asp">here</a>.</body>
            */
            
            Debug.WriteLine("Hit Step2_PostLogin");
            
            var companyIdEscaped = HttpUtility.UrlEncode(CompanyId);
            var userIdEscaped = HttpUtility.UrlEncode(UserId);
            var pwdEscaped = HttpUtility.UrlEncode(Password);

            var fieldsToPost = new Dictionary<string, string>
                {
                    {"CompanyID", companyIdEscaped},
                    {"B2", "++Log-in++"},
                    {"LoginName", userIdEscaped},
                    {"LoginPass", pwdEscaped}
                };

            const string postUrl = "https://au.loan-score.com/LSLoginPQ.asp";
            var postData = String.Join("&", fieldsToPost.Select(v => v.Key + "=" + v.Value));
            const string referer = "https://au.loan-score.com/relogin.asp?MID=3";
            
            var responseObj = ConnectionMethods.LoanScorePost(postUrl, SessionCookies, postData, referer);
            SessionCookies.Add(responseObj.ResponseCookies);

            //TODO: check the response HTML and see if we're really logged in
            if (true)
                IsCompleteConnection = true;

        }

        public AvailableLoansList Step3_GetPipeline()
        {
            /* Request: GET https://au.loan-score.com/listpipeline3.asp HTTP/1.1
            Accept: text/html, application/xhtml+xml, * /*
            Referer: https://au.loan-score.com/relogin.asp?MID=3
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko
            DNT: 1
            Accept-Encoding: gzip, deflate
            Host: au.loan-score.com
            Cookie: SessionFarm%5FGUID=%7B506D0195%2D06BD%2D485B%2DA3F1%2D6EDBD78DCA12%7D; ASPSESSIONIDQSCRCQQC=CHDLLAMAJCLBMICHLMANKEFO; LSau=BID=208528&User=167301&ID=167301
            Connection: Keep-Alive
            Cache-Control: no-cache
            */

            /* Response: HTTP/1.1 200 OK
            Date: Sun, 24 Nov 2013 04:52:52 GMT
            Server: Microsoft-IIS/6.0
            X-Powered-By: ASP.NET
            Pragma: no-cache
            Content-Length: 14082
            Content-Type: text/html
            Expires: Sun, 24 Nov 2013 04:52:52 GMT
            Cache-control: no-cache
            */

            Debug.WriteLine("Hit Step3_GetPipeline");

            const string getUrl = "https://au.loan-score.com/listpipeline3.asp";
            const string referer = "https://au.loan-score.com/relogin.asp?MID=3";

            var responseObj = ConnectionMethods.LoanScoreGet(getUrl, SessionCookies, referer);

            var responseHtmlDoc = new HtmlDocument();
            responseHtmlDoc.LoadHtml(responseObj.ResponseHtml);

            
            //var links = responseHtmlDoc.DocumentNode
            //                           .Descendants("a")
            //                           .Where(x => x.Attributes["href"] != null
            //                                       && x.Attributes["href"].Value.StartsWith("F16.asp?AppNo="))
            //                                       .ToList();


            // BEWARE: The html of this page is really malformed, don't trust how
            // it's formatted by any dev tools, view raw source instead
            // /html/body/div/center/form/table/tbody/tr[3]/td/table/tbody <-- how Chrome Dev Tools thinks it should be
            // /html[1]/body[1]/div[1]/center[1]/table[1]/tr[3]/td[1]/table[1]/tr[1]/tr[1]/td[2]/a[1]


            var rowrWrapper = responseHtmlDoc.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/center[1]/table[1]/tr[3]/td[1]/table[1]/tr[1]");

            var rows = rowrWrapper.Descendants("tr");
            var availLoanList = new AvailableLoansList();

            foreach (var loanRow in rows)
            {
                var loanResult = new LoanSearchResultItem();
                var allCols = loanRow.Descendants("td").ToList();
                var firstLink = allCols[1].Descendants("a").FirstOrDefault();
                if (firstLink != null)
                {
                    var appNum = firstLink.Attributes["href"].Value.Split('=')[1];
                    loanResult.BorrNameRaw = firstLink.InnerText;
                    loanResult.AppNum = appNum;
                    loanResult.BorrFirstName = loanResult.BorrNameRaw.Split(',')[1];
                    loanResult.BorrLastName = loanResult.BorrNameRaw.Split(',')[0];
                }
                var loanNumCol = allCols[6];
                loanResult.LoanNum = loanNumCol.InnerText.Trim();
                loanResult.IsSelected = MainWindowVM.SelectedBorrDir.BorrDirName.StartsWith(loanResult.BorrLastName, StringComparison.InvariantCultureIgnoreCase);

                availLoanList.Add(loanResult);
            }

            return availLoanList;
        }

        public string Step4_GetUploadCredentials(string appNum)
        {
            /* Request: GET https://au.loan-score.com/dodocsauto.aspx?AppNo=1524018&md=1 HTTP/1.1
            Accept: text/html, application/xhtml+xml, * /*
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko
            Accept-Encoding: gzip, deflate
            Host: au.loan-score.com
            DNT: 1
            Connection: Keep-Alive
            Cookie: LSau=BID=208528&User=167301&ID=167301; SessionFarm%5FGUID=%7B95596B79%2D13CF%2D4C64%2DB0FA%2DD42866765C7A%7D; ASPSESSIONIDSCQRTQQR=OBDLKJGBBHIDMFGGGOGDMOFF; ASPSESSIONIDSCTQRSQR=EEPDIAHBPMLELPMEDBICKALE
            */

            /* Response: HTTP/1.1 200 OK
            Date: Mon, 25 Nov 2013 05:39:11 GMT
            Server: Microsoft-IIS/6.0
            X-Powered-By: ASP.NET
            X-AspNet-Version: 2.0.50727
            Pragma: no-cache
            Cache-Control: no-cache
            Pragma: no-cache
            Expires: -1
            Content-Type: text/html; charset=utf-8
            Content-Length: 455
            */

            Debug.WriteLine("Hit Step4_GetUploadCredentials");
            //Debug.WriteLine("SLEEPING THREAD");
            //System.Threading.Thread.Sleep(4000);

            var getUrl = "https://au.loan-score.com/dodocsauto.aspx?AppNo=" + appNum + "&md=1";

            var responseObj = ConnectionMethods.LoanScoreGet(getUrl, SessionCookies);

            var responseHtmlDoc = new HtmlDocument();
            responseHtmlDoc.LoadHtml(responseObj.ResponseHtml);

            var loanNum = "";

            /* this is out response
            <body onload="form1.submit();">
            <form name="form1" method="POST" action="https://docutrac.plazahomemortgage.com:8080/brokeruploadpage/Default.aspx?doctracfn=DOCUPLOADLS&loannumber=4813110236">
            <input type="hidden" name="xEU" value="loanscore" />
            <input type="hidden" name="xEP" value="xa6t170L" />
            </form>
            </body>
             */

            var formNode = responseHtmlDoc.DocumentNode
                .Descendants("form").FirstOrDefault();

            if (formNode != null)
            {
                if (formNode.Attributes["action"] != null)
                {
                    //Redundant as we already got it from the loan row, maybe compare instead?
                    var postUrl = formNode.Attributes["action"].Value;
                    var index = postUrl.LastIndexOf("loannumber=", StringComparison.OrdinalIgnoreCase);
                    loanNum = postUrl.Substring(index).Split('=')[1]; //make sure we're getting the number right
                }
            }

            var inputNodes = responseHtmlDoc.DocumentNode
                                            .Descendants("input").ToList();

            if (inputNodes.Any())
            {
            //var inputNodes = formNode.Descendants("input").ToList();
                var xEUNode = inputNodes.FirstOrDefault(n => n.Attributes["name"].Value == "xEU");
                if (xEUNode != null)
                    xEUVal = xEUNode.Attributes["value"].Value;

                var xEPNode = inputNodes.FirstOrDefault(n => n.Attributes["name"].Value == "xEP");
                if (xEPNode != null)
                    xEPVal = xEPNode.Attributes["value"].Value;
            }

            return loanNum;
        }

        public Tuple<string,string> Step5_GetUploadPage(string appNum, string loanNum)
        {
            //need to start by getting this called after we've selected out loan and files
            /* Request POST https://docutrac.plazahomemortgage.com:8080/brokeruploadpage/Default.aspx?doctracfn=DOCUPLOADLS&loannumber=4813110236 HTTP/1.1
            Accept: text/html, application/xhtml+xml, * /*
            Referer: https://au.loan-score.com/dodocsauto.aspx?AppNo=1524018&md=1
            Accept-Language: en-US
            User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko
            Content-Type: application/x-www-form-urlencoded
            Accept-Encoding: gzip, deflate
            Host: docutrac.plazahomemortgage.com:8080
            Content-Length: 26
            DNT: 1
            Connection: Keep-Alive
            Cache-Control: no-cache
            Cookie: __utma=59321836.892677913.1372375566.1385257182.1385357372.21; __utmz=59321836.1372375566.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); __utmb=59321836.1.10.1385357372; __utmc=59321836; ASP.NET_SessionId=v3acyh5goqhccuubmucvd5ft

            xEU=loanscore&xEP=xa6t170L
            */

            /* Response HTTP/1.1 200 OK
            Cache-Control: private
            Content-Type: text/html; charset=utf-8
            Server: Microsoft-IIS/7.5
            X-AspNet-Version: 4.0.30319
            X-Powered-By: ASP.NET
            Date: Mon, 25 Nov 2013 05:39:18 GMT
            Content-Length: 22154
            */

            Debug.WriteLine("Hit Step5_GetUploadPage");
            //Debug.WriteLine("SLEEPING THREAD");
            //System.Threading.Thread.Sleep(4000);

            var fieldsToPost = new Dictionary<string, string>
                {
                    {"xEU", xEUVal},
                    {"xEP", xEPVal}
                };
            var postData = String.Join("&", fieldsToPost.Select(v => v.Key + "=" + v.Value));

            var postUrl = "https://docutrac.plazahomemortgage.com:8080/brokeruploadpage/Default.aspx?doctracfn=DOCUPLOADLS&loannumber=" + loanNum;
            var referer = "https://au.loan-score.com/dodocsauto.aspx?AppNo=" + appNum + "&md=1";

            var dtCookies = new CookieCollection();
            var responseObj = DocuTracConnectionMethods.DocuTracPost(postUrl, "docutrac.plazahomemortgage.com:8080", SessionCookies, postData, referer);

            dtCookies.Add(responseObj.ResponseCookies);
            SessionCookies.Add(responseObj.ResponseCookies);

            var responseHtmlDoc = new HtmlDocument();
            responseHtmlDoc.LoadHtml(responseObj.ResponseHtml);
            


            // /html[1]/body[1]/form[1]/div[1]
            
            //var hiddenInputs = responseHtmlDoc.DocumentNode.Descendants("/html[1]/body[1]/form[1]/div[1]");
            // //img[@class="itemImage"] --We can also use searching by ID instead of xPath below
            var viewStateNode = responseHtmlDoc.DocumentNode.SelectSingleNode("//input[@name='__VIEWSTATE']");
            //var viewStateNode2 = responseHtmlDoc.DocumentNode.Descendants("input").Where(n => n.Attributes["name"].Value == "__VIEWSTATE");
            //var viewStateNode3 = responseHtmlDoc.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']");
            var eventValidationNode = responseHtmlDoc.DocumentNode.SelectSingleNode("//input[@name='__EVENTVALIDATION']");

            var viewState = "";
            if (viewStateNode != null)
                viewState = viewStateNode.Attributes["value"].Value;

            var eventValidation = "";
            if (eventValidationNode != null)
                eventValidation = eventValidationNode.Attributes["value"].Value;

            return new Tuple<string, string>(viewState, eventValidation);

        }

    }
}
