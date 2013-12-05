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
using ProcessorsToolkit.ViewModel.PRMG;

namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for SecretQuestionUC.xaml
    /// </summary>
    public partial class SecretQuestionUC : UserControl
    {
        private UploadWindowVM _parentVM;

        public SecretQuestionUC(UploadWindowVM parentVM)
        {
            _parentVM = parentVM;

            InitializeComponent();
            Loaded += SecretQuestionRequest_Loaded;
        }

        private void SecretQuestionRequest_Loaded(object sender, RoutedEventArgs e)
        {
            secretQTB.Text = _parentVM.CurrPRMGSession.SecretQuestionQuestion;
            secretAnswerTB.Focus();
            Cursor = Cursors.Arrow;
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            _parentVM.CurrPRMGSession.SecretQuestionAnswer = secretAnswerTB.Text;
            _parentVM.OnReceivedSecretAnswer();
        }
    }
}
