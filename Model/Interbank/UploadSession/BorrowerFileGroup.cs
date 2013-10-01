using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using ProcessorsToolkit.ViewModel;
using ProcessorsToolkit.ViewModel.InterbankUploadWindow;



namespace ProcessorsToolkit.Model.Interbank.UploadSession
{
    public class BorrowerFileGroup : ObservableCollection<FileToUpload>
        // System.Collections.Concurrent.BlockingCollection<FileToUpload> //  List<FileToUpload>
    {
    }
}
