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
using ProcessorsToolkit.Model.UploadSession.PRMG;
using ProcessorsToolkit.ViewModel.PRMGUploadWindow;

namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for ConfirmedSentFiles.xaml
    /// </summary>
    public partial class ConfirmedSentFiles : UserControl
    {
        public ConfirmedSentFiles()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            UploadWindowVM.SpawnedWindow.Close();
        }

        private void FilesListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var header1String = "Uploaded " +
                               UploadWindowVM.WorkingFileList.Count(
                                   f => f.UploadProgress == FileToUpload.FileUploadStages.Completed) + " files" +
                                Environment.NewLine;

            var tbStringCompleted =
                UploadWindowVM.WorkingFileList.Where(f => f.UploadProgress == FileToUpload.FileUploadStages.Completed)
                              .Aggregate("",
                                         (current, fileUpload) =>
                                         current + (fileUpload.NameWithExt + Environment.NewLine));

            var separatorString = "==========================" + Environment.NewLine;

            var header2String = "Failed " +
                                UploadWindowVM.WorkingFileList.Count(
                                    f => f.UploadProgress != FileToUpload.FileUploadStages.Completed) + " files" +
                                Environment.NewLine;

            var tbStringIncomplete = UploadWindowVM.WorkingFileList.Where(
                f => f.UploadProgress != FileToUpload.FileUploadStages.Completed)
                                                   .Aggregate("",
                                                              (current, fileUpload) =>
                                                              current + (fileUpload.NameWithExt + Environment.NewLine));


            FilesListBox.Text = header1String + tbStringCompleted + separatorString + header2String + tbStringIncomplete;

        }
    }
}
