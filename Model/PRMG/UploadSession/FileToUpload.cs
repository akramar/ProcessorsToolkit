﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProcessorsToolkit.ViewModel.PRMGUploadWindow;

namespace ProcessorsToolkit.Model.UploadSession.PRMG
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
        //public string ContainerKey {get;set;}
        // public string PRMGLoanNum {get;set;}
        //public string PRMGBorrName {get; set;}
        //public LoanSearchResultItem MatchedLoan { get; set; }
        //public string[] SchemaIds {get;set;}
        public string DocTypeId //This needs to be turned into a translator from DocSchemaIds
        {
            get
            {
                switch (UploadWindowVM.SelectedUploadType)
                {
                    case UploadWindowVM.UploadTypes.Submission:
                        return UploadWindowVM.DocSchemaIds.FirstOrDefault(dsi => dsi.Value == "-1").Key;

                    case UploadWindowVM.UploadTypes.PTDConditions:
                        return UploadWindowVM.DocSchemaIds.FirstOrDefault(dsi => dsi.Value == "0").Key;
                }
                return "";
            }
        }

        public
            System.Windows.Media.Brush ListBoxTextColor
        {
            get
            {
                switch (this.UploadProgress)
                {
                    case FileUploadStages.Unstarted:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(00, 00, 00));
                    case FileUploadStages.Started:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(80, 54, 241));
                    case FileUploadStages.Completed:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(00, 166, 81));
                    case FileUploadStages.Failed:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(247, 00, 00));
                    default:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(00, 00, 00));
                }
            }
        }

        public FileToUpload()
        {
            this.Id = Guid.NewGuid().ToString();
            this.UploadProgress = FileUploadStages.Unstarted;
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
