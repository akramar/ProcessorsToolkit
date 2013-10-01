using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ProcessorsToolkit.Model.Form
{
    internal class InterbankForm : PDFFormBase
    {

        public InterbankForm(FormTypes formType, FannieData srcBorrData, BorrDir borrDir)
        {
            FormType = formType;
            SrcBorrData = srcBorrData;
            BorrDirectory = borrDir;
        }

        public static string LenderkName = "Interbank Mortgage Company";
        public static string LenderAddrFull = "333 Knightsbridge Parkway, Suite 210, Lincolnshire, IL 60069";

        public enum FormTypes
        {
            SubmissionForm,
            InitialDisclosuresCA,
            InitialDisclosuresNonCA
        }

        public FormTypes FormType { get; set; }

        public override void Fill()
        {
            switch (FormType)
            {
                case FormTypes.SubmissionForm:
                    FormFilename = "Interbank_SubmissionForm_rev4b.pdf";
                    FormFieldsVals = GetSubmissionFormFields(SrcBorrData);
                    break;

                case FormTypes.InitialDisclosuresCA:
                    FormFilename = "Interbank_DisclosurePkg_rev4b_(CA).pdf";
                    FormFieldsVals = GetInitialDiscFormFields(SrcBorrData);
                    break;

                case FormTypes.InitialDisclosuresNonCA:
                    FormFilename = "Interbank_DisclosurePkg_rev4b_(Non-CA).pdf";
                    FormFieldsVals = GetInitialDiscFormFields(SrcBorrData);
                    break;

                default:
                    return;
            }
            FillTheForm();
        }

        private static Dictionary<string, string> GetSubmissionFormFields(FannieData srcBorrData)
        {
            if (srcBorrData == null)
                throw new Exception("No borrowwer data provided");

            var vals = new Dictionary<string, string>
                {
                    {"Borrs_Names_Combined", srcBorrData.BorrsNamesCombined},
                    {"Borr1_Email", srcBorrData.BorrEmail},
                    {"Broker_Company", srcBorrData.OriginatorCompany},
                    {"Broker_Contact_Name", srcBorrData.ProcessorName},
                    {"Broker_Email", srcBorrData.OriginatorEmail},
                    {"Broker_Fax", srcBorrData.OriginatorFax_Pretty},
                    {"Broker_Phone", srcBorrData.OriginatorPhone_Pretty},
                    {"Est_Close_Date", srcBorrData.COE_Pretty},
                    {"IMC_Loan_Num", ""},
                    {"Loan_Purpose", srcBorrData.LoanPurpose},
                    {"Occ_Type", srcBorrData.Occupancy}
                };

            return vals;
        }

        private static Dictionary<string, string> GetInitialDiscFormFields(FannieData srcBorrData)
        {

            if (srcBorrData == null)
                throw new Exception("No borrowwer data provided");

            var vals = new Dictionary<string, string>
                {
                    {"Borrs_Names_Combined", srcBorrData.BorrsNamesCombined},
                    {"Borr1_Name", srcBorrData.BorrName_Combined},
                    {"Borr1_DOB", srcBorrData.BorrDOB_Pretty},
                    {"Borr1_Home_Street", srcBorrData.BorrAddrStreet},
                    {"Borr1_Home_CSZ", srcBorrData.BorrCSZ},
                    {"Borr1_Phone", srcBorrData.BorrPhone_Pretty},
                    {"Borr1_SSN", srcBorrData.BorrSSN_Pretty},
                    //CoBorrower
                    {"Borr2_Name", srcBorrData.CoBorrName_Combined},
                    {"Borr2_DOB", srcBorrData.CoBorrDOB_Pretty},
                    {"Borr2_Home_Street", srcBorrData.CoBorrAddrStreet},
                    {"Borr2_Home_CSZ", srcBorrData.CoBorrCSZ},
                    {"Borr2_Phone", srcBorrData.CoBorrPhone_Pretty},
                    {"Borr2_SSN", srcBorrData.CoBorrSSN_Pretty},
                    //Broker - Originator
                    {"Broker_Company", srcBorrData.OriginatorCompany},
                    //{"Broker_Loan_Num", srcBorrData.OriginatorLoanNum},
                    {"Broker_NMLS", srcBorrData.OriginatorNMLS},
                    {"Broker_Name", srcBorrData.OriginatorName},
                    {"Broker_Street", srcBorrData.OriginatorCompanyStreet},
                    {"Broker_City", srcBorrData.OriginatorCompanyCity},
                    {"Broker_State", srcBorrData.OriginatorCompanyState},
                    {"Broker_Zip", srcBorrData.OriginatorCompanyZip},
                    {"Disclosure_Date", srcBorrData.DisclosureDate.ToString("MM/dd/yyyy")},
                    {"Disclosure_Day", srcBorrData.DisclosureDate.Day.ToString(CultureInfo.InvariantCulture)}, //this is generated inside PDF script, but not well
                    {"Disclosure_Month", srcBorrData.DisclosureDate.ToString("MMMM")},
                    {"Disclosure_Year", srcBorrData.DisclosureDate.Year.ToString(CultureInfo.InvariantCulture)},
                    /*{"Fee_Disc_Date", srcBorrData},
                    {"Fee_Disc_Year", srcBorrData},*/
                    {"IMC_Loan_Num", ""},
                    {"LO_Name", srcBorrData.OriginatorName},
                    {"Lender_Name", LenderkName},
                    {"Lender_Address_Full", LenderAddrFull},
                    //{"Lender_Pay_Addtl_Flat", srcBorrData},
                    {"Loan_Amount", srcBorrData.LoanAmtGross.ToString(CultureInfo.InvariantCulture)},
                    {"Loan_Purpose", srcBorrData.LoanPurpose},
                    {"Loan_Term", srcBorrData.LoanTermMonths.ToString(CultureInfo.InvariantCulture)},
                    {"New_Note_Rate", srcBorrData.InterestRate.ToString(CultureInfo.InvariantCulture)},
                    {"Processor_Name", srcBorrData.ProcessorName},
                    {"Subject_Street", srcBorrData.SubjAddrStreet},
                    {"Subject_City", srcBorrData.SubjAddrCity},
                    {"Subject_State", srcBorrData.SubjAddrState},
                    {"Subject_Zip", srcBorrData.SubjAddrZip}
                };


            /* omitted fields
                        Executed for Reason	
                        Fees
                        Initial
                        Lender_Pay_Percent
                        Lender_Pay_Total
                        Loan_Cost
                        Loan_DTI
                        Loan_LTV
                        Loan_Orig
                        Loan_PITI
                        Loan_Program
                        Loan_Start_Date
                        MIN
                        Prep - Date
                        Preparer Name
                        Rate_Lock_Date
                        Rate_Lock_Expiration
                        Rate_Lock_Period
                        Rate_Locked	
                        */

            return vals;
        }
    }
}