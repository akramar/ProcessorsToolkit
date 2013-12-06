using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace ProcessorsToolkit.Model.Interbank.UploadSession
{
    public class Session
    {

        public Dictionary<string,string> LoanIdsNames { get; set; } 
        public CookieCollection SessionCookies; // = new CookieContainer();
        public Dictionary<string, string> FormVals = new Dictionary<string, string>();
        //private UploadWindowVM _parentVM;
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsCompleteConnection { get; set; }
        private List<string> PipelineRows { get; set; }
        private List<string> LoanIds { get; set; }
        //public LoanSearchResultItem SelectedLoan { get; set; }
        public List<LoanCondition> SelectedLoansConditions { get; set; } 
        

        public Session()
        {
            SelectedLoansConditions = new List<LoanCondition>();
            LoanIdsNames = new Dictionary<string, string>();
            LoanIds = new List<string>();
            PipelineRows = new List<string>();
            IsCompleteConnection = false;
            SessionCookies = new CookieCollection();
        }

        //Step 1 - 
        public void Step1_FetchHomepage() //https://portal.interbankwholesale.com/IBPortal/Login.aspx 
        {
            /* replied
            HTTP/1.1 200 OK
            Cache-Control: private
            Content-Type: text/html; charset=utf-8
            Server: Microsoft-IIS/7.0
            Set-Cookie: ASP.NET_SessionId=no34bx4kb15juj4lbavf2ciz; path=/; HttpOnly
            X-AspNet-Version: 4.0.30319
            X-Powered-By: ASP.NET
            Date: Fri, 27 Sep 2013 08:45:26 GMT
            Content-Length: 20511
            Set-Cookie: BNI_BARRACUDA_LB_COOKIE=0000000000000000000000002406a8c000005000; Path=/; Max-age=2400; HttpOnly
            */

            System.Diagnostics.Debug.WriteLine("Hit Step1_FetchHomepage");
            var responseObj =
                ConnectionMethods.IBWGet("https://portal.interbankwholesale.com/IBPortal/Login.aspx", SessionCookies);
            SessionCookies.Add(responseObj.ResponseCookies);
            fillformvals(responseObj.ResponseHtml);
        }

        //Step 2 - 
        public void Step2_PostUsername()
        {
            /* sent:
            POST https://portal.interbankwholesale.com/IBPortal/Login.aspx HTTP/1.1
            Host: portal.interbankwholesale.com
            Connection: keep-alive
            Content-Length: 5652
            Cache-Control: max-age=0
            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*//*;q=0.8
            Origin: https://portal.interbankwholesale.com
            User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1652.0 Safari/537.36
            Content-Type: application/x-www-form-urlencoded
            Referer: https://portal.interbankwholesale.com/IBPortal/Login.aspx
            Accept-Encoding: gzip,deflate,sdch
            Accept-Language: en-US,en;q=0.8
            Cookie: ASP.NET_SessionId=no34bx4kb15juj4lbavf2ciz; BNI_BARRACUDA_LB_COOKIE=0000000000000000000000002406a8c000005000
            ++a whole bunch of data captured

            replied:
            HTTP/1.1 302 Found
            Cache-Control: private, no-cache="Set-Cookie"
            Content-Type: text/html; charset=utf-8
            Location: /IBPortal/Dashboard.aspx
            Server: Microsoft-IIS/7.0
            X-AspNet-Version: 4.0.30319
            Set-Cookie: .ASPXINTERBANK=3D0E96AC4928D80AA7520F10B67196FE8C1F819E73D93A9858345BE51A0C303500082B54D629E7DF0D6764F4B6FF9015E9AB627BAE86A35A6B6B4285CE46D70F6CAADD555E2E3C4F618F234B03E0B6ACAA4BFDEB6AB6692038B0E9E05EF127B8C6594CA55FE78DB110F808C22A9DC3A185C4F1E5CC154DB70270EC1DCF51314C31B4B603D72807F4E97DA42703755D5ECE24305616F9CB6FDDB66FF4207D146C; path=/
            Set-Cookie: .ASPXINTERBANK=B989A9D09EE0B7D343624EEC68B65A3AD2B5C3259604F8A50139AFCD90E65522B2E027837164F15C2D6835DFF0DD0FBEC0CE994C3BD18742FEB22C34EB2500DF36876C30DD44C10C0E84DB5BB79DAFE2AC6EB298F1304118FB9FD573945D5BDEBC2AAC39797F9E81BA3D8BE2A23795A2CD88D7295F43E034DF92B340931753A6FCDDDF065D2B7313FF7D5F4935A40834819B09B85D83C9557260CE95A50D4856; expires=Fri, 27-Sep-2013 09:31:24 GMT; path=/; HttpOnly
            Set-Cookie: .ASPXINTERBANK=48BE739604BA3471D0DE458EF65EE5B6A78C85C0F5C083AA3BAD389FD4784AD0B936EE2930BC4DD83A3888B52D9038670073F71C8AD7021502FD464710D941E961D28168920F3F109EA2AC6E39D07AB663979E7E15AADD8D65047F6B2FBEAEA6FB7F27ACCE4347A69924EE87693F9CCF9C11F3E104E7D54782F4948B822DABA2BA0B4BD727BD89266DAE1E99FA0D013BD26DB9D4657EECFE3E9B141BA3216605; path=/; HttpOnly
            X-Powered-By: ASP.NET
            Date: Fri, 27 Sep 2013 08:51:24 GMT
            Content-Length: 20911
            Set-Cookie: BNI_BARRACUDA_LB_COOKIE=0000000000000000000000002406a8c000005000; Path=/; Max-age=2400; HttpOnly
            */


            System.Diagnostics.Debug.WriteLine("Hit Step2_PostUsername");

            /*
            var postParams = new Dictionary<string, string>()
                {
                    {"&ctl00_mainMenu_ClientState=", ""},
                    {"&ctl00_BreadCrumbSiteMap_ClientState=", ""},
                    {"&ctl00%24ContentPlaceHolder1%24loginToDataTrac%24UserName=", "akramar%40houseloans.net"},
                    {"&ctl00%24ContentPlaceHolder1%24loginToDataTrac%24Password=", "INTERBANKPASSWORD, escaped?"},
                    {"&ctl00%24ContentPlaceHolder1%24loginToDataTrac%24LoginButton=", "Log+In"},
                    {"&ctl00_radToolTip_ClientState=", ""},
                    {"&ctl00%24textBoxSearch=", ""}
                };
            */
            var usernameEscaped = HttpUtility.UrlEncode(Username);
            var pwdEscaped = HttpUtility.UrlEncode(Password);

            FormVals.Add("ctl00%24ContentPlaceHolder1%24loginToDataTrac%24UserName", usernameEscaped);
            FormVals.Add("ctl00%24ContentPlaceHolder1%24loginToDataTrac%24Password", pwdEscaped);
            FormVals.Add("ctl00%24ContentPlaceHolder1%24loginToDataTrac%24LoginButton", "Log+In");
            FormVals.Add("ctl00%24textBoxSearch", "");

            //omit extras & and = above
            //var postData = string.Join("", postParams.Select(p => p.Key + p.Value));
            var postData = String.Join("&", FormVals.Select(v => v.Key + "=" + v.Value));
            var referer = "https://portal.interbankwholesale.com/IBPortal/Login.aspx";


            var responseObj = ConnectionMethods.IBWPost("https://portal.interbankwholesale.com/IBPortal/Login.aspx",
                                                        SessionCookies, postData, referer);
            SessionCookies.Add(responseObj.ResponseCookies);

        }

        //Step 3 - 
        public void Step3_FetchDashboard()
        {
            /*
            GET https://portal.interbankwholesale.com/IBPortal/Dashboard.aspx HTTP/1.1
            Host: portal.interbankwholesale.com
            Connection: keep-alive
            Cache-Control: max-age=0
            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/
            /*;q=0.8
            User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1652.0 Safari/537.36
            Referer: https://portal.interbankwholesale.com/IBPortal/Login.aspx
            Accept-Encoding: gzip,deflate,sdch
            Accept-Language: en-US,en;q=0.8
            Cookie: ASP.NET_SessionId=no34bx4kb15juj4lbavf2ciz; .ASPXINTERBANK=48BE739604BA3471D0DE458EF65EE5B6A78C85C0F5C083AA3BAD389FD4784AD0B936EE2930BC4DD83A3888B52D9038670073F71C8AD7021502FD464710D941E961D28168920F3F109EA2AC6E39D07AB663979E7E15AADD8D65047F6B2FBEAEA6FB7F27ACCE4347A69924EE87693F9CCF9C11F3E104E7D54782F4948B822DABA2BA0B4BD727BD89266DAE1E99FA0D013BD26DB9D4657EECFE3E9B141BA3216605; BNI_BARRACUDA_LB_COOKIE=0000000000000000000000002406a8c000005000
            */

            System.Diagnostics.Debug.WriteLine("Hit Step3_FetchDashboard");

            var responseObj = ConnectionMethods.IBWGet("https://portal.interbankwholesale.com/IBPortal/Dashboard.aspx",
                                                       SessionCookies,
                                                       "https://portal.interbankwholesale.com/IBPortal/Login.aspx");
            SessionCookies.Add(responseObj.ResponseCookies);
            if (true)
                IsCompleteConnection = true;

        }

        //Step 4 - GET the pipeline
        public void Step4_GetPipeline()
        {   /*
            GET https://portal.interbankwholesale.com/IBPortal/General/Pipeline.aspx HTTP/1.1
            Host: portal.interbankwholesale.com
            Connection: keep-alive
            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*//*;q=0.8
            User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1652.0 Safari/537.36
            Referer: https://portal.interbankwholesale.com/IBPortal/Dashboard.aspx
            Accept-Encoding: gzip,deflate,sdch
            Accept-Language: en-US,en;q=0.8
            Cookie: ASP.NET_SessionId=si4p5yie5zzkjfglhailwuja; .ASPXINTERBANK=F400225B2197BC9C2CB03EC4DAC06FC56C109F011F8F1DD479BC7302E51CC88DABC210200371FAFAFB2C06C14166BAE39B0821B366346022D5CBAC59A91DF994778DCBA0BB6C2C524D960BCFF0649C8F9C07A0CC35A81678CC3625EA976DA4C29FD89CAE7BCB09BE0A4A2B3C87A523335C87FBA1A4391B301EC57462925D1843C855EE3FF25A1AEC837AD71469C0FA5D82A6F0084D341120CCF0D9CADCAEF764; BNI_BARRACUDA_LB_COOKIE=0000000000000000000000002406a8c000005000
            */

            System.Diagnostics.Debug.WriteLine("Hit Step4_GetPipeline");

            var responseObj =
                ConnectionMethods.IBWGet("https://portal.interbankwholesale.com/IBPortal/General/Pipeline.aspx",
                                         SessionCookies, "https://portal.interbankwholesale.com/IBPortal/Dashboard.aspx");
            SessionCookies.Add(responseObj.ResponseCookies);

            fillformvals(responseObj.ResponseHtml);
            FormVals["ctl00_mainMenu_ClientState"] = "";
            FormVals["ctl00_BreadCrumbSiteMap_ClientState"] = "";
            FormVals["ctl00%24ContentPlaceHolder1%24dropdownlistKeyDates"] = "Changed";
            FormVals["ctl00%24ContentPlaceHolder1%24dropdownlistLoanPurpose"] = "All";

            var searchDateFrom = HttpUtility.UrlEncode(DateTime.UtcNow.AddMonths(-3).ToString("d"));
            var searchDateTo = HttpUtility.UrlEncode(DateTime.UtcNow.ToString("d"));

            FormVals["ctl00%24ContentPlaceHolder1%24textboxFromDate"] = searchDateFrom;
            FormVals["ctl00%24ContentPlaceHolder1%24textboxToDate"] = searchDateTo;
            FormVals["ctl00%24ContentPlaceHolder1%24textBoxQuickFind"] = "";
            //_formVals["ctl00%24ContentPlaceHolder1%24radGridPipeline%24ctl00%24ctl04%24GECBtnExpandColumn"] = "+"; //ctl00$ContentPlaceHolder1$radGridPipeline$ctl00$ctl04$GECBtnExpandColumn <- UW Denied
                                                                                                                   //ctl00$ContentPlaceHolder1$radGridPipeline$ctl00$ctl07$GECBtnExpandColumn <- UW approved

            FormVals["ctl00_ContentPlaceHolder1_radGridPipeline_ClientState"] = "";
            FormVals["ctl00_radToolTip_ClientState"] = "";
            FormVals["ctl00%24textBoxSearch"] = "";

            var responseHtmlDoc = new HtmlDocument();
            responseHtmlDoc.LoadHtml(responseObj.ResponseHtml);


            foreach (HtmlNode node in responseHtmlDoc.DocumentNode.Descendants("input"))
                if (node.Attributes["type"] != null && node.Attributes["type"].Value == "submit" && node.Attributes["name"].Value.StartsWith("ctl00$ContentPlaceHolder1$radGridPipeline$ctl00$"))
                    PipelineRows.Add(HttpUtility.UrlEncode(node.Attributes["name"].Value));





        }
        //Step 5 - POST pipeline searches
        public Dictionary<string, string> Step5_PostSearchQueries()
        {   /*
            POST https://portal.interbankwholesale.com/IBPortal/General/Pipeline.aspx HTTP/1.1
            Host: portal.interbankwholesale.com
            Connection: keep-alive
            Content-Length: 212243
            Cache-Control: max-age=0
            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*//*;q=0.8
            Origin: https://portal.interbankwholesale.com
            User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1652.0 Safari/537.36
            Content-Type: application/x-www-form-urlencoded
            Referer: https://portal.interbankwholesale.com/IBPortal/General/Pipeline.aspx
            Accept-Encoding: gzip,deflate,sdch
            Accept-Language: en-US,en;q=0.8
            Cookie: ASP.NET_SessionId=si4p5yie5zzkjfglhailwuja; .ASPXINTERBANK=F400225B2197BC9C2CB03EC4DAC06FC56C109F011F8F1DD479BC7302E51CC88DABC210200371FAFAFB2C06C14166BAE39B0821B366346022D5CBAC59A91DF994778DCBA0BB6C2C524D960BCFF0649C8F9C07A0CC35A81678CC3625EA976DA4C29FD89CAE7BCB09BE0A4A2B3C87A523335C87FBA1A4391B301EC57462925D1843C855EE3FF25A1AEC837AD71469C0FA5D82A6F0084D341120CCF0D9CADCAEF764; BNI_BARRACUDA_LB_COOKIE=0000000000000000000000002406a8c000005000
            */
            System.Diagnostics.Debug.WriteLine("Hit Step5_PostSearchQueries");

            //var postData = String.Join("&", _formVals.Select(v => v.Key + "=" + v.Value));
            var finalPageOutput = "";

            //TODO: can we make these requests in parallel?
            foreach (var rowCtrl in PipelineRows)
            {
                System.Diagnostics.Debug.WriteLine("Getting row: " + rowCtrl);

                var postData = String.Join("&", FormVals.Select(v => v.Key + "=" + v.Value));

                var postDataWithRow = postData + "&" + rowCtrl + "=+";
                
                var responseObj =
                    ConnectionMethods.IBWPost("https://portal.interbankwholesale.com/IBPortal/General/Pipeline.aspx",
                                              SessionCookies, postDataWithRow,
                                              "https://portal.interbankwholesale.com/IBPortal/General/Pipeline.aspx");

                SessionCookies.Add(responseObj.ResponseCookies);

                // "7%2F1%2F2013"
                var searchDateFrom = HttpUtility.UrlEncode(DateTime.UtcNow.AddMonths(-3).ToString("d"));
                // "9%2F29%2F2013"
                var searchDateTo = HttpUtility.UrlEncode(DateTime.UtcNow.ToString("d"));

                fillformvals(responseObj.ResponseHtml);
                FormVals["ctl00_mainMenu_ClientState"] = "";
                FormVals["ctl00_BreadCrumbSiteMap_ClientState"] = "";
                FormVals["ctl00%24ContentPlaceHolder1%24dropdownlistKeyDates"] = "Changed";
                FormVals["ctl00%24ContentPlaceHolder1%24dropdownlistLoanPurpose"] = "All";
                FormVals["ctl00%24ContentPlaceHolder1%24textboxFromDate"] = searchDateFrom;
                FormVals["ctl00%24ContentPlaceHolder1%24textboxToDate"] = searchDateTo;
                FormVals["ctl00%24ContentPlaceHolder1%24textBoxQuickFind"] = "";
                FormVals["ctl00_ContentPlaceHolder1_radGridPipeline_ClientState"] = "";
                FormVals["ctl00_radToolTip_ClientState"] = "";
                FormVals["ctl00%24textBoxSearch"] = "";

                //if (_pipelineRows.Last() == rowCtrl)
                finalPageOutput = responseObj.ResponseHtml;

            }

            
            var responseHtmlDoc = new HtmlDocument();
            responseHtmlDoc.LoadHtml(finalPageOutput);
            
            //TODO: we need to get the actual borr name, not just loan num
            //TODO: we include this in the separate thread, add them to common collection
            var loanRows = responseHtmlDoc.DocumentNode.Descendants("tr")
                .Where(d => d.Attributes.Contains("class") && (d.Attributes["class"].Value.Contains("rgRow") || d.Attributes["class"].Value.Contains("rgAltRow")));

            var borrNames = loanRows.Where(n => n.Descendants("td").Count() >= 7).Select(n => n.Descendants("td").ElementAt(6).InnerText);

            //var links = loanRows.Select(r => r.Descendants("a").First());
            
            var links = responseHtmlDoc.DocumentNode.Descendants("a")
                .Where(n => n.Attributes.Contains("href") && n.Attributes["href"].Value.StartsWith("LoanInfo.aspx?filenum="));


            //LoanInfo.aspx?filenum=

            LoanIds = links.Select(l => l.InnerText).ToList();

            LoanIdsNames.Clear();
            LoanIdsNames = LoanIds.Zip(borrNames, (k, v) => new {k, v}).ToDictionary(x => x.k, x => x.v);

            return LoanIdsNames;
        }

        public List<LoanCondition> FillLoanConditions(LoanSearchResultItem selectedLoan)
        {
            //"https://portal.interbankwholesale.com/IBPortal/Docs/UploadDocument.aspx?filenum=88284420&queue=1"
            var queryUrl = String.Format("https://portal.interbankwholesale.com/IBPortal/Docs/UploadDocument.aspx?filenum={0}&queue=1", selectedLoan.IBWLoanNum);

            var responseObjUploadPg = ConnectionMethods.IBWGet(queryUrl, SessionCookies);

            SessionCookies.Add(responseObjUploadPg.ResponseCookies);
            fillformvals(responseObjUploadPg.ResponseHtml);

            var responseHtmlDoc = new HtmlDocument();
            responseHtmlDoc.LoadHtml(responseObjUploadPg.ResponseHtml);

            var conditionsRows = responseHtmlDoc.GetElementbyId("ctl00_ContentPlaceHolder1_gridViewConditions_ctl00").Descendants("tbody").First().Descendants("tr");

            //var conditions = new List<LoanCondition>();
            SelectedLoansConditions.Clear();

            foreach (var row in conditionsRows)
            {
                var boxes = row.Descendants("td").ToArray();
                SelectedLoansConditions.Add(new LoanCondition
                {
                    CheckboxName = boxes[0].Descendants("span").ElementAt(0).Descendants("input").First().GetAttributeValue("name", ""),
                    PriorTo = boxes[1].InnerText == "D" ? LoanCondition.PriorToTypes.D : LoanCondition.PriorToTypes.F,
                    Number = Convert.ToInt32(boxes[2].InnerText),
                    Description = boxes[3].Descendants("span").First().InnerText.Replace(Environment.NewLine, " "),
                    ConditionId = boxes[4].InnerText
                });
            }

            SelectedLoansConditions.Sort((c1, c2) => c1.Number.CompareTo(c2.Number));
            
            return SelectedLoansConditions;

        }



        //Step 6 - 
        //Step 7 - 
        //Step 8 - 
        

        //This can really be cut down to just getting __viewstate __eventvalidtion, etc
        private void fillformvals(string responseHtml)
        {
            FormVals = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(responseHtml))
            {
                var responseHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                responseHtmlDoc.LoadHtml(responseHtml);

                //Find the question submit form

                //HtmlNode questionTypeNode = responseHtmlDoc.DocumentNode.SelectSingleNode("pqestionCode"); //qestion not a typo?

                foreach (HtmlNode node in responseHtmlDoc.DocumentNode.Descendants("input"))
                    if (node.Attributes["type"] != null && node.Attributes["type"].Value == "hidden")
                        FormVals.Add(System.Web.HttpUtility.UrlEncode(node.GetAttributeValue("name", "")), System.Web.HttpUtility.UrlEncode(node.GetAttributeValue("value", "")));

            }



        }
    }
}