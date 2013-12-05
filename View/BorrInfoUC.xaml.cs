using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.View
{
    /// <summary>
    /// Interaction logic for BorrInfoUC.xaml
    /// </summary>
    public partial class BorrInfoUC : UserControl
    {
        public BorrInfoUC()
        {
            MainWindowVM.BorrInfoCtrl = this;
            DataContext = new BorrInfoUCVM {View = this};

            InitializeComponent();

            var borrInfoUCVM = DataContext as BorrInfoUCVM;
            borrInfoUCVM.SelectedBorrDataChanged += BorrInfoVM_BorrInfoUpdated;
        }
        
        void BorrInfoVM_BorrInfoUpdated(object sender, EventArgs e)
        {
            var borrInfoUCVM = DataContext as BorrInfoUCVM;
            if (borrInfoUCVM == null)
                return;

            MainInfoSP.DataContext = null;
            MainInfoSP.DataContext = borrInfoUCVM;
        }
    }
}
