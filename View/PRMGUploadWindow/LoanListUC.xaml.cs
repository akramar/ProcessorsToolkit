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
using ProcessorsToolkit.Model.UploadSession.PRMG;
using ProcessorsToolkit.ViewModel;
using ProcessorsToolkit.ViewModel.PRMGUploadWindow;

namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for LoanListUC.xaml
    /// </summary>
    public partial class LoanListUC : UserControl
    {
        //private readonly UploadWindowVM _parentVM;

        public LoanListUC()//UploadWindowVM parentVM)
        {
            //_parentVM = parentVM;

            InitializeComponent();
            Loaded += LoanListUC_Loaded;

            UploadWindowVM.LoanListLoaded += UploadWindowVMOnLoanListLoaded;
        }

        private void UploadWindowVMOnLoanListLoaded(object sender, EventArgs eventArgs)
        {

            LoanOptionsCombo.Dispatcher.Invoke(new Action(() =>
                {
                    LoanOptionsCombo.IsEnabled = true;
                    LoanOptionsCombo.ItemsSource = null;
                    LoanOptionsCombo.ItemsSource = UploadWindowVM.AllLoansAvailable;
                    LoanOptionsCombo.DisplayMemberPath = "DisplayName";
                    LoanOptionsCombo.SelectedValuePath = "ContId";
                }));

        }

        void LoanListUC_Loaded(object sender, RoutedEventArgs e)
        {
           

            //var loanDDLTask = new Task(LoadLoanOptions);
            //loanDDLTask.Start();

            

            LoadLoanOptions();
            //LoadLoanOptions();
        }

        private void LoadLoanOptions()
        {
            //_parentVM.AllLoansAvailable = new AvailableLoansList(_parentVM);

            if (UploadWindowVM.AllLoansAvailable == null)
                UploadWindowVM.AllLoansAvailable = new AvailableLoansList();

            else
                UploadWindowVM.AllLoansAvailable.Clear();



            // var loanComboLoaderScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var listDownloader = new Action(() => UploadWindowVM.AllLoansAvailable.LoadList());
            var listUiLoader = new Action(UploadWindowVM.OnLoanListLoaded);

            var listUiMatcher = new Action(() =>
                {
                    UploadWindowVM.TargetLoanItem = UploadWindowVM.AllLoansAvailable.FirstOrDefault(l => l.IsSelected);

                    if (UploadWindowVM.TargetLoanItem == null)
                        return;

                    var selectedVal = UploadWindowVM.TargetLoanItem.ContId;

                    lock (selectedVal)
                    {
                        LoanOptionsCombo.Dispatcher.Invoke(new Action(() =>
                            {
                                if (selectedVal != null)
                                    LoanOptionsCombo.SelectedValue = selectedVal;
                            }));
                    }
                });


            Task task = Task.Factory.StartNew(listDownloader)
                            .ContinueWith(delegate { listUiLoader(); })
                            .ContinueWith(delegate { listUiMatcher(); });



            // var taskDownload = Task.Factory.StartNew(listDownloader);
            //var taskUiLoad = taskDownload.ContinueWith(t => listUiLoader(), loanComboLoaderScheduler);

            //var taskListMatcher = taskUiLoad.ContinueWith(t => listUiMatcher(), loanComboLoaderScheduler);
        }

        private void ChangedLoanSelection(object sender, SelectionChangedEventArgs e)
        {
            var currloan = LoanOptionsCombo.SelectedItem as LoanSearchResultItem;

            if (LoanOptionsCombo.SelectedValue != null && UploadWindowVM.AllLoansAvailable != null)
            {
                var targetLoan = LoanOptionsCombo.SelectedItem as LoanSearchResultItem;

                UploadWindowVM.TargetLoanItem = targetLoan;
            }
               


               // UploadWindowVM.TargetLoanItem = UploadWindowVM.AllLoansAvailable
                //                                    .FirstOrDefault(
                //                                        l => l.ContId == (string) LoanOptionsCombo.SelectedValue);

            //else
             //   UploadWindowVM.TargetLoanItem = null;
        }

        private void alreadyHavePTDsCB_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;

            if (cb == null || cb.IsChecked == null)
                return;

            UploadWindowVM.SelectedUploadType = cb.IsChecked == true 
                ? UploadWindowVM.UploadTypes.PTDConditions 
                : UploadWindowVM.UploadTypes.Submission;

        }

        private void AlreadyHavePtdsCb_OnChecked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null || cb.IsChecked == null)
                return;
            UploadWindowVM.AlreadyHavePTDs = (bool) cb.IsChecked;
        }
    }
}
