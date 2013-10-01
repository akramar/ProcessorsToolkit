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
using ProcessorsToolkit.Model.UploadSession.PRMG;
using ProcessorsToolkit.ViewModel.PRMGUploadWindow;

namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for PRMGLoginUC.xaml
    /// </summary>
    public partial class PRMGLoginUC : UserControl
    {
        //private readonly UploadWindowVM _parentVM;

        public PRMGLoginUC()//UploadWindowVM parentVM)
        {
            //_parentVM = parentVM;


            InitializeComponent();
            usernameTB.Text = "alakra";
            passwordTB.Password = "ynaIXFz3dfdJf!";

        }

        private void InitialConnect_Click(object sender, RoutedEventArgs e)
        {
            //if (UploadWindowVM.CurrImgFlowSession == null)
            //    UploadWindowVM.CurrImgFlowSession = new ImageFlowSession();


           // UploadWindowVM.CurrPRMGSession.Username = usernameTB.Text;
            //UploadWindowVM.CurrPRMGSession.Password = passwordTB.Password;

            //var handler = UploadWindowVM.ReceivedPRMGCredentials;
            //if (handler != null) handler(this, EventArgs.Empty);


            UploadWindowVM.OnReceivedPRMGCredentials(usernameTB.Text, passwordTB.Password);
        }
    }
}


