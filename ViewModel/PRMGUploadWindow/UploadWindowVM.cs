using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
//using ImageFlowUploader;
using ProcessorsToolkit.Model.PRMG.UploadSession;
using ProcessorsToolkit.Model.UploadSession.PRMG;
using ProcessorsToolkit.View.PRMGUploadWindow;

namespace ProcessorsToolkit.ViewModel.PRMGUploadWindow
{
    public static class UploadWindowVM
    {
        public static string ImgFlowSessionKey { get; set; }
        public static string ImgFlowContainerKey { get; set; }

        public static BorrowerFileGroup WorkingFileList;
        public static PRMGLoginSession CurrPRMGSession { get; set; }
        public static ImageFlowSession CurrImgFlowSession { get; set; }
        public static AvailableLoansList AllLoansAvailable;
        public static LoanSearchResultItem TargetLoanItem { get; set; }
        public static UploadTypes? SelectedUploadType { get; set; }
        public static bool AlreadyHavePTDs { get; set; }
        public static MainWindow SpawnedWindow { get; set; } 
        public static Dictionary<string, string> DocSchemaIds = new Dictionary<string, string>
        {            
            // Name the "Conditions" doctype just "Other PTD/PTF"
            //{"209638", "Submission Pkg"}, {"209654", "Conditions"},
            {"209638", "-1"}, {"209654", "0"},
            {"209681", "1"}, {"209684", "2"}, {"209685", "3"}, {"209686", "4"}, {"209687", "5"}, {"209688", "6"},
            {"209689", "7"}, {"209690", "8"}, {"209691", "9"}, {"209692", "10"}, {"209693", "11"}, {"209694", "12"},
            {"209695", "13"}, {"209696", "14"}, {"209697", "15"}, {"209698", "16"}, {"209699", "17"}, {"209700", "18"},
            {"209701", "19"}, {"209702", "20"}, {"209703", "21"}, {"209704", "22"}, {"209705", "23"}, {"209706", "24"},
            {"209707", "25"}, {"209708", "26"}, {"209709", "27"}, {"209710", "28"}, {"209711", "29"}, {"209712", "30"},
            {"209713", "31"}, {"209714", "32"}, {"209715", "33"}, {"209716", "34"}, {"209717", "35"}, {"209718", "36"},
            {"209719", "37"}, {"209720", "38"}, {"209721", "39"}, {"209722", "40"}, {"209733", "41"}, {"209734", "42"},
            {"209735", "43"}, {"209736", "44"}, {"209737", "45"}, {"209738", "46"}, {"209739", "47"}, {"209740", "48"},
            {"209741", "49"}, {"209742", "50"}, {"209743", "51"}, {"209744", "52"}, {"209745", "53"}, {"209746", "54"},
            {"209747", "55"}
        };
        
        //public static CookieContainer SessionCookies = new CookieContainer();

        //public event EventHandler CloseOwningWindow;

        public enum UploadTypes
        {
            Submission,
            PTDConditions
        }


        

        public static void LoadEvents()
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

        }


        public static void UnloadVM()
        {
            CurrPRMGSession.LogOutSession();
            UnloadEvents();
            TargetLoanItem = null;
            AlreadyHavePTDs = false;
            CurrImgFlowSession = null;
            CurrPRMGSession = null;
            SelectedUploadType = null;
            ImgFlowContainerKey = null;
            ImgFlowSessionKey = null;
            AllLoansAvailable = null;
        }

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

        

        //*****************Step1 -- Get user and pwd, attempt login

        public static event EventHandler ReceivedPRMGCredentials;
        public static void OnReceivedPRMGCredentials(string username, string pwd)
        {
            if (CurrPRMGSession == null)
                CurrPRMGSession = new PRMGLoginSession();

            CurrPRMGSession.Username = username;
            CurrPRMGSession.Password = pwd;

            var handler = ReceivedPRMGCredentials;
            if (handler != null)
                handler(null, EventArgs.Empty);
        }

