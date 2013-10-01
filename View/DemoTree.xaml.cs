using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProcessorsToolkit.View
{
    /// <summary>
    /// Interaction logic for DemoTree.xaml
    /// </summary>
    public partial class DemoTree : Window
    {
        public DemoTree()
        {

            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        private ObservableCollection<DirectoryEntry> entries = new ObservableCollection<DirectoryEntry>();
        private ObservableCollection<DirectoryEntry> subEntries = new ObservableCollection<DirectoryEntry>();

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string s in Directory.GetLogicalDrives())
            {
                DirectoryEntry d = new DirectoryEntry(s, s, "<Driver>", "<DIR>", Directory.GetLastWriteTime(s),
                                                      "Images/dir.gif", EntryType.Dir);
                entries.Add(d);
            }
            this.listView1.DataContext = entries;

        }

        private void listViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = e.Source as ListViewItem;

            DirectoryEntry entry = item.DataContext as DirectoryEntry;

            if (entry.Type == EntryType.Dir)
            {
                subEntries.Clear();

                foreach (string s in Directory.GetDirectories(entry.Fullpath))
                {
                    DirectoryInfo dir = new DirectoryInfo(s);
                    DirectoryEntry d = new DirectoryEntry(
                        dir.Name, dir.FullName, "<Folder>", "<DIR>",
                        Directory.GetLastWriteTime(s),
                        "Images/folder.gif", EntryType.Dir);
                    subEntries.Add(d);
                }
                foreach (string f in Directory.GetFiles(entry.Fullpath))
                {
                    FileInfo file = new FileInfo(f);
                    DirectoryEntry d = new DirectoryEntry(
                        file.Name, file.FullName, file.Extension, file.Length.ToString(),
                        file.LastWriteTime,
                        "Images/file.gif", EntryType.File);
                    subEntries.Add(d);
                }

                listView2.DataContext = subEntries;
            }
        }
    }

    public enum EntryType
    {
        Dir,
        File
    }

    public class DirectoryEntry
    {
        public DirectoryEntry(string name, string fullname, string ext, string size, DateTime date, string imagepath,
                              EntryType type)
        {
            Name = name;
            Fullpath = fullname;
            Ext = ext;
            Size = size;
            Date = date;
            Imagepath = imagepath;
            Type = type;
        }

        public string Name { get; set; }

        public string Ext { get; set; }

        public string Size { get; set; }

        public DateTime Date { get; set; }

        public string Imagepath { get; set; }

        public EntryType Type { get; set; }

        public string Fullpath { get; set; }
    }
}