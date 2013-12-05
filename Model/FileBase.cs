using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProcessorsToolkit.Model
{
    public class FileBase : IComparable
    {
        public FileBase(){}
        public FileBase(string filePath, bool createIfNotExists)
        {
            Fullpath = filePath;
            if (createIfNotExists && !File.Exists(Fullpath))
                File.Create(Fullpath);

            if (File.Exists(Fullpath))
            {
                LastModified = File.GetLastWriteTime(Fullpath);
                //SizeKB
            }
        }   
        

        public string DisplayName { get; set; }

        public string Ext { get { return Path.GetExtension(Fullpath); } }
        
        public string SizeKB { get; set; }

        public DateTime LastModified { get; set; }

        public string Imagepath { get; set; }

        //public EntryType Type { get; set; }

        public string Fullpath { get; set; }

        public bool IsSelected { get; set; }

        public bool IsBeingEdited { get; set; }

        public string FileDirectory { get { return Path.GetDirectoryName(Fullpath); } }
        public string FileNameOnlyWithExt { get { return Path.GetFileName(Fullpath); } }
        public string FileNameOnlyNoExt { get { return Path.GetFileNameWithoutExtension(Fullpath); } }
        public ImageSource EditImgPath { get { return IsBeingEdited ? CancelImgPath : PencilImgPath; } }

        public ImageSource CheckImgPath { get { return new BitmapImage(new Uri("pack://application:,,,/Images/check.png")); } }

        public ImageSource CancelImgPath { get { return new BitmapImage(new Uri("pack://application:,,,/Images/cancel.png")); } }

        public ImageSource PencilImgPath { get { return new BitmapImage(new Uri("pack://application:,,,/Images/rename.png")); } }


        public bool IsAlreadyOpen()
        {
            var isOpen = true;
            try
            {
                using (Stream stream = new FileStream(Fullpath, FileMode.Open))
                {
                    isOpen = false;
                }
            }
            catch (Exception ex)
            {
                isOpen = true;
            }
            return isOpen;
        }


        public bool CanBePDFed { get {
            var pdfableExts = new[] { ".jpg", ".jpe", ".jpeg", ".png", ".txt", ".bmp", ".tif", ".tiff", ".docx" }; //".doc",
                return pdfableExts.Any(ext => String.Equals(ext, Ext, StringComparison.InvariantCultureIgnoreCase));
        } }

        public bool IsPDF { get { return String.Equals(".pdf", Ext, StringComparison.InvariantCultureIgnoreCase); } }


        public Exception RenameFile(string newNameRaw)
        {
            var newNameClear = StripIllegalChars(newNameRaw);
            //this is messy
            var newPath = FileDirectory + "\\" + newNameClear;

            if (newPath == Fullpath)
                return null; //No change

            if (IsAlreadyOpen())
                return new Exception("File already open!");

            if (File.Exists(newPath))
                return new Exception("File already exists!");



            try
            {
                File.Move(Fullpath, newPath);
                DisplayName = newNameClear;
                Fullpath = newPath;
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        public static string StripIllegalChars(string input)
        {
            var illegalChars = Path.GetInvalidFileNameChars();
            return illegalChars.Aggregate(input, (current, illegalChar) => current.Replace(illegalChar.ToString(CultureInfo.InvariantCulture), ""));
        }

        public static int? StripNonNumbers(string input)
        {
            var newNumbStr = input.ToCharArray().Where(c => Char.IsNumber(c)).Aggregate("", (current, c) => current + c);
            return (String.IsNullOrEmpty(newNumbStr)) ?  (int?) null : Convert.ToInt32(newNumbStr);
        }

        #region IComparable Members


        //http://kiwigis.blogspot.com/2010/03/how-to-sort-obversablecollection.html
        public int CompareTo(object obj)
        {
            var file = obj as FileBase;
            if (file == null)
                throw new ArgumentException("Object is not a file");

            return String.Compare(DisplayName, file.DisplayName, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}
