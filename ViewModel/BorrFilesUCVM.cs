using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ProcessorsToolkit.Model;

namespace ProcessorsToolkit.ViewModel
{
    public class BorrFilesUCVM
    {
        public ObservableCollection<FileBase> FolderFiles { get; set; }

        public BorrDir SelectedBorrDir { get; set; }

        //public string SelectedFolderPath { get; set; }


        public BorrFilesUCVM()
        {
            
            FolderFiles = new ObservableCollection<FileBase>();

            MainWindowVM.SelectedBorrChanged += (sender, args) =>
                {
                    SelectedBorrDir = args.CurrBorr;
                };

            MainWindowVM.SelectedPathChanged += (sender, args) => LoadBorrFiles();
        }


        public void LoadBorrFiles()
        {
            if (FolderFiles.Count > 0)
                FolderFiles.Clear();

            foreach (var filePath in Directory.GetFiles(MainWindowVM.SelectedBorrDir.FullActivePath))
            {

                var fileInfo = new FileInfo(filePath);

                FolderFiles.Add(new FileBase()
                    {
                        LastModified = fileInfo.LastWriteTime,
                        Ext = fileInfo.Extension,
                        Fullpath = filePath,
                        Imagepath = "",
                        DisplayName = fileInfo.Name,
                        SizeMB = (fileInfo.Length/1024).ToString(CultureInfo.InvariantCulture) + "KB",
                        //IsBeingEdited = true
                    });
            }
        }

        public void FileSelectionChanged(FileBase selectedFile, bool isChecked)
        {

            foreach (var folderFile in FolderFiles.Where(f => f.Fullpath == selectedFile.Fullpath))
                folderFile.IsSelected = true;


        }
    }
}
