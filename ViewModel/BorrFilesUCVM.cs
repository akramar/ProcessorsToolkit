using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ProcessorsToolkit.Annotations;
using ProcessorsToolkit.Model;
using ProcessorsToolkit.View;
using System.Threading;

namespace ProcessorsToolkit.ViewModel
{
    public class BorrFilesUCVM : INotifyPropertyChanged
    {
        public event EventHandler<SelectedFileChangedEventArgs> SelectedFileChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        
        public ObservableCollection<FileBase> FolderFiles { get; set; }
        public BorrDir SelectedBorrDir { get; set; }
        private BorrDirWatcher _borrDirWatcher;
        private Thread _borrDirWatcherThread;
        public BorrFilesUC View { get; set; }
        public FileBase SelectedFile { get; set; }

        
        private bool _isPDFSelected;
        public bool IsPDFSelected
        {
            get { return _isPDFSelected; }
            set { _isPDFSelected = value; OnPropertyChanged("IsPDFSelected"); }
        }

        private bool _isPDFableFileSelected;
        public bool IsPDFableFileSelected
        {
            get { return _isPDFableFileSelected; }
            set { _isPDFableFileSelected = value; OnPropertyChanged("IsPDFableFileSelected"); }
        }
        

        public BorrFilesUCVM()
        {
            FolderFiles = new ObservableCollection<FileBase>();

            //MainWindowVM.SelectedDirPathChanged += (sender, args) => LoadBorrFiles();

            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            MainWindowVM.View.Loaded += delegate(object sender, RoutedEventArgs args)
                {
                    var borrFilesUCVM = MainWindowVM.BorrFilesCtrl.DataContext as BorrFilesUCVM;
                    if (borrFilesUCVM != null)
                        borrFilesUCVM.SelectedFileChanged += borrFilesUCVM_SelectedFileChanged;

                };

            borrFoldersUCVM.SelectedBorrChanged += (sender, args) =>
                {
                    if (args != null)
                        SelectedBorrDir = args.CurrBorrDir;
                };

            borrFoldersUCVM.SelectedPathChanged += (sender, args) => LoadBorrFiles();

            //FolderFiles.CollectionChanged += FolderFiles_CollectionChanged;

        }

        private void borrFilesUCVM_SelectedFileChanged(object sender, SelectedFileChangedEventArgs e)
        {
            
            if (e == null || e.CurrFile == null)
            {
                IsPDFSelected = false;
                IsPDFableFileSelected = false;
            }

            else
            {
                IsPDFSelected = e.CurrFile.IsPDF;
                IsPDFableFileSelected = e.CurrFile.CanBePDFed;
            }
             
        }

        public virtual void OnSelectedFileChanged(SelectedFileChangedEventArgs e)
        {
            if (e != null)
                SelectedFile = e.CurrFile;
            var handler = SelectedFileChanged;
            if (handler != null) handler(this, e);
        }

        public void LoadBorrFiles()
        {
            /*
            var borrInfoUCVM = MainWindowVM.BorrInfoCtrl.DataContext as BorrInfoUCVM;
            if (borrInfoUCVM == null)
                return;


            FolderFiles.CollectionChanged -= (sender, args) => {
                                                                   if (borrInfoUCVM != null)
                                                                       borrInfoUCVM.FindAndFillFannieModel();
            }; ;
            */
            if (_borrDirWatcher != null)
            {
                _borrDirWatcher.DoReload -= BorrDirWatcherOnDoReload;
                _borrDirWatcher = null;
            }
            if (_borrDirWatcherThread != null)
                _borrDirWatcherThread.Join();

            FolderFiles.CollectionChanged -= FolderFiles_CollectionChanged;

            lock (FolderFiles)
            {

                if (FolderFiles.Count > 0)
                    FolderFiles.Clear();
                //FolderFiles = new ObservableCollection<FileBase>();

                var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
                if (borrFoldersUCVM == null)
                    return;

                if (MainWindowVM.SelectedBorrDir == null)
                    return;

                foreach (var filePath in Directory.GetFiles(MainWindowVM.SelectedBorrDir.FullActivePath))
                {
                    var fileInfo = new FileInfo(filePath);

                    try
                    {
                        FolderFiles.Add(new FileBase
                            {
                                LastModified = fileInfo.LastWriteTime,
                                Fullpath = filePath,
                                Imagepath = "",
                                DisplayName = fileInfo.Name,
                                SizeKB = (fileInfo.Length/1024).ToString(CultureInfo.InvariantCulture) + "KB",
                                //IsBeingEdited = true
                            });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
            }

            _borrDirWatcherThread = new Thread(() =>
                {
                    _borrDirWatcher = new BorrDirWatcher(MainWindowVM.SelectedBorrDir.FullActivePath);
                    _borrDirWatcher.DoReload += BorrDirWatcherOnDoReload;
                }) {Name = "BorrDirWatcher"};
            _borrDirWatcherThread.Start();

            FolderFiles.CollectionChanged += FolderFiles_CollectionChanged;
            //FolderFiles.CollectionChanged += (o, args) => borrInfoUCVM.FindAndFillFannieModel();

        }

        public void FolderFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LoadBorrFiles();

            var borrInfoUCVM = MainWindowVM.BorrInfoCtrl.DataContext as BorrInfoUCVM;
            if (borrInfoUCVM == null)
                return;

            borrInfoUCVM.FindAndFillFannieModel();

            
        }

        private void BorrDirWatcherOnDoReload(object sender, EventArgs eventArgs)
        {
            //This makes the Action on the UI thread (cause it's the VM of the BorrFilesUC)
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action) LoadBorrFiles);
        }

        /*
        public void FileSelectionChanged(FileBase selectedFile, bool isChecked)
        {

            foreach (var folderFile in FolderFiles.Where(f => f.Fullpath == selectedFile.Fullpath))
                folderFile.IsSelected = true;
        }
        */

        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
