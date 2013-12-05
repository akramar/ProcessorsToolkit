using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using ProcessorsToolkit.Model.PRMG.UploadSession;
using ProcessorsToolkit.Model.Interbank.UploadSession;
using ProcessorsToolkit.ViewModel.Interbank;

namespace ProcessorsToolkit.View.Interbank.UploadWindow
{
    /// <summary>
    /// Interaction logic for LoanAndFileSelectorUC.xaml
    /// </summary>
    public partial class LoanAndFileSelectorUC : UserControl
    {
        //private UploadWindowVM _parentVM;
        
        public LoanAndFileSelectorUC()//UploadWindowVM parentVM)
        {
            //_parentVM = parentVM;
            InitializeComponent();
        }
        
        private void LoanAndFileSelectorUC_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadLoanCombo();

            LoadFileSelector();


        }


        private void LoadLoanCombo()
        {
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            Task.Factory.StartNew(new Action(() =>
                LoanOptionsCombo.Dispatcher.Invoke(new Action(() =>
                {
                    LoanOptionsCombo.IsEnabled = true;
                    LoanOptionsCombo.ItemsSource = null;
                    LoanOptionsCombo.ItemsSource = vm.AllLoansAvailable;
                    LoanOptionsCombo.DisplayMemberPath = "DisplayName";
                    LoanOptionsCombo.SelectedValuePath = "IBWLoanNum";
                }))))
                .ContinueWith(task1 =>
                    {
                        if (vm.TargetLoanItem == null)
                            return;

                        var selectedVal = vm.TargetLoanItem.IBWLoanNum;
                        lock (selectedVal)
                        {
                            LoanOptionsCombo.Dispatcher.Invoke(new Action(() =>
                                {
                                    if (selectedVal != null)
                                        LoanOptionsCombo.SelectedValue = selectedVal;
                                }));
                        }
                    });
        }


        private void LoadFileSelector()
        {
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            vm.WorkingFileList = new BorrowerFileGroup(vm);

            vm.WorkingFileList.LoadAllFiles();

            //fl = new BorrowerFileGroup().LoadAllFiles();
            FilesListBox.DataContext = vm.WorkingFileList;
            FilesListBox.ItemsSource = vm.WorkingFileList;
        }

        private void ChangedLoanSelection(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            //var currloan = LoanOptionsCombo.SelectedItem as LoanSearchResultItem;

            if (LoanOptionsCombo.SelectedValue == null || vm.AllLoansAvailable == null)
                return;

            foreach (var loan in vm.AllLoansAvailable.Where(loan => loan.IsSelected))
                loan.IsSelected = false;

            var targetLoan = LoanOptionsCombo.SelectedItem as LoanSearchResultItem;

            var loanSearchResultItem = vm.AllLoansAvailable.FirstOrDefault(l => l == targetLoan);
            if (loanSearchResultItem != null)
                loanSearchResultItem.IsSelected = true;
        }

        private void cbxSelect_CheckChanged(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;

            if (cb == null || cb.IsChecked == null)
                return;

            var clickedFile = cb.DataContext as FileToUpload;

            if (clickedFile != null)
                clickedFile.IsSelected = (bool)cb.IsChecked;
        }


        private void ContinueBtn_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            vm.OnDoneSelectingBorrAndFiles();
        }
    }
}
