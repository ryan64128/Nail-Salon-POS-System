using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public interface IPrinterModule
    {
        void printOrder(IInvoice invoice);

        void printDrawerCashReport(ICashReport report);
    }
}
