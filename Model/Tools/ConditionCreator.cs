using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.Model.Tools
{
    class ConditionCreator
    {

        private Window _owningWindow;
        private readonly BorrSubDir _folderToUse;

        //public ConditionCreator(Window owningWindow, BorrSubDir folderToUse = null)
        public ConditionCreator(BorrSubDir folderToUse = null)
        {
            //_owningWindow = owningWindow;
            _folderToUse = folderToUse;

            if (folderToUse == null)
                _folderToUse = CreateConditionFolder();
        }

        public void CreateCondition(int conditionNum, string conditionText)
        {
            var newFilename = conditionNum.ToString("D3") + " - " + FileBase.StripIllegalChars(conditionText) + ".txt";
            var newFilePath = _folderToUse.Fullpath + "\\" + newFilename;

            if (File.Exists(newFilePath))
                return;

            File.Create(newFilePath);
        }

        private static BorrSubDir CreateConditionFolder()
        {
            var newFolderName = "conditions-" + DateTime.Now.ToString("MM.dd.yy");
            var newCondFolder = new BorrSubDir(newFolderName, MainWindowVM.SelectedBorrDir.FullRootPath + "\\" + newFolderName);
            newCondFolder.CreateIfNotExists();
            return newCondFolder;
        }

        private List<BorrSubDir> GetCurrentConditionFolders()
        {
            return MainWindowVM.SelectedBorrDir.SubDirs.Where(sd => sd.FolderName.StartsWith("conditions")).ToList();

        }




    }
}
