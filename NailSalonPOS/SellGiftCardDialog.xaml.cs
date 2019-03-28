using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BusinessLogicController;

namespace NailSalonWPF
{
    /// <summary>
    /// Interaction logic for SellGiftCardDialog.xaml
    /// </summary>
    public partial class SellGiftCardDialog : Window
    {
        BusinessLogic bl;

        public SellGiftCardDialog(BusinessLogic bl)
        {
            InitializeComponent();
            this.bl = bl;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_0(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = AmountTextBox.Text + "0";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = AmountTextBox.Text + "1";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = AmountTextBox.Text + "2";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = AmountTextBox.Text + "3";
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = AmountTextBox.Text + "4";
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = AmountTextBox.Text + "5";
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = AmountTextBox.Text + "6";
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = AmountTextBox.Text + "7";
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = AmountTextBox.Text + "8";
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = AmountTextBox.Text + "9";
        }
    }
}
