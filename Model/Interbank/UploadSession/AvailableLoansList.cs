using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using ProcessorsToolkit.ViewModel;
using ProcessorsToolkit.ViewModel.InterbankUploadWindow;

namespace ProcessorsToolkit.Model.Interbank.UploadSession
{
    public class AvailableLoansList : ObservableCollection<LoanSearchResultItem>
    {
        //private readonly UploadWindowVM _parentVM;

        public AvailableLoansList()//UploadWindowVM parentVM)
        {
            //_parentVM = parentVM;
        }

        
        public void LoadList()
        {
            
        }

        public LoanSearchResultItem GetMatchedLoanByContId(string containerId)
        {            
            return this.FirstOrDefault(l => l.ContId == containerId);
        }
    }
    public class LoanSearchResultItem
    {
        public string BorrNameRaw { get; set; }
        public string BorrLastName { get; set; }
        public string BorrFirstName { get; set; }
        public string PRMGLoanNum { get; set; }
        public string LoanAmt { get; set; }
        public string ContId { get; set; }
        public string DisplayName { get { return String.Format("{0}, {1} #{2}", BorrLastName, BorrFirstName, PRMGLoanNum); } }
        public bool IsSelected { get; set; }
    }
}
