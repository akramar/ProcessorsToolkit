using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;

namespace ProcessorsToolkit.Model.Interbank.UploadSession
{
    class SingleFileUpload
    {
        public static int HttpUploadFile(string filePath, List<Tuple<string, string>> formItemsList,
             CookieCollection sessionCookies, string loanNum)
        {
            int responseCode = 0;
            if (false) // skip for debug, we know this portion works
            {
                System.Threading.Thread.Sleep(2000);
                responseCode = 0;

            }
            else
            {
                //http://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data

                string postingUrl =
                    String.Format("https://portal.interbankwholesale.com/IBPortal/Docs/UploadDocument.aspx?filenum={0}&queue=1",
                        loanNum);

                System.Diagnostics.Debug.WriteLine(string.Format("Uploading {0} to {1}", filePath, postingUrl));
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");



                var wr = (HttpWebRequest)WebRequest.Create(postingUrl);
                wr.Timeout = 1200000;
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.KeepAlive = true;
                wr.Credentials = CredentialCache.DefaultCredentials;
                wr.CookieContainer = new CookieContainer();
                wr.CookieContainer.Add(sessionCookies);
                wr.Accept = "text/html, application/xhtml+xml, */*";
                wr.Referer = postingUrl;


                wr.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";

                //wr.TransferEncoding = "gzip, deflate";
                //wr.SendChunked = true;
                //wr.CachePolicy = System.Net.Cache.RequestCacheLevel.NoCacheNoStore;

                //int threadAvail;
                //int threadMax;
                //System.Threading.ThreadPool.GetAvailableThreads(out threadAvail, out threadMax);


                Stream rs = wr.GetRequestStream();

                const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                
                foreach (var formpair in formItemsList)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    var formitem = string.Format(formdataTemplate, formpair.Item1, formpair.Item2);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
                rs.Write(boundarybytes, 0, boundarybytes.Length);

                //TODO: Improve using http://stackoverflow.com/questions/7124797/httprequest-and-post
                /************ This is the actual file upload ****************/
                var fileNameOnly = Path.GetFileName(filePath);
                const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                var header = string.Format(headerTemplate, "ctl00$ContentPlaceHolder1$fileUpload", fileNameOnly, "application/pdf");
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
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
