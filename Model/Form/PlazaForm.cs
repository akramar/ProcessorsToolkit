using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ProcessorsToolkit.Model.Forms;

namespace ProcessorsToolkit.Model.Form
{
    class PlazaForm :PDFFormBase
    {


        public PlazaForm(FormTypes formType, FannieData srcBorrData, BorrDir borrDir)
        {
            FormType = formType;
            SrcBorrData = srcBorrData;
            BorrDirectory = borrDir;

        }

        public static string LenderkName = "Plaza Home Mortgage";
        public static string LenderAddrFull = "";

        public enum FormTypes
        {
            AntiSteering,
            SSA89s
        }

        public FormTypes FormType { get; set; }

        public override void Fill()
        {
            switch (FormType)
            {
                case FormTypes.AntiSteering:
                    FormFilename = "Plaza_Anti-Steering.pdf";
                    FormFieldsVals = GetAntiSteeringFields(SrcBorrData);
                    break;

                case FormTypes.SSA89s:
                    FormFilename = "Plaza_SSA89_rev1.pdf";
                    FormFieldsVals = GetSSA89Fields(SrcBorrData);
                    break;

                default:
                    return;
            }
            FillTheForm();
        }

        private static Dictionary<string, string> GetAntiSteeringFields(FannieData srcBorrData)
        {
             if (srcBorrData == null)
                throw new Exception("No borrowwer data provided");

            var vals = new Dictionary<string, string>
                {
                    {"Borr1_Name", srcBorrData.BorrName_Combined},
                    {"Borr2_Name", srcBorrData.CoBorrName_Combined},
                    {"Borrower_Names_Combined", srcBorrData.BorrsNamesCombined},
                    {"Disclosure_Date", srcBorrData.DisclosureDate.ToString("MM/dd/yyyy")},
                    {"Interest_Rate", srcBorrData.InterestRate.ToString(CultureInfo.InvariantCulture)},
                    {"Originator_Company", srcBorrData.OriginatorCompany}
                };
            return vals;
        }

        private static Dictionary<string, string> GetSSA89Fields(FannieData srcBorrData)
        {
             if (srcBorrData == null)
                throw new Exception("No borrowwer data provided");

            var vals = new Dictionary<string, string>
                {
                    {"Borr1_DOB", srcBorrData.BorrDOB_Pretty},
                    {"Borr1_Home_CSZ", srcBorrData.BorrCSZ},
                    {"Borr1_Home_Street", srcBorrData.BorrAddrStreet},
                    {"Borr1_Name", srcBorrData.BorrName_Combined},
                    {"Borr1_Phone", srcBorrData.BorrPhone_Pretty},
                    {"Borr1_SSN", srcBorrData.BorrSSN_Pretty},
                    {"Borr2_DOB", srcBorrData.CoBorrDOB_Pretty},
                    {"Borr2_Home_CSZ", srcBorrData.CoBorrCSZ},
                    {"Borr2_Home_Street", srcBorrData.CoBorrAddrStreet},
                    {"Borr2_Name", srcBorrData.CoBorrName_Combined},
                    {"Borr2_Phone", srcBorrData.CoBorrPhone_Pretty},
                    {"Borr2_SSN", srcBorrData.CoBorrSSN_Pretty},
                    {"Disclosure_Date", srcBorrData.DisclosureDate.ToString("MM/dd/yyyy")}
                };

            return vals;
        }
    }
}
