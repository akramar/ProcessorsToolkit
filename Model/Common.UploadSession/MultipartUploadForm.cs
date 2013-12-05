using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessorsToolkit.Model.Common.UploadSession
{
    /// <summary>
    /// This class probably shouldn't be used to include the uploading file 
    /// because it will turn the whole thing into a string, vs reading it out to
    /// the request stream directly in SingleFileUpload. Only use this for the form fields.
    /// </summary>
    class MultipartUploadForm : List<UploadFormField>
    {
        //public string Boundary { get; set; }

        public MultipartUploadForm()
        {
            

        }

        
    }
    /*
    internal class UploadFormField
    {
        //public string ContentDisposition { get; set; }
        public string Name { get; set; }
        //public string ContentType { get; set; }
        public string Body { get; set; }
        //public string Boundary { get; set; }

        public new string ToString()
        {
            const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

            var builder = new StringBuilder();

            //builder.Append("\r\n--" + boundary + "\r\n");
            builder.Append(String.Format(formdataTemplate, Name, Body));

            return builder.ToString();
        }
    }*/
    internal class UploadFormField
    {
        // Octet header looks like:
        // -----------------------------7dd34e19e0964
        // Content-Disposition: form-data; name="ctl00$File2"; filename=""
        // Content-Type: application/octet-stream

        public UploadFormField(ContentTypes contentType)
        {
            ContentType = contentType;
        }

        public string Name { get; set; }
        public string Filename { get; set; }
        public ContentTypes ContentType { get; set; }
        public string Body { get; set; }

        public new string ToString()
        {
            var builder = new StringBuilder();
            switch (ContentType)
            {
                case ContentTypes.None:
                    {
                        const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                        builder.Append(String.Format(formdataTemplate, Name, Body));
                        break;
                    }

                case ContentTypes.OctetStream:
                    {
                        const string formdataTemplate =
                            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n{2}";
                        builder.Append(String.Format(formdataTemplate, Name, Filename, Body));
                        break;
                    }
            }
            return builder.ToString();
        }
    }
    
    internal enum ContentTypes { None, OctetStream }

}