        private static void UploadWindowVM_ReceivedPRMGCredentials(object sender, EventArgs e)
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
        public static event EventHandler HaveSecretQuestion;
        public static void OnHaveSecretQuestion()
        {
            var handler = HaveSecretQuestion;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private static void UploadWindowVM_HaveSecretQuestion(object sender, EventArgs e)
        {
            
        }

        //*****************Step2.5 - have secret answer 
        public static event EventHandler ReceivedSecretAnswer;
        public static void OnReceivedSecretAnswer()
        {
            var handler = ReceivedSecretAnswer;
            if (handler != null) 
                handler(null, EventArgs.Empty);
        }

        private static void UploadWindowVM_ReceivedSecretAnswer(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
                {
                    if (!String.IsNullOrEmpty(CurrPRMGSession.SecretQuestionAnswer))
                    {
                        CurrPRMGSession.Step4_PostSecretAnswer();
                        CurrPRMGSession.Step6_PostLoginPassword();
                        CurrPRMGSession.Step8_GetImageFlowLaunchData();
                    }
                }).ContinueWith(task =>
                                OnSuccessfulPRMGLogin(), TaskScheduler.FromCurrentSynchronizationContext());

            // http://reedcopsey.com/2010/04/19/parallelism-in-net-part-17-think-continuations-not-callbacks/

        }


        //*****************Step3 - Log into PRMG

        public static event EventHandler SuccessfulPRMGLogin;

        public static void OnSuccessfulPRMGLogin()
        {
            var handler = SuccessfulPRMGLogin;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private static void UploadWindowVM_SuccessfulPRMGLogin(object sender, EventArgs e)
        {
            if (CurrImgFlowSession == null)
                CurrImgFlowSession = new ImageFlowSession();

            Task.Factory.StartNew(() =>
                {
                    CurrImgFlowSession.Step1_FetchImageFlowLogin();
                    CurrImgFlowSession.Step2_WeShouldBeDoneByHere();

                }).ContinueWith(task => 
                    OnSuccessfulImgFlowLogin(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        //*****************Step4 - Log in to ImageFlow

        public static event EventHandler SuccessfulImgFlowLogin;

        public static void OnSuccessfulImgFlowLogin()
        {
            var handler = SuccessfulImgFlowLogin;
            if (handler != null) handler(null, EventArgs.Empty);
        }
        private static void UploadWindowVM_SuccessfulImgFlowLogin(object sender, EventArgs e)
        {
            /*
            Task.Factory.StartNew(() =>
            {

            }).ContinueWith(task =>
                    , 
                    TaskScheduler.FromCurrentSynchronizationContext());*/
        }


        //*****************Step5 - successful loading of loans
        public static event EventHandler LoanListLoaded;
        public static void OnLoanListLoaded()
        {
            var handler = LoanListLoaded;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private static void UploadWindowVM_LoanListLoaded(object sender, EventArgs e)
        {
        }


        //*****************Step6 -- Done selecting files
        public static event EventHandler DoneSelectingFiles;
        public static void OnDoneSelectingFiles()
        {
            var handler = DoneSelectingFiles;
            if (handler != null) handler(null, EventArgs.Empty);
        }
        private static void UploadWindowVM_DoneSelectingFiles(object sender, EventArgs e)
        {
            WorkingFileList.StartQueue();


        }


        //*****************Step8 - Done uploading A file
        public static event EventHandler DoneUploadingSingleFile;
        public static void OnDoneUploadingSingleFile()
        {
            var handler = DoneUploadingSingleFile;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private static void UploadWindowVM_DoneUploadingSingleFile(object sender, EventArgs e)
        {



        }


        //*****************Step8 - Done uploading all files
        public static event EventHandler DoneUploadingAllFiles;
        public static void OnDoneUploadingAllFiles()
        {
            var handler = DoneUploadingAllFiles;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private static void UploadWindowVM_DoneUploadingAllFiles(object sender, EventArgs e)
        {



        }

    }
}
