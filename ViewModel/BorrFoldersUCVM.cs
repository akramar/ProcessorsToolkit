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
        public ObservableCollection<BorrDir> BorrDirs { get; set; }
        public BorrDir SelectedBorrDir { get { return BorrDirs.FirstOrDefault(ld => ld.IsCurrentBorr); } }
        public BorrFoldersUC View { get; set; }

        public event EventHandler<SelectedBorrChangedEventArgs> SelectedBorrChanged;
        public event EventHandler<SelectedPathChangedEventArgs> SelectedPathChanged;
        public event EventHandler AvailableFoldersUpdated;


        public BorrFoldersUCVM()
        {
            BorrDirs = new ObservableCollection<BorrDir>();
            //LoadBorrFolders();

            AvailableFoldersUpdated += MainWindowVM_AvailableFoldersUpdated;

            //SelectedPathChanged += (o, args) => OnSelectedPathChanged(args); // infinite loop!
        }


        public virtual void OnSelectedPathChanged(SelectedPathChangedEventArgs e)
        {
            var handler = SelectedPathChanged;
            if (handler != null) handler(this, e);
        }

        public virtual void OnSelectedBorrChanged(SelectedBorrChangedEventArgs e)
        {
            if (e != null && e.CurrBorrDir != null)
                MainWindowVM.SelectedBorrDir = e.CurrBorrDir;

            var handler = SelectedBorrChanged;
            if (handler != null) handler(this, e);
        }


        void MainWindowVM_AvailableFoldersUpdated(object sender, EventArgs e)
        {
            BorrDirs = new ObservableCollection<BorrDir>();
            LoadBorrFolders();
            View.RefreshFolderList();
        }
        
        public void LoadBorrFolders()
        {

            //var modal = new DemoTree();

            //modal.Show();


            var availableDirs = Properties.Settings.Default.AvailableFolders;

            foreach (var loanFolderSrc in availableDirs)
            {
                foreach (var borrFolder in Directory.GetDirectories(loanFolderSrc))
                {
                    var dirInfo = new DirectoryInfo(borrFolder);
                    var borrName = dirInfo.Name;

                    if (!borrName.StartsWith("_") && !borrName.StartsWith("."))
                        BorrDirs.Add(new BorrDir(borrName, borrFolder));

                }
            }

            //TODO: sort

            //
            //this.listView1.DataContext = entries;

        }

        public void SetNewBorrDir(BorrDir clickedBorrDir)
        {
            foreach (var dir in BorrDirs.Where(t => t.IsCurrentBorr))
                dir.IsCurrentBorr = false;

            foreach (var dir in BorrDirs.Where(t => t.SubDirs != null))
                dir.SubDirs = null;

            BorrDirs.First(dir => dir == clickedBorrDir).IsCurrentBorr = true;

            BorrDirs.First(dir => dir == clickedBorrDir).LoadSubDirs(); //unnecessary comparing?

            OnSelectedBorrChanged(new SelectedBorrChangedEventArgs {CurrBorrDir = SelectedBorrDir});
            
            OnSelectedPathChanged(new SelectedPathChangedEventArgs {CurrPath = SelectedBorrDir.FullRootPath});
        }

        public void SetNewBorrSubDir(BorrSubDir borrSubDir)
        {
            foreach (var sd in SelectedBorrDir.SubDirs.Where(sd => sd.IsOpen))
                sd.IsOpen = false;

            SelectedBorrDir.SubDirs.First(sd => sd.FolderName == borrSubDir.FolderName).IsOpen = true;
            
            System.Diagnostics.Debug.WriteLine("Actual path: " + borrSubDir.Fullpath);
            System.Diagnostics.Debug.WriteLine("Calculated path: " + SelectedBorrDir.FullActivePath);

            OnSelectedPathChanged(new SelectedPathChangedEventArgs { CurrPath = SelectedBorrDir.FullActivePath });
        }

        public void OnAvailableFoldersUpdated()
        {
            var handler = AvailableFoldersUpdated;
            if (handler != null)
                handler(null, EventArgs.Empty);

            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            MainWindowVM.OnClearBorrSelected();
            borrFoldersUCVM.OnSelectedBorrChanged(new SelectedBorrChangedEventArgs {CurrBorrDir = null});

            borrFoldersUCVM.OnSelectedPathChanged(new SelectedPathChangedEventArgs {CurrPath = null});
        }
    }
}
