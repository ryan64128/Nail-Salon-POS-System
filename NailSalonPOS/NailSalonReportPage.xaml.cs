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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLogicController;

namespace NailSalonWPF
{
    /// <summary>
    /// Interaction logic for NailSalonReportPage.xaml
    /// </summary>
    public partial class NailSalonReportPage : Window
    {
        BusinessLogic bl;
        public NailSalonReportPage(BusinessLogic bl)
        {
            this.bl = bl;
            InitializeComponent();
            startDatePicker.SelectedDate = DateTime.Today;
            customStartDatePicker.SelectedDate = DateTime.Today;
            customEndDatePicker.SelectedDate = DateTime.Today;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            TextBoxReport.Text = bl.processGetDailySalesReport((DateTime)startDatePicker.SelectedDate).toString();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            TextBoxReport.Text = bl.processGetHourlySalesReport((DateTime)startDatePicker.SelectedDate).ToString();
        }

        private void Button_Click_Generate_Custom(object sender, RoutedEventArgs e)
        {
            TextBoxReport.Text = bl.processGetCustomSalesReport((DateTime)customStartDatePicker.SelectedDate, (DateTime)customEndDatePicker.SelectedDate).toString();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
