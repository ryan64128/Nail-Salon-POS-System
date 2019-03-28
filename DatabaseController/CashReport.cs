using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    class CashReport : ICashReport
    {
        public int NumberOfOrders { get; set; }
        public decimal Cash { get; set; }
        public decimal Credit { get; set; }
        public decimal Gift { get; set; }
        public decimal NetSales { get; set; }
        public decimal Tax { get; set; }
        public decimal GrossSales { get; set; }
        public decimal CashToCount { get; set; }

        public CashReport(int num, decimal cash, decimal credit, decimal gift, decimal net, decimal tax, decimal grossSales, decimal cashToCount)
        {
            this.NumberOfOrders = num;
            this.Cash = cash;
            this.Credit = credit;
            this.Gift = gift;
            this.NetSales = net;
            this.Tax = tax;
            this.GrossSales = grossSales;
            this.CashToCount = cashToCount;
        }

    }
}
