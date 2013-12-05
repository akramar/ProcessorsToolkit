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
using ProcessorsToolkit.Model;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.View
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenuUC : UserControl
    {
        public MainMenuUC()
        {
            DataContext = new MainMenuUCVM { View = this };
            MainWindowVM.MainMenuCtrl = this;

            InitializeComponent();

            Initialized += MainMenuUC_Initialized;
            Loaded += MainMenu_Loaded;
        }

        void MainMenuUC_Initialized(object sender, EventArgs e)
        {
            var mainMenuUC = sender as MainMenuUC;
            if (mainMenuUC == null)
                return;

            MainWindowVM.MainMenuCtrl = mainMenuUC;
        }

        private void MainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            var mainMenuUC = sender as MainMenuUC;
            if (mainMenuUC == null)
                return;

            var mainMenuUCVM = DataContext as MainMenuUCVM;
            if (mainMenuUCVM == null)
                return;

            MainMenuCtrl.DataContext = mainMenuUCVM;

            /*
            var borrFilesUCVM = MainWindowVM.BorrFilesCtrl.DataContext as BorrFilesUCVM;
            if (borrFilesUCVM != null)
                borrFilesUCVM.SelectedFileChanged += MainWindowVM_SelectedFileChanged;

           // var borrDataUCVM = MainWindowVM.BorrInfoCtrl.DataContext as BorrInfoUCVM;
          //  if (borrDataUCVM !=null)
            //    borrDataUCVM
            
            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM != null)
            {
                borrFoldersUCVM.SelectedBorrChanged += BorrFoldersUCVMOnSelectedBorrChanged;
                borrFoldersUCVM.SelectedPathChanged += BorrFoldersUCVMOnSelectedPathChanged;
            }
            */
        }
/*
        private void MainWindowVM_SelectedFileChanged(object sender, SelectedFileChangedEventArgs e)
        {
            var mainMenuUCVM = DataContext as MainMenuUCVM;
            if (mainMenuUCVM == null)
                return;

            //mainMenuUCVM.SelectedFile = e.CurrFile;

            //mainMenuUCVM.IsPDFableFileSelected = e.CurrFile.CanBePDFed;


        }
        
        private void BorrFoldersUCVMOnSelectedBorrChanged(object sender, SelectedBorrChangedEventArgs e)
        {
            var mainMenuUCVM = DataContext as MainMenuUCVM;
            if (mainMenuUCVM == null)
                return;

            //mainMenuUCVM.SelectedBorrDir = e.CurrBorr;
        }

        private void BorrFoldersUCVMOnSelectedPathChanged(object sender, SelectedDirChangedEventArgs e)
        {
            var mainMenuUCVM = DataContext as MainMenuUCVM;
            if (mainMenuUCVM == null)
                return;
        }

        */
        private void FileMenu_Click(object sender, RoutedEventArgs e)
        {
            var clickeditem = sender as MenuItem;
            if (clickeditem == null)
                return;

            var mainMenuUCVM = DataContext as MainMenuUCVM;
            if (mainMenuUCVM == null)
                return;

            mainMenuUCVM.FileMenuClick(clickeditem.CommandParameter.ToString());//, this);
        }

        private void FormsMenu_Click(object sender, RoutedEventArgs e)
        {
            var clickeditem = sender as MenuItem;
            if (clickeditem == null)
                return;

            var mainMenuUCVM = DataContext as MainMenuUCVM;
            if (mainMenuUCVM == null)
                return;

            mainMenuUCVM.FormMenuClick(clickeditem.CommandParameter.ToString());
        }

        private void Tools_Click(object sender, RoutedEventArgs e)
        {
            var clickeditem = sender as MenuItem;
            if (clickeditem == null)
                return;

            var mainMenuUCVM = DataContext as MainMenuUCVM;
            if (mainMenuUCVM == null)
                return;

            mainMenuUCVM.ToolsMenuClick(clickeditem.CommandParameter.ToString());//, this);//, this);
        }

        private void UploadTo_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowVM.SelectedBorrDir == null || MainWindowVM.SelectedBorrDir.FullActivePath == null)
            {
                var alertBox = new AlertBoxWin
                    {
                        AlertText = "No folder has been selected!"
                    };
                alertBox.ShowDialog();
                return;
            }

            var clickeditem = sender as MenuItem;
            if (clickeditem == null)
                return;

            var mainMenuUCVM = DataContext as MainMenuUCVM;
            if (mainMenuUCVM == null)
                return;

            mainMenuUCVM.UploadMenuClick((string)clickeditem.CommandParameter);
        }
    }
}
