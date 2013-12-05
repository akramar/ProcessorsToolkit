using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ProcessorsToolkit.Model.Tools
{
    class PDFUnlocker
    {

         private FileBase SourceFile { get; set; }
         public PDFUnlocker(FileBase sourceFile)
        {
            SourceFile = sourceFile;
        }

         public FileBase Unlock()
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
                 pdfStamp.FormFlattening = false;
                 pdfStamp.FreeTextFlattening = false;

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
             var fileSaveLoc = srcFile.FileDirectory + "\\" + srcFileName.Insert(srcFileName.LastIndexOf('.'), " (unlocked)");

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
