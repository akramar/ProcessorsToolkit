using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ProcessorsToolkit.Model;
using ProcessorsToolkit.View;

namespace ProcessorsToolkit.ViewModel
{
    public class BorrInfoUCVM : ObservableObject
    {
        public event EventHandler<SelectedBorrDataChangedEventArgs> SelectedBorrDataChanged;

        //protected BorrDir SelectedBorrDir { get; set; } //this is duplicate

        public FannieData BorrData { get; set; }
        public BorrInfoUC View { get; set; }
        
        public BorrInfoUCVM()
        {
            BorrData = new FannieData();

            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;
            
            borrFoldersUCVM.SelectedBorrChanged += (sender, args) => FindAndFillFannieModel();
        }

        public void OnSelectedBorrDataChanged(SelectedBorrDataChangedEventArgs e)
        {
            if (e != null)
                BorrData = e.CurrData; //re-assigning this is kind of redudant, but want to make sure BorrData gets assigned 
            var handler = SelectedBorrDataChanged;
            if (handler != null) handler(null, e);
        }

        internal void FindAndFillFannieModel()
        {
            BorrData = new FannieData();
            var eventArgs = new SelectedBorrDataChangedEventArgs();

            var borrFoldersUCVM = MainWindowVM.BorrFoldersCtrl.DataContext as BorrFoldersUCVM;
            if (borrFoldersUCVM == null)
                return;

            if (borrFoldersUCVM.SelectedBorrDir != null)
            {

                var latestFannieFile = Directory.GetFiles(borrFoldersUCVM.SelectedBorrDir.FullRootPath)
                                         .Where(file => file.EndsWith(".fnm", true, CultureInfo.InvariantCulture))
                                         .Select(filePath => new FileInfo(filePath))
                                         .OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
                if (latestFannieFile != null)
                    BorrData.LoadFannieFile(latestFannieFile);

                eventArgs.CurrData = BorrData;
            }
            
            OnSelectedBorrDataChanged(eventArgs);
        }
        
    }
}
