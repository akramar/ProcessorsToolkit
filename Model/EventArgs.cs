using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessorsToolkit.Model
{

    public class SelectedBorrChangedEventArgs : EventArgs
    {
        public BorrDir CurrBorr { get; set; }
    }

    public class SelectedDirChangedEventArgs : EventArgs
    {
        public string CurrPath { get; set; }
    }
    public class SelectedBorrDataChangedEventArgs : EventArgs
    {
        public FannieData CurrData { get; set; }
    }
}
