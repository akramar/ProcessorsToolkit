using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.View.Tools
{
    /// <summary>
    /// Interaction logic for AllFilesListerWindow.xaml
    /// </summary>
    public partial class AllFilesListerWindow : Window
    {


        public AllFilesListerWindow()
        {
            InitializeComponent();

            Loaded += AllFilesListerWindow_Loaded;
        }

        private void AllFilesListerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadList(true);


        }

        public void LoadList(bool removePrefix)
        {
            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            Title = (borrFoldersUCVM.SelectedBorrDir.SubDirs.Any(sd => sd.IsOpen))
                        ? borrFoldersUCVM.SelectedBorrDir.BorrDirName + " > " +
                          borrFoldersUCVM.SelectedBorrDir.SubDirs.First(sd => sd.IsOpen).FolderName
                        : borrFoldersUCVM.SelectedBorrDir.BorrDirName;

            var allViewableFilesNames = Directory.GetFiles(borrFoldersUCVM.SelectedBorrDir.FullActivePath)
                                                 .Select(f => new FileInfo(f).Name).ToList();

            var borrName = borrFoldersUCVM.SelectedBorrDir.BorrDirName;

            var listerString = "";

            foreach (var filesName in allViewableFilesNames)
            {
                if (removePrefix && filesName.StartsWith(borrName))
                    listerString += filesName.Substring(borrName.Length).TrimStart();
                else
                    listerString += filesName;

                listerString += Environment.NewLine;
            }


           // var listerString = allViewableFilesNames.Aggregate("",
           //                                                    (current, addtl) =>
           //                                                    current + (addtl + Environment.NewLine));

            FilesListBox.Text = listerString;

            /*
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            var selectedFilesList = vm.WorkingFileList.Where(f => f.IsSelected).ToList();

            var header1String = "Uploaded " +
                                selectedFilesList.Count(
                                    f => f.UploadProgress == FileToUpload.FileUploadStages.Completed) + " files" +
                                Environment.NewLine;

            var tbStringCompleted =
                selectedFilesList.Where(f => f.UploadProgress == FileToUpload.FileUploadStages.Completed)
                              .Aggregate("", //can use or omit seed?
                                         (current, fileUpload) =>
                                         current + (fileUpload.NameWithExt + Environment.NewLine));

            var separatorString = "==========================" + Environment.NewLine;

            var header2String = "Failed " +
                                selectedFilesList.Count(f => f.UploadProgress != FileToUpload.FileUploadStages.Completed) +
                                " files" + Environment.NewLine;

            var tbStringIncomplete = selectedFilesList.Where(
                f => f.UploadProgress != FileToUpload.FileUploadStages.Completed)
                                                   .Aggregate("", //can use or omit seed?
                                                              (current, fileUpload) =>
                                                              current + (fileUpload.NameWithExt + Environment.NewLine));


            FilesListBox.Text = header1String + tbStringCompleted + separatorString + header2String + tbStringIncomplete;

                */

        }

        
        private void RemoveBorrName_Clicked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null || cb.IsChecked == null)
                return;

            LoadList((bool) cb.IsChecked);
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
