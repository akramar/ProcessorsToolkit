using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProcessorsToolkit.Model;
using ProcessorsToolkit.Model.Form;
using ProcessorsToolkit.Model.Forms;
using ProcessorsToolkit.View;

//using ProcessorsToolkit.View;

namespace ProcessorsToolkit.ViewModel
{
    public class MainWindowVM : ObservableObject
    {

        public static BorrDir SelectedBorrDir { get; set; }
        //public static string SelectedFolderPath { get; set; }
        public static FannieData SelectedBorrData { get; set; }


        public static event EventHandler<SelectedBorrChangedEventArgs> SelectedBorrChanged;
        public static event EventHandler<SelectedDirChangedEventArgs> SelectedPathChanged;
        public static event EventHandler<SelectedBorrDataChangedEventArgs> SelectedBorrDataChanged;


        public static void OnSelectedBorrChanged(SelectedBorrChangedEventArgs e)
        {
            SelectedBorrDir = e.CurrBorr;
            var handler = SelectedBorrChanged;
            if (handler != null) handler(null, e);
        }

        public static void OnSelectedPathChanged(SelectedDirChangedEventArgs e)
        {
            //var subDir = BorrDir.GetSubDir(SelectedBorrDir.BorrName, e.CurrPath);

           // if (!String.IsNullOrEmpty(subDir))
           //     SelectedBorrDir.SubDirs.First(sd => sd.FolderName == subDir).IsOpen = true;

            //SelectedFolderPath = e.CurrPath;
            e.CurrPath = SelectedBorrDir.FullActivePath;

            var handler = SelectedPathChanged;
            if (handler != null) handler(null, e);
        }

        public static void OnSelectedBorrDataChanged(SelectedBorrDataChangedEventArgs e)
        {
            SelectedBorrData = e.CurrData;
            var handler = SelectedBorrDataChanged;
            if (handler != null) handler(null, e);
        }

        public MainWindowVM()
        {
            //BorrInfoUCVM.BorrInfoUpdated += (sender, args) => OnSelectedBorrDataChanged(args);
        }

        private enum FormClickActions { FillForm, OpenBrowser }

        public static void FormMenuClick(string selectedFormCommand)
        {
            PDFFormBase filledForm = null;
            FormClickActions clickAction;
            string urlToOpen = "";

            switch (selectedFormCommand)
            {
                case "IBW_Initial":
                    object formType = SelectedBorrData.SubjAddrState == "CA"
                                          ? InterbankForm.FormTypes.InitialDisclosuresCA
                                          : InterbankForm.FormTypes.InitialDisclosuresNonCA;
                    filledForm = new InterbankForm((InterbankForm.FormTypes) formType, SelectedBorrData, SelectedBorrDir);
                    clickAction = FormClickActions.FillForm;
                    break;
                case "IBW_Submission":
                    filledForm = new InterbankForm(InterbankForm.FormTypes.SubmissionForm, SelectedBorrData,
                                                   SelectedBorrDir);
                    clickAction = FormClickActions.FillForm;
                    break;
                case "IBW_FormSite":
                    urlToOpen = "http://www.interbankwholesale.com/broker-forms/";
                    clickAction = FormClickActions.OpenBrowser;
                    break;
                case "PRMG_Initial":
                    filledForm = new PRMGForm(PRMGForm.FormTypes.InitialDisclosures, SelectedBorrData, SelectedBorrDir);
                    clickAction = FormClickActions.FillForm;
                    break;
                case "PRMG_Submission":
                    filledForm = new PRMGForm(PRMGForm.FormTypes.SubmissionForm, SelectedBorrData, SelectedBorrDir);
                    clickAction = FormClickActions.FillForm;
                    break;
                case "PHM_Anti-Steering":
                    filledForm = new PlazaForm(PlazaForm.FormTypes.AntiSteering, SelectedBorrData, SelectedBorrDir);
                    clickAction = FormClickActions.FillForm;
                    break;
                case "PHM_SSA-89":
                    filledForm = new PlazaForm(PlazaForm.FormTypes.SSA89s, SelectedBorrData, SelectedBorrDir);
                    clickAction = FormClickActions.FillForm;
                    break;
                default:
                    return;
            }

            if (clickAction == FormClickActions.FillForm && filledForm != null)
            {
                filledForm.Fill();
                filledForm.OpenForm();
            }
            else if (clickAction == FormClickActions.OpenBrowser && urlToOpen != "")
            {
                System.Diagnostics.Process.Start(urlToOpen);
            }
        }

        public static void UploadToLender(string lenderCode)
        {
            
            switch (lenderCode)
            {
                case "IBW":
                    break;
                case "PRMG":
                    break;
                    
            }


        }
       
        /*
        public void LoadSubDirs(ListViewItem item)
        {
            

        }

        public ICommand ConvertTextCommand
        {
            get { return new DelegateCommand(LoadSubDirs(null)); }
        }
         */
    }
}
