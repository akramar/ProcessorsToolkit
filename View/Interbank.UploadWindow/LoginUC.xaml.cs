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
using ProcessorsToolkit.ViewModel.Interbank;

namespace ProcessorsToolkit.View.Interbank.UploadWindow
{
    /// <summary>
    /// Interaction logic for LoginUC.xaml
    /// </summary>
    public partial class LoginUC : UserControl
    {
        public LoginUC()
        {
            
            InitializeComponent();


            if (Properties.Settings.Default.Credentials == null)
            {
                Properties.Settings.Default.Credentials = new List<SettingTypes.LenderLogin>();
                Properties.Settings.Default.Save();
            }

            var availLogins = Properties.Settings.Default.Credentials;

            var ibwLogin = availLogins.FirstOrDefault(l => l.LenderId == "IBW");

            if (ibwLogin != null)
            {
                UsernameTB.Text = ibwLogin.Username;
                PasswordTB.Password = ibwLogin.Password;
                SaveCredCB.IsChecked = true;
            }
        }

        private void InitialConnect_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as UploadWindowVM;
            if (vm == null)
                return;

            var saveCred = SaveCredCB.IsChecked != null && (bool)SaveCredCB.IsChecked;

            var availLogins = Properties.Settings.Default.Credentials;
            if (availLogins != null)
            {
                if (saveCred)
                {
                    if (availLogins.Any(l => l.LenderId == "IBW"))
                    {
                        availLogins.First(l => l.LenderId == "IBW").Username = UsernameTB.Text;
                        availLogins.First(l => l.LenderId == "IBW").Password = PasswordTB.Password;
                    }
                    else
                    {
                        var newLogin = new SettingTypes.LenderLogin()
                        {
                            LenderId = "IBW",
                            Username = UsernameTB.Text,
                            Password = PasswordTB.Password
                        };
                        Properties.Settings.Default.Credentials.Add(newLogin);
                    }
                }

                else
                {
                    availLogins.RemoveAll(l => l.LenderId == "IBW");
                }

                Properties.Settings.Default.Save();
            }

            vm.OnReceivedCredentials(UsernameTB.Text, PasswordTB.Password);
        }
    }
}
