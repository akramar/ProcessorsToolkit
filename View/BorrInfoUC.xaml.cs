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
        

        public BorrInfoUCVM CurrBorrInfoVM;

        public BorrInfoUC()
        {
            CurrBorrInfoVM = new BorrInfoUCVM();
            //CurrBorrInfoVM.BorrData.BorrNameFirst = "none";

            InitializeComponent();

            MainInfoSP.DataContext = CurrBorrInfoVM.BorrData;

            MainWindowVM.SelectedBorrDataChanged += BorrInfoVM_BorrInfoUpdated;



        }

        void BorrInfoVM_BorrInfoUpdated(object sender, EventArgs e)
        {
            MainInfoSP.DataContext = null;
            MainInfoSP.DataContext = CurrBorrInfoVM.BorrData;

        }
    }
}
