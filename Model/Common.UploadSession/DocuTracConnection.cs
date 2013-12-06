using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace ProcessorsToolkit.Model.Common.UploadSession
{
    class DocuTracConnection
    {
        public enum DocuTracLoginType {FormPost, SessionDataParamenter} // Plaza uses form post, PRMG uses SessionData parameter
        private readonly CookieCollection _sessionCookies = new CookieCollection();

        // https://docutrac.plazahomemortgage.com/xsuite/xapps/xdoc/containerViewer/default.aspx?xProjectId=1000&xToolId=CONTAINERVIEWER&xContainerKey=08%3bPQ
        string _containerViewPath = "/xsuite/xapps/xdoc/containerViewer/default.aspx?xProjectId=1000&xToolId=CONTAINERVIEWER&xContainerKey="; //URL encode: "08%3bPQ";
        // https://docutrac.plazahomemortgage.com/xsuite/xapps/xdoc/docUpload/default.aspx?xProjectId=1000&xToolId=CONTAINERVIEWER&xContainerKey=08%3bPQ
        string _containerUploadPath = "/xsuite/xapps/xdoc/docUpload/default.aspx?xProjectId=1000&xToolId=CONTAINERVIEWER&xContainerKey="; //URL encode: "08%3bPQ";
        // upload URL: https://imageflow.prmg.net:443/xsuite/xapps/xdoc/webservice/xWsProcess.aspx 
        // Plaza loan lookup: https://docutrac.plazahomemortgage.com/zapp/xapps/dmd/Controls/containerLookup.aspx?xProjectId=1000&xAttName=&xAttValue=&
        // PRMG loan lookup: https://imageflow.prmg.net/zapp/Avista/loanSearchCritiaFrame.aspx?clickAction=&


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtCookies"></param>
        /// <param name="dtHostHeader">The lender's local header, like "http://imageflow.prmg.net:443" or "https://docutrac.plazahomemortgage.com"</param>
        /// <param name="dtLoginType">Plaza is a form post, PRMG is a parameter at end of login Url</param>
        /// <param name="credentialsToPost"></param>
        /// <param name="containerId"></param>
        public void Blah(CookieCollection dtCookies, string dtHostHeader, string containerId) //, DocuTracLoginType dtLoginType, string credentialsToPost)
        {
            _sessionCookies.Add(dtCookies);
            var headerToUse = (dtHostHeader.StartsWith("https://")) ? dtHostHeader.Replace("https://", "") : dtHostHeader;
            headerToUse = (headerToUse.Contains(':')) ? headerToUse.Split(':')[0] : headerToUse;
            var getViewContainerUrl = dtHostHeader + _containerViewPath + HttpUtility.UrlEncode(containerId);

            var responseObj = DocuTracConnectionMethods.DocuTracGet(getViewContainerUrl, headerToUse, _sessionCookies);
            
            _sessionCookies.Add(responseObj.ResponseCookies);



        }

        


        public MultipartUploadForm UploadForm(string containerKey, string loanId, string borrNameRaw, string fileNameFull, string docTypeId)
        {
            var multipartForm = new MultipartUploadForm()
                {
                    new UploadFormField(ContentTypes.None) {Name = "xProjectId", Body = "1000"},
                    new UploadFormField(ContentTypes.None) {Name = "xDefaultDocType", Body = ""},
                    new UploadFormField(ContentTypes.None) {Name = "xDefaultColor", Body = "0"},
                    new UploadFormField(ContentTypes.None) {Name = "xFixedSizeWidth", Body = ""},
                    new UploadFormField(ContentTypes.None) {Name = "xHideTitleBar", Body = "False"},
                    new UploadFormField(ContentTypes.None) {Name = "xBatchInc", Body = "0"},
                    new UploadFormField(ContentTypes.None) {Name = "xProjectId", Body = "1000"},
                    //new Tuple<string,string>("xContainerKey","214085"), <- PRMG
                    //new Tuple<string,string>("xContainerKey","08;PQ"), <- Plaza test borr
                    new UploadFormField(ContentTypes.None) {Name = "xContainerKey", Body = containerKey},
                    new UploadFormField(ContentTypes.None) {Name = "xWsSuccessMsg", Body = "Document upload successful."},
                    new UploadFormField(ContentTypes.None) {Name = "xWsNamespace", Body = "XDOC.INBOX.FILEUPLOAD"},
                    new UploadFormField(ContentTypes.None) {Name = "xWsPostDialog", Body = "/xsuite/xapps/std/axStdWebservicePostDialog.aspx"},
                    new UploadFormField(ContentTypes.None) {Name = "xWsPostUrl", Body = "/xsuite/xapps/xdoc/webservice/xWsProcess.aspx"},
                    new UploadFormField(ContentTypes.None) {Name = "xWsCallback", Body = "parent.gxMainController.submitCallBack"},
                    new UploadFormField(ContentTypes.None) {Name = "xColor", Body = "1"},
                    new UploadFormField(ContentTypes.None) {Name = "xContainerKey", Body = containerKey}, //duplicated here, see if removable
                    new UploadFormField(ContentTypes.None) {Name = "XCA29xContainerId", Body = ""},
                    //new Tuple<string,string>("XCA29xContainerRef","3254506880"), <- PRMG
                    new UploadFormField(ContentTypes.None) {Name = "XCA29xContainerRef", Body = loanId}, //At plaza, is LoanId, not AppId
                     //new Tuple<string,string>("xContAtt","3254506880"), <- PRMG lenderId, same at Plaza
                    new UploadFormField(ContentTypes.None) {Name = "xContAtt", Body = loanId},
                    //new Tuple<string,string>("xContAtt","TODD JONES"),
                    //new Tuple<string, string>("xContAtt", _parentVM.TargetLoanItem.BorrNameRaw), <- PRMG
                    new UploadFormField(ContentTypes.None) {Name = "xContAtt", Body = borrNameRaw},
                    //With Plaza, this includes the property address
                    //new Tuple<string, string>("XCA33_schemaIds", "209638"), <- PRMG
                    //new Tuple<string, string>("XCA33_schemaIds", "209654"), <- PRMG
                    //new Tuple<string, string>("XCA33_schemaIds", "209681"), <- PRMG
                    new UploadFormField(ContentTypes.None) {Name = "xFileName", Body = fileNameFull},
                    new UploadFormField(ContentTypes.None) {Name = "xDocTypeId", Body = docTypeId}

                };

            multipartForm.Add(new UploadFormField(ContentTypes.None){Name = "xWsPayload", Body = PayloadEnvelope.CreateEnvelopeString(multipartForm)});
            multipartForm.ForEach(val => System.Diagnostics.Debug.WriteLine("name: {0}   body: {1}", val.Name, val.Body));

            return multipartForm;
        }

        private static class PayloadEnvelope
        {
            public static string CreateEnvelopeString(MultipartUploadForm inputForm)
            {
                /*PRMG: <envelope xProjectId="1000" ><file xProjectId="1000" xBatchId="" 
                 * xSchemaId="209638" xContainerKey="215743" xContainerRef="3254508469" 
                 * xContainerId="" xColor="" xFileName="C:\fakepath\Moyer - W2 2010.pdf" ></file></envelope> */

                /*Plaza: <envelope xProjectId="1000" ><file xProjectId="1000" xBatchId=""
                 * xSchemaId="210282" xContainerKey="08;PQ" xContainerRef="4813110268" 
                 * xContainerId="" xColor="" xFileName="C:\fakepath\Test Document.pdf" ></file></envelope> */

                var doc = new XmlDocument();
                var el = (XmlElement) doc.AppendChild(doc.CreateElement("envelope"));
                el.SetAttribute("xProjectId", inputForm.First(v => v.Name == "xProjectId").Body);
                var envChild = (XmlElement) el.AppendChild(doc.CreateElement("file"));
                envChild.SetAttribute("xProjectId", inputForm.First(v => v.Name == "xProjectId").Body);
                envChild.SetAttribute("xBatchId", "");
                envChild.SetAttribute("xSchemaId", inputForm.First(v => v.Name == "xDocTypeId").Body);
                //TODO: seems prone to error, check later
                envChild.SetAttribute("xContainerKey", inputForm.First(v => v.Name == "xContainerKey").Body);
                envChild.SetAttribute("xContainerRef", inputForm.First(v => v.Name == "XCA29xContainerRef").Body);
                envChild.SetAttribute("xContainerId", "");
                envChild.SetAttribute("xColor", inputForm.First(v => v.Name == "xColor").Body);
                envChild.SetAttribute("xFileName", inputForm.First(v => v.Name == "xFileName").Body);

                return doc.OuterXml;
            }
        }
    }

    internal class DocuTracConnectionMethods : ConnectionMethodsBase
    {
        //private const string DocuTracHost = "imageflow.prmg.net";

        protected internal static GetResponse DocuTracGet(string targetUrl, string hostHeader, CookieCollection cookies, string referer = "")
        {
            return Get(targetUrl, hostHeader, cookies, referer);
        }

        protected internal static PostResponse DocuTracPost(string targetUrl, string hostHeader, CookieCollection cookies, string postData,
                                                           string referer = "")
        {
            return Post(targetUrl, hostHeader, cookies, postData, referer);
        }
    }
}
