﻿using System;
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
using ProcessorsToolkit.ViewModel.PRMG;

namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for UploadedProgressUC.xaml
    /// </summary>
    public partial class UploadedProgressUC : UserControl
    {
        private readonly UploadWindowVM _parentVM;

        public UploadedProgressUC(UploadWindowVM parentVM)
        {
            _parentVM = parentVM;

            InitializeComponent();

            _parentVM.DoneUploadingSingleFile += UploadWindowVMOnDoneUploadingSingleFile;
            _parentVM.DoneUploadingAllFiles += UploadWindowVM_DoneUploadingAllFiles;
        }
        
        private void FilesListLB_OnLoaded(object sender, RoutedEventArgs e)
        {
            FilesListLB.ItemsSource = _parentVM.WorkingFileList.Where(f => f.IsSelected);
            

        }

        private void UploadWindowVMOnDoneUploadingSingleFile(object sender, EventArgs eventArgs)
        {

            FilesListLB.Dispatcher.Invoke(new Action(() => FilesListLB.Items.Refresh()));

        }

        void UploadWindowVM_DoneUploadingAllFiles(object sender, EventArgs e)
        {
        }

    }
}
