using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProcessorsToolkit.Model.Interbank.UploadSession;
using ProcessorsToolkit.View;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //public MainWindowVM CurrMainWindowVM;

        public MainWindow()
        {
            //CurrMainWindowVM = new MainWindowVM();

            InitializeComponent();



            Loaded += MainWindow_Loaded;
            
        }

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            

        }

        

        private void BorrFoldersUC_Loaded(object sender, RoutedEventArgs e)
        {
            var loadedborrfoldersUC = sender as BorrFoldersUC;

            if (loadedborrfoldersUC == null)
                return;

            loadedborrfoldersUC.CurrBorrFoldersVM.SelectedBorrChanged += (s, args) 
                => MainWindowVM.OnSelectedBorrChanged(args);

            loadedborrfoldersUC.CurrBorrFoldersVM.SelectedPathChanged += (o, args)
                => MainWindowVM.OnSelectedPathChanged(args);

                /*
            if (loadedborrfoldersUC != null)
            {
                if (loadedborrfoldersUC.CurrBorrFoldersVM.SelectedBorr != null)
                    CurrMainWindowVM.SelectedBorr = loadedborrfoldersUC.CurrBorrFoldersVM.SelectedBorr;
                if (loadedborrfoldersUC.CurrBorrFoldersVM.SelectedBorr != null)
                    CurrMainWindowVM.SelectedFolderPath = loadedborrfoldersUC.CurrBorrFoldersVM.SelectedBorr.Fullpath;
            }*/
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void BorrInfoUC_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void FormsMenu_Click(object sender, RoutedEventArgs e)
        {
            var clickeditem = sender as MenuItem;
            if (clickeditem == null)
                return;

            MainWindowVM.FormMenuClick(clickeditem.CommandParameter.ToString());
        }

        private void UploadTo_Click(object sender, RoutedEventArgs e)
        {
            var clickeditem = sender as MenuItem;
            if (clickeditem == null)
                return;


            if (MainWindowVM.SelectedBorrDir == null || MainWindowVM.SelectedBorrDir.FullActivePath == null)
            {
                var MB = new Xceed.Wpf.Toolkit.MessageBox
                    {
                        Caption = "",
                        Text = "No folder has been selected!"
                    };

                MB.ShowDialog();
                //MB.ShowMessageBox("", "!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if ((string) clickeditem.CommandParameter == "PRMG")
            {
                var newwin = new PRMGUploadWindow.MainWindow {Owner = this};
                ViewModel.PRMGUploadWindow.UploadWindowVM.SpawnedWindow = newwin;
                newwin.Show();
            }
            else if ((string) clickeditem.CommandParameter == "IBW")
            {
                var newVM = new ViewModel.InterbankUploadWindow.UploadWindowVM();

                var sess = ViewModel.InterbankUploadWindow.UploadWindowVM.WebsiteSession;

                sess = new Session();
                
                sess.Step1_FetchHomepage();
                sess.Step2_PostUsername();
                sess.Step3_FetchDashboard();
                sess.Step4_GetPipeline();
                sess.Step5_PostSearchQueries();



            }
        }
    }
}