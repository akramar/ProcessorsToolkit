using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ProcessorsToolkit.Model.PRMG.UploadSession
{
    public class FileToUpload
    {
        public string Id { get; private set; }
        //@"C:\Users\Alain Kramar\Documents\Loans\Clark, Mike & Mary\Clark - Appraisal confirmation.pdf",
        public string PathFull { get; set; }
        public string NameWithExt { get { return System.IO.Path.GetFileName(this.PathFull); } }
        public string NameNoExt { get { return System.IO.Path.GetFileNameWithoutExtension(this.PathFull); } }
        public bool IsSelected { get; set; }
        public FileUploadStages UploadProgress { get; set; }
        public DateTime TimeAdded { get; set; }
        public string ConditionId { get; set; }
        //public string ContainerKey {get;set;}
        // public string PRMGLoanNum {get;set;}
        //public string PRMGBorrName {get; set;}
        //public LoanSearchResultItem MatchedLoan { get; set; }
        //public string[] SchemaIds {get;set;}

        public static string ConditionNumToDocTypeId(string conditionNum) //This needs to be turned into a translator from DocSchemaIds
        {
            return DocSchemaIds.FirstOrDefault(dsi => dsi.Value == conditionNum).Key;
            /*             get
            {
                switch (UploadWindowVM.SelectedUploadType)
                {
                    case UploadWindowVM.UploadTypes.Submission:
                        return UploadWindowVM.DocSchemaIds.FirstOrDefault(dsi => dsi.Value == "-1").Key;

                    case UploadWindowVM.UploadTypes.PTDConditions:
                        return UploadWindowVM.DocSchemaIds.FirstOrDefault(dsi => dsi.Value == "0").Key;
                }
                return "";
            }*/
        }

        public static Dictionary<string, string> DocSchemaIds = new Dictionary<string, string> //AKA doctypeIDS? 
        {            
            // Name the "Conditions" doctype just "Other PTD/PTF" 
            //{"209638", "Submission Pkg"}, {"209654", "Conditions"},
            {"209638", "-1"}, {"209654", "0"},
            {"209681", "1"}, {"209684", "2"}, {"209685", "3"}, {"209686", "4"}, {"209687", "5"}, {"209688", "6"},
            {"209689", "7"}, {"209690", "8"}, {"209691", "9"}, {"209692", "10"}, {"209693", "11"}, {"209694", "12"},
            {"209695", "13"}, {"209696", "14"}, {"209697", "15"}, {"209698", "16"}, {"209699", "17"}, {"209700", "18"},
            {"209701", "19"}, {"209702", "20"}, {"209703", "21"}, {"209704", "22"}, {"209705", "23"}, {"209706", "24"},
            {"209707", "25"}, {"209708", "26"}, {"209709", "27"}, {"209710", "28"}, {"209711", "29"}, {"209712", "30"},
            {"209713", "31"}, {"209714", "32"}, {"209715", "33"}, {"209716", "34"}, {"209717", "35"}, {"209718", "36"},
            {"209719", "37"}, {"209720", "38"}, {"209721", "39"}, {"209722", "40"}, {"209733", "41"}, {"209734", "42"},
            {"209735", "43"}, {"209736", "44"}, {"209737", "45"}, {"209738", "46"}, {"209739", "47"}, {"209740", "48"},
            {"209741", "49"}, {"209742", "50"}, {"209743", "51"}, {"209744", "52"}, {"209745", "53"}, {"209746", "54"},
            {"209747", "55"}
        };

        public Brush ListBoxTextColor
        {
            get
            {
                switch (UploadProgress)
                {
                    case FileUploadStages.Unstarted:
                        return new SolidColorBrush(Color.FromRgb(00, 00, 00));
                    case FileUploadStages.Started:
                        return new SolidColorBrush(Color.FromRgb(80, 54, 241));
                    case FileUploadStages.Completed:
                        return new SolidColorBrush(Color.FromRgb(00, 166, 81));
                    case FileUploadStages.Failed:
                        return new SolidColorBrush(Color.FromRgb(247, 00, 00));
                    default:
                        return new SolidColorBrush(Color.FromRgb(00, 00, 00));
                }
            }
        }

        public FileToUpload()
        {
            Id = Guid.NewGuid().ToString();
            UploadProgress = FileUploadStages.Unstarted;
        }

        public enum FileUploadStages
        {
            Unstarted,
            Started,
            Completed,
            Failed
        }
    }
}
