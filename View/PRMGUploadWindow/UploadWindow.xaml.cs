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
using ProcessorsToolkit.ViewModel.PRMG;

//using Xceed.Wpf.Toolkit.Primitives;


namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for FileSelectorWindow.xaml
    /// </summary>
    public partial class UploadWindow : Window
    {
        public UploadWindow()
        {
            DataContext = new UploadWindowVM { View = this };





            //CurrVM.CloseOwningWindow += (sender, args) => this.Close();

            InitializeComponent();


            //if (!UploadWindowVM.EventsLoaded)
             //   UploadWindowVM.LoadEvents();

            //ContentArea.Content = new FileSelectorUC();
           //if PRMG

            var uploadWindowVM = DataContext as UploadWindowVM;

            ContentArea.Content = new LoginUC(uploadWindowVM);

            uploadWindowVM.ReceivedPRMGCredentials += UploadWinVMOnReceivedPRMGCredentials;
            uploadWindowVM.HaveSecretQuestion += UploadWinVMOnHaveSecretQuestion;
            uploadWindowVM.ReceivedSecretAnswer += UploadWinVM_ReceivedSecretAnswer;
            uploadWindowVM.SuccessfulImgFlowLogin += UploadWinVMOnSuccessfulImgFlowLogin;
            uploadWindowVM.DoneSelectingFiles += UploadWindowVMOnDoneSelectingFiles;
            uploadWindowVM.DoneUploadingAllFiles += UploadWindowVMOnDoneUploadingAllFiles;

            //Task.Factory.StartNew(StatusThread);

            //var statusLbl = new Task(StatusThread);
            //statusLbl.Start();
        }

        

        private void UploadWinVMOnReceivedPRMGCredentials(object sender, EventArgs eventArgs)
        {
            //This fires out of order, has to do with event stack 
            //ContentArea.Content = null;

            /*ContentArea.Content = new Xceed.Wpf.Toolkit.BusyIndicator()
            {
                IsBusy = true,
                Content = "Connecting to PRMG..."
            };
            */
            ContentArea.Content = new BusyUC
            {
                StatusMsg = "Connecting to PRMG..."
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
            var uploadWindowVM = DataContext as UploadWindowVM;
            if (uploadWindowVM== null)
                return;
            
            ContentArea.Content = new SecretQuestionUC(uploadWindowVM);
        }

        void UploadWinVM_ReceivedSecretAnswer(object sender, EventArgs e)
        {
            /*
            ContentArea.Content = new Xceed.Wpf.Toolkit.BusyIndicator()
            {
                IsBusy = true,
                Content = "Finishing connection..."
            };*/

            ContentArea.Content = new BusyUC
            {
                StatusMsg = "Finishing connection..."
            };
        }

        private void UploadWinVMOnSuccessfulImgFlowLogin(object sender, EventArgs eventArgs)
        {
            var uploadWindowVM = DataContext as UploadWindowVM;
            if (uploadWindowVM == null)
                return;

            ContentArea.Content = new LoanAndFileSelectorUC(uploadWindowVM);
        }


        private void UploadWindowVMOnDoneSelectingFiles(object sender, EventArgs eventArgs)
        {
            var uploadWindowVM = DataContext as UploadWindowVM;
            if (uploadWindowVM == null)
                return;

            ContentArea.Content = new UploadedProgressUC(uploadWindowVM);
        }

        private void UploadWindowVMOnDoneUploadingAllFiles(object sender, EventArgs eventArgs)
        {
            var uploadWindowVM = DataContext as UploadWindowVM;
            if (uploadWindowVM == null)
                return;

            ContentArea.Dispatcher.Invoke(new Action(() =>
                {
                    ContentArea.Content = new ConfirmedSentFilesUC(uploadWindowVM);
                }));
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
            //UploadWindowVM.UnloadVM();


        }
    }
}
