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
using ProcessorsToolkit.Model.UploadSession.PRMG;
using ProcessorsToolkit.ViewModel.PRMGUploadWindow;

namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for FileSelectorUC.xaml
    /// </summary>
    public partial class FileSelectorUC : UserControl
    {
        private BorrowerFileGroup fl;// = new BorrowerFileGroup();

        public FileSelectorUC()
        {
            InitializeComponent();
        }


        private void FilesListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            //var fl = new BorrowerFileGroup().LoadAllFiles();

            UploadWindowVM.WorkingFileList = new BorrowerFileGroup();

            UploadWindowVM.WorkingFileList.LoadAllFiles();

            //fl = new BorrowerFileGroup().LoadAllFiles();
            FilesListBox.DataContext = UploadWindowVM.WorkingFileList;
            FilesListBox.ItemsSource = UploadWindowVM.WorkingFileList;
        }

        private void cbxSelect_CheckChanged(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;

            if (cb == null || cb.IsChecked == null)
                return;

            //var fileSrc = fl.FirstOrDefault(f => f.PathFull == cb.CommandParameter.ToString());

            //if (fileSrc == null)
            //    return;

            //fileSrc.IsSelected = (bool) cb.IsChecked;

          //  foreach (var file in UploadWindowVM.WorkingFileList.Where(f => f.IsSelected))
          //      System.Diagnostics.Debug.WriteLine(file.PathFull);


            var clickedFile = cb.DataContext as FileToUpload;

            if (clickedFile != null) 
                clickedFile.IsSelected = (bool) cb.IsChecked;


            //FilesListBox.Items.Refresh();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            UploadWindowVM.OnDoneSelectingFiles();
            
        }
    }
}
