using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ProcessorsToolkit.Model;
using ProcessorsToolkit.View;

namespace ProcessorsToolkit.ViewModel
{
    public class BorrFoldersUCVM
    {
        public event EventHandler<SelectedBorrChangedEventArgs> SelectedBorrChanged;
        public event EventHandler<SelectedDirChangedEventArgs> SelectedPathChanged;


        protected virtual void OnSelectedPathChanged(SelectedDirChangedEventArgs e)
        {
            var handler = SelectedPathChanged;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnSelectedBorrChanged(SelectedBorrChangedEventArgs e)
        {
            var handler = SelectedBorrChanged;
            if (handler != null) handler(this, e);
        }


        public ObservableCollection<BorrDir> BorrDirs { get; set; }

        public BorrDir SelectedBorr
        {
            get { return BorrDirs.First(ld => ld.IsCurrentBorr); }
        }

        public BorrFoldersUCVM()
        {


            BorrDirs = new ObservableCollection<BorrDir>();

            LoadBorrFolders();

        }

        public void LoadBorrFolders()
        {

            //var modal = new DemoTree();

            //modal.Show();

            var loanFolderSrc = @"C:\Users\Alain Kramar\Documents\Loans";

            foreach (var borrFolder in Directory.GetDirectories(loanFolderSrc))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(borrFolder);
                var borrName = dirInfo.Name;

                if (!borrName.StartsWith("_") && !borrName.StartsWith("."))
                    BorrDirs.Add(new BorrDir(borrName, borrFolder));

            }


            //this.listView1.DataContext = entries;

        }

        public void SetNewBorrDir(BorrDir borrDir)
        {
            foreach (var ld in BorrDirs.Where(t => t.IsCurrentBorr))
                ld.IsCurrentBorr = false;

            foreach (var ld in BorrDirs.Where(t => t.SubDirs != null))
                ld.SubDirs = null;


            BorrDirs.First(ld => ld == borrDir).IsCurrentBorr = true;

            BorrDirs.First(ld => ld == borrDir).LoadSubDirs(); //unnecessary comparing?

            OnSelectedBorrChanged(new SelectedBorrChangedEventArgs {CurrBorr = SelectedBorr});

            
            OnSelectedPathChanged(new SelectedDirChangedEventArgs { CurrPath = SelectedBorr.FullRootPath });

        }

        public void SetNewBorrSubDir(BorrSubDir borrSubDir)
        {
            foreach (var sd in SelectedBorr.SubDirs.Where(sd => sd.IsOpen))
                sd.IsOpen = false;

            SelectedBorr.SubDirs.First(sd => sd.FolderName == borrSubDir.FolderName).IsOpen = true;


            //var subDir = BorrDir.GetSubDir(SelectedBorrDir.BorrName, e.CurrPath);

            // if (!String.IsNullOrEmpty(subDir))
            //     SelectedBorrDir.SubDirs.First(sd => sd.FolderName == subDir).IsOpen = true;

            //SelectedFolderPath = e.CurrPath;
            //e.CurrPath = SelectedBorrDir.FullActivePath;

            System.Diagnostics.Debug.WriteLine("Actual path: " + borrSubDir.Fullpath);
            System.Diagnostics.Debug.WriteLine("Calculated path: " + SelectedBorr.FullActivePath);

            OnSelectedPathChanged(new SelectedDirChangedEventArgs { CurrPath = SelectedBorr.FullActivePath });
        }

        
    }
}
