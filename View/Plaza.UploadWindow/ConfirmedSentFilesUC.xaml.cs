using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProcessorsToolkit.Model.Plaza.UploadSession;
using ProcessorsToolkit.ViewModel.Plaza;

namespace ProcessorsToolkit.View.Plaza.UploadWindow
{
    /// <summary>
    /// Interaction logic for ConfirmedSentFiles.xaml
    /// </summary>
    public partial class ConfirmedSentFilesUC : UserControl
    {
        public ConfirmedSentFilesUC()
        {
            InitializeComponent();
        }
        
        private void FilesListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
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
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            vm.View.Close();
        }
    }
}
