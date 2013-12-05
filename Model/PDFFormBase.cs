using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using iTextSharp.text.pdf;

namespace ProcessorsToolkit.Model
{
    abstract class PDFFormBase
    {

        protected Dictionary<string, string> FormFieldsVals { get; set; }
        //protected string FormFilename { get; set; }
        protected string FormName { get; set; }
        public BorrDir BorrDirectory { get; set; }
        public FannieData SrcBorrData { get; set; }
        //private string DestFilePath { get; set; }
        public string LenderNameShort { get; set; }
        public static string LenderName { get; set; }
        public static string LenderAddrFull { get; set; }
        private FileInfo DestFile { get; set; }

        protected PDFFormBase()
        {
            FormFieldsVals = new Dictionary<string, string>();
        }


        public abstract void Fill();

        protected void DownloadFormTemplate()//string lenderName, string formName)
        {
            var formFetcher = new FormFetcher();
            formFetcher.LoadFormReference();

            DestFile = formFetcher.DownloadForm(LenderNameShort, FormName, BorrDirectory.FullRootPath);


        }

        
        protected void FillTheForm()
        {
           /* if (String.IsNullOrEmpty(FormFilename))
                throw new Exception("No form path provided");

            if (BorrDirectory == null)
                throw new Exception("No borrower directory provided");
            
            //using (var pdfFlat = new MemoryStream())
            */



            //using (var fileOut = new FileStream(DestFile.FullName, FileMode.Open, FileAccess.Write))
            using (var ms = new MemoryStream())
            {
                //use a temp or memory stream here assource
                //var pdfReader = new PdfReader(AssemblePathToForm(@"C:\Users\Alain Kramar\Documents\Loans\_forms", FormFilename));
                var pdfReader = new PdfReader(DestFile.FullName);

                //var pdfStamp = new PdfStamper(pdfReader, fileOut);
                var pdfStamp = new PdfStamper(pdfReader, ms);
                //var fields = pdfStamp.AcroFields;

                var fieldList = new List<string>();

                foreach (DictionaryEntry field in pdfStamp.AcroFields.Fields)
                {
                    fieldList.Add((string) field.Key);
                    //FormFieldsVals.Add(field.ToString(), "");

                    var valToInsert = FormFieldsVals.FirstOrDefault(f => f.Key == (string) field.Key);
                    //if (valToInsert != null)
                    if (valToInsert.Key != null && valToInsert.Value != null)
                        pdfStamp.AcroFields.SetField((string)field.Key, valToInsert.Value);


                    //<!!! // pick up here, need to assign
                }

                //pdfReader.AcroFields.SetField()

                pdfReader.Close();
                pdfStamp.FormFlattening = false;
                pdfStamp.FreeTextFlattening = false;
                pdfStamp.Writer.CloseStream = false;
                pdfStamp.Close();

                ms.WriteTo(new FileStream(DestFile.FullName, FileMode.Create, FileAccess.Write));
            }


            /*
            var sb = new StringBuilder();
            foreach (var de in pdfReader.AcroFields.Fields)
            {
                sb.Append(de.Key.ToString() + Environment.NewLine);
            }
                */
        }

        public void OpenForm()
        {
            System.Diagnostics.Process.Start(DestFile.FullName);
        }

        public static string AssemblePathToForm(string borrDirPath, string formFilename)
        {
            return borrDirPath + "\\" + formFilename;
        }


    }
}
