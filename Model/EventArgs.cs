using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessorsToolkit.Model
{

    public class SelectedBorrChangedEventArgs : EventArgs
    {
        public BorrDir CurrBorrDir { get; set; }
    }

    public class SelectedPathChangedEventArgs : EventArgs
    {
        public string CurrPath { get; set; }
    }
    public class SelectedBorrDataChangedEventArgs : EventArgs
    {
        public FannieData CurrData { get; set; }
    }

    public class SelectedFileChangedEventArgs : EventArgs
    {
        public FileBase CurrFile { get; set; }
    }
}
