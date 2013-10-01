using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProcessorsToolkit.Model
{
    public class FileBase
    {


        public string DisplayName { get; set; }

        public string Ext { get; set; }

        public string SizeMB { get; set; }

        public DateTime LastModified { get; set; }

        public string Imagepath { get; set; }

        //public EntryType Type { get; set; }

        public string Fullpath { get; set; }

        public bool IsSelected { get; set; }

        public bool IsBeingEdited { get; set; }


        
        public ImageSource EditImgPath
        {
            get { return IsBeingEdited ? CheckImgPath : PencilImgPath; }
        }
        public ImageSource CheckImgPath
        {
            get
            {
                var srcUri = "pack://application:,,,/Images/check.png";
                return (ImageSource)new BitmapImage(new Uri(srcUri));
            }
        }


        public ImageSource PencilImgPath
        {
            get
            {
                var srcUri = "pack://application:,,,/Images/rename.png";
                return (ImageSource)new BitmapImage(new Uri(srcUri));
            }
        }
    }
}
