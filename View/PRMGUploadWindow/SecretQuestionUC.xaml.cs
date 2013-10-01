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
using ProcessorsToolkit.ViewModel.PRMGUploadWindow;

namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for SecretQuestionUC.xaml
    /// </summary>
    public partial class SecretQuestionUC : UserControl
    {
        //private UploadWindowVM _parentVM;


        public SecretQuestionUC()//UploadWindowVM parentVM)
        {
            //_parentVM = parentVM;

            InitializeComponent();
            Loaded += SecretQuestionRequest_Loaded;


        }

        private void SecretQuestionRequest_Loaded(object sender, RoutedEventArgs e)
        {

            secretQTB.Text = UploadWindowVM.CurrPRMGSession.SecretQuestionQuestion;
            secretAnswerTB.Focus();
            //secretAnswerTB.Text = "apples";
            Cursor = Cursors.Arrow;
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            UploadWindowVM.CurrPRMGSession.SecretQuestionAnswer = secretAnswerTB.Text;

            UploadWindowVM.OnReceivedSecretAnswer();
        }
    }
}
