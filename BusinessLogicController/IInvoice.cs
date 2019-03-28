using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public interface IInvoice
    {
        ILineItem addLineItem(IServiceItem serviceItem, string quantity, IDatabaseAccess db);
        ILineItem addLineItemGiftCard(string quantity, IDatabaseAccess db);

        string getIdAsString();
        void addLinePayment(ILinePayment linePayment, IDatabaseAccess db);
        ITotal getTotal();
        string printLineItemList();
        List<ILineItem> getLineItemList();
        void removeLineItemFromList(ILineItem item);
        string getInvoiceId();
        void setIsPaid();
        bool getIsPaid();
        void setTotal(ITotal total);
        string toString();
        IEmployee getEmployee();
        string getIsVoid();
        void setIsVoid();
    }
}
