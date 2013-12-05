using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.Model.Interbank.UploadSession
{
    public class AvailableLoansList : ObservableCollection<LoanSearchResultItem>
    {

    }

    public class LoanSearchResultItem
    {
        public string BorrNameRaw { get; set; } //not used
        public string BorrLastName { get; set; }
        public string BorrFirstName { get; set; } //not used
        public string IBWLoanNum { get; set; }
        public string LoanAmt { get; set; } //not used
        public string DisplayName { get { return String.Format("{0} #{1}", BorrLastName, IBWLoanNum); } }
        public bool IsSelected { get; set; }
    }
}
