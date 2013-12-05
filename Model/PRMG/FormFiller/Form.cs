using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ProcessorsToolkit.Model.PRMG.FormFiller
{
    internal class Form : PDFFormBase
    {

        public Form(FormTypes formType, FannieData srcBorrData, BorrDir borrDir)
        {
            FormType = formType;
            SrcBorrData = srcBorrData;
            BorrDirectory = borrDir;
            LenderName = "Paramount Residential Mortgage Group";
            LenderNameShort = "PRMG";
            LenderAddrFull = "1265 Corona Pointe Ct. Suite 301, Corona, CA 92879";
        }

        public enum FormTypes
        {
            SubmissionForm,
            InitialDisclosures
        }

        public FormTypes FormType { get; set; }

        public override void Fill()
        {

            switch (FormType)
            {
                case FormTypes.SubmissionForm:
                    FormName = "SubmissionForm";
                    FormFieldsVals = GetSubmissionFormFields(SrcBorrData);
                    break;

                case FormTypes.InitialDisclosures:
                    FormName = "InitialDisc";
                    FormFieldsVals = GetInitialDiscFormFields(SrcBorrData);
                    break;

                default:
                    return;
            }
            DownloadFormTemplate();
            FillTheForm();
        }

        private static Dictionary<string, string> GetSubmissionFormFields(FannieData srcBorrData)
        {
            if (srcBorrData == null)
                return null;

            var vals = new Dictionary<string, string>
                {
                    //"topmostSubform[0].Page1[0].Text64[0]"

                    {"topmostSubform[0].Page1[0].Text4[0]", (srcBorrData.OriginatorName + ", " + srcBorrData.OriginatorCompany)},
                    {"topmostSubform[0].Page1[0].Text5[0]", srcBorrData.OriginatorPhone_Pretty},
                    {"topmostSubform[0].Page1[0].Text6[0]", srcBorrData.ProcessorName},
                    {"topmostSubform[0].Page1[0].Text7[0]", srcBorrData.OriginatorFax_Pretty},
                    {"topmostSubform[0].Page1[0].Text9[0]", srcBorrData.OriginatorEmail},
                    {"topmostSubform[0].Page1[0].Text10[0]", srcBorrData.BorrName_Combined},
                    {"topmostSubform[0].Page1[0].Text11[0]", srcBorrData.BorrSSN_Pretty},
                    {"topmostSubform[0].Page1[0].Text12[0]", srcBorrData.BorrEmail},
                    {"topmostSubform[0].Page1[0].Text13[0]", srcBorrData.CoBorrName_Combined},
                    {"topmostSubform[0].Page1[0].Text14[0]", srcBorrData.CoBorrSSN_Pretty},
                    {"topmostSubform[0].Page1[0].Text15[0]", srcBorrData.CoBorrEmail},
                    {"topmostSubform[0].Page1[0].Text16[0]", srcBorrData.SubjAddrStreet},
                    {"topmostSubform[0].Page1[0].Text17[0]", srcBorrData.SubjAddrCity},
                    {"topmostSubform[0].Page1[0].Text18[0]", srcBorrData.SubjAddrState},
                    {"topmostSubform[0].Page1[0].Text19[0]", srcBorrData.SubjAddrZip},
                    {"topmostSubform[0].Page1[0].Combo_Box24[0]", srcBorrData.Occupancy}, //questionable
                    {"topmostSubform[0].Page1[0].Combo_Box26[0]", srcBorrData.LoanPurpose}, //questionable
                    {"topmostSubform[0].Page1[0].Text29[0]", srcBorrData.InterestRate.ToString(CultureInfo.InvariantCulture)},
                    {"topmostSubform[0].Page1[0].Text38[0]", srcBorrData.LoanAmtBase.ToString(CultureInfo.InvariantCulture)},
                    {"topmostSubform[0].Page1[0].Text39[0]", srcBorrData.PurchPrice.ToString(CultureInfo.InvariantCulture)},
                    {"topmostSubform[0].Page1[0].Text40[0]", srcBorrData.LoanAmtGross.ToString(CultureInfo.InvariantCulture)},
                    {"topmostSubform[0].Page1[0].Text41[0]", srcBorrData.AppraisedValue.ToString(CultureInfo.InvariantCulture)},
                    {"topmostSubform[0].Page1[0].Text51[0]", srcBorrData.LTV.ToString(CultureInfo.InvariantCulture)}
                };

            return vals;
        }

        private static Dictionary<string, string> GetInitialDiscFormFields(FannieData srcBorrData)
        {

            if (srcBorrData == null)
                return null;

            var vals = new Dictionary<string, string>
                {
                    {"Borr_LastName", srcBorrData.BorrNameLast},
                    {"Borr1_DOB", srcBorrData.BorrDOB_Pretty},
                    {"Borr1_Name", srcBorrData.BorrName_Combined},
                    {"Borr1_SSN", srcBorrData.BorrSSN_Pretty},
                    {"Borr1_Home_Street", srcBorrData.BorrAddrStreet},
                    {"Borr1_Home_CSZ", srcBorrData.BorrCSZ},
                    {"Borr1_Phone", srcBorrData.BorrPhone_Pretty},
                    {"Borr2_DOB", srcBorrData.CoBorrDOB_Pretty},
                    {"Borr2_Name", srcBorrData.CoBorrName_Combined},
                    {"Borr2_SSN", srcBorrData.CoBorrSSN_Pretty},
                    {"Borr2_Home_Street", srcBorrData.CoBorrAddrStreet},
                    {"Borr2_Home_CSZ", srcBorrData.CoBorrCSZ},
                    {"Borr2_Phone", srcBorrData.CoBorrPhone_Pretty},
                    {"Borrowers_Combined", srcBorrData.BorrsNamesCombined},
                    {"Broker_Address", (srcBorrData.OriginatorAddr_Combined + " NMLS: " + srcBorrData.OriginatorCompanyNMLS)},
                    {"Broker_Comp_Name", srcBorrData.OriginatorCompany},
                    {"Broker_LO_Combined", (srcBorrData.OriginatorName + ", " + srcBorrData.OriginatorCompany)},
                    {"LO_Name", srcBorrData.OriginatorName},
                    {"Lender_Name_Addr", (LenderName + ", " + LenderAddrFull)},
                    {"Loan_Amount", srcBorrData.LoanAmtGross.ToString(CultureInfo.InvariantCulture)},
                    {"Prop_Address", srcBorrData.SubjAddr_Combined},
                    {"Signature_Date", srcBorrData.DisclosureDate.ToString("MM/dd/yyyy")},
                    {"Signature_Date_Borr1", srcBorrData.DisclosureDate.ToString("MM/dd/yyyy")},
                    {"Signature_Date_Borr2", srcBorrData.DisclosureDate.ToString("MM/dd/yyyy")}
                };
            return vals;
        }
    }
}
