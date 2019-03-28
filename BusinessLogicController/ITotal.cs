using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public interface ITotal
    {
        string printPaymentList();
        void addLinePayment(ILinePayment linePayment);
        void updateTotal(decimal subtotal, decimal tax, decimal total);

        string ToString(IDatabaseAccess db);
        decimal getSubtotal();

        decimal getTax();

        decimal getTotal();

        List<ILinePayment> getLinePaymentList();
    }
}
