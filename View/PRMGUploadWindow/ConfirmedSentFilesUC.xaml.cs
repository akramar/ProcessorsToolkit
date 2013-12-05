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
using ProcessorsToolkit.Model.PRMG.UploadSession;
using ProcessorsToolkit.ViewModel.PRMG;

namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for ConfirmedSentFiles.xaml
    /// </summary>
    public partial class ConfirmedSentFilesUC : UserControl
    {
        private readonly UploadWindowVM _parentVM;

        public ConfirmedSentFilesUC(UploadWindowVM parentVM)
        {
            _parentVM = parentVM;
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _parentVM.View.Close();
        }

        private void FilesListBox_OnLoaded(object sender, RoutedEventArgs e)
        {

            var selectedFilesList = _parentVM.WorkingFileList.Where(f => f.IsSelected).ToList();
            
            var header1String = "Uploaded " +
                                selectedFilesList.Count(
                                    f => f.UploadProgress == FileToUpload.FileUploadStages.Completed) + " files" +
                                Environment.NewLine;

            var tbStringCompleted =
                selectedFilesList.Where(f => f.UploadProgress == FileToUpload.FileUploadStages.Completed)
                              .Aggregate("", (current, fileUpload) =>
                                         current + (fileUpload.NameWithExt + Environment.NewLine));

            var separatorString = "==========================" + Environment.NewLine;

            var header2String = "Failed " +
                                selectedFilesList.Count(f => f.UploadProgress != FileToUpload.FileUploadStages.Completed) + " files" +
                                Environment.NewLine;

            var tbStringIncomplete = selectedFilesList.Where(
                f => f.UploadProgress != FileToUpload.FileUploadStages.Completed)
                                                   .Aggregate("",
                                                              (current, fileUpload) =>
                                                              current + (fileUpload.NameWithExt + Environment.NewLine));


            FilesListBox.Text = header1String + tbStringCompleted + separatorString + header2String + tbStringIncomplete;
        }
    }
}
