using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public interface IDailySalesReport
    {
        DateTime getDate();

        decimal getCashSales();

        decimal getCreditSales();
        decimal getGiftSales();

        decimal getNetSales();

        decimal getTax();

        decimal getGrossSales();

        string toString();
    }
}
