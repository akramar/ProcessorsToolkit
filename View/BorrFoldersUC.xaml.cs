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
        public BorrFoldersUCVM CurrBorrFoldersVM;
        private bool _doubleClickedSubDir; //gotta be a better way to catch subdirectory double clicks

        public BorrFoldersUC()
        {
            CurrBorrFoldersVM = new BorrFoldersUCVM();
            
            InitializeComponent();

            this.ListView1.DataContext = CurrBorrFoldersVM;
        }

        private void OnSelectedBorrower(object s, MouseButtonEventArgs e)
        {

        }

        private void listViewItem_MouseDoubleClick(object s, MouseButtonEventArgs e)
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

            CurrBorrFoldersVM.SetNewBorrDir(entry);

            this.ListView1.Items.Refresh();
        }

        private void Subfolder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           

            var item = e.Source as ListViewItem;

            if (item == null)
                return;

            var entry = item.DataContext as BorrSubDir;

            if (entry == null)
                return;

            CurrBorrFoldersVM.SetNewBorrSubDir(entry);

            _doubleClickedSubDir = true;
        }
    }
}
