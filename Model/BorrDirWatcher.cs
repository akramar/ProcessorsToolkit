using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProcessorsToolkit.Model
{
    class BorrDirWatcher : FileSystemWatcher
    {
        public event EventHandler DoReload;

        public BorrDirWatcher(string path): base()
        {
            Path = path;
            NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                           | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            Changed += new FileSystemEventHandler(OnChanged);
            Created += new FileSystemEventHandler(OnChanged);
            Deleted += new FileSystemEventHandler(OnChanged);
            Renamed += new RenamedEventHandler(OnRenamed);

            EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var handler = DoReload;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            var handler = DoReload;
            if (handler != null) handler(this, EventArgs.Empty);
        }


    }
}
