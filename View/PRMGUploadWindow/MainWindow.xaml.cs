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
using System.Windows.Shapes;
using ProcessorsToolkit.ViewModel;
using ProcessorsToolkit.ViewModel.PRMGUploadWindow;
using Xceed.Wpf.Toolkit.Primitives;


namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for FileSelectorWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public UploadWindowVM UploadWinVM;

        public MainWindow()
        {
            //UploadWinVM = new UploadWindowVM();

            //CurrVM.CloseOwningWindow += (sender, args) => this.Close();

            InitializeComponent();
            UploadWindowVM.LoadEvents();

            //ContentArea.Content = new FileSelectorUC();
           //if PRMG
            ContentArea1.Content = new PRMGLoginUC();//UploadWinVM);

            UploadWindowVM.ReceivedPRMGCredentials += UploadWinVMOnReceivedPRMGCredentials;
            UploadWindowVM.HaveSecretQuestion += UploadWinVMOnHaveSecretQuestion;
            UploadWindowVM.ReceivedSecretAnswer += UploadWinVM_ReceivedSecretAnswer;
            UploadWindowVM.SuccessfulImgFlowLogin += UploadWinVMOnSuccessfulImgFlowLogin;
            UploadWindowVM.DoneSelectingFiles += UploadWindowVMOnDoneSelectingFiles;
            UploadWindowVM.DoneUploadingAllFiles += UploadWindowVMOnDoneUploadingAllFiles;

            //Task.Factory.StartNew(StatusThread);

            //var statusLbl = new Task(StatusThread);
            //statusLbl.Start();
        }

        

        private void UploadWinVMOnReceivedPRMGCredentials(object sender, EventArgs eventArgs)
        {
            //This fires out of order, has to do with event stack 
            //ContentArea.Content = null;

            ContentArea1.Content = new Xceed.Wpf.Toolkit.BusyIndicator()
            {
                IsBusy = true,
                Content = "Connecting to PRMG..."
            };

          /*  ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    ContentArea.Content = new Xceed.Wpf.Toolkit.BusyIndicator()
                        {
                            IsBusy = true
                        };
                }));
           * */
        }

        private void UploadWinVMOnHaveSecretQuestion(object sender, EventArgs eventArgs)
        {
            ContentArea1.Content = new SecretQuestionUC();
        }

        void UploadWinVM_ReceivedSecretAnswer(object sender, EventArgs e)
        {
            ContentArea1.Content = new Xceed.Wpf.Toolkit.BusyIndicator()
            {
                IsBusy = true,
                Content = "Finishing connection..."
            };

        }

        private void UploadWinVMOnSuccessfulImgFlowLogin(object sender, EventArgs eventArgs)
        {
            
            ContentArea1.Content = new LoanListUC();
            ContentArea1.Height = 100;
            ContentArea2.Content = new TextBlock()
                {
                    Text = "Current loan folder is " + MainWindowVM.SelectedBorrDir.BorrName,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
            ContentArea2.Height = 30;
            ContentArea3.Content = new FileSelectorUC();
        }


        private void UploadWindowVMOnDoneSelectingFiles(object sender, EventArgs eventArgs)
        {
            ContentArea1.Content = new UploadedProgressUC();
            ContentArea2.Content = null;
            ContentArea2.Visibility = Visibility.Collapsed;
            //ContentArea2.Margin = new Thickness(0.0);
            
            ContentArea3.Content = null;
            ContentArea3.Visibility = Visibility.Collapsed;

        }

        private void UploadWindowVMOnDoneUploadingAllFiles(object sender, EventArgs eventArgs)
        {
            ContentArea1.Content = new ConfirmedSentFiles();

        }


        /*
        public void StatusThread()
        {
            UploadWindowVM.ReceivedPRMGCredentials += (sender, args) 
                 => Dispatcher.Invoke((Action)(()
                => { StatusLbl.Content = "1) Received creds"; }));

            UploadWindowVM.ReceivedSecretAnswer += (sender, args) 
                 => Dispatcher.Invoke((Action)(()
                => { StatusLbl.Content = "2) Received secret answer"; }));

            UploadWindowVM.SuccessfulPRMGLogin += (sender, args)
                => Dispatcher.Invoke((Action)(()
                    => { StatusLbl.Content = "3) Logged into PRMG"; }));

            UploadWindowVM.SuccessfulImgFlowLogin += (sender, args)
                 => Dispatcher.Invoke((Action)(()
                => { StatusLbl.Content = "4) Logged into Imageflow"; }));

        }
        */
        private void UploadWindow_OnClosed(object sender, EventArgs e)
        {
            UploadWindowVM.UnloadVM();


        }
    }
}
