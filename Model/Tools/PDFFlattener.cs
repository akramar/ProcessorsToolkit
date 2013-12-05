using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ProcessorsToolkit.Model.Tools
{
    internal class PDFFlattener
    {
        private FileBase SourceFile { get; set; }
        public PDFFlattener(FileBase sourceFile)
        {
            SourceFile = sourceFile;
        }

        /*
        public void Flatten(FileBase fileSrc)
        {

            var doc = new Document(PageSize.LETTER, 5, 5, 5, 5);
            using (var FSOut = new FileStream(DestFile.Fullpath, FileMode.Create))
            {
                var pdfWriter = PdfWriter.GetInstance(doc, FSOut);
                

                doc.Open();

                var pageWidth = doc.PageSize.Width - (10f + 10f);
                var pageHeight = doc.PageSize.Height - (10f + 10f);

                srcImg.SetAbsolutePosition(10f, 10f);
                srcImg.ScaleToFit(pageWidth, pageHeight);
                doc.Add(srcImg);

                doc.Close();
                pdfWriter.Close();
            }

        }

        

        public void Flatten2()
        {
            using (var fileOut = new FileStream(SourceFile.Fullpath, FileMode.Open, FileAccess.Write))
           {
                //use a temp or memory stream here assource
                //var pdfReader = new PdfReader(AssemblePathToForm(@"C:\Users\Alain Kramar\Documents\Loans\_forms", FormFilename));
                var pdfReader = new PdfReader(SourceFile.Fullpath);

                //var pdfStamp = new PdfStamper(pdfReader, fileOut);
                var pdfStamp = new PdfStamper(pdfReader, ms);
                //var fields = pdfStamp.AcroFields;

                var fieldList = new List<string>();

                foreach (DictionaryEntry field in pdfStamp.AcroFields.Fields)
                {
                    fieldList.Add((string)field.Key);
                    //FormFieldsVals.Add(field.ToString(), "");

                    var valToInsert = FormFieldsVals.FirstOrDefault(f => f.Key == (string)field.Key);
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

                ms.WriteTo(new FileStream(DestFile.FullName, FileMode.Create, FileAccess.Write));
            }
        }
        */

        public FileBase Flatten3()
        {

            var newDocument = new Document();
            var newFilePath = GetUniqueFilename(SourceFile);
            //using (var fileIn = new FileStream(SourceFile.Fullpath, FileMode.Open, FileAccess.Write))
           // {
                
                using (var fileOut = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                {

                    //var pdfWriter = PdfWriter.GetInstance(newDocument, fileOut);
                    var pdfReader = new PdfReader(SourceFile.Fullpath);


                    var pdfStamp = new PdfStamper(pdfReader, fileOut);

                    for (int i = 0; i < pdfReader.NumberOfPages; i++)
                    {
                        var overcontent = pdfStamp.GetOverContent(i);

                    }
                    //newDocument.Open();
                    //var pdfContentBytes = pdfStamp..DirectContent;


                    /*
                    for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                    {

                        newDocument.NewPage();
                        var importedPage = pdfStamp.GetImportedPage(pdfReader, page);
                        pdfContentBytes.AddTemplate(importedPage, 0, 0);


                    }

                    pdfWriter.st


                    fileOut.Flush();
                    newDocument.Close();
                    fileOut.Close();
                    */

                    pdfReader.Close();
                    pdfStamp.FormFlattening = true;
                    pdfStamp.FreeTextFlattening = true;
                    
                    pdfStamp.Writer.CloseStream = true;
                    pdfStamp.Close();
                }
            //}

            return new FileBase(newFilePath, false);
        }

        private static string GetUniqueFilename(FileBase srcFile) //this whole thing should be made into an extension
        {
            //DestFilePath = AssemblePathToForm(BorrDirectory.FullRootPath, FormFilename); 
            var srcFileName = srcFile.FileNameOnlyWithExt;
            var fileSaveLoc = srcFile.FileDirectory + "\\" + srcFileName.Insert(srcFileName.LastIndexOf('.'), " (flat)");

            var fileIter = 0;
            while (File.Exists(fileSaveLoc))
            {
                fileIter++;
                if (!File.Exists(fileSaveLoc.Insert((fileSaveLoc.LastIndexOf('.')), fileIter.ToString(" (0)"))))
                    break;
            }
            if (fileIter != 0)
                fileSaveLoc = fileSaveLoc.Insert((fileSaveLoc.LastIndexOf('.')), fileIter.ToString(" (0)"));

            return fileSaveLoc;
        }
    }
}
