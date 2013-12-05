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
using ProcessorsToolkit.Model.PRMG.UploadSession;
using ProcessorsToolkit.ViewModel;
using ProcessorsToolkit.ViewModel.PRMG;

namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for LoanListUC.xaml
    /// </summary>
    public partial class LoanAndFileSelectorUC : UserControl
    {
        private readonly UploadWindowVM _parentVM;

        public LoanAndFileSelectorUC(UploadWindowVM parentVM)
        {
            _parentVM = parentVM;

            InitializeComponent();
            Loaded += LoanListUC_Loaded;

            _parentVM.LoanListLoaded += UploadWindowVMOnLoanListLoaded;
        }

        void LoanListUC_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLoanOptions();
        }

        private void FilesListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            _parentVM.WorkingFileList = new BorrowerFileGroup(_parentVM);

            _parentVM.WorkingFileList.LoadAllFiles();

            FilesListBox.DataContext = _parentVM.WorkingFileList;
            FilesListBox.ItemsSource = _parentVM.WorkingFileList;
        }

        private void UploadWindowVMOnLoanListLoaded(object sender, EventArgs eventArgs)
        {
            LoanOptionsCombo.Dispatcher.Invoke(new Action(() =>
                {
                    LoanOptionsCombo.IsEnabled = true;
                    LoanOptionsCombo.ItemsSource = null;
                    LoanOptionsCombo.ItemsSource = _parentVM.AllLoansAvailable;
                    LoanOptionsCombo.DisplayMemberPath = "DisplayName";
                    LoanOptionsCombo.SelectedValuePath = "ContId";
                }));
        }

        private void LoadLoanOptions()
        {
            _parentVM.AllLoansAvailable = new AvailableLoansList(_parentVM.CurrImgFlowSession);

            var listDownloader = new Action(() => _parentVM.AllLoansAvailable.LoadList());
            var listUiLoader = new Action(_parentVM.OnLoanListLoaded);

            var listUiMatcher = new Action(() =>
                {
                    _parentVM.TargetLoanItem = _parentVM.AllLoansAvailable.FirstOrDefault(l => l.IsSelected);

                    if (_parentVM.TargetLoanItem == null)
                        return;

                    var selectedVal = _parentVM.TargetLoanItem.ContId;

                    lock (selectedVal)
                    {
                        LoanOptionsCombo.Dispatcher.Invoke(new Action(() =>
                            {
                                if (selectedVal != null)
                                    LoanOptionsCombo.SelectedValue = selectedVal;
                            }));
                    }
                });

            Task.Factory.StartNew(listDownloader)
                .ContinueWith(delegate { listUiLoader(); })
                .ContinueWith(delegate { listUiMatcher(); });
        }

        private void ChangedLoanSelection(object sender, SelectionChangedEventArgs e)
        {
            var currloan = LoanOptionsCombo.SelectedItem as LoanSearchResultItem;

            if (LoanOptionsCombo.SelectedValue != null && _parentVM.AllLoansAvailable != null)
            {
                var targetLoan = LoanOptionsCombo.SelectedItem as LoanSearchResultItem;

                _parentVM.TargetLoanItem = targetLoan;
            }
            
        }

        private void alreadyHavePTDsCB_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;

            if (cb == null || cb.IsChecked == null)
                return;

            _parentVM.SelectedUploadType = cb.IsChecked == true 
                ? UploadWindowVM.UploadTypes.PTDConditions 
                : UploadWindowVM.UploadTypes.Submission;

            //_parentVM.AlreadyHavePTDs = (bool)cb.IsChecked;

            //TODO: enable/disable dropdown list

            FilesListBox.Items.Refresh();

        }
        
        private void LoanAndFileSelectorUC_OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void ConditionCombo_OnInitialized(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;

            if (cb == null)
                return;

            var itemFile = cb.DataContext as FileToUpload;

            foreach (var dsi in FileToUpload.DocSchemaIds)
                if (dsi.Value != "-1") //skip the submission indicator, we're flagging with PTD checkbox
                    cb.Items.Add(dsi.Value);

            cb.SelectedIndex = 0;

            cb.IsEnabled = _parentVM.SelectedUploadType == UploadWindowVM.UploadTypes.PTDConditions;
            
        }

        private void ConditionCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;

            if (cb == null)
                return;

            var itemFile = cb.DataContext as FileToUpload;

            if (itemFile == null)
                return;

            itemFile.ConditionId = (string) cb.SelectedValue;
        }

        private void cbxSelect_CheckChanged(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;

            if (cb == null || cb.IsChecked == null)
                return;

            var clickedFile = cb.DataContext as FileToUpload;

            if (clickedFile != null)
                clickedFile.IsSelected = (bool)cb.IsChecked;

            //FilesListBox.Items.Refresh();
        }

        private void UploadBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _parentVM.OnDoneSelectingFiles();
        }

        


    }
}
