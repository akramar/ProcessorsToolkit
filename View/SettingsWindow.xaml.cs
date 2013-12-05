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
using System.Windows.Shapes;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.View
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        List<string> _directoriesList;

        public SettingsWindow()
        {
            InitializeComponent();
            EditBtn.IsEnabled = false;
            BindSourceFoldersLB();
        }

        private void BindSourceFoldersLB()
        {
            _directoriesList = new List<string>(Properties.Settings.Default.AvailableFolders.Cast<string>());
            _directoriesList.Sort();
            SourceFoldersLB.ItemsSource = _directoriesList;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            borrFoldersUCVM.OnAvailableFoldersUpdated();
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            var newSrcDlg = new SettingsWindow_AddEditDirectory() {Owner = this, Title = "New Directory"};
            newSrcDlg.Closed += (s, dialogEvents) => BindSourceFoldersLB();
            newSrcDlg.ShowDialog();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SourceFoldersLB.SelectedItem == null) 
                return;

            var editSrcDlg = new SettingsWindow_AddEditDirectory(SourceFoldersLB.SelectedItem.ToString())
                {
                    Owner = this,
                    Title = "Edit Directory"
                };
            editSrcDlg.Closed += (s, dialogEvents) => BindSourceFoldersLB();
            editSrcDlg.ShowDialog();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SourceFoldersLB.SelectedItem == null) 
                return;

            var dirToDelete = SourceFoldersLB.SelectedItem.ToString();
            Properties.Settings.Default.AvailableFolders.Remove(dirToDelete);
            Properties.Settings.Default.Save();
            BindSourceFoldersLB();
        }

        private void SourceFoldersLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditBtn.IsEnabled = SourceFoldersLB.SelectedItem != null;
        }
        
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
