using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using ProcessorsToolkit.Model;
using ProcessorsToolkit.Model.Interbank.UploadSession;
using ProcessorsToolkit.View.InterbankUploadWindow;

namespace ProcessorsToolkit.ViewModel.InterbankUploadWindow
{
    public class UploadWindowVM
    {


        public static BorrowerFileGroup WorkingFileList;
        public static AvailableLoansList AllLoansAvailable;
        public static LoanSearchResultItem TargetLoanItem { get; set; }
        //public static UploadTypes? SelectedUploadType { get; set; }
        public static bool AlreadyHavePTDs { get; set; }
        public static View.MainWindow SpawnedWindow { get; set; }
        public static Session WebsiteSession { get; set; }
        
    }
}
