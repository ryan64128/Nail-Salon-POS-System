using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    class DailySalesReport : IDailySalesReport
    {
        private DateTime salesDate;
        private decimal cashSales;
        private decimal creditSales;
        private decimal giftSales;
        private decimal netSales;
        private decimal taxTotal;
        private decimal grossSales;

        public DailySalesReport(DateTime date, decimal cash, decimal credit, decimal gift, decimal net, decimal tax, decimal gross)
        {
            salesDate = date;
            cashSales = cash;
            creditSales = credit;
            giftSales = gift;
            netSales = net;
            taxTotal = tax;
            grossSales = gross;
        }

        public DateTime getDate()
        {
            return salesDate;
        }

        public decimal getCashSales()
        {
            return cashSales;
        }

        public decimal getCreditSales()
        {
            return creditSales;
        }

        public decimal getGiftSales()
        {
            return giftSales;
        }

        public decimal getNetSales()
        {
            return netSales;
        }

        public decimal getTax()
        {
            return taxTotal;
        }

        public decimal getGrossSales()
        {
            return grossSales;
        }

        public string toString()
        {
            return "Daily Sales Report\n" + salesDate.ToString("MM-dd-yyyy") + "\n" +
                   "----------------------------------------\n" +
                   "Cash Sales\t\t" + cashSales.ToString(@"$#,##0.00;-$#,##0.00").PadLeft(10, ' ') + "\n" +
                   "Credit Sales\t\t" + creditSales.ToString(@"$#,##0.00;-$#,##0.00").PadLeft(10, ' ') + "\n" +
                   "Gift Redeem\t\t" + giftSales.ToString(@"$#,##0.00;-$#,##0.00").PadLeft(10, ' ') + "\n" +
                   "Net Sales\t\t" + netSales.ToString(@"$#,##0.00;-$#,##0.00").PadLeft(10, ' ') + "\n" +
                   "----------------------------------------\n" +
                   "Tax\t\t\t" + taxTotal.ToString(@"$#,##0.00;-$#,##0.00").PadLeft(10, ' ') + "\n" +
                   "Gross Sales\t\t" + grossSales.ToString(@"$#,##0.00;-$#,##0.00").PadLeft(10, ' ');
        }
    }
}
