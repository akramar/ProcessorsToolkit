using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ProcessorsToolkit.Model
{
    public class FannieData
    {
        //http://lmhelp.velocify.com/entries/21662265-Fannie-Mae-3-2-FM3-2-FannieMae-File-Format

        public bool HasFannieData { get; set; }
        public string BorrNameFirst { get; set; }
        public string BorrNameMI { get; set; }
        public string BorrNameLast { get; set; }
        public string BorrName_Combined { get { return (String.Join(" ", new[] { BorrNameFirst, BorrNameMI, BorrNameLast }).Trim()); } }
        public string BorrSSN { get; set; }
        public string BorrSSN_Pretty { get { return (BorrSSN != null) ? PrettySSN(BorrSSN) : ""; } }
        public DateTime? BorrDOB { get; set; }
        public string BorrDOB_Pretty { get { return (BorrDOB != null) ? ((DateTime)BorrDOB).ToShortDateString() : ""; } }
        public string BorrPhone { get; set; }
        public string BorrPhone_Pretty { get { return (BorrPhone != null) ? PrettyPhoneNum(BorrPhone) : ""; } }
        public string BorrEmail { get; set; }
        public string BorrAddrStreet { get; set; }
        public string BorrAddrCity { get; set; }
        public string BorrAddrState { get; set; }
        public string BorrAddrZip { get; set; }
        public string BorrCSZ { get { return CombineCityStateZip(BorrAddrCity, BorrAddrState, BorrAddrZip); } }
        public string CoBorrNameFirst { get; set; }
        public string CoBorrNameMI { get; set; }
        public string CoBorrNameLast { get; set; }
        public string CoBorrName_Combined {get { return String.Join(" ", new[] {CoBorrNameFirst, CoBorrNameMI, CoBorrNameLast}).Trim(); }}
        public string CoBorrSSN { get; set; }
        public string CoBorrSSN_Pretty { get { return (CoBorrSSN != null) ? PrettySSN(CoBorrSSN) : ""; } }
        public DateTime? CoBorrDOB { get; set; }
        public string CoBorrDOB_Pretty { get { return (CoBorrDOB != null) ? ((DateTime) CoBorrDOB).ToShortDateString() : ""; } }
        public string CoBorrPhone { get; set; }
        public string CoBorrPhone_Pretty { get { return (CoBorrPhone != null) ? PrettyPhoneNum(CoBorrPhone) : ""; } }
        public string CoBorrEmail { get; set; }
        public string CoBorrAddrStreet { get; set; }
        public string CoBorrAddrCity { get; set; }
        public string CoBorrAddrState { get; set; }
        public string CoBorrAddrZip { get; set; }
        public string CoBorrCSZ { get { return CombineCityStateZip(CoBorrAddrCity, CoBorrAddrState, CoBorrAddrZip); } }
        public string SubjAddrStreet { get; set; }
        public string SubjAddrCity { get; set; }
        public string SubjAddrState { get; set; }
        public string SubjAddrZip { get; set; }
        public double LoanAmtBase { get; set; }
        public double FundingFeeFinanced { get; set; }
        public int LoanTermMonths { get; set; }
        public double InterestRate { get; set; }
        public double PurchPrice { get; set; }
        public string LoanPurpose { get; set; } 
        public string Occupancy { get; set; }
        public string OriginatorName { get; set; }
        public string OriginatorNMLS { get; set; }
        //public string OriginatorLoanNum { get; set; } // need to fetch
        public string OriginatorPhone { get; set; }
        public string OriginatorPhone_Pretty { get { return (OriginatorPhone != null) ? PrettyPhoneNum(OriginatorPhone) : ""; } }
        public string OriginatorFax { get; set; } //need to fetch from fnm
        public string OriginatorFax_Pretty { get { return (OriginatorFax != null) ? PrettyPhoneNum(OriginatorFax) : ""; } }
        public string OriginatorCompany { get; set; }
        public string OriginatorCompanyNMLS { get; set; }
        public string OriginatorCompanyStreet { get; set; }
        public string OriginatorCompanyCity { get; set; }
        public string OriginatorCompanyState { get; set; }
        public string OriginatorCompanyZip { get; set; }

        public DateTime? OriginatorSigDate { get; set; }
        public string OriginatorSigDate_Pretty { get { return (OriginatorSigDate != null) ? ((DateTime)OriginatorSigDate).ToShortDateString() : ""; } }
        public string OriginatorAddress { get; set; } //need to fetch

        //Additional properties, these don't exist in FNM file
        public DateTime? COE { get; set; }
        public string COE_Pretty { get; set; }
        public string ProcessorName { get; set; }
        public string OriginatorEmail { get; set; }
        public string OriginatorAddr_Combined { get { return (OriginatorCompanyStreet + ", " + CombineCityStateZip(OriginatorCompanyCity, OriginatorCompanyState, OriginatorCompanyZip));} }
        public double AppraisedValue { get; set; }
        public string SubjAddr_Combined { get { return SubjAddrStreet + ", " + CombineCityStateZip(SubjAddrCity, SubjAddrState, SubjAddrZip); } }
        //TODO: include appraised value data from FNM
        public double LTV { get { return (!LoanAmtBase.Equals(0.0) && !PurchPrice.Equals(0.0)) ? Math.Round(((LoanAmtBase / PurchPrice)*100), 3) : 0.0; } }
        public string LTV_Pretty { get { return (!LTV.Equals(0.0)) ? String.Format("{0:P3}", LTV/100) : "N/A"; } }
        public double LoanAmtGross { get { return (!FundingFeeFinanced.Equals(0.0) ? (LoanAmtBase + FundingFeeFinanced) : LoanAmtBase); } }
        public DateTime DisclosureDate
        {
            get
            {
                if (OriginatorSigDate == null || OriginatorSigDate == DateTime.MinValue)
                    return DateTime.Now;

                return (DateTime) OriginatorSigDate;
            }
        }
        public string BorrsNamesCombined
        {
            get
            {
                if (CoBorrName_Combined == String.Empty)
                    return BorrName_Combined;
                return BorrName_Combined + ", " + CoBorrName_Combined;
            }
        }
        


        public void LoadFannieFile(FileInfo fannieFileInfo)
        {

            //var fileInfo = new FileInfo(pathToFile);

            //http://stackoverflow.com/questions/8037070/whats-the-fastest-way-to-read-a-text-file-line-by-line

            //var fannieDataLines = File.ReadAllLines(fannieFileInfo.FullName).ToList();
            var fannieDataLines = File.ReadLines(fannieFileInfo.FullName).ToList();


            //Address line
            var addrString = fannieDataLines.FirstOrDefault(l => l.StartsWith("02A"));
            if (addrString != null)
            {
                SubjAddrStreet = addrString.Substring(3, 50).Trim();
                SubjAddrCity = addrString.Substring(53, 35).Trim();
                SubjAddrState = addrString.Substring(88, 2).Trim();
                SubjAddrZip = addrString.Substring(90, 9).Trim();
            }
            //Loan purpose line
            var loanPurpString = fannieDataLines.FirstOrDefault(l => l.StartsWith("02B"));
            if (loanPurpString != null)
            {
                var loanPurp = loanPurpString.Substring(5, 2);
                switch (loanPurp)
                {
                    case "05":
                        LoanPurpose = "Refinance";
                        break;
                    case "16":
                        LoanPurpose = "Purchase";
                        break;
                }

                var occ = loanPurpString.Substring(87, 1);
                switch (occ)
                {
                    case "1":
                        Occupancy = "Primary";
                        break;
                    case "2":
                        Occupancy = "Secondary";
                        break;
                    case "D":
                        Occupancy = "Investment";
                        break;
                }
            }


            //Borrower line
            var borrNameString = fannieDataLines.FirstOrDefault(l => l.StartsWith("03ABW"));
            if (borrNameString != null)
            {
                BorrSSN = borrNameString.Substring(5, 9);
                BorrNameFirst = borrNameString.Substring(14, 35).Trim();
                BorrNameMI = borrNameString.Substring(39, 35).Trim();
                BorrNameLast = borrNameString.Substring(84, 39).Trim();
                BorrPhone = borrNameString.Substring(123, 10).Trim();
                BorrEmail = borrNameString.Substring(159).Trim();
                BorrDOB = ParseDate(borrNameString.Substring(151, 8).Trim());
            }
            //CoBorrower line
            var coBorrNameString = fannieDataLines.FirstOrDefault(l => l.StartsWith("03AQZ"));
            if (coBorrNameString != null)
            {
                CoBorrSSN = coBorrNameString.Substring(5, 9);
                CoBorrNameFirst = coBorrNameString.Substring(14, 35).Trim();
                CoBorrNameMI = coBorrNameString.Substring(39, 35).Trim();
                CoBorrNameLast = coBorrNameString.Substring(84, 39).Trim();
                CoBorrPhone = coBorrNameString.Substring(123, 10).Trim();
                CoBorrEmail = coBorrNameString.Substring(159).Trim();
                CoBorrDOB = ParseDate(coBorrNameString.Substring(151, 8).Trim());
            }
            //Borrower Addresses
             var borrAddrString = fannieDataLines.FirstOrDefault(l => l.StartsWith("03C" + BorrSSN));
             if (borrAddrString != null)
             {
                 BorrAddrStreet = borrAddrString.Substring(14, 50).Trim();
                 BorrAddrCity = borrAddrString.Substring(64, 35).Trim();
                 BorrAddrState = borrAddrString.Substring(99, 2);
                 BorrAddrZip = borrAddrString.Substring(101, 9).Trim();
             }
            //CoBorr addresses
             var coBorrAddrString = fannieDataLines.FirstOrDefault(l => l.StartsWith("03C" + CoBorrSSN));
             if (coBorrAddrString != null)
             {
                 CoBorrAddrStreet = coBorrAddrString.Substring(14, 50).Trim();
                 CoBorrAddrCity = coBorrAddrString.Substring(64, 35).Trim();
                 CoBorrAddrState = coBorrAddrString.Substring(99, 2);
                 CoBorrAddrZip = coBorrAddrString.Substring(101, 9).Trim();
             }
            //Loan and Purch amounts
            var loanDetails1 = fannieDataLines.FirstOrDefault(l => l.StartsWith("01A"));
            if (loanDetails1 != null)
            {
                var loanbase = loanDetails1.Substring(136, 11).Trim();
                if (loanbase != String.Empty)
                    LoanAmtBase = Double.Parse(loanbase);
                var rate = loanDetails1.Substring(147, 5).Trim();
                if (rate != String.Empty)
                    InterestRate = Double.Parse(rate);
                var termMonths = loanDetails1.Substring(152, 3).Trim();
                if (termMonths != String.Empty)
                    LoanTermMonths = int.Parse(termMonths);
            }
            var loanDetails2 = fannieDataLines.First(l => l.StartsWith("07A"));
            if (loanDetails2 != null)
            {
                var purchPrice = loanDetails2.Substring(9, 11).Trim();
                if (purchPrice != String.Empty)
                    PurchPrice = Double.Parse(purchPrice);


                var fundingFeeFinanced = loanDetails2.Substring(161).Trim();
                if (fundingFeeFinanced != String.Empty)
                    FundingFeeFinanced = Double.Parse(fundingFeeFinanced);
            }
            //Broker data
            var brokerString = fannieDataLines.FirstOrDefault(l => l.StartsWith("10BT"));
            if (brokerString != null)
            {
                OriginatorName = brokerString.Substring(4, 60).Trim();
                OriginatorSigDate = ParseDate(brokerString.Substring(64, 8).Trim());
                OriginatorPhone = brokerString.Substring(72, 10).Trim();
                OriginatorCompany = brokerString.Substring(82, 35).Trim();
                OriginatorNMLS = fannieDataLines.First(l => l.StartsWith("ADSLoanOriginatorID")).Substring(38).Trim();
                OriginatorCompanyNMLS =
                    fannieDataLines.First(l => l.StartsWith("ADSLoanOriginationCompanyID")).Substring(38).Trim();
                OriginatorCompanyStreet = brokerString.Substring(117, 70).Trim();
                OriginatorCompanyCity = brokerString.Substring(187, 35).Trim();
                OriginatorCompanyState = brokerString.Substring(222, 2);
                OriginatorCompanyZip = brokerString.Substring(224, 9).Trim();
            }

            HasFannieData = true;
        }

        private static DateTime ParseDate(string dateRaw)
        {
            DateTime dt;
            DateTime.TryParseExact(dateRaw, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            return dt;
        }

        private static string PrettyPhoneNum(string number)
        {
            return number.Length != 10 ? number : number.Insert(3, "-").Insert(7, "-");
        }

        private static string PrettySSN(string ssn)
        {
            return ssn.Length != 9 ? ssn : ssn.Insert(3, "-").Insert(6, "-");
        }

        private static string CombineCityStateZip(string city, string state, string zip)
        {
            return String.Concat(city, ", ", state, " ", zip);
        }

    }
}
