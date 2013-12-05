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
using ProcessorsToolkit.ViewModel.Plaza;

namespace ProcessorsToolkit.View.Plaza.UploadWindow
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

            var plazaLogin = availLogins.FirstOrDefault(l => l.LenderId == "PHM");

            if (plazaLogin != null)
            {
                CompanyIdTB.Text = plazaLogin.CompanyId;
                UserIdTB.Text = plazaLogin.Username;
                PasswordTB.Password = plazaLogin.Password;
                UserEmlTB.Text = plazaLogin.Email;
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
                    if (availLogins.Any(l => l.LenderId == "PHM"))
                    {
                        availLogins.First(l => l.LenderId == "PHM").CompanyId = CompanyIdTB.Text;
                        availLogins.First(l => l.LenderId == "PHM").Username = UserIdTB.Text;
                        availLogins.First(l => l.LenderId == "PHM").Password = PasswordTB.Password;
                        availLogins.First(l => l.LenderId == "PHM").Email = UserEmlTB.Text;
                    }
                    else
                    {
                        var newLogin = new SettingTypes.LenderLogin
                            {
                            LenderId = "PHM",
                            CompanyId = CompanyIdTB.Text,
                            Username = UserIdTB.Text,
                            Password = PasswordTB.Password,
                            Email = UserEmlTB.Text
                        };
                        Properties.Settings.Default.Credentials.Add(newLogin);
                    }
                }

                else
                {
                    availLogins.RemoveAll(l => l.LenderId == "PHM");
                }

                Properties.Settings.Default.Save();
            }

            vm.OnReceivedCredentials(CompanyIdTB.Text, UserIdTB.Text, PasswordTB.Password, UserEmlTB.Text);
        }
    }
}
