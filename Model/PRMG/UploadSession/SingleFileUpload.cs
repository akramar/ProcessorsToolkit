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

namespace ProcessorsToolkit.Model.PRMG.UploadSession
{
    class SingleFileUpload
    {
        public static int HttpUploadFile(/*string url,*/ string file, string paramName, string contentType, List<Tuple<string, string>> formItemsList,
            string sessionKey, string containerKey, CookieCollection sessionCookies)
        {
            int responseCode = 0;
            if (false) // skip for debug, we know this portion works
            {
                System.Threading.Thread.Sleep(2000);
                responseCode = 1;

            }
            else
            {
                //http://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data

                string url = "https://imageflow.prmg.net:443/xsuite/xapps/xdoc/webservice/xWsProcess.aspx";

                System.Diagnostics.Debug.WriteLine(string.Format("Uploading {0} to {1}", file, url));
                //log.Debug(string.Format("Uploading {0} to {1}", file, url));
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");



                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                wr.Timeout = 1200000;
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.KeepAlive = true;
                wr.Credentials = CredentialCache.DefaultCredentials;
                //wr.CookieContainer = ImageFlowDataFetcher.CookieThief.GetUriCookieContainer(new Uri("https://imageflow.prmg.net"));
                wr.CookieContainer = new CookieContainer();
                wr.CookieContainer.Add(sessionCookies);
                wr.Accept = "text/html, application/xhtml+xml, */*";

                //Referer: https://imageflow.prmg.net/xsuite/xapps/xdoc/docUpload/default.aspx?xProjectId=1000&xToolId=DOCUMENTUPLOAD&sessiondata=2930647566935531520&xContainerKey=215743

                wr.Referer = "https://imageflow.prmg.net/xsuite/xapps/xdoc/docUpload/default.aspx?xProjectId=1000&xToolId=DOCUMENTUPLOAD&sessiondata="
                 + sessionKey + "&xContainerKey=" + containerKey;


                wr.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";

                //wr.TransferEncoding = "gzip, deflate";
                //wr.SendChunked = true;
                //wr.CachePolicy = System.Net.Cache.RequestCacheLevel.NoCacheNoStore;

                //int threadAvail;
                //int threadMax;
                //System.Threading.ThreadPool.GetAvailableThreads(out threadAvail, out threadMax);


                Stream rs = wr.GetRequestStream();

                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                //foreach (string key in nvc.Keys)
                foreach (var formpair in formItemsList)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    //string formitem = string.Format(formdataTemplate, key, nvc[key]);
                    string formitem = string.Format(formdataTemplate, formpair.Item1, formpair.Item2);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, paramName, file, contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                WebResponse wresp = null;


                try
                {
                    wresp = wr.GetResponse();
                    Stream stream2 = wresp.GetResponseStream();
                    StreamReader reader2 = new StreamReader(stream2);
                    System.Diagnostics.Debug.WriteLine(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
                    responseCode = 1;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error uploading file", ex);
                    if (wresp != null)
                    {
                        wresp.Close();
                        wresp = null;
                    }
                }
                finally
                {
                    wr = null;

                }
            }            
            return responseCode;
        }
    }
}
