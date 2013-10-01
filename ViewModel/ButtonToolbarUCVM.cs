using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProcessorsToolkit.Model;
using ProcessorsToolkit.Model.Forms;

namespace ProcessorsToolkit.ViewModel
{
    public class ButtonToolbarUCVM
    {

        public BorrDir SelectedBorrDir { get; set; }
        public FannieData BorrData { get; set; }

        public ButtonToolbarUCVM()
        {
            MainWindowVM.SelectedBorrChanged += (sender, args) =>
                {
                    SelectedBorrDir = args.CurrBorr;
                };

            MainWindowVM.SelectedBorrDataChanged += (sender, args) =>
                {
                    BorrData = args.CurrData;
                };
        }

        public void FillPDFButtonClicked()
        {
            /*var formType = BorrData.SubjAddrState == "CA"
                               ? InterbankForm.FormTypes.InitialDisclosuresCA
                               : InterbankForm.FormTypes.InitialDisclosuresNonCA;

            var interbankForm = new InterbankForm(formType, BorrData, SelectedBorrDir);

            interbankForm.Fill();*/

            var formType = PRMGForm.FormTypes.InitialDisclosures;

            var prmgForm = new PRMGForm(formType, BorrData, SelectedBorrDir);

            prmgForm.Fill();
            
        }
    }
}
