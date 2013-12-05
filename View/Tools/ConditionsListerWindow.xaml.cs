using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.View.Tools
{
    /// <summary>
    /// Interaction logic for ConditionsListerWindow.xaml
    /// </summary>
    public partial class ConditionsListerWindow : Window
    {

        public ConditionsListerWindow()
        {
            InitializeComponent();

            Loaded += ConditionsListerWindow_Loaded;
        }

        void ConditionsListerWindow_Loaded(object sender, RoutedEventArgs e)
        {

            

            LoadList(true, true);
        }

        private void LoadList(bool conditionFilesOnly, bool removeExtension)
        {
            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            var selectedCondsFolder = borrFoldersUCVM.SelectedBorrDir.SubDirs.FirstOrDefault(sd => sd.IsOpen);
            if (selectedCondsFolder != null)
                Title = "Conditions for " + borrFoldersUCVM.SelectedBorrDir.BorrDirName + " from '" + selectedCondsFolder.FolderName + "' folder";

            var filterString = conditionFilesOnly ? "*.txt" : "*";

            var allViewableFilesNames = Directory.GetFiles(borrFoldersUCVM.SelectedBorrDir.FullActivePath, filterString)
                                              .Select(f => new FileInfo(f).Name).ToList();

            var listerString = "";
            foreach (var filesName in allViewableFilesNames)
            {
                if (filesName.EndsWith(".txt"))
                    listerString += filesName.Remove(filesName.Length - 4);

                else if (!conditionFilesOnly)
                {
                    if (removeExtension)
                        listerString += filesName.Remove(filesName.LastIndexOf('.'));
                    else
                        listerString += filesName;
                }
                
                listerString += Environment.NewLine;
            }

            //var listerString = viewableFilesNames.Aggregate("", (current, addtl) => current + (addtl + Environment.NewLine));

            FilesListBox.Text = listerString;
        }

        private void ConditionsOnly_CheckClicked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null || cb.IsChecked == null)
                return;
            
            LoadList((bool)cb.IsChecked, NoExtCB.IsChecked != null && (bool) NoExtCB.IsChecked);
        }

        private void NoExtension_CheckClicked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null || cb.IsChecked == null)
                return;

            LoadList(CondsOnlyCB.IsChecked != null && (bool) CondsOnlyCB.IsChecked, (bool) cb.IsChecked);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
