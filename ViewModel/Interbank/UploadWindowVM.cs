using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using ProcessorsToolkit.Model.Interbank.UploadSession;
using ProcessorsToolkit.View.Interbank.UploadWindow;

namespace ProcessorsToolkit.ViewModel.Interbank
{
    public class UploadWindowVM
    {
        public BorrowerFileGroup WorkingFileList;
        public AvailableLoansList AllLoansAvailable;
        public LoanSearchResultItem TargetLoanItem { get { return AllLoansAvailable.FirstOrDefault(l => l.IsSelected); }}
        //public  UploadTypes? SelectedUploadType { get; set; }
        public bool AlreadyHavePTDs { get; set; }
        public UploadWindow View { get; set; }
        public Session WebsiteSession { get; set; }


        public UploadWindowVM()
        {
            ReceivedCredentials += UploadWindowVM_ReceivedCredentials;
            LoggedIntoSite += UploadWindowVM_LoggedIntoSite;
            RetrievedLoanIds += UploadWindowVM_RetrievedLoanIds;
            DoneSelectingBorrAndFiles += UploadWindowVMDoneSelectingBorrAndFiles;
            DoneFetchingConditions += UploadWindowVM_DoneFetchingConditions;
            DoneMatchingConditions += UploadWindowVM_DoneMatchingConditions;
            DoneUploadingAllFiles += UploadWindowVM_DoneUploadingAllFiles;
            
            AllLoansAvailable = new AvailableLoansList();
            WebsiteSession = new Session();
        }
        

        //*****************Step1 -- Get user and pwd, attempt login
        public event EventHandler ReceivedCredentials;
        public virtual void OnReceivedCredentials(string username, string pwd)
        {
            WebsiteSession.Username = username;
            WebsiteSession.Password = pwd;

            var handler = ReceivedCredentials;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler FetchingHomepage;

        protected virtual void OnFetchingHomepage()
        {
            var handler = FetchingHomepage;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler LoggingIn;

        protected virtual void OnLoggingIn()
        {
            var handler = LoggingIn;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler FetchingDashboard;

        protected virtual void OnFetchingDashboard()
        {
            var handler = FetchingDashboard;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        void UploadWindowVM_ReceivedCredentials(object sender, EventArgs e)
        {
            OnFetchingHomepage();

            Task.Factory.StartNew(() => WebsiteSession.Step1_FetchHomepage())
                .ContinueWith(t => OnLoggingIn(), TaskScheduler.FromCurrentSynchronizationContext())
                .ContinueWith(t => WebsiteSession.Step2_PostUsername())
                .ContinueWith(t => OnFetchingDashboard(), TaskScheduler.FromCurrentSynchronizationContext())
                .ContinueWith(t => WebsiteSession.Step3_FetchDashboard())
                .ContinueWith(task =>
                    {
                        if (WebsiteSession.IsCompleteConnection)
                            OnLoggedIntoSite();
                        //else
                       //     throw new Exception("Not logged in");
                    }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        //*****************Step2 -- Alert that logged in
        public event EventHandler LoggedIntoSite;
        protected virtual void OnLoggedIntoSite()
        {
            var handler = LoggedIntoSite;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void UploadWindowVM_LoggedIntoSite(object sender, EventArgs e)
        {
            //Task<int> task1 = Task.Factory.StartNew(o => { return ((int)o * 2); }, 3);
            //Console.WriteLine(String.Format("Task value {0}", task1.Result));

            //var startNew = Task<string>.Factory.StartNew((o) => ("holy " + o), "cow");
            //Console.WriteLine(startNew.Result);

            Task<Dictionary<string, string>>.Factory.StartNew(() =>
                {
                    WebsiteSession.Step4_GetPipeline();
                    return null;
                }
                ).ContinueWith(t => WebsiteSession.Step5_PostSearchQueries(), 0)
                .ContinueWith(task =>
                {
                    foreach (var item in task.Result.Select(loan => new LoanSearchResultItem
                    {
                        BorrLastName = loan.Value,
                        IBWLoanNum = loan.Key,
                        IsSelected = MainWindowVM.SelectedBorrDir.BorrDirName.StartsWith(loan.Value, StringComparison.InvariantCultureIgnoreCase)
                    }))
                    {
                        AllLoansAvailable.Add(item);
                    }
                    OnRetrievedLoanIds();
                }, TaskScheduler.FromCurrentSynchronizationContext());

            


            /*var loanIdsNames = new Dictionary<string, string>();
            Task.Factory.StartNew(() =>
                {
                    WebsiteSession.Step4_GetPipeline();
                    loanIdsNames = WebsiteSession.Step5_PostSearchQueries();

                }).ContinueWith(task =>
                    {

                        foreach (var item in loanIdsNames.Select(loan => new LoanSearchResultItem
                            {
                                BorrLastName = loan.Value,
                                IBWLoanNum = loan.Key,
                                IsSelected = MainWindowVM.SelectedBorrDir.BorrDirName.StartsWith(loan.Value,StringComparison.InvariantCultureIgnoreCase)
                            }))
                        {
                            AllLoansAvailable.Add(item);
                        }
                        OnRetrievedLoanIds();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
             */
        }

        //*****************Step3 -- Retrieved all loans
        public event EventHandler RetrievedLoanIds;
        protected virtual void OnRetrievedLoanIds()
        {
            var handler = RetrievedLoanIds;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        void UploadWindowVM_RetrievedLoanIds(object sender, EventArgs e)
        {
            
        }

        //*****************Step4 -- Done selecting files
        public event EventHandler DoneSelectingBorrAndFiles;
        protected internal virtual void OnDoneSelectingBorrAndFiles()
        {
            var handler = DoneSelectingBorrAndFiles;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void UploadWindowVMDoneSelectingBorrAndFiles(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
                {
                    WebsiteSession.FillLoanConditions(TargetLoanItem);
                }).ContinueWith(task => OnDoneFetchingConditions(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        //*****************Step5 -- Done downloading conditions for requested loan
        public event EventHandler DoneFetchingConditions;
        protected virtual void OnDoneFetchingConditions()
        {
            var handler = DoneFetchingConditions;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        void UploadWindowVM_DoneFetchingConditions(object sender, EventArgs e)
        {

        }

        //*****************Step6 -- Done matching 
        public event EventHandler DoneMatchingConditions;
        protected internal virtual void OnDoneMatchingConditions()
        {
            var handler = DoneMatchingConditions;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        void UploadWindowVM_DoneMatchingConditions(object sender, EventArgs e)
        {
            WorkingFileList.StartQueue();
        }


        //*****************Step7 -- Uploaded single file events
        public event EventHandler DoneUploadingSingleFile;
        protected internal virtual void OnDoneUploadingSingleFile()
        {
            var handler = DoneUploadingSingleFile;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        //*****************Step8 -- Done uploading all, display results window
        public event EventHandler DoneUploadingAllFiles;
        protected internal virtual void OnDoneUploadingAllFiles()
        {
            var handler = DoneUploadingAllFiles;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        void UploadWindowVM_DoneUploadingAllFiles(object sender, EventArgs e)
        {

        }

        

        



    }
}
