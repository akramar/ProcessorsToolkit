using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// Interaction logic for OptionsWindow_AddEditDirectory.xaml
    /// </summary>
    public partial class SettingsWindow_AddEditDirectory : Window
    {
        private readonly bool _isNewDirectory;
        private readonly string _originalDirectory;

        /// <summary>
        /// Use this to create a new directory
        /// </summary>
        public SettingsWindow_AddEditDirectory()
        {
            InitializeComponent();
            _isNewDirectory = true;
            DirectoryTB.Focus();
        }


        /// <summary>
        /// Use this to edit an existing directory
        /// </summary>
        public SettingsWindow_AddEditDirectory(string subjectDirectory)
        {
            InitializeComponent();
            DirectoryTB.Text = subjectDirectory;
            DirectoryTB.Focus();
            _isNewDirectory = false;
            _originalDirectory = subjectDirectory;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DirectoryTB.Text == String.Empty)
                return;

            if (_isNewDirectory)
                AddNewDirectory();

            else
                EditExistingDirectory();

            //this.Owner.
            //this.Close();
        }

        private void AddNewDirectory()
        {
            var currentDirectories = Properties.Settings.Default.AvailableFolders;
            var directoryProvided = DirectoryTB.Text.Trim();

            if (currentDirectories.Contains(directoryProvided) == false)
            {
                var directoryInfo = new System.IO.DirectoryInfo(directoryProvided);
                if (directoryInfo.Exists)
                {
                    Properties.Settings.Default.AvailableFolders.Add(directoryProvided);
                    Properties.Settings.Default.Save();
                    Close();
                }
                else
                    ResultLbl.Content = "Doesn't Exist!";
            }
            else
                ResultLbl.Content = "Duplicate Entry!";
        }

        private void EditExistingDirectory()
        {
            Properties.Settings.Default.AvailableFolders.Remove(_originalDirectory);
            Properties.Settings.Default.Save();
            AddNewDirectory();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Alternative: add browser from shell: http://computer-programming-forum.com/4-csharp/29241881400cde8c.htm

            var dlg = new System.Windows.Forms.FolderBrowserDialog {Description = "Browse to loans folder..."};

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //MessageBox.Show(dlg.SelectedPath);

                DirectoryTB.Text = dlg.SelectedPath;
            }

        }

    }
}
