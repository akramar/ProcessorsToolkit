using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using ProcessorsToolkit.ViewModel;
using ProcessorsToolkit.ViewModel.Interbank;

namespace ProcessorsToolkit.Model.Interbank.UploadSession
{
    public class BorrowerFileGroup : ObservableCollection<FileToUpload>
    {

        public event EventHandler FileListHasChanged;
        public event EventHandler FileListHasCompleted;
        //private List<Tuple<string, string>> _formValsTemplate; //this is passing in more than we need
        private UploadWindowVM _parentVM { get; set; }
        

        //Passing in the VM seems wrong here
        public BorrowerFileGroup(UploadWindowVM parentVM)
        {
            _parentVM = parentVM;

            FileListHasChanged += (sender, args) => _parentVM.OnDoneUploadingSingleFile();
            FileListHasCompleted += (sender, args) => _parentVM.OnDoneUploadingAllFiles();
        }

        public BorrowerFileGroup LoadAllFiles()
        {
            if (Count > 0)
                Clear();

            foreach (var filePath in Directory.GetFiles(MainWindowVM.SelectedBorrDir.FullActivePath))
            {
                if (filePath.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                    Add(new FileToUpload
                        {
                            UploadProgress = FileToUpload.FileUploadStages.Unstarted,
                            PathFull = filePath,
                            TimeAdded = DateTime.UtcNow,
                            //MatchedLoan = UploadWindowVM.TargetLoanItem,
                            IsSelected = false
                        });
            }
            return this;
        }

        public void StartQueue()
        {
            Task.Factory.StartNew(FileWorker)
                .ContinueWith(task => _parentVM.OnDoneUploadingAllFiles(),
                              TaskScheduler.FromCurrentSynchronizationContext());
        }
        public void FileWorker()
        {
            //_formVals.Clear();
            //This is a kind of messy conversion from Dictionary to non-unique tuples for upload form
            //foreach (var pair in _parentVM.WebsiteSession.FormVals)
            //    _formVals.Add(new Tuple<string, string>(pair.Key, pair.Value));
            

            foreach (
                var file in this.Where(
                    f => f.UploadProgress == FileToUpload.FileUploadStages.Unstarted
                    && f.IsSelected))
            {

                var formVals = ConvertFormForUpload(file.AttachedLoanCondition,
                                                    _parentVM.WebsiteSession.FormVals, file.NameNoExt);
                file.UploadProgress = FileToUpload.FileUploadStages.Started;
                var listUpdatedEvent = FileListHasChanged;
                if (listUpdatedEvent != null)
                    listUpdatedEvent(this, null);

                int uploadresponsecode =
                    SingleFileUpload.HttpUploadFile(
                        file.PathFull,
                        formVals,
                        _parentVM.WebsiteSession.SessionCookies,
                        _parentVM.TargetLoanItem.IBWLoanNum);

                file.UploadProgress = uploadresponsecode == 1
                                          ? FileToUpload.FileUploadStages.Completed
                                          : FileToUpload.FileUploadStages.Failed;

                if (listUpdatedEvent != null)
                    listUpdatedEvent(this, null);

            }
            var listCompletedEvent = FileListHasCompleted;
            if (listCompletedEvent != null)
                listCompletedEvent(this, null);
        }

        private static List<Tuple<string, string>> ConvertFormForUpload(LoanCondition condition,
                                                                        Dictionary<string, string> formVals,
                                                                        string fileNameNoExt)
        {
            //var filePath = @"C:\Users\Alain Kramar\Documents\Loans\Morgan\conditions\108 Morgan - VOE (Olivia) complete.pdf";
            var viewState = System.Web.HttpUtility.UrlDecode(formVals.First(v => v.Key == "__VIEWSTATE").Value);
            var eventValidation = System.Web.HttpUtility.UrlDecode(formVals.First(v => v.Key == "__EVENTVALIDATION").Value);

            var formItems = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("ctl00_ScriptManager1_TSM", ""),
                    new Tuple<string, string>("__EVENTTARGET", ""),
                    new Tuple<string, string>("__EVENTARGUMENT", ""),
                    new Tuple<string, string>("__VIEWSTATE", viewState),
                    new Tuple<string, string>("__EVENTVALIDATION", eventValidation),
                    new Tuple<string, string>("ctl00_mainMenu_ClientState", ""),
                    new Tuple<string, string>("ctl00_BreadCrumbSiteMap_ClientState", ""),
                    new Tuple<string, string>("ctl00_ContentPlaceHolder1_RadProgressManager1_ClientState", ""),
                    new Tuple<string, string>("ctl00$ContentPlaceHolder1$textBoxTitle", fileNameNoExt),
                    new Tuple<string, string>("ctl00_ContentPlaceHolder1_RadProgressArea1_ClientState", ""),
                    new Tuple<string, string>("ctl00$ContentPlaceHolder1$buttonUploadDocument", "Upload Document"),
                    new Tuple<string, string>("ctl00_ContentPlaceHolder1_buttonUploadDocument_ClientState", ""),
                    //new Tuple<string, string>("ctl00$ContentPlaceHolder1$gridViewConditions$ctl00$ctl06$checkBoxSelectCondition", "on"),//this is a condition checkbox
                    new Tuple<string, string>(condition.CheckboxName, "on"),
                    new Tuple<string, string>("ctl00_ContentPlaceHolder1_gridViewConditions_ClientState", ""),
                    new Tuple<string, string>("ctl00_radToolTip_ClientState", ""),
                    new Tuple<string, string>("ctl00$textBoxSearch", "")
                };


            //var morganLoanNum = "88284420";
            
            //var uploadCode = new SingleFileUpload();

            //uploadCode.HttpUploadFile(filePath, formItems, SessionCookies, morganLoanNum);

            return formItems;
        }
    }
}
