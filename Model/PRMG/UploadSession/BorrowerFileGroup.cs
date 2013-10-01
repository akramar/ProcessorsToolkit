using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;
using System.Xml;
//using ImageFlowUploader;
using ProcessorsToolkit.Model.PRMG.UploadSession;
using ProcessorsToolkit.Model.UploadSession.PRMG;
using ProcessorsToolkit.ViewModel;
using ProcessorsToolkit.ViewModel.PRMGUploadWindow;


namespace ProcessorsToolkit.Model.PRMG.UploadSession
{
    public class BorrowerFileGroup : ObservableCollection<FileToUpload>
        // System.Collections.Concurrent.BlockingCollection<FileToUpload> //  List<FileToUpload>
    {
        //public delegate void FileListChanged(object s, EventArgs e);
        public event EventHandler FileListHasChanged;
        //public delegate void ListCompleted(object s, EventArgs e);
        public event EventHandler FileListHasCompleted;
        private List<Tuple<string, string>> _formVals;
        //public string BorrName { get; set; }
        //private UploadWindowVM _parentVM;


        public BorrowerFileGroup() //UploadWindowVM parentVM)
        {
            //_parentVM = parentVM;

            //FileListHasChanged += UploadWindowVM.DoneUploadingSingleFile;
            FileListHasChanged += (sender, args) => UploadWindowVM.OnDoneUploadingSingleFile();
            FileListHasCompleted += (sender, args) => UploadWindowVM.OnDoneUploadingAllFiles();
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
            //string postUrl = "https://imageflow.prmg.net:443/xsuite/xapps/xdoc/webservice/xWsProcess.aspx";

            //var upTask = new Task(FileWorker);
            //upTask.Start();
            //upTask.ContinueWith(UploadWindowVM.OnDoneUploadingFiles(),  TaskScheduler.FromCurrentSynchronizationContext())


            Task.Factory.StartNew(FileWorker)
                .ContinueWith(task => UploadWindowVM.OnDoneUploadingAllFiles(),
                              TaskScheduler.FromCurrentSynchronizationContext());



            /* TaskScheduler UploadScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Action ProcessFileList = new Action(() => { this.StartQueue(); });
            Action WrapUp = new Action(() =>
            {
                var listCompletedEvent = FileListHasCompleted;
                if (listCompletedEvent != null)
                    listCompletedEvent(this, null);
            });

            var TaskUpload = Task.Factory.StartNew(() => ProcessFileList());
            var AlertCompletion = TaskUpload.ContinueWith(t => WrapUp(), UploadScheduler);
            */


            //var TaskUploader = Task.Factory.StartNew(() => FileWorker());

        }

        public void FileWorker()
        {
            foreach (
                var file in UploadWindowVM.WorkingFileList.Where(
                    f => f.UploadProgress == FileToUpload.FileUploadStages.Unstarted 
                    && f.IsSelected))
            {
                file.UploadProgress = FileToUpload.FileUploadStages.Started;
                var listUpdatedEvent = FileListHasChanged;
                if (listUpdatedEvent != null)
                    listUpdatedEvent(this, null);

                int uploadresponsecode =
                    SingleFileUpload.HttpUploadFile(
                        file.PathFull, "file", "application/pdf", GetFormValues_v2(file),
                        UploadWindowVM.ImgFlowSessionKey,
                        UploadWindowVM.ImgFlowContainerKey,
                        UploadWindowVM.CurrImgFlowSession.SessionCookies);

                if (uploadresponsecode == 1)
                    file.UploadProgress = FileToUpload.FileUploadStages.Completed;
                else
                    file.UploadProgress = FileToUpload.FileUploadStages.Failed;

                if (listUpdatedEvent != null)
                    listUpdatedEvent(this, null);

            }
            var listCompletedEvent = FileListHasCompleted;
            if (listCompletedEvent != null)
                listCompletedEvent(this, null);
        }

        //Deprecated
        public List<Tuple<string, string>> GetFormValues(HtmlAgilityPack.HtmlDocument sourceDoc)
        {
            //HtmlAgilityPack.HtmlNode uploadForm = entiredoc.GetElementbyId("xForm");

            //var inputElements = uploadForm.Descendants("input");
            var inputElements = sourceDoc.DocumentNode.Descendants("input");
            var inputValsList = new List<Tuple<string, string>>();


            foreach (var node in inputElements)
            {
                //if (node.Attributes["id"] != null)
                //    System.Diagnostics.Debug.WriteLine("id: " + node.Attributes["id"].Value);
                if (node.Attributes["name"] != null)
                {
                    System.Diagnostics.Debug.WriteLine("name: " + node.Attributes["name"].Value);
                    if (node.Attributes["value"] != null)
                    {
                        System.Diagnostics.Debug.WriteLine("value: " + node.Attributes["value"].Value);
                        //inputValuesHT.Add(node.Attributes["name"].Value, node.Attributes["value"].Value);
                        inputValsList.Add(new Tuple<string, string>(node.Attributes["name"].Value,
                                                                    node.Attributes["value"].Value));

                    }
                    else
                    {
                        //inputValuesHT.Add(node.Attributes["name"].Value, "");
                        inputValsList.Add(new Tuple<string, string>(node.Attributes["name"].Value, ""));

                    }
                }
            }

            //inputValuesHT.Remove("xFile");
            inputValsList.RemoveAll(t => t.Item1 == "xFile");

            //inputValuesHT.Remove("xWsPayload");            
            inputValsList.RemoveAll(t => t.Item1 == "xWsPayload");

            //inputValues.Remove("xProjectId");
            //inputValsList.RemoveAll(t => t.Item1 == "xProjectId");

            //inputValuesHT.Add("xDocTypeId", "209654"); //Specified as 'conditions' 
            // submission 209638
            inputValsList.Add(new Tuple<string, string>("xDocTypeId", "209638"));

            //inputValues.Add("xProjectId", "1000");



            return inputValsList;

        }

        public List<Tuple<string, string>> GetFormValues_v2(FileToUpload sourceFile)
        {
            var newFormVals = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("xProjectId", "1000"),
                    new Tuple<string, string>("xDefaultDocType", ""),
                    new Tuple<string, string>("xDefaultColor", "0"),
                    new Tuple<string, string>("xFixedSizeWidth", ""),
                    new Tuple<string, string>("xHideTitleBar", "False"),
                    new Tuple<string, string>("xBatchInc", "0"),
                    new Tuple<string, string>("xProjectId", "1000"),
                    //new Tuple<string,string>("xContainerKey","214085"),
                    //new Tuple<string, string>("xContainerKey", sourceFile.MatchedLoan.ContId),
                    new Tuple<string, string>("xContainerKey", UploadWindowVM.TargetLoanItem.ContId),
                    new Tuple<string, string>("xWsSuccessMsg", "Document upload successful."),
                    new Tuple<string, string>("xWsNamespace", "XDOC.INBOX.FILEUPLOAD"),
                    new Tuple<string, string>("xWsPostDialog", "/xsuite/xapps/std/axStdWebservicePostDialog.aspx"),
                    new Tuple<string, string>("xWsPostUrl", "/xsuite/xapps/xdoc/webservice/xWsProcess.aspx"),
                    new Tuple<string, string>("xWsCallback", "parent.gxMainController.submitCallBack"),
                    //new Tuple<string,string>("xWsPayload",""), //This is the envelope added below
                    //new Tuple<string,string>("xFile",""), //This is removed
                    new Tuple<string, string>("xColor", "1"),
                    //new Tuple<string,string>("XCA29xContainerKey","214085"),
                    //new Tuple<string, string>("XCA29xContainerKey", sourceFile.MatchedLoan.ContId),
                    new Tuple<string, string>("xContainerKey", UploadWindowVM.TargetLoanItem.ContId),
                    new Tuple<string, string>("XCA29xContainerId", ""),
                    //new Tuple<string,string>("XCA29xContainerRef","3254506880"),
                    //new Tuple<string, string>("XCA29xContainerRef", sourceFile.MatchedLoan.PRMGLoanNum),
                    new Tuple<string, string>("XCA29xContainerRef", UploadWindowVM.TargetLoanItem.PRMGLoanNum),
                    //new Tuple<string,string>("xContAtt","3254506880"),
                    //new Tuple<string, string>("xContAtt", sourceFile.MatchedLoan.PRMGLoanNum),
                    new Tuple<string, string>("xContAtt", UploadWindowVM.TargetLoanItem.PRMGLoanNum),
                    //new Tuple<string,string>("xContAtt","TODD JONES"),
                    //new Tuple<string, string>("xContAtt", sourceFile.MatchedLoan.BorrNameRaw),
                    new Tuple<string, string>("xContAtt", UploadWindowVM.TargetLoanItem.BorrNameRaw),
                    new Tuple<string, string>("XCA33_schemaIds", "209638"),
                    new Tuple<string, string>("XCA33_schemaIds", "209654"),
                    new Tuple<string, string>("XCA33_schemaIds", "209681"),
                    new Tuple<string, string>("xDocTypeId", sourceFile.DocTypeId),
                    new Tuple<string, string>("xFileName", sourceFile.NameWithExt)
                };
            newFormVals.Add(new Tuple<string, string>("xWsPayload", PayloadEnvelope.CreateEnvelopeString(newFormVals)));

            newFormVals.ForEach(
                val => System.Diagnostics.Debug.WriteLine("name: {0}   value: {1}", val.Item1, val.Item2));

            return newFormVals;
        }

