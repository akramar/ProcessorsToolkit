using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProcessorsToolkit.Model;
using ProcessorsToolkit.Model.Tools;

namespace ProcessorsToolkit.View.Tools
{
    /// <summary>
    /// Interaction logic for CreateConditionWindow.xaml
    /// </summary>
    public partial class CreateConditionWindow : Window
    {
        public CreateConditionWindow()
        {
            InitializeComponent();
        }

        private void CondNumTbOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var newNum = FileBase.StripNonNumbers(CondNumTB.Text);

            CondNumTB.Text = newNum == null ? "" : newNum.ToString();
        }


        private void CondNumTB_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var newNum = FileBase.StripNonNumbers(CondNumTB.Text);

            CondNumTB.Text = newNum == null ? "000" : ((int) newNum).ToString("D3");
        }


        private void SaveAndCreate_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            var condText = CondTextTB.Text;
            var condNum = Convert.ToInt32(CondNumTB.Text);
            var cc = new ConditionCreator();
            cc.CreateCondition(condNum, condText);

            CondNumTB.Text = "";
            CondTextTB.Text = "";
            CondNumTB.Focus();
        }

        private void Close_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            Close();
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            e.Handled = !IsTextAllowed(e.Text);

        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void CondNumTB_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CondNumTB.SelectAll();
        }

    }
}
