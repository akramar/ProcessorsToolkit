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
using System.Windows.Threading;
using ProcessorsToolkit.ViewModel.Interbank;

namespace ProcessorsToolkit.View.Interbank.UploadWindow
{
    /// <summary>
    /// Interaction logic for UploadedProgressUC.xaml
    /// </summary>
    public partial class UploadedProgressUC : UserControl
    {
        public UploadedProgressUC()
        {
            InitializeComponent();

        }

        private void FilesListLB_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            vm.DoneUploadingSingleFile += VMOnDoneUploadingSingleFile;
            vm.DoneUploadingAllFiles += VMOnDoneUploadingAllFiles;

            FilesListLB.ItemsSource = vm.WorkingFileList.Where(f => f.IsSelected);
            
        }
        
        private void VMOnDoneUploadingSingleFile(object sender, EventArgs eventArgs)
        {
            FilesListLB.Dispatcher.Invoke(new Action(() => FilesListLB.Items.Refresh()));
        }

        private void VMOnDoneUploadingAllFiles(object sender, EventArgs eventArgs)
        {
            
        }
    }
}