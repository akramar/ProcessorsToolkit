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
using ProcessorsToolkit.ViewModel.Interbank;

namespace ProcessorsToolkit.View.Interbank.UploadWindow
{
    /// <summary>
    /// Interaction logic for UploadWindow.xaml
    /// </summary>
    public partial class UploadWindow : Window
    {
        public UploadWindow()
        {
            DataContext = new UploadWindowVM {View = this};

            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var ctx = DataContext as UploadWindowVM;

            if (ctx != null)
            {
                //ctx.ReceivedCredentials += CtxOnReceivedCredentials;
                ctx.FetchingHomepage += ctx_FetchingHomepage;
                ctx.LoggingIn += ctx_LoggingIn;
                ctx.FetchingDashboard += ctx_FetchingDashboard;
                ctx.LoggedIntoSite += CtxOnLoggedIntoSite;
                ctx.RetrievedLoanIds += CtxOnRetrievedLoanIds;
                ctx.DoneSelectingBorrAndFiles += CtxOnDoneSelectingBorrAndFiles;
                ctx.DoneFetchingConditions += CtxOnDoneFetchingConditions;
                ctx.DoneMatchingConditions += CtxOnDoneMatchingConditions;
                ctx.DoneUploadingAllFiles += CtxOnDoneUploadingAllFiles;
            }

            ContentArea.Content = new LoginUC {DataContext = ctx};
        }

        private void CtxOnReceivedCredentials(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
            {
                ContentArea.Content = new BusyUC
                {
                    StatusMsg = "Have credentials..."
                };
            }));
        }

        private void ctx_FetchingHomepage(object sender, EventArgs e)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
            {
                ContentArea.Content = new BusyUC
                {
                    StatusMsg = "Fetching login page..."
                };
            }));
        }

        private void ctx_LoggingIn(object sender, EventArgs e)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
            {
                ContentArea.Content = new BusyUC
                {
                    StatusMsg = "Posting login..."
                };
            }));
        }

        void ctx_FetchingDashboard(object sender, EventArgs e)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
            {
                ContentArea.Content = new BusyUC
                {
                    StatusMsg = "Fetching dashboard..."
                };
            }));
        }
        

        private void CtxOnLoggedIntoSite(object sender, EventArgs eventArgs)
        {
            //ContentArea.Dispatcher.BeginInvoke(new Action(() =>
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    //TODO: this isn't getting called until conditions are already downloaded, something wrong with order of events
                    ContentArea.Content = new BusyUC
                    {
                        StatusMsg = "Downloading loan list..."
                    };
                }));
        }

        private void CtxOnRetrievedLoanIds(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    var ctx = DataContext as UploadWindowVM;
                    ContentArea.Content = new LoanAndFileSelectorUC {DataContext = ctx};
                }));
        }
        
        private void CtxOnDoneSelectingBorrAndFiles(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    /*ContentArea.Content = new Xceed.Wpf.Toolkit.BusyIndicator
                        {
                            IsBusy = true,
                            Content = "Downloading loan conditions..."
                        };*/
                    ContentArea.Content = new BusyUC
                    {
                        StatusMsg = "Downloading loan conditions..."
                    };
                }));
        }

        private void CtxOnDoneFetchingConditions(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    var ctx = DataContext as UploadWindowVM;
                    ContentArea.Content = new ConditionMatcherUC {DataContext = ctx};
                }));
        }

        private void CtxOnDoneMatchingConditions(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    var ctx = DataContext as UploadWindowVM;
                    ContentArea.Content = new UploadedProgressUC {DataContext = ctx};
                }));
        }


        private void CtxOnDoneUploadingAllFiles(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    var ctx = DataContext as UploadWindowVM;
                    ContentArea.Content = new ConfirmedSentFilesUC { DataContext = ctx };
                }));
        }





        private void UploadWindow_OnClosed(object sender, EventArgs e)
        {
            //Probably want to null out the viewmodel
        }
    }
}
