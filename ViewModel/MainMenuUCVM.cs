using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ProcessorsToolkit.Model;
using ProcessorsToolkit.Model.Tools;
using ProcessorsToolkit.View;
using ProcessorsToolkit.View.Tools;

namespace ProcessorsToolkit.ViewModel
{
    public class MainMenuUCVM : INotifyPropertyChanged
    {
        private enum FormClickActions { FillForm, OpenBrowser }
        public MainMenuUC View { get; set; }
        
        private bool _isPDFSelected;
        public bool IsPDFSelected
        {
            get { return _isPDFSelected; }
            set { _isPDFSelected = value; NotifyPropertyChanged("IsPDFSelected"); }
        }

        private bool _isPDFableFileSelected;
        public bool IsPDFableFileSelected
        {
            get { return _isPDFableFileSelected; }
            set { _isPDFableFileSelected = value; NotifyPropertyChanged("IsPDFableFileSelected"); }
        }
        
        private bool _isConditionFolderOpen;
        public bool IsConditionFolderOpen
        {
            get { return _isConditionFolderOpen; }
            set { _isConditionFolderOpen = value; NotifyPropertyChanged("IsConditionFolderOpen"); }
        }

        private bool _isAFolderOpen;
        public bool IsAFolderOpen
        {
            get { return _isAFolderOpen; }
            set { _isAFolderOpen = value; NotifyPropertyChanged("IsAFolderOpen"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainMenuUCVM()
        {
            MainWindowVM.View.Loaded += (sender, args) => LoadMenuTriggerEvents();
        }
        
        public void LoadMenuTriggerEvents()
        {
            var borrFilesUCVM = MainWindowVM.BorrFilesCtrl.DataContext as BorrFilesUCVM;
            if (borrFilesUCVM != null)
                borrFilesUCVM.SelectedFileChanged += borrFilesUCVM_SelectedFileChanged;

            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM != null)
            {
                borrFoldersUCVM.SelectedPathChanged += borrFoldersUCVM_SelectedPathChanged;
                borrFoldersUCVM.SelectedBorrChanged += borrFoldersUCVM_SelectedBorrChanged;
            }

        }
        
        void borrFilesUCVM_SelectedFileChanged(object sender, SelectedFileChangedEventArgs e)
        {
            if (e == null || e.CurrFile == null)
            {
                IsPDFSelected = false;
                IsPDFableFileSelected = false;
            }
            else
            {
                IsPDFSelected = e.CurrFile.IsPDF;
                IsPDFableFileSelected = e.CurrFile.CanBePDFed;
            }
        }

        void borrFoldersUCVM_SelectedPathChanged(object sender, SelectedPathChangedEventArgs e)
        {
            if (e == null || e.CurrPath == null)
                IsAFolderOpen = false;
            else
                IsAFolderOpen = true;
            
            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM != null && borrFoldersUCVM.SelectedBorrDir != null)
            {
                IsConditionFolderOpen = (borrFoldersUCVM.SelectedBorrDir.ActiveSubDir != null &&
                    borrFoldersUCVM.SelectedBorrDir.ActiveSubDir.Fullpath == borrFoldersUCVM.SelectedBorrDir.FullActivePath &&
                    borrFoldersUCVM.SelectedBorrDir.ActiveSubDir.FolderName.StartsWith("conditions", StringComparison.InvariantCultureIgnoreCase));
            }
        }
        
        void borrFoldersUCVM_SelectedBorrChanged(object sender, SelectedBorrChangedEventArgs e)
        {
            if (e == null || e.CurrBorrDir == null)
                IsAFolderOpen = false;
            else
                IsAFolderOpen = true;
        }

        public void FileMenuClick(string selectedFileCommand)
        {
            switch (selectedFileCommand)
            {
                case "Settings":
                    var optWindow = new SettingsWindow { Owner = MainWindowVM.View };
                    optWindow.ShowDialog();
                    break;

                case "Exit":
                    MainWindowVM.View.Close();
                    break;
            }
        }

        public void FormMenuClick(string selectedFormCommand)
        {
            PDFFormBase filledForm = null;
            FormClickActions clickedAction;
            var urlToOpen = "";

            var borrInfoUCVM = MainWindowVM.BorrInfoCtrl.DataContext as BorrInfoUCVM;
            if (borrInfoUCVM == null)
                return;
            
            switch (selectedFormCommand)
            {
                case "IBW_Initial":
                    object formType = borrInfoUCVM.BorrData.SubjAddrState == "CA"
                                          ? Model.Interbank.FormFiller.Form.FormTypes.InitialDisclosuresCA
                                          : Model.Interbank.FormFiller.Form.FormTypes.InitialDisclosuresNonCA;
                    filledForm = new Model.Interbank.FormFiller.Form((Model.Interbank.FormFiller.Form.FormTypes)formType, borrInfoUCVM.BorrData,
                                                   MainWindowVM.SelectedBorrDir);
                    clickedAction = FormClickActions.FillForm;
                    break;
                case "IBW_Submission":
                    filledForm = new Model.Interbank.FormFiller.Form(Model.Interbank.FormFiller.Form.FormTypes.SubmissionForm, borrInfoUCVM.BorrData,
                                                   MainWindowVM.SelectedBorrDir);
                    clickedAction = FormClickActions.FillForm;
                    break;
                case "IBW_FormSite":
                    urlToOpen = "http://www.interbankwholesale.com/broker-forms/";
                    clickedAction = FormClickActions.OpenBrowser;
                    break;
                case "PRMG_Initial":
                    filledForm = new Model.PRMG.FormFiller.Form(Model.PRMG.FormFiller.Form.FormTypes.InitialDisclosures, borrInfoUCVM.BorrData,
                                              MainWindowVM.SelectedBorrDir);
                    clickedAction = FormClickActions.FillForm;
                    break;
                case "PRMG_Submission":
                    filledForm = new Model.PRMG.FormFiller.Form(Model.PRMG.FormFiller.Form.FormTypes.SubmissionForm, borrInfoUCVM.BorrData,
                                              MainWindowVM.SelectedBorrDir);
                    clickedAction = FormClickActions.FillForm;
                    break;
                case "PHM_Anti-Steering":
                    filledForm = new Model.Plaza.Form(Model.Plaza.Form.FormTypes.AntiSteering, borrInfoUCVM.BorrData,
                                               MainWindowVM.SelectedBorrDir);
                    clickedAction = FormClickActions.FillForm;
                    break;
                case "PHM_SSA-89":
                    filledForm = new Model.Plaza.Form(Model.Plaza.Form.FormTypes.SSA89s, borrInfoUCVM.BorrData,
                                               MainWindowVM.SelectedBorrDir);
                    clickedAction = FormClickActions.FillForm;
                    break;
                default:
                    return;
            }

            if (clickedAction == FormClickActions.FillForm && filledForm != null)
            {
                var statuswin = new DownloadProgressWindow {Owner = MainWindowVM.View};

                Task.Factory.StartNew(() => filledForm.Fill())
                    .ContinueWith(task =>
                        {
                            statuswin.Close(); //unlock UI when filled
                            filledForm.OpenForm();
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                statuswin.ShowDialog(); //ShowDialog locks the UI thread while form downloads in background in above thread
            }
            else if (clickedAction == FormClickActions.OpenBrowser && urlToOpen != "")
            {
                System.Diagnostics.Process.Start(urlToOpen);
            }
        }

        public void UploadMenuClick(string lenderCode)
        {
            switch (lenderCode)
            {
                case "PRMG":
                    {
                        var newUploadWin = new View.PRMGUploadWindow.UploadWindow {Owner = MainWindowVM.View};
                        newUploadWin.Show();
                    }
                    break;
                case "IBW":
                    {
                        var newUploadWin = new View.Interbank.UploadWindow.UploadWindow {Owner = MainWindowVM.View};
                        newUploadWin.Show();
                    }
                    break;
                case "PHM":
                    {
                        var newUploadWin = new View.Plaza.UploadWindow.UploadWindow {Owner = MainWindowVM.View};
                        newUploadWin.Show();
                    }
                    break;
            }

        }

        public void ToolsMenuClick(string selectedToolsCommand)
        {
            FileBase newFile = null;
            var borrFilesUCVM = MainWindowVM.BorrFilesCtrl.DataContext as BorrFilesUCVM;
            if (borrFilesUCVM == null)
                return;

            switch (selectedToolsCommand)
            {
                case "NewConds":
                    var newCondWin = new CreateConditionWindow {Owner = MainWindowVM.View};
                    newCondWin.Show();
                    break;
                case "ListAllConds":
                    var fileCondWindow = new ConditionsListerWindow
                        {
                            Owner = MainWindowVM.View,
                            Height = MainWindowVM.View.Height - 10.0
                        };
                    fileCondWindow.Show();
                    break;
                case "UnlockPDF":
                    var pdfUnlocked = new PDFUnlocker(borrFilesUCVM.SelectedFile);
                    newFile = pdfUnlocked.Unlock();
                    break;
                case "FlattenPDF":
                    var pdfFlattener = new PDFFlattener(borrFilesUCVM.SelectedFile);
                    newFile = pdfFlattener.Flatten3();
                    break;
                case "ListAllFiles":
                    var fileListWindow = new AllFilesListerWindow
                    {
                        Owner = MainWindowVM.View,
                        Height = MainWindowVM.View.Height - 10.0
                    };
                    fileListWindow.Show();
                    break;
                case "ConvertToPDF":
                    var pdfCreator = new PDFCreator(borrFilesUCVM.SelectedFile);
                    newFile = pdfCreator.ConvertToPDF();
                    break;
            }

            if (newFile == null) 
                return;

            var pathToOpen = String.Format(@"""{0}""", newFile.Fullpath);
            System.Diagnostics.Process.Start(pathToOpen);
        }
    }
}
