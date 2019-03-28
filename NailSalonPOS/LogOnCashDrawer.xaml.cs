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
    /// Interaction logic for LogOnCashDrawer.xaml
    /// </summary>
    public partial class LogOnCashDrawer : Window
    {
        private List<IEmployee> empList;
        public LogOnCashDrawer(BusinessLogic bl)
        {
            InitializeComponent();
            empList = bl.loadEmployeeList();
            for (int i = 0; i < empList.Count; i++)
            {
                EmployeeComboBox.Items.Add(empList.ElementAt(i));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_0(object sender, RoutedEventArgs e)
        {
            PasswordBoxPIN.Password = PasswordBoxPIN.Password + "0";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PasswordBoxPIN.Password = PasswordBoxPIN.Password + "1";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            PasswordBoxPIN.Password = PasswordBoxPIN.Password + "2";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            PasswordBoxPIN.Password = PasswordBoxPIN.Password + "3";
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            PasswordBoxPIN.Password = PasswordBoxPIN.Password + "4";
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            PasswordBoxPIN.Password = PasswordBoxPIN.Password + "5";
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            PasswordBoxPIN.Password = PasswordBoxPIN.Password + "6";
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            PasswordBoxPIN.Password = PasswordBoxPIN.Password + "7";
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            PasswordBoxPIN.Password = PasswordBoxPIN.Password + "8";
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            PasswordBoxPIN.Password = PasswordBoxPIN.Password + "9";
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
