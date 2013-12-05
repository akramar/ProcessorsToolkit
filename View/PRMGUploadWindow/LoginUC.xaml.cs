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
using ProcessorsToolkit.ViewModel.PRMG;

namespace ProcessorsToolkit.View.PRMGUploadWindow
{
    /// <summary>
    /// Interaction logic for PRMGLoginUC.xaml
    /// </summary>
    public partial class LoginUC : UserControl
    {
        private readonly UploadWindowVM _parentVM;

        public LoginUC(UploadWindowVM parentVM)
        {
            _parentVM = parentVM; //TODO: Change this into using datacontext like we do with Interbank login UC

            InitializeComponent();

            if (Properties.Settings.Default.Credentials == null)
            {
                Properties.Settings.Default.Credentials = new List<SettingTypes.LenderLogin>();
                Properties.Settings.Default.Save();
            }

            var availLogins = Properties.Settings.Default.Credentials;

            var prmgLogin = availLogins.FirstOrDefault(l => l.LenderId == "PRMG");

            if (prmgLogin != null)
            {
                UsernameTB.Text = prmgLogin.Username;
                PasswordTB.Password = prmgLogin.Password;
                SaveCredCB.IsChecked = true;
            }
        }

        private void InitialConnect_Click(object sender, RoutedEventArgs e)
        {
            var saveCred = SaveCredCB.IsChecked != null && (bool) SaveCredCB.IsChecked;

            var availLogins = Properties.Settings.Default.Credentials;
            if (availLogins != null)
            {
                if (saveCred)
                {
                    if (availLogins.Any(l => l.LenderId == "PRMG"))
                    {
                        availLogins.First(l => l.LenderId == "PRMG").Username = UsernameTB.Text;
                        availLogins.First(l => l.LenderId == "PRMG").Password = PasswordTB.Password;
                    }
                    else
                    {
                        var newLogin = new SettingTypes.LenderLogin()
                            {
                                LenderId = "PRMG",
                                Username = UsernameTB.Text,
                                Password = PasswordTB.Password
                            };
                        Properties.Settings.Default.Credentials.Add(newLogin);
                    }
                }

                else
                {
                    availLogins.RemoveAll(l => l.LenderId == "PRMG");
                }

                Properties.Settings.Default.Save();
            }

            _parentVM.OnReceivedPRMGCredentials(UsernameTB.Text, PasswordTB.Password);
        }
    }
}


