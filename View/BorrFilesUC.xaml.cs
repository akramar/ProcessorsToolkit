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
        public BorrFilesUC()
        {
            MainWindowVM.BorrFilesCtrl = this;
            DataContext = new BorrFilesUCVM {View = this};
            
            InitializeComponent();

            Loaded += BorrFilesUC_Loaded;
        }

        void BorrFilesUC_Loaded(object sender, RoutedEventArgs e)
        {
            var borrFilesUCVM = DataContext as BorrFilesUCVM;
            if (borrFilesUCVM == null)
                return;

            //FilesListView.DataContext = borrFilesUCVM.FolderFiles;
            FilesListView.ItemsSource = borrFilesUCVM.FolderFiles;
        }

        private void FilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == null)
                return;

            var lv = sender as ListView;
            if (lv == null) 
                return;

            var fb = lv.SelectedItem as FileBase;
            if (fb == null)
                return;

            var borrFilesUCVM = DataContext as BorrFilesUCVM;
            if (borrFilesUCVM == null)
                return;

            var args = new SelectedFileChangedEventArgs {CurrFile = fb};
            borrFilesUCVM.OnSelectedFileChanged(args);
        }

        public void ResetScroll()
        {

            // Get the border of the listview (first child of a listview)
            var border = VisualTreeHelper.GetChild(FilesListView, 0) as Decorator;
            if (border != null)
            {
                // Get scrollviewer
                var scrollViewer = border.Child as ScrollViewer;
                if (scrollViewer != null)
                {
                    // center the Scroll Viewer...
                    //double center = scrollViewer.ScrollableHeight / 2.0;
                    double top = 0.0;
                    scrollViewer.ScrollToVerticalOffset(top);
                }
            }
        }

        private void FileItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //We keep using this snippet, move into an extension onto MouseButtonEventArgs
            var item = e.Source as ListViewItem;

            if (item == null)
                return;

            var entry = item.DataContext as FileBase;

            if (entry == null)
                return;


            e.Handled = true;

            var pathToOpen = String.Format(@"""{0}""", entry.Fullpath);

            System.Diagnostics.Process.Start(pathToOpen);

        }

        private void RenameImgClick_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null)
                return;

            var image = sender as Image;

            if (image == null)
                return;

            var item = (FileBase) image.DataContext;


            //var item2 = ListView1.Items.CurrentItem;

            var borrFilesUCVM = DataContext as BorrFilesUCVM;
            if (borrFilesUCVM == null)
                return;


            //TODO: working on setting focus to subjec textbox

            if (item.IsBeingEdited)
            {
                item.IsBeingEdited = false;

            }
            else
            {
                foreach (var f in borrFilesUCVM.FolderFiles.Where(f => f.IsBeingEdited))
                    f.IsBeingEdited = false;


                if (item.IsAlreadyOpen())
                {
                    /*                    var MB = new Xceed.Wpf.Toolkit.MessageBox
                    {
                        Caption = "Renaming Error",
                        Text = "File already open!"
                    };

                    MB.ShowDialog();*/
                    
                    var alertBox = new AlertBoxWin
                        {
                            AlertText = "File already open!"
                        };
                    alertBox.ShowDialog();
                    
                    
                    //MessageBox.Show("File already open!");

                    return;

                }

                item.IsBeingEdited = true;
                /*
                var selectedFile = CurrBorrFilesVM.FolderFiles.FirstOrDefault(f => f.Fullpath == item.Fullpath);
                if (selectedFile != null)
                {
                    selectedFile.IsBeingEdited = true;
                }*/

            }

            //var index = CurrBorrFilesVM.FolderFiles.IndexOf(item);

            //var lvi1 = ListView1.Items.GetItemAt(index) as ListViewItem;

            //CurrBorrFilesVM.FolderFiles.FirstOrDefault(f => f.Fullpath == item.Fullpath).IsBeingEdited = true;

            //ListView1.DataContext = CurrBorrFilesVM;

            FilesListView.Items.Refresh();



            /*
            var lvi2 = ListView1.Items.GetItemAt(index) as ListViewItem;
            
            if (lvi1 != null)
            {
                ListView1.ScrollIntoView(lvi1);


                lvi1.IsSelected = true;
                Keyboard.Focus(lvi1);
            }
            */

        }

        private void FilesListView_OnSizeChanged(object sender, SizeChangedEventArgs e)
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


            const int col2 = 60;
            const int col3 = 140;
            const int col4 = 60;

            var col1 = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - (col2 + col3 + col4) - 10;
            
            gView.Columns[0].Width = col1;
            gView.Columns[1].Width = col2;
            gView.Columns[2].Width = col3;
            gView.Columns[3].Width = col4;

        }

        private void RenamingTB_OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            if (sender == null)
                return;

            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (tb.IsEnabled)
            {

                //System.Windows.Input.FocusManager.SetFocusedElement();

                tb.Focusable = true;
                Keyboard.Focus(tb);

                // tb.Focus();
                //tb.SelectAll();


            }

            var item = tb.DataContext as FileBase;

        }

        //This should be moved to a style rule (datatrigger)
        private void FilenameTextbox_OnInitialized(object sender, EventArgs e)
        {
            if (sender == null)
                return;

            var tb = sender as TextBox;
            if (tb == null)
                return;

            var fileBase = tb.DataContext as FileBase;
            if (fileBase == null) 
                return;
            
            if (MainWindowVM.SelectedBorrDir.ActiveSubDir != null )
                if (MainWindowVM.SelectedBorrDir.ActiveSubDir.Fullpath == MainWindowVM.SelectedBorrDir.FullActivePath)
                    if (MainWindowVM.SelectedBorrDir.ActiveSubDir.FolderName.StartsWith("conditions", StringComparison.InvariantCultureIgnoreCase))
                    {
                        tb.Foreground = fileBase.Ext == ".txt" ? Brushes.Red : Brushes.DarkGreen;
                    }

            if (!fileBase.IsBeingEdited) 
                return;

            tb.Focusable = true;
            Keyboard.Focus(tb);
            tb.Select(0, fileBase.DisplayName.LastIndexOf('.'));

            //FilesListView.SelectedItem = fileBase; //this works
            //FilesListView.SelectedItem = null; //troubleshooting out
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (sender == null)
                return;

            var tb = sender as TextBox;

            if (tb == null)
                return;

            var item = tb.DataContext as FileBase;
            if (item == null)
                return;

            var borrFilesUCVM = DataContext as BorrFilesUCVM;
            if (borrFilesUCVM == null)
                return;

            if (e.Key == Key.Escape)
            {
                tb.Text = item.DisplayName;
                tb.Select(0, 0);
                item.IsBeingEdited = false;
                FilesListView.Items.Refresh();
                //borrFilesUCVM.FolderFiles_CollectionChanged(null, null);
            }

            if (e.Key == Key.Return)
            {
                var exceptionIncurred = item.RenameFile(tb.Text);

                if (exceptionIncurred == null)
                    item.IsBeingEdited = false;

                else
                {
                    /*
                    var MB = new Xceed.Wpf.Toolkit.MessageBox
                    {
                        Caption = "Renaming Error",
                        Text = exceptionIncurred.Message
                    };
                    MB.ShowDialog();
                    */
                    var alertBox = new AlertBoxWin
                    {
                        AlertText = exceptionIncurred.Message
                    };
                    alertBox.ShowDialog();
                }


                //ListView1.ItemsSource = null;
                //ListView1.ItemsSource = CurrBorrFilesVM.FolderFiles;

                //CurrBorrFilesVM.LoadBorrFiles(); //see if this fixes re-sorting

                //CurrBorrFilesVM.FolderFiles.

                borrFilesUCVM.FolderFiles_CollectionChanged(null, null);

                FilesListView.Items.Refresh();
            }
        }

        private void FilesListView_OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;

            if (e.Effects == DragDropEffects.Copy)
            {
                Cursor = Cursors.Hand;
            }
        }
        
    }
}
