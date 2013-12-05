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
    /// Interaction logic for BorrFoldersUC.xaml
    /// </summary>
    public partial class BorrFoldersUC : UserControl
    {
        //public BorrFoldersUCVM CurrBorrFoldersVM;
        private bool _doubleClickedSubDir; //gotta be a better way to catch subdirectory double clicks

        public BorrFoldersUC()
        {
            MainWindowVM.BorrFoldersCtrl = this;
            DataContext = new BorrFoldersUCVM {View = this};
            
            InitializeComponent();

            BorrFoldersLV.DataContext = DataContext;

            //Initialized += BorrFoldersUC_Initialized;
            Loaded += BorrFoldersUC_Loaded;

        }

        void BorrFoldersUC_Initialized(object sender, EventArgs e)
        {
            var borrFoldersUC = sender as BorrFoldersUC;
            if (borrFoldersUC == null)
                return;

            MainWindowVM.BorrFoldersCtrl = borrFoldersUC;
        }

        void BorrFoldersUC_Loaded(object sender, RoutedEventArgs e)
        {
            var borrFoldersUC = sender as BorrFoldersUC;
            if (borrFoldersUC == null)
                return;

            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            var mainMenuUCVM = MainWindowVM.MainMenuCtrl.DataContext as MainMenuUCVM;
            if (mainMenuUCVM == null)
                return;

            borrFoldersUCVM.LoadBorrFolders();

            //borrFoldersUCVM.SelectedBorrChanged += (s, args) => //Causes infinite loop!
              //  borrFoldersUCVM.OnSelectedBorrChanged(args);

            //borrFoldersUCVM.SelectedBorrChanged += (o, args) =>  //this is backwards, should be attaching from mainmenuvm
             //   mainMenuUCVM.SelectedBorrChanged(args != null && args.CurrBorr != null);//, MainMenu);


            /*
        if (loadedborrfoldersUC != null)
        {
            if (loadedborrfoldersUC.CurrBorrFoldersVM.SelectedBorr != null)
                CurrMainWindowVM.SelectedBorr = loadedborrfoldersUC.CurrBorrFoldersVM.SelectedBorr;
            if (loadedborrfoldersUC.CurrBorrFoldersVM.SelectedBorr != null)
                CurrMainWindowVM.SelectedFolderPath = loadedborrfoldersUC.CurrBorrFoldersVM.SelectedBorr.Fullpath;
        }*/
        }
        

        public void RefreshFolderList()
        {
            BorrFoldersLV.ItemsSource = null;

            var borrFoldersUCVM = DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM != null)
                BorrFoldersLV.ItemsSource = borrFoldersUCVM.BorrDirs;

            BorrFoldersLV.Items.Refresh();
        }

        private void OnSelectedBorrower(object s, MouseButtonEventArgs e)
        {

        }

        private void BorrFolder_MouseDoubleClick(object s, MouseButtonEventArgs e)
        {
            if (_doubleClickedSubDir)
            {
                _doubleClickedSubDir = false;
                return;
            }

            var item = e.Source as ListViewItem;

            if (item == null)
                return;

            var entry = item.DataContext as BorrDir;

            if (entry == null)
                return;

            var borrFoldersUCVM = DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;


            borrFoldersUCVM.SetNewBorrDir(entry);

            BorrFoldersLV.Items.Refresh();
        }

        private void Subfolder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = e.Source as ListViewItem;

            if (item == null)
                return;

            var entry = item.DataContext as BorrSubDir;

            if (entry == null)
                return;

            var borrFoldersUCVM = DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            borrFoldersUCVM.SetNewBorrSubDir(entry);

            _doubleClickedSubDir = true;
        }


        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var borrFoldersUCVM = DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            BorrFoldersLV.ItemsSource = borrFoldersUCVM.BorrDirs
                .Where(bd => bd.BorrDirName.StartsWith(SearchFoldersTB.Text, StringComparison.InvariantCultureIgnoreCase));
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SearchFoldersTB.Text = null;
            Keyboard.ClearFocus();

            var borrFoldersUCVM = DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            foreach (var dir in borrFoldersUCVM.BorrDirs.Where(f => f.IsCurrentBorr))
            {
                dir.SubDirs = null;
                dir.IsCurrentBorr = false;
            }

            if (borrFoldersUCVM.SelectedBorrDir != null)
                borrFoldersUCVM.SelectedBorrDir.SubDirs = null;

            BorrFoldersLV.UnselectAll();
            BorrFoldersLV.Items.Refresh();
            MainWindowVM.OnClearBorrSelected();
        }
    }
}