        private static class PayloadEnvelope
        {
            public static string CreateEnvelopeString(List<Tuple<string, string>> formInputsList)
            {
                /*<envelope xProjectId="1000" ><file xProjectId="1000" xBatchId="" 
                xSchemaId="209638" xContainerKey="215743" xContainerRef="3254508469" 
                xContainerId="" xColor="" xFileName="C:\fakepath\Moyer - W2 2010.pdf" ></file>
                    </envelope>*/

                XmlDocument doc = new XmlDocument();
                XmlElement el = (XmlElement) doc.AppendChild(doc.CreateElement("envelope"));
                el.SetAttribute("xProjectId", formInputsList.First(t => t.Item1 == "xProjectId").Item2);
                    // "1000");// formInputs["xProjectId"]);
                XmlElement envChild = (XmlElement) el.AppendChild(doc.CreateElement("file"));
                envChild.SetAttribute("xProjectId", formInputsList.First(t => t.Item1 == "xProjectId").Item2);
                    // "1000");// formInputs["xProjectId"]);
                envChild.SetAttribute("xBatchId", ""); //formInputsList.First(t => t.Item1 == "xBatchId").Item2);
                envChild.SetAttribute("xSchemaId", formInputsList.First(t => t.Item1 == "xDocTypeId").Item2);
                    //TODO: seems prone to error, check later
                envChild.SetAttribute("xContainerKey", formInputsList.First(t => t.Item1 == "xContainerKey").Item2);
                envChild.SetAttribute("xContainerRef", formInputsList.First(t => t.Item1 == "XCA29xContainerRef").Item2);
                envChild.SetAttribute("xContainerId", "");
                    // formInputsList.First(t => t.Item1 == "xContainerId").Item2);
                envChild.SetAttribute("xColor", formInputsList.First(t => t.Item1 == "xColor").Item2);
                envChild.SetAttribute("xFileName", formInputsList.First(t => t.Item1 == "xFileName").Item2);


                string envString = doc.OuterXml;

                return envString;
            }

            //public static string CreateEnvelopeString_v2(sourcefile
        }
    }
}
