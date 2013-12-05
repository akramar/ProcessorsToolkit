using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using System.Xml;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.Model.PRMG.UploadSession
{
    public class AvailableLoansList : ObservableCollection<LoanSearchResultItem>
    {
        //private readonly UploadWindowVM _parentVM;
        private readonly ImageFlowSession _imageFlowSession;

        public AvailableLoansList(ImageFlowSession imageFlowSession)//UploadWindowVM parentVM)
        {
            _imageFlowSession = imageFlowSession;
        }

        
        public void LoadList()
        {
            //var newList = new List<Tuple<string, string, string>>();

            const string searchUrl = "https://imageflow.prmg.net/zapp/Avista/loanSearchCritiaPro.aspx";
            const string postData = "clickAction=&lastname=&ssn=&loanNumber=&creationDate=&submitSearch=Search";

            /*var request = (HttpWebRequest)WebRequest.Create(searchUrl);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Referer = "https://imageflow.prmg.net/zapp/Avista/loanSearchCritiaFrame.aspx?clickAction=&";
            request.Accept = "*//*"; //<-- remove a slash to fix this comment thing
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            request.Host = "imageflow.prmg.net";
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(UploadWindowVM.CurrImgFlowSession.SessionCookies);// _parentVM.CurrImgFlowSession.SessionCookies; //Globals.OurImageFlowSession.sessionCookies;// ImageFlowDataFetcher.CookieThief.GetUriCookieContainer(new Uri("https://imageflow.prmg.net"));

            const string postData = "clickAction=&lastname=&ssn=&loanNumber=&creationDate=&submitSearch=Search";
            var enconding = new ASCIIEncoding();
            string responseHtml = null;
            var byte1 = enconding.GetBytes(postData);
            try
            {
                request.ContentLength = byte1.Length;
                request.GetRequestStream().Write(byte1, 0, byte1.Length);
                

                var response = (HttpWebResponse) request.GetResponse();
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                    responseHtml = new StreamReader(responseStream).ReadToEnd();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                //response.Close();
                request.GetRequestStream().Dispose();
            }
            if (responseHtml == null) 
                return;
            */

            var responseObj = ConnectionMethods.ImgFlowPost(searchUrl, _imageFlowSession.SessionCookies,
                                                         postData);
            _imageFlowSession.SessionCookies.Add(responseObj.ResponseCookies);         
       
            var searchResponseHtml = new HtmlAgilityPack.HtmlDocument();
            searchResponseHtml.LoadHtml(responseObj.ResponseHtml);

            var foundMatchingloan = false;
            var tableBody = searchResponseHtml.DocumentNode.SelectSingleNode("//tbody");
            foreach (var row in tableBody.SelectNodes("tr"))
            {
                var cells = row.SelectNodes("th|td");
                if (cells.Count == 3)
                {
                    //input type="checkbox" name="selectedLoan" onClick="selectedLoan(216492);"/>
                    //if (row.Descendants("input") != null)
                    if (row.Descendants("input").Any())
                    {
                        var childNodes = row.ChildNodes; //["//input"];// .SelectNodes("//input");
                        var singleInput = childNodes[0].Descendants().First();

                        //HtmlNode inputNode = row.SelectSingleNode("//input");
                        if (singleInput != null && singleInput.Attributes.Contains("onClick"))
                        {
                            var containerIdRaw = singleInput.GetAttributeValue("onClick", String.Empty);
                            if (containerIdRaw != String.Empty)
                            {
                                //container id: //input type="checkbox" name="selectedLoan" onClick="selectedLoan(216492);"/>
                                //name: cells[0].InnerText
                                //loan num: cells[1].InnerText
                                //loan amt: cells[2].InnerText
                                
                                var newLoanItem = new LoanSearchResultItem();

                                var expression = new Regex(@"selectedLoan\((.+)\);");
                                var matches = expression.Matches(containerIdRaw);
                                if (matches[0].Groups.Count >= 2)
                                    newLoanItem.ContId = matches[0].Groups[1].Value;

                                newLoanItem.BorrNameRaw = cells[0].InnerText;
                                expression = new Regex(@"(.+), (.+)");
                                matches = expression.Matches(newLoanItem.BorrNameRaw);
                                if (matches[0].Groups.Count >= 3)
                                {
                                    newLoanItem.BorrLastName = matches[0].Groups[1].Value;
                                    newLoanItem.BorrFirstName = matches[0].Groups[2].Value;
                                }

                                newLoanItem.PRMGLoanNum = cells[1].InnerText;
                                newLoanItem.LoanAmt = cells[2].InnerText.Replace("$","").Replace("&nbsp;","");

                                //This is where we're checking if the current loan being added mached MainWindow's selected borr 
                                if (!foundMatchingloan &&
                                    MainWindowVM.SelectedBorrDir.BorrDirName.StartsWith(newLoanItem.BorrLastName,
                                                                                     StringComparison
                                                                                         .InvariantCultureIgnoreCase))
                                {
                                    newLoanItem.IsSelected = true;
                                    foundMatchingloan = true;
                                }
                                Add(newLoanItem);

                            }
                        }
                    }
                }
            }
        }

        public LoanSearchResultItem GetMatchedLoanByContId(string containerId)
        {            
            return this.FirstOrDefault(l => l.ContId == containerId);
        }
    }
    public class LoanSearchResultItem
    {
        public string BorrNameRaw { get; set; }
        public string BorrLastName { get; set; }
        public string BorrFirstName { get; set; }
        public string PRMGLoanNum { get; set; }
        public string LoanAmt { get; set; }
        public string ContId { get; set; }
        public string DisplayName { get { return String.Format("{0}, {1} #{2}", BorrLastName, BorrFirstName, PRMGLoanNum); } }
        public bool IsSelected { get; set; }
    }
}
