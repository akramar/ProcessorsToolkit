using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;

namespace ProcessorsToolkit.Model.Common.UploadSession
{
    class SingleFileUpload
    {
        public static int HttpUploadFile(string postingUrl, string filePath, MultipartUploadForm form,
             CookieCollection sessionCookies, string uploadingFileFieldname)
        {
            var responseCode = 0;
            if (false) // skip for debug, we know this portion works
            {
                System.Diagnostics.Debug.WriteLine(String.Format("TEST Uploading {0} to {1}", filePath, postingUrl));

                System.Threading.Thread.Sleep(4000);
                responseCode = 0;

            }
            else
            {
                //http://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data
                

                System.Diagnostics.Debug.WriteLine(string.Format("Uploading {0} to {1}", filePath, postingUrl));
                var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                

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

                //TODO: mess around with compression settings below
                //wr.TransferEncoding = "gzip, deflate";
                //wr.SendChunked = true;
                //wr.CachePolicy = System.Net.Cache.RequestCacheLevel.NoCacheNoStore;

                //int threadAvail;
                //int threadMax;
                //System.Threading.ThreadPool.GetAvailableThreads(out threadAvail, out threadMax);


                var rs = wr.GetRequestStream();

                //const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                var boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                /************ This is the form fields before the file upload ****************/
                foreach (var field in form)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    var fieldBytes = Encoding.UTF8.GetBytes(field.ToString());
                    rs.Write(fieldBytes, 0, fieldBytes.Length);
                }

                rs.Write(boundarybytes, 0, boundarybytes.Length);

                //TODO: Improve using http://stackoverflow.com/questions/7124797/httprequest-and-post
                
                /************ This is the actual file upload ****************/
                var fileNameOnly = Path.GetFileName(filePath);
                const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                var header = string.Format(headerTemplate, uploadingFileFieldname, fileNameOnly, "application/pdf");
                var headerbytes = Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                //TODO: This is using ASCII but form.ToString() is using UTF8? we're all mismatched
                var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
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
