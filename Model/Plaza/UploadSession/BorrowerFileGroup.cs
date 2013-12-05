using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using ProcessorsToolkit.Model.Common.UploadSession;
using ProcessorsToolkit.ViewModel;
using ProcessorsToolkit.ViewModel.Plaza;

namespace ProcessorsToolkit.Model.Plaza.UploadSession
{
    public class BorrowerFileGroup : ObservableCollection<FileToUpload>
    {
        public event EventHandler FileListHasChanged;
        public event EventHandler FileListHasCompleted;
        private UploadWindowVM ParentVM { get; set; }

        //Passing in the VM seems wrong here
        public BorrowerFileGroup(UploadWindowVM parentVM)
        {
            ParentVM = parentVM;

            FileListHasChanged += (sender, args) => ParentVM.OnDoneUploadingSingleFile();
            FileListHasCompleted += (sender, args) => ParentVM.OnDoneUploadingAllFiles();
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
                            IsSelected = false
                        });
            }
            return this;
        }

        public void StartQueue()
        {

            var bw = new BackgroundWorker();
            bw.DoWork += (o, args) => FileWorker();
            bw.RunWorkerCompleted += (o, args) =>
                {
                    //ParentVM.OnDoneUploadingAllFiles(); //We're already doing this
                };

            bw.RunWorkerAsync();


            
            //FileWorker();
        }
        private void FileWorker()
        {
            //_formVals.Clear();
            //This is a kind of messy conversion from Dictionary to non-unique tuples for upload form
            //foreach (var pair in _parentVM.WebsiteSession.FormVals)
            //    _formVals.Add(new Tuple<string, string>(pair.Key, pair.Value));
            

            foreach (var file in this.Where(f => f.UploadProgress == FileToUpload.FileUploadStages.Unstarted
                    && f.IsSelected))
            {

                //var formVals = ConvertFormForUpload(file.AttachedLoanCondition,
                //                                    _parentVM.WebsiteSession.FormVals, file.NameNoExt);
                
                var form = new MultipartUploadForm
                    {
                        new UploadFormField(ContentTypes.None) {Name = "__EVENTTARGET", Body = "ctl00$btn_Upload"},
                        new UploadFormField(ContentTypes.None) {Name = "__EVENTARGUMENT", Body = ""},
                        new UploadFormField(ContentTypes.None) {Name = "__LASTFOCUS", Body = ""},
                        new UploadFormField(ContentTypes.None) {Name = "__VIEWSTATE", Body = ParentVM.WebsiteSession.ViewState},
                        new UploadFormField(ContentTypes.None) {Name = "__EVENTVALIDATION", Body = ParentVM.WebsiteSession.EventValidation},
                        new UploadFormField(ContentTypes.None) {Name = "ctl00$TextBox1", Body = ParentVM.TargetLoanItem.LoanNum}, //4813110236
                        new UploadFormField(ContentTypes.None) {Name = "ctl00$DropDownList1", Body = "Loan Submission"},
                        new UploadFormField(ContentTypes.None) {Name = "ctl00$EMailR", Body = ParentVM.NotifEmail},
                        new UploadFormField(ContentTypes.OctetStream) {Name = "ctl00$File2", Filename = "", Body = ""},
                        new UploadFormField(ContentTypes.OctetStream) {Name = "ctl00$File3", Filename = "", Body = ""},
                        new UploadFormField(ContentTypes.OctetStream) {Name = "ctl00$File4", Filename = "", Body = ""},
                        new UploadFormField(ContentTypes.OctetStream) {Name = "ctl00$File5", Filename = "", Body = ""},
                        new UploadFormField(ContentTypes.OctetStream) {Name = "ctl00$File6", Filename = "", Body = ""},
                        new UploadFormField(ContentTypes.OctetStream) {Name = "ctl00$File7", Filename = "", Body = ""},
                        new UploadFormField(ContentTypes.OctetStream) {Name = "ctl00$File8", Filename = "", Body = ""},
                        new UploadFormField(ContentTypes.OctetStream) {Name = "ctl00$File9", Filename = "", Body = ""},
                        new UploadFormField(ContentTypes.OctetStream) {Name = "ctl00$File10", Filename = "", Body = ""}
                    };
                
                file.UploadProgress = FileToUpload.FileUploadStages.Started;
                var listUpdatedEvent = FileListHasChanged;
                if (listUpdatedEvent != null)
                    listUpdatedEvent(this, null);

                var postingUrl = "https://docutrac.plazahomemortgage.com:8080/brokeruploadpage/Default.aspx?doctracfn=DOCUPLOADLS&loannumber=" + ParentVM.TargetLoanItem.LoanNum;
                
                var uploadresponsecode =
                    SingleFileUpload.HttpUploadFile(postingUrl, file.PathFull, form,
                    ParentVM.WebsiteSession.SessionCookies, "ctl00$File1");

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

        
    }
}
