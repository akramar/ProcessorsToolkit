using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ProcessorsToolkit.Model;

namespace ProcessorsToolkit.ViewModel
{
    public class BorrInfoUCVM : ObservableObject
    {
        protected BorrDir SelectedBorrDir { get; set; }
        public FannieData BorrData { get; set; }

        /*public static event EventHandler<SelectedBorrDataChangedEventArgs> BorrInfoUpdated;

        private static void OnBorrInfoUpdated(SelectedBorrDataChangedEventArgs e)
        {
            var handler = BorrInfoUpdated;
            if (handler != null) handler(null, e);
        }
        */

        public BorrInfoUCVM()
        {
            BorrData = new FannieData();

            MainWindowVM.SelectedBorrChanged += (sender, args) =>
                {
                    SelectedBorrDir = args.CurrBorr;
                    FindAndFillFannieModel();
                };
        }

        private void FindAndFillFannieModel()
        {
            BorrData = new FannieData();

            var latestFNM = Directory.GetFiles(SelectedBorrDir.FullRootPath)
                                         .Where(file => file.EndsWith(".fnm", true, CultureInfo.InvariantCulture))
                                         .Select(filePath => new FileInfo(filePath))
                                         .OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
            if (latestFNM != null)
                BorrData.LoadFannieFile(latestFNM);

            var eventArgs = new SelectedBorrDataChangedEventArgs {CurrData = BorrData};

           // OnBorrInfoUpdated(eventArgs);

            MainWindowVM.OnSelectedBorrDataChanged(eventArgs);
        }


    }
}
