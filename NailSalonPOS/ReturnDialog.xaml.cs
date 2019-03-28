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
    /// Interaction logic for ReturnDialog.xaml
    /// </summary>
    public partial class ReturnDialog : Window
    {
        private BusinessLogic bl;
        private List<IInvoice> orderList;
        private int currentInvoiceIndex;

        public ReturnDialog(BusinessLogic bl)
        {
            InitializeComponent();
            orderDatePicker.SelectedDate = DateTime.Today;
            this.bl = bl;
            orderList = bl.processGetInvoiceList((DateTime)orderDatePicker.SelectedDate);
            if (orderList != null)
            {
                currentInvoiceIndex = orderList.Count - 1;
                System.Console.WriteLine("*&* invoice count = " + orderList.Count);
                textBoxOrders.Text = orderList.ElementAt(currentInvoiceIndex).toString();
            }
            else
            {
                textBoxOrders.Text = "";
            }

        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_Prev(object sender, RoutedEventArgs e)
        {
            if (orderList != null)
            {
                if (currentInvoiceIndex > 0)
                {
                    currentInvoiceIndex--;
                    textBoxOrders.Text = orderList.ElementAt(currentInvoiceIndex).toString();
                }
            }
        }

        private void Button_Click_Next(object sender, RoutedEventArgs e)
        {
            if (orderList != null)
            {
                if (currentInvoiceIndex < orderList.Count - 1)
                {
                    currentInvoiceIndex++;
                    textBoxOrders.Text = orderList.ElementAt(currentInvoiceIndex).toString();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            orderList = bl.processGetInvoiceList((DateTime)orderDatePicker.SelectedDate);
            if (orderList != null)
            {
                currentInvoiceIndex = orderList.Count - 1;
                textBoxOrders.Text = orderList.ElementAt(currentInvoiceIndex).toString();
            }
            else
            {
                textBoxOrders.Text = "";
            }
        }

        private void Button_Click_Return(object sender, RoutedEventArgs e)
        {
            bl.voidOrder(orderList.ElementAt(currentInvoiceIndex));
            orderList.ElementAt(currentInvoiceIndex).setIsVoid();
            bl.printReceipt(orderList.ElementAt(currentInvoiceIndex));
            MessageBox.Show("Order Returned");
            orderList = bl.processGetInvoiceList((DateTime)orderDatePicker.SelectedDate);
            if (orderList != null)
            {
                textBoxOrders.Text = orderList.ElementAt(currentInvoiceIndex).toString();
            }
            else
            {
                textBoxOrders.Text = "";
            }
        }

        private void Button_Click_Print(object sender, RoutedEventArgs e)
        {
            bl.printReceipt(orderList.ElementAt(currentInvoiceIndex));
        }
    }
}
