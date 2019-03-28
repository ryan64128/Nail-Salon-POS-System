using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public interface ILineItem
    {
        string getId();
        /*ILineItem addLineItem(string serviceItemId, string quantity, IServiceItem serviceItem, IDatabaseAccess db, IInvoice invoice);
        string getLineItemQuantity(string id);
        void voidLineItem(AbstractLineItem item);
        void removeLineItem(string lineItemId);*/
        //string getLineItemById(int id);
        //string getLineItems();
        string getName();

        int getQuantity();

        decimal getPrice();
    }
}
