using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public interface ICashReport
    {
        int NumberOfOrders { get; set; }
        decimal Cash { get; set; }
        decimal Credit { get; set; }
        decimal Gift { get; set; }
        decimal NetSales { get; set; }
        decimal Tax { get; set; }
        decimal GrossSales { get; set; }
        decimal CashToCount { get; set; }
    }
}
