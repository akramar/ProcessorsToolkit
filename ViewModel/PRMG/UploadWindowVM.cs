using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ProcessorsToolkit.Model.Common.UploadSession;
using ProcessorsToolkit.Model.PRMG.UploadSession;
using ProcessorsToolkit.View.PRMGUploadWindow;
//using System.Net;

namespace ProcessorsToolkit.ViewModel.PRMG
{
    public class UploadWindowVM
    {
        public  string ImgFlowSessionKey { get; set; }
        public  string ImgFlowContainerKey { get; set; }
        public CookieCollection DocuTracCookies { get; set; }

        public  BorrowerFileGroup WorkingFileList;
        public  PRMGLoginSession CurrPRMGSession { get; set; }
        public  ImageFlowSession CurrImgFlowSession { get; set; }
        public  AvailableLoansList AllLoansAvailable;
        public  LoanSearchResultItem TargetLoanItem { get; set; }
        public  UploadTypes? SelectedUploadType { get; set; }
        //public  bool AlreadyHavePTDs { get; set; }
        //public static UploadWindow SpawnedWindow { get; set; } 
        //public static bool EventsLoaded { get; set; }

        public UploadWindow View { get; set; }
        
        
        
        //public static CookieContainer SessionCookies = new CookieContainer();

        //public event EventHandler CloseOwningWindow;

        public enum UploadTypes
        {
            Submission,
            PTDConditions
        }
        

        //public static void LoadEvents()
        public UploadWindowVM()
        {
            ReceivedPRMGCredentials += UploadWindowVM_ReceivedPRMGCredentials;
            HaveSecretQuestion += UploadWindowVM_HaveSecretQuestion;
            ReceivedSecretAnswer += UploadWindowVM_ReceivedSecretAnswer;
            SuccessfulPRMGLogin += UploadWindowVM_SuccessfulPRMGLogin;
            SuccessfulImgFlowLogin += UploadWindowVM_SuccessfulImgFlowLogin;
            LoanListLoaded += UploadWindowVM_LoanListLoaded;
            DoneSelectingFiles += UploadWindowVM_DoneSelectingFiles;
            //Don't really like this being called twice
            //WorkingFileList.FileListHasChanged += OnDoneUploadingSingleFile(); 
            DoneUploadingSingleFile += UploadWindowVM_DoneUploadingSingleFile;
            DoneUploadingAllFiles += UploadWindowVM_DoneUploadingAllFiles;
            //EventsLoaded = true;
        }

        /*
        public static void UnloadVM()
        {
            CurrPRMGSession.LogOutSession();
            //UnloadEvents();
            TargetLoanItem = null;
            AlreadyHavePTDs = false;
            CurrImgFlowSession = null;
            CurrPRMGSession = null;
            SelectedUploadType = null;
            ImgFlowContainerKey = null;
            ImgFlowSessionKey = null;
            AllLoansAvailable = null;
        }
        */
        /*
        private static void UnloadEvents() //this is not great
        {
            ReceivedPRMGCredentials -= UploadWindowVM_ReceivedPRMGCredentials;
            HaveSecretQuestion -= UploadWindowVM_HaveSecretQuestion;
            ReceivedSecretAnswer -= UploadWindowVM_ReceivedSecretAnswer;
            SuccessfulPRMGLogin -= UploadWindowVM_SuccessfulPRMGLogin;
            SuccessfulPRMGLogin -= UploadWindowVM_SuccessfulPRMGLogin;
            SuccessfulImgFlowLogin -= UploadWindowVM_SuccessfulImgFlowLogin;
            DoneSelectingFiles -= UploadWindowVM_DoneSelectingFiles;
            DoneUploadingAllFiles -= UploadWindowVM_DoneUploadingAllFiles;
        }
        */
        

        //*****************Step1 -- Get user and pwd, attempt login

        public event EventHandler ReceivedPRMGCredentials;
        public void OnReceivedPRMGCredentials(string username, string pwd)
        {
            CurrPRMGSession = new PRMGLoginSession { Username = username, Password = pwd };

            var handler = ReceivedPRMGCredentials;
            if (handler != null)
                handler(null, EventArgs.Empty);
        }

        private void UploadWindowVM_ReceivedPRMGCredentials(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
                {
                    CurrPRMGSession.Step1_FetchHomepage();
                    CurrPRMGSession.Step2_PostUsername();
                    CurrPRMGSession.Step3_ChallengeQuestion();
                }).ContinueWith(task =>
                                OnHaveSecretQuestion(),
                                TaskScheduler.FromCurrentSynchronizationContext());
        }

