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
using ProcessorsToolkit.Model.Interbank.UploadSession;
using ProcessorsToolkit.ViewModel.Interbank;

namespace ProcessorsToolkit.View.Interbank.UploadWindow
{
    /// <summary>
    /// Interaction logic for ConditionMatcherUC.xaml
    /// </summary>
    public partial class ConditionMatcherUC : UserControl
    {
        public ConditionMatcherUC()
        {
            InitializeComponent();
        }

        private void ConditionMatcherUC_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            FilesListBox.ItemsSource = vm.WorkingFileList.Where(f => f.IsSelected);
            FilesListBox.DataContext = vm.WorkingFileList.Where(f => f.IsSelected);
        }

        private void AddConditionsToCombo(object sender, EventArgs e)
        { 
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            var sourceCombo = sender as ComboBox;

            if (sourceCombo == null)
             return;

            sourceCombo.ItemsSource = vm.WebsiteSession.SelectedLoansConditions;
            sourceCombo.DisplayMemberPath = "DisplayNameShort";
            sourceCombo.SelectedValuePath = "UniqueId";

            /*if ((bool)alreadyHavePTDsCB.IsChecked) //allow all conditions
                {
                    foreach (KeyValuePair<string, string> docSchema in Globals.DocSchemaIds)
                        sourceCombo.Items.Add(new ComboBoxItem() { Uid = docSchema.Key, Content = docSchema.Value });
                    sourceCombo.SelectedIndex = 1;
                }
                else
                {
                    sourceCombo.Items.Add(new ComboBoxItem() { Uid = "209638", Content = "Submission Pkg" });
                    sourceCombo.Items.Add(new ComboBoxItem() { Uid = "209654", Content = "Conditions" });
                    sourceCombo.SelectedIndex = 0;
                }
             
            }*/
        }

        private void FileCondChanged(object sender, SelectionChangedEventArgs e)
        {
            var sourceCombo = sender as ComboBox;

            if (sourceCombo == null)
                return;

            var fileToUpload = sourceCombo.DataContext as FileToUpload;

            var selectedCond = sourceCombo.SelectedItem as LoanCondition;

            if (fileToUpload == null) 
                return;
            
            fileToUpload.AttachedLoanCondition = selectedCond;

            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            var workingList = vm.WorkingFileList;

            if (vm.WorkingFileList.Where(f => f.IsSelected).All(f => f.AttachedLoanCondition != null))
                UploadBtn.IsEnabled = true;

        }

        private void UploadBtn_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            vm.OnDoneMatchingConditions();
        }
    }
}
