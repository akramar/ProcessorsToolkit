using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ProcessorsToolkit.Model
{
    internal class FormFetcher
    {
        public FormReference FormRef { get; set; }

        public void CreateForm() //trash this, not used
        {

            var serializer = new XmlSerializer(typeof (FormReference));
            using (var reader = XmlReader.Create("locations.xml"))
            {
                Info info = (Info) serializer.Deserialize(reader);
                //List<Location> locations = info.Locations;
                // do whatever you wanted to do with those locations
            }
        }

        public void LoadFormReference()
        {


            var a = Assembly.GetExecutingAssembly();
            using (var stream = a.GetManifestResourceStream("ProcessorsToolkit.Model.FormReference.xml"))
                if (stream != null)
                    using (var reader = XmlReader.Create(stream))
                    {

                        var serializer = new XmlSerializer(typeof(FormReference));
                        //var lenders = (List<Lender>) serializer.Deserialize(stream);

                        //var reference = (FormReference)serializer.Deserialize(reader);
                        FormRef = (FormReference)serializer.Deserialize(reader);
                        var lenders = FormRef.Lenders;

                    }

        }

        public FileInfo DownloadForm(string lenderName, string formName, string savePathRoot)
        {


            var lender = FormRef.Lenders.FirstOrDefault(l => l.Name == lenderName);
            if (lender == null)
                return null;

            var form = lender.Forms.FirstOrDefault(f => f.Name == formName);

            if (form == null)
                return null;



            //DestFilePath = AssemblePathToForm(BorrDirectory.FullRootPath, FormFilename);
            var fileSaveLoc = savePathRoot + "\\" + form.Filename;
            
            var fileIter = 0;
            while (File.Exists(fileSaveLoc))
            {
                fileIter++;
                if (!File.Exists(fileSaveLoc.Insert((fileSaveLoc.LastIndexOf('.')), fileIter.ToString(" (0)"))))
                    break;
            }
            if (fileIter != 0)
                fileSaveLoc = fileSaveLoc.Insert((fileSaveLoc.LastIndexOf('.')), fileIter.ToString(" (0)"));



            //http://processorstoolkit.coaha.net/forms/IBW/Interbank_DisclosurePkg_rev4b_(CA).pdf
            var fileRemoteLoc = "http://processorstoolkit.coaha.net/forms/" + lender.Name + "/" + form.Filename;
            //var fileSaveLoc = savePathRoot + "\\" + form.Name;
            
            using (var wc = new WebClient())
            {
                wc.DownloadFile(fileRemoteLoc, fileSaveLoc);
            }

            var fi = new FileInfo(fileSaveLoc);

            return fi;
        }
    }
}
