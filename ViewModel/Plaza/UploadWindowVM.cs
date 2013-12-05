using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using ProcessorsToolkit.Model.Plaza.UploadSession;
using ProcessorsToolkit.View.Plaza.UploadWindow;

namespace ProcessorsToolkit.ViewModel.Plaza
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
        public string NotifEmail { get; set; }


        public UploadWindowVM()
        {
            ReceivedCredentials += UploadWindowVM_ReceivedCredentials;
            LoggedIntoSite += UploadWindowVM_LoggedIntoSite;
            FailedLoginToSite += UploadWindowVM_FailedLoginToSite;
            RetrievedLoans += UploadWindowVM_RetrievedLoans;
            DoneSelectingBorrAndFiles += UploadWindowVM_DoneSelectingBorrAndFiles;
            DoneGettingUploadCredentials += UploadWindowVM_OnDoneGettingUploadCredentials;
            DoneGettingUploadPage += UploadWindowVM_OnDoneGettingUploadPage;
            //DoneFetchingConditions += UploadWindowVM_DoneFetchingConditions;
            //DoneMatchingConditions += UploadWindowVM_DoneMatchingConditions;
            DoneUploadingAllFiles += UploadWindowVM_DoneUploadingAllFiles;
            
            AllLoansAvailable = new AvailableLoansList();
            WebsiteSession = new Session();
        }



        //***************** Events
        public event EventHandler ReceivedCredentials;
        public virtual void OnReceivedCredentials(string companyId, string userId, string pwd, string email)
        {
            WebsiteSession.CompanyId = companyId;
            WebsiteSession.UserId = userId;
            WebsiteSession.Password = pwd;
            NotifEmail = email;

            var handler = ReceivedCredentials;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler GettingHomepage;
        protected virtual void OnGettingHomepage()
        {
            var handler = GettingHomepage;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler PostingLogin;
        protected virtual void OnPostingLogin()
        {
            var handler = PostingLogin;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        
        public event EventHandler LoggedIntoSite;
        protected virtual void OnLoggedIntoSite()
        {
            var handler = LoggedIntoSite;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler FailedLoginToSite;
        protected virtual void OnFailedLoginToSite()
        {
            var handler = FailedLoginToSite;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler GettingPipeline;
        protected virtual void OnGettingPipeline()
        {
            var handler = GettingPipeline;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler RetrievedLoans;
        protected virtual void OnRetrievedLoans()
        {
            var handler = RetrievedLoans;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler DoneSelectingBorrAndFiles;
        protected internal virtual void OnDoneSelectingBorrAndFiles()
        {
            var handler = DoneSelectingBorrAndFiles;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler DoneGettingUploadCredentials;
        protected virtual void OnDoneGettingUploadCredentials()
        {
            var handler = DoneGettingUploadCredentials;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler DoneGettingUploadPage;
        protected virtual void OnDoneGettingUploadPage()
        {
            var handler = DoneGettingUploadPage;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        /*
        public event EventHandler DoneMatchingConditions;
        protected internal virtual void OnDoneMatchingConditions()
        {
            var handler = DoneMatchingConditions;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        */
        public event EventHandler DoneUploadingSingleFile;
        protected internal virtual void OnDoneUploadingSingleFile()
        {
            var handler = DoneUploadingSingleFile;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler DoneUploadingAllFiles;
        protected internal virtual void OnDoneUploadingAllFiles()
        {
            var handler = DoneUploadingAllFiles;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        //***************** Upload steps

        //*****************Step1 -- Credentials entered and login button clicked
        void UploadWindowVM_ReceivedCredentials(object sender, EventArgs e)
        {
            OnGettingHomepage();

            Task.Factory.StartNew(() => WebsiteSession.Step1_GetHomepage())
                .ContinueWith(t =>
                    {
                        OnPostingLogin();
                    }, TaskScheduler.FromCurrentSynchronizationContext())
                .ContinueWith(t =>
                    {
                        WebsiteSession.Step2_PostLogin();
                    })
                .ContinueWith(t =>
                    {
                        if (WebsiteSession.IsCompleteConnection)
                            OnLoggedIntoSite();
                        else
                            OnFailedLoginToSite();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        
        //*****************Step2 -- Alert that logged in or failed
        /*
        private async Task<int> GetWebPageHtmlSizeAsync()
        {
            var client = new HttpClient();
            var html = await client.GetAsync("http://www.example.com/");
            return html.Length;
        }
        *
         */

        


        private void UploadWindowVM_LoggedIntoSite(object sender, EventArgs e)
        {
            OnGettingPipeline();

            //Task<int> task1 = Task.Factory.StartNew(o => { return ((int)o * 2); }, 3);
            //Console.WriteLine(String.Format("Task value {0}", task1.Result));

            //var startNew = Task<string>.Factory.StartNew((o) => ("holy " + o), "cow");
            //Console.WriteLine(startNew.Result);

            //<!-- // need to return loan list from GetPipeline()

            //var t = new Thread(() => GetPipeline());
            //t.Start();


            var bw = new BackgroundWorker();
            bw.DoWork += (o, args) => { args.Result = WebsiteSession.Step3_GetPipeline(); };
            bw.RunWorkerCompleted += (o, args) =>
                {
                    AllLoansAvailable = (AvailableLoansList) args.Result;
                    OnRetrievedLoans();
                };
            bw.RunWorkerAsync();


            /*
            Task<AvailableLoansList>.Factory.StartNew(() =>
                {
                   return GetPipeline();
                }, TaskCreationOptions.LongRunning).ContinueWith(t =>
                    {
                        AllLoansAvailable = t.Result;
                        OnRetrievedLoans();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
             */
        }

        /*
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

            

        */
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
        

        private void UploadWindowVM_FailedLoginToSite(object sender, EventArgs e)
        {
            
        }

        //*****************Step3 -- Retrieved all loans
        void UploadWindowVM_RetrievedLoans(object sender, EventArgs e)
        {
            
        }

        //*****************Step4 -- Done selecting files
        

        private void UploadWindowVM_DoneSelectingBorrAndFiles(object sender, EventArgs e)
        {
            var appNum = String.Copy(TargetLoanItem.AppNum);
            Task.Factory.StartNew(() => WebsiteSession.Step4_GetUploadCredentials(appNum))
                .ContinueWith(task => OnDoneGettingUploadCredentials(), TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void UploadWindowVM_OnDoneGettingUploadCredentials(object sender, EventArgs eventArgs)
        {
            var appNum = String.Copy(TargetLoanItem.AppNum);
            var loanNum = String.Copy(TargetLoanItem.LoanNum);
            
            var bw = new BackgroundWorker();
            bw.DoWork += (o, args) => { args.Result = WebsiteSession.Step5_GetUploadPage(appNum, loanNum); };
            bw.RunWorkerCompleted += (o, args) =>
            {
                WebsiteSession.ViewState = ((Tuple<string,string>)args.Result).Item1;
                WebsiteSession.EventValidation = ((Tuple<string,string>)args.Result).Item2;
                OnDoneGettingUploadPage();
            };

            bw.RunWorkerAsync();
        }

        //*****************Step5 -- Done downloading conditions for requested loan


        private void UploadWindowVM_OnDoneGettingUploadPage(object sender, EventArgs eventArgs)
        {/*
            Task.Factory.StartNew(() =>
                {
                    lock (WorkingFileList)
                    {
                        WorkingFileList.StartQueue();
                    }
                })
                .ContinueWith(t => OnDoneUploadingAllFiles(), TaskScheduler.FromCurrentSynchronizationContext());
            */
            WorkingFileList.StartQueue();

        }


        void UploadWindowVM_DoneFetchingConditions(object sender, EventArgs e)
        {

        }

        //*****************Step6 -- Done matching 
        

        void UploadWindowVM_DoneMatchingConditions(object sender, EventArgs e)
        {
            //WorkingFileList.StartQueue();
        }


        //*****************Step7 -- Uploaded single file events
        


        //*****************Step8 -- Done uploading all, display results window
        

        void UploadWindowVM_DoneUploadingAllFiles(object sender, EventArgs e)
        {

        }

        

        



    }
}
