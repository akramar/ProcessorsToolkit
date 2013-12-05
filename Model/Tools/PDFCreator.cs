using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.IO.Packaging;

namespace ProcessorsToolkit.Model.Tools
{
    internal class PDFCreator
    {
        private FileBase SourceFile { get; set; }
        private FileBase DestFile { get; set; }
        private SourceToConvertTypes TypeOfSource { get; set; }

        public PDFCreator(FileBase srcFile)
        {

            SourceFile = srcFile;

            var imgExts = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tif", ".tiff" };

            if (imgExts.Any(ext => String.Equals(ext, srcFile.Ext, StringComparison.InvariantCultureIgnoreCase)))
                TypeOfSource = SourceToConvertTypes.Image;

            if (String.Equals(srcFile.Ext, ".txt", StringComparison.InvariantCultureIgnoreCase))
                TypeOfSource = SourceToConvertTypes.Text;

            if (String.Equals(srcFile.Ext, ".doc", StringComparison.InvariantCultureIgnoreCase))
                TypeOfSource = SourceToConvertTypes.WordDoc;
            
            if (String.Equals(srcFile.Ext, ".docx", StringComparison.InvariantCultureIgnoreCase))
                TypeOfSource = SourceToConvertTypes.WordDocX;

            if (String.Equals(srcFile.Ext, ".html", StringComparison.InvariantCultureIgnoreCase) ||
                (String.Equals(srcFile.Ext, ".htm", StringComparison.InvariantCultureIgnoreCase)))
                TypeOfSource = SourceToConvertTypes.Html;
        }

        public FileBase ConvertToPDF()
        {
            DestFile = new FileBase(GetNewFilePath(SourceFile), false);

            switch (TypeOfSource)
            {
                case SourceToConvertTypes.Image:
                    ImgToPDF();
                    return DestFile;

                case SourceToConvertTypes.Text:
                    TxtToPDF();
                    return DestFile;

                case SourceToConvertTypes.WordDoc:
                    break;

                case SourceToConvertTypes.WordDocX:
                    DocXToPDF();
                    return DestFile;

                case SourceToConvertTypes.Html:
                    HtmlToPDF();
                    return DestFile;
            }
            return null;
        }


        private enum SourceToConvertTypes
        {
            Image,
            Text,
            WordDoc,
            WordDocX,
            Html
        }

        private static string GetNewFilePath(FileBase srcFile)
        {
            var destPath = srcFile.FileDirectory + "\\";
            var newFilename = srcFile.FileNameOnlyNoExt + ".pdf";
            var newFullPath = destPath + newFilename;

            if (File.Exists(newFullPath)) //Gotta be a cleaner way to do this
            {
                var fileIter = 1;
                var origPath = String.Copy(newFullPath);
                //newFullPath = newFullPath.Insert(newFullPath.Length - 4, " ()");
                while (File.Exists(newFullPath))
                {
                    var cntStr = String.Format(" ({0})", fileIter);
                    newFullPath = origPath.Insert(origPath.Length - 4, cntStr);
                    fileIter++;
                }
            }
            return newFullPath;
        }

        private void ImgToPDF()
        {
            var doc = new Document(PageSize.LETTER, 5, 5, 5, 5);
            using (var FSOut = new FileStream(DestFile.Fullpath, FileMode.Create))
            {
                var pdfWriter = PdfWriter.GetInstance(doc, FSOut);
                var srcImg = Image.GetInstance(SourceFile.Fullpath);
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

        private void TxtToPDF()
        {
            using (var FSOut = new FileStream(DestFile.Fullpath, FileMode.Create))
            {
                var doc = new Document(PageSize.LETTER, 25, 25, 25, 25);
                var pdfWriter = PdfWriter.GetInstance(doc, FSOut);
                //var srcImg = Image.GetInstance(SourceFile.Fullpath);

                var srcParas = File.ReadAllLines(SourceFile.Fullpath).ToList();
                if (srcParas.Count == 0)
                    srcParas.Add(" ");

                doc.Open();
                var pageWidth = doc.PageSize.Width - (10f + 10f);
                var pageHeight = doc.PageSize.Height - (10f + 10f);
                
                foreach (var paraElem in srcParas.Select(srcPara => new Paragraph(srcPara)))
                {
                    paraElem.SpacingAfter = 10f;
                    doc.Add(paraElem);
                }
                doc.Close();
                pdfWriter.Close();
            }
        }

        private void DocXToPDF()
        {
            var htmlOut = "";

            byte[] byteArray = File.ReadAllBytes(SourceFile.Fullpath);
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(byteArray, 0, byteArray.Length);
                using (var doc = WordprocessingDocument.Open(memoryStream, true))
                {
                    var settings = new HtmlConverterSettings()
                        {
                            PageTitle = "Test pg title"
                        };
                    XElement html = HtmlConverter.ConvertToHtml(doc, settings);

                    // Note: the XHTML returned by ConvertToHtmlTransform contains objects of type
                    // XEntity. PtOpenXmlUtil.cs defines the XEntity class. See
                    // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
                    // for detailed explanation.
                    //
                    // If you further transform the XML tree returned by ConvertToHtmlTransform, you
                    // must do it correctly, or entities do not serialize properly.

                    File.WriteAllText(SourceFile.FileDirectory + "\\" + "output.html",
                                      html.ToStringNewLineOnAttributes());

                    htmlOut = html.ToStringNewLineOnAttributes();
                }
            }

            HtmlToPDF(htmlOut);


            /*
            using (var FSOut = new FileStream(DestFile.Fullpath, FileMode.Create))
            {
                var doc = new Document(PageSize.LETTER, 25, 25, 25, 25);
                var pdfWriter = PdfWriter.GetInstance(doc, FSOut);
                //var srcImg = Image.GetInstance(SourceFile.Fullpath);
                //var srcParas = File.ReadAllLines(SourceFile.Fullpath);
                doc.Open();

                using (WordprocessingDocument document = WordprocessingDocument.Open("test.docx", false))
            {
                    // Register namespace

            XNamespace w = "http://schemas.microsoft.com/office/2006/relationships/";

            // Element shortcuts

            XName w_r = w + "r";
            XName w_ins = w + "ins";
            XName w_hyperlink = w + "hyperlink";

                    XDocument xDoc = XDocument.Load(
            XmlReader.Create(
            new StreamReader(document.MainDocumentPart.GetStream())
            )
            );
                doc.Close();
                pdfWriter.Close();
            }
                * */
        }

        private void DocToPDF()
        {
            



        }

        private void HtmlToPDF(string htmlSrc = null)
        {
            if (htmlSrc == null)
                htmlSrc = File.ReadAllText(SourceFile.Fullpath);

            using (var FSOut = new FileStream(DestFile.Fullpath, FileMode.Create))
            {
                TextReader txtReader = new StringReader(htmlSrc);
                var doc = new Document(PageSize.LETTER, 25, 25, 25, 25);


                var pdfWriter = PdfWriter.GetInstance(doc, FSOut);
                var htmlWorker = new HTMLWorker(doc);

                doc.Open();
                htmlWorker.StartDocument();
                htmlWorker.Parse(txtReader);
                htmlWorker.EndDocument();
                htmlWorker.Close();
                doc.Close();
                pdfWriter.Close();
            }
        }


    }
}
