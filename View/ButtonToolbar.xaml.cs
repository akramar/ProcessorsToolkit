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
using ProcessorsToolkit.Model;
using ProcessorsToolkit.ViewModel;

namespace ProcessorsToolkit.View
{
    /// <summary>
    /// Interaction logic for ButtonToolbar.xaml
    /// </summary>
    public partial class ButtonToolbar : UserControl
    {

        public ButtonToolbarUCVM CurrButtonToolbarUCVM;


        public ButtonToolbar()
        {
            CurrButtonToolbarUCVM = new ButtonToolbarUCVM();

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrButtonToolbarUCVM.FillPDFButtonClicked();
        }
    }
}
