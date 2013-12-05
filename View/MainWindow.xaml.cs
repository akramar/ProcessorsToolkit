using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
//using ProcessorsToolkit.Model.Interbank.UploadSession;
//using ProcessorsToolkit.View;
using ProcessorsToolkit.Model;
using ProcessorsToolkit.ViewModel;
//using ProcessorsToolkit.ViewModel.InterbankUploadWindow;

namespace ProcessorsToolkit.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            //CurrMainWindowVM = new MainWindowVM();

            //var mainWindowVM = DataContext as MainWindowVM;
            //if (mainWindowVM == null)
            //   return;

            MainWindowVM.View = this;

            InitializeComponent(); //the datacontext is set in the view


            Loaded += MainWindow_Loaded;

        }
        
        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void FindExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var borrFoldersUC = MainWindowVM.BorrFoldersCtrl as BorrFoldersUC;
            if (borrFoldersUC == null)
                return;

            borrFoldersUC.SearchFoldersTB.Focus();
        }






    }
}