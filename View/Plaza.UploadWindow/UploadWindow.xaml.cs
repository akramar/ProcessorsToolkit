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
using ProcessorsToolkit.ViewModel.Plaza;

namespace ProcessorsToolkit.View.Plaza.UploadWindow
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
                ctx.GettingHomepage += CtxGettingHomepage;
                ctx.PostingLogin += CtxPostingLogin;
                ctx.LoggedIntoSite += CtxOnLoggedIntoSite;
                ctx.FailedLoginToSite += CtxOnFailedLoginToSite;
                ctx.GettingPipeline += CtxGettingPipeline;
                ctx.RetrievedLoans += CtxOnRetrievedLoans;
                ctx.DoneSelectingBorrAndFiles += CtxOnDoneSelectingBorrAndFiles;
                ctx.DoneGettingUploadCredentials += CtxOnDoneGettingUploadCredentials;
                ctx.DoneGettingUploadPage += CtxOnDoneGettingUploadPage;
                //ctx.DoneFetchingConditions += CtxOnDoneFetchingConditions;
                //ctx.DoneMatchingConditions += CtxOnDoneMatchingConditions;
                ctx.DoneUploadingAllFiles += CtxOnDoneUploadingAllFiles;

                ContentArea.Content = new LoginUC { DataContext = ctx };
            }
        }


        private void CtxGettingHomepage(object sender, EventArgs e)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
            {
                ContentArea.Content = new BusyUC
                {
                    StatusMsg = "Fetching login page..."
                };
            }));
        }

        private void CtxPostingLogin(object sender, EventArgs e)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
            {
                ContentArea.Content = new BusyUC
                {
                    StatusMsg = "Posting login..."
                };
            }));
        }
        
        private void CtxOnLoggedIntoSite(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
            {
                ContentArea.Content = new BusyUC
                {
                    StatusMsg = "Login successful..."
                };
            }));
        }
        
        private void CtxOnFailedLoginToSite(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
            {
                var ctx = DataContext as UploadWindowVM;
                if (ctx != null)
                    ContentArea.Content = new LoginUC {DataContext = ctx};
            }));
        }

        void CtxGettingPipeline(object sender, EventArgs e)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
            {
                ContentArea.Content = new BusyUC
                {
                    StatusMsg = "Fetching pipeline..."
                };
            }));
        }

        private void CtxOnRetrievedLoans(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    var ctx = DataContext as UploadWindowVM;
                    if (ctx != null)
                        ContentArea.Content = new LoanAndFileSelectorUC {DataContext = ctx};
                }));
        }
        
        private void CtxOnDoneSelectingBorrAndFiles(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    ContentArea.Content = new BusyUC
                        {
                            StatusMsg = "Fetching upload credentials..."
                        };
                }));
        }

        
        private void CtxOnDoneGettingUploadCredentials(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    ContentArea.Content = new BusyUC
                        {
                            StatusMsg = "Fetching upload page..."
                        };
                    //<!-- //frozen here
                }));
        }

        private void CtxOnDoneGettingUploadPage(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    var ctx = DataContext as UploadWindowVM;
                    if (ctx != null)
                        ContentArea.Content = new UploadedProgressUC {DataContext = ctx};
                }));
        }

        /*
        private void CtxOnDoneFetchingConditions(object sender, EventArgs eventArgs)
        {
            
        }
        
        private void CtxOnDoneMatchingConditions(object sender, EventArgs eventArgs)
        {
            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    var ctx = DataContext as UploadWindowVM;
                    ContentArea.Content = new UploadedProgressUC {DataContext = ctx};
                }));
        }
        */

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
