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
    /// Interaction logic for BorrFilesUC.xaml
    /// </summary>
    public partial class BorrFilesUC : UserControl
    {

        public BorrFilesUCVM CurrBorrFilesVM;

        public BorrFilesUC()
        {
            CurrBorrFilesVM = new BorrFilesUCVM();

            InitializeComponent();
            this.ListView1.DataContext = CurrBorrFilesVM;
        }

        private void ListView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void listViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //We keep using this snippet, move into an extension onto MouseButtonEventArgs
            var item = e.Source as ListViewItem;

            if (item == null)
                return;

            var entry = item.DataContext as FileBase;

            if (entry == null)
                return;



            e.Handled = true; //TODO:  it's still selecting text!

            System.Diagnostics.Process.Start(entry.Fullpath);



            
        }

        private void ButtonBase_CheckChanged(object sender, RoutedEventArgs e)
        {
            //http://stackoverflow.com/questions/15480279/wpf-check-box-check-changed-handling
            //
            var cb = sender as CheckBox;


            if (cb == null || cb.IsChecked == null)
                return;

            var fileSrc = CurrBorrFilesVM.FolderFiles.First(f => f.Fullpath == cb.CommandParameter.ToString());

            if (fileSrc == null)
                return;



            //
            //   cb.IsChecked


            //  var entry = item.DataContext as FileBase;

            // if (entry == null)
            //     return;

            CurrBorrFilesVM.FileSelectionChanged(fileSrc, (bool) cb.IsChecked);
        }


        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null)
                return;

            var image = sender as Image;

            if (image == null)
                return;

            var item = (FileBase) image.DataContext;

            //TODO: working on setting focus to subjec textbox

            foreach (var f in CurrBorrFilesVM.FolderFiles.Where(f => f.IsBeingEdited))
                f.IsBeingEdited = false;

            var selectedFile = CurrBorrFilesVM.FolderFiles.FirstOrDefault(f => f.Fullpath == item.Fullpath);
            if (selectedFile != null)
                selectedFile.IsBeingEdited = true;
                //CurrBorrFilesVM.FolderFiles.FirstOrDefault(f => f.Fullpath == item.Fullpath).IsBeingEdited = true;
      
            //ListView1.DataContext = CurrBorrFilesVM;
            ListView1.Items.Refresh();

            
        }

        private void ListView1_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView == null) 
                return;

            var gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
                // 35; // take into account vertical scrollbar
            /*var col1 = 0.50;
            var col2 = 0.20;
            var col3 = 0.15;
            var col4 = 0.15;

            gView.Columns[0].Width = workingWidth*col1;
            gView.Columns[1].Width = workingWidth*col2;
            gView.Columns[2].Width = workingWidth*col3;
            gView.Columns[3].Width = workingWidth*col4;*/

           
            var col2 = 60;
            var col3 = 130;
            var col4 = 60;

            var col1 = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - (col2 + col3 + col4) - 10;


             gView.Columns[0].Width = col1;
            gView.Columns[1].Width = col2;
            gView.Columns[2].Width = col3;
            gView.Columns[3].Width = col4;

        }

    }
}