        //*****************Step2 - have secret question 
        public event EventHandler HaveSecretQuestion;
        public void OnHaveSecretQuestion()
        {
            var handler = HaveSecretQuestion;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private void UploadWindowVM_HaveSecretQuestion(object sender, EventArgs e)
        {
            
        }

        //*****************Step2.5 - have secret answer 
        public event EventHandler ReceivedSecretAnswer;
        public void OnReceivedSecretAnswer()
        {
            var handler = ReceivedSecretAnswer;
            if (handler != null)
                handler(null, EventArgs.Empty);
        }

        private void UploadWindowVM_ReceivedSecretAnswer(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
                {
                    if (!String.IsNullOrEmpty(CurrPRMGSession.SecretQuestionAnswer))
                    {
                        //TODO: we need to check result of each step to make sure still on track
                        CurrPRMGSession.Step4_PostSecretAnswer();
                        CurrPRMGSession.Step6_PostLoginPassword();
                        CurrPRMGSession.Step8_GetImageFlowLaunchData();
                        DocuTracCookies = CurrPRMGSession.Step9_InitiateDocuTrac();
                        var dtSess = new DocuTracConnection();
                        dtSess.Blah(DocuTracCookies, "http://imageflow.prmg.net:443", "215743");
                        ImgFlowSessionKey = CurrPRMGSession.ImgFlowSessionKey;

                    }
                }).ContinueWith(task =>
                                OnSuccessfulPRMGLogin(), TaskScheduler.FromCurrentSynchronizationContext());

            // http://reedcopsey.com/2010/04/19/parallelism-in-net-part-17-think-continuations-not-callbacks/
        }


        //*****************Step3 - Log into PRMG

        public event EventHandler SuccessfulPRMGLogin;
        public void OnSuccessfulPRMGLogin()
        {
            var handler = SuccessfulPRMGLogin;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private void UploadWindowVM_SuccessfulPRMGLogin(object sender, EventArgs e)
        {
            CurrImgFlowSession = new ImageFlowSession();

            Task.Factory.StartNew(() =>
                {
                    CurrImgFlowSession.Step1_FetchImageFlowLogin(ImgFlowSessionKey);
                    CurrImgFlowSession.Step2_WeShouldBeDoneByHere();
                }).ContinueWith(task => 
                    OnSuccessfulImgFlowLogin(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        //*****************Step4 - Log in to ImageFlow

        public event EventHandler SuccessfulImgFlowLogin;
        public void OnSuccessfulImgFlowLogin()
        {
            var handler = SuccessfulImgFlowLogin;
            if (handler != null) handler(null, EventArgs.Empty);
        }
        private void UploadWindowVM_SuccessfulImgFlowLogin(object sender, EventArgs e)
        {
            /*
            Task.Factory.StartNew(() =>
            {

            }).ContinueWith(task =>
                    , 
                    TaskScheduler.FromCurrentSynchronizationContext());*/
        }


        //*****************Step5 - successful loading of loans
        public event EventHandler LoanListLoaded;
        public void OnLoanListLoaded()
        {
            var handler = LoanListLoaded;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private void UploadWindowVM_LoanListLoaded(object sender, EventArgs e)
        {
        }


        //*****************Step6 -- Done selecting files
        public event EventHandler DoneSelectingFiles;
        public void OnDoneSelectingFiles()
        {
            var handler = DoneSelectingFiles;
            if (handler != null) handler(null, EventArgs.Empty);
        }
        private void UploadWindowVM_DoneSelectingFiles(object sender, EventArgs e)
        {
            WorkingFileList.StartQueue();
        }


        //*****************Step8 - Done uploading A file
        public event EventHandler DoneUploadingSingleFile;
        public void OnDoneUploadingSingleFile()
        {
            var handler = DoneUploadingSingleFile;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private void UploadWindowVM_DoneUploadingSingleFile(object sender, EventArgs e)
        {



        }


        //*****************Step8 - Done uploading all files
        public event EventHandler DoneUploadingAllFiles;
        public void OnDoneUploadingAllFiles()
        {
            var handler = DoneUploadingAllFiles;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private void UploadWindowVM_DoneUploadingAllFiles(object sender, EventArgs e)
        {



        }

    }
}
