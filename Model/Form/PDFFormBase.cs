using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;

namespace ProcessorsToolkit.Model.Form
{
    abstract class PDFFormBase
    {

        protected Dictionary<string, string> FormFieldsVals { get; set; }
        protected string FormFilename { get; set; }
        public BorrDir BorrDirectory { get; set; }
        public FannieData SrcBorrData { get; set; }
        private string DestFilePath { get; set; }

        protected PDFFormBase()
        {
            FormFieldsVals = new Dictionary<string, string>();
        }

        protected void DownloadFormTemplate()
        {
            
        }

        public abstract void Fill();

        
        protected void FillTheForm()
        {
            if (String.IsNullOrEmpty(FormFilename))
                throw new Exception("No form path provided");

            if (BorrDirectory == null)
                throw new Exception("No borrower directory provided");
            
            //using (var pdfFlat = new MemoryStream())

            DestFilePath = AssemblePathToForm(BorrDirectory.FullRootPath, FormFilename);
            int fileIter = 0;
            while (File.Exists(DestFilePath))
            {
                fileIter++;
                if (!File.Exists(DestFilePath.Insert((DestFilePath.Length - 4), fileIter.ToString(" (0)"))))
                    break;
            }
            if (fileIter != 0)
                DestFilePath = DestFilePath.Insert((DestFilePath.Length - 4), fileIter.ToString(" (0)"));



            using (var fileOut = new FileStream(DestFilePath, FileMode.Create, FileAccess.Write))
            {
                //use a temp or memory stream here assource
                var pdfReader = new PdfReader(AssemblePathToForm(@"C:\Users\Alain Kramar\Documents\Loans\_forms", FormFilename));

                var pdfStamp = new PdfStamper(pdfReader, fileOut);
                //var fields = pdfStamp.AcroFields;

                var fieldList = new List<string>();

                foreach (DictionaryEntry field in pdfStamp.AcroFields.Fields)
                {
                    fieldList.Add((string) field.Key);
                    //FormFieldsVals.Add(field.ToString(), "");

                    var valToInsert = FormFieldsVals.FirstOrDefault(f => f.Key == (string) field.Key);
                    //if (valToInsert != null)
                    if (valToInsert.Key != null)
                        pdfStamp.AcroFields.SetField((string)field.Key, valToInsert.Value);


                    //<!!! // pick up here, need to assign
                }

                //pdfReader.AcroFields.SetField()

                pdfReader.Close();
                pdfStamp.FormFlattening = false;
                pdfStamp.FreeTextFlattening = false;
                pdfStamp.Writer.CloseStream = false;
                pdfStamp.Close();

                
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
            System.Diagnostics.Process.Start(DestFilePath);
        }

        public static string AssemblePathToForm(string borrDirPath, string formFilename)
        {
            return borrDirPath + "\\" + formFilename;
        }


    }
}
