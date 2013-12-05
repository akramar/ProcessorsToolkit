using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProcessorsToolkit.Model
{
    public class BorrSubDir
    {
        public string FolderName { get; set; }
        public string Fullpath { get; set; }

        public bool IsOpen { get; set; }

        public BorrSubDir(string folderName, string fullPath)
        {
            FolderName = folderName;
            Fullpath = fullPath;
        }

        public ImageSource FolderImgPath
        {
            get
            {
                var srcUri = IsOpen ? "pack://application:,,,/Images/Folder_Open.png" : "pack://application:,,,/Images/Folder_Closed.png";
                return (ImageSource)new BitmapImage(new Uri(srcUri));
            }
        }

        public void CreateIfNotExists()
        {
            if (!Directory.Exists(Fullpath))
                Directory.CreateDirectory(Fullpath);
        }


    }
}
