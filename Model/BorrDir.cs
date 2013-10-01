using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProcessorsToolkit.Model
{
    public class BorrDir
    {
        public string BorrName { get; set; }
        public string FullRootPath { get; set; }
        public bool IsCurrentBorr { get; set; }
        public ObservableCollection<BorrSubDir> SubDirs { get; set; }
        public string FullActivePath { get
        {
            if (SubDirs.Any(sd => sd.IsOpen))
                return FullRootPath + "\\" + SubDirs.First(sd => sd.IsOpen).FolderName;

            else
                return FullRootPath;
        }}
       

        public BorrDir(string borrName, string fullPath)
        {
            BorrName = borrName;
            FullRootPath = fullPath;
        }

        // dir watcher
        // http://social.msdn.microsoft.com/Forums/vstudio/en-US/6ec4b09e-da13-4aea-95db-b3e822fc6b5b/how-uses-c-to-monitor-under-some-directory-some-file-change-

        

        public ImageSource FolderImgPath
        {
            get
            {
                //return IsCurrentBorr ? "Images\\Folder_Open.png" : "Images\\Folder_Closed.png";

                var srcUri = IsCurrentBorr ? "pack://application:,,,/Images/Folder_Open.png" : "pack://application:,,,/Images/Folder_Closed.png";

                return (ImageSource) new BitmapImage(new Uri(srcUri));

                // Properties.Resources.Folder_Open : Properties.Resources.Folder_Closed;
                //ImageSource imageSource = new BitmapImage(new Uri("C:\\FileName.gif"));

                //image1.Source = imageSource;
            }
        }

        public void LoadSubDirs()
        {
            if (SubDirs == null)
                SubDirs = new ObservableCollection<BorrSubDir>();

            foreach (var subDir in Directory.GetDirectories(FullRootPath))
            {
                var dirInfo = new DirectoryInfo(subDir);
                var dirName = dirInfo.Name;

                if (!dirName.StartsWith("_") && !dirName.StartsWith("."))
                    SubDirs.Add(new BorrSubDir(dirName, subDir));

            }
        }

        public static string GetSubDir(string borrFolderName, string fullPath)
        {
            //regx: Billingsley\\\\(.+)
            var expression = new Regex(borrFolderName + @"\\\\(.+)");
            var matches = expression.Matches(fullPath);
            if (matches.Count > 0 && matches[0].Groups.Count >= 2)
                return matches[0].Groups[1].Value;

            return "";
        }
    }
}
