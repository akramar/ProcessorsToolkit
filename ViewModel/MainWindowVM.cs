using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProcessorsToolkit.Model;
using ProcessorsToolkit.Model.Interbank.FormFiller;
using ProcessorsToolkit.Model.PRMG.FormFiller;
using ProcessorsToolkit.View;

//using ProcessorsToolkit.View;

namespace ProcessorsToolkit.ViewModel
{
    public class MainWindowVM : ObservableObject
    {
        //public static event EventHandler<SelectedBorrChangedEventArgs> SelectedBorrChanged;
        //public static event EventHandler<SelectedDirChangedEventArgs> SelectedDirPathChanged;
        //public static event EventHandler<SelectedBorrDataChangedEventArgs> SelectedBorrDataChanged;
        public static event EventHandler ClearSelectedBorr;

        public static BorrDir SelectedBorrDir { get; set; } // THIS NEEDS TO BE CONSOLIDATED WITH BorrFoldersUCVM
        //public static string SelectedFolderPath { get; set; }
        //public static FannieData SelectedBorrData { get; set; }
        //public static FileBase SelectedFile { get; set; } //This should be in BorrFilesVM (looser coupling)
        //public static MainMenuUCVM MainMenu { get; set; }

        //public static Menu MainMenuObj { get; set; }


        

        public static MainWindow View { get; set; }
        public static BorrFoldersUC BorrFoldersCtrl;
        public static MainMenuUC MainMenuCtrl;
        public static BorrInfoUC BorrInfoCtrl;
        public static BorrFilesUC BorrFilesCtrl;

        public MainWindowVM()
        {
            //BorrInfoUCVM.BorrInfoUpdated += (sender, args) => OnSelectedBorrDataChanged(args);
            ClearSelectedBorr += MainWindowVM_ClearSelectedBorr;
            //SelectedFileChanged += MainWindowVM_SelectedFileChanged;

            //TestCommand = FindCommand;


        }
        /*
        public static RoutedCommand FindCommand = new RoutedCommand();

        private void ExecutedFindCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Custom Command Executed");
        }
        private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var target = e.Source as Control;

            if (target != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }


        public void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }


        public ICommand TestCommand { get; private set; }
        */
        /*
        private static ICommand _findBorrNameCommand;
        public static ICommand FindBorrNameCommand
        {
            get
            {

                if (_findBorrNameCommand == null)
                {

                    //_findBorrNameCommand = new RelayCommand(param => this.CloseCurrentCedarFile(param));
                }
                return _findBorrNameCommand;
            }
        }*/

        /*
        public static void OnSelectedFileChanged(SelectedFileChangedEventArgs e)
        {
            var handler = SelectedFileChanged;
            if (e != null)
                SelectedFile = e.CurrFile;
            if (handler != null) handler(null, e);
        }
        */
        
        
        /*public static void OnSelectedBorrChanged(SelectedBorrChangedEventArgs e)
        {
            if (e != null && e.CurrBorr != null)
                SelectedBorrDir = e.CurrBorr;
            var handler = SelectedBorrChanged;
            if (handler != null) handler(null, e);
        }
        */
        /*
        public static void OnSelectedPathChanged(SelectedDirChangedEventArgs e)
        {
            //var subDir = BorrDir.GetSubDir(SelectedBorrDir.BorrName, e.CurrPath);

           // if (!String.IsNullOrEmpty(subDir))
           //     SelectedBorrDir.SubDirs.First(sd => sd.FolderName == subDir).IsOpen = true;

            //SelectedFolderPath = e.CurrPath;
            if (e != null && e.CurrPath != null)
                e.CurrPath = SelectedBorrDir.FullActivePath;

            var handler = SelectedDirPathChanged;
            if (handler != null) handler(null, e);
        }
        */
        /*
        public static void OnSelectedBorrDataChanged(SelectedBorrDataChangedEventArgs e)
        {
            if (e != null)
                SelectedBorrData = e.CurrData;
            var handler = SelectedBorrDataChanged;
            if (handler != null) handler(null, e);
        }
        */
        public static void OnClearBorrSelected()
        {
            var handler = ClearSelectedBorr;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        
        void MainWindowVM_ClearSelectedBorr(object sender, EventArgs e)
        {
            var borrFoldersUCVM = BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            SelectedBorrDir = null;
            SelectedBorrDir = null;
            borrFoldersUCVM.OnSelectedBorrChanged(null);
            //OnSelectedBorrDataChanged( null);
            //OnSelectedPathChanged(null);


            //BorrFoldersUCVM.OnSelectedPathChanged(new SelectedDirChangedEventArgs() {CurrPath = null});
            
            borrFoldersUCVM.OnSelectedPathChanged(new SelectedPathChangedEventArgs() {CurrPath = null});
        }

        

        /*
        public void LoadSubDirs(ListViewItem item)
        {
            

        }

        public ICommand ConvertTextCommand
        {
            get { return new DelegateCommand(LoadSubDirs(null)); }
        }
         */
    }
}
