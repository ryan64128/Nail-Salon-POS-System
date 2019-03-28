using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    public class LineItem : ILineItem
    {
        private int lineItemId;
        private int quantity;
        private IServiceItem serviceItem;
        private IGiftCardSale giftCardSale;

        public LineItem(int id, int quant, IServiceItem s)
        {
            lineItemId = id;
            quantity = quant;
            serviceItem = s;
            giftCardSale = null;
        }

        public LineItem(int id, IGiftCardSale g)
        {
            lineItemId = id;
            quantity = 1;
            giftCardSale = g;
            serviceItem = null;
        }

        public string getId()
        {
            return this.lineItemId.ToString();
        }
        public override string ToString()
        {
            if (serviceItem != null)
                return serviceItem.getName() + "\n\tQuantity\tPrice\n\t" + this.quantity.ToString() + "\t\t" + (serviceItem.getPrice() * this.quantity).ToString("C2");
            return "GIFT CARD" + "\n\tQuantity\tPrice\n\t" + this.quantity.ToString() + "\t\t" + (giftCardSale.getPrice()).ToString("C2");
        }

        public string toPrinterString()
        {
            if (serviceItem != null)
                return serviceItem.getName() + "\n     Quantity     Price\n     " + this.quantity.ToString() + "     " + serviceItem.getPrice().ToString("C2") + "\n\n";
            return "GIFT CARD\n     Quantity     Price\n     " + this.quantity.ToString() + "     " + giftCardSale.getPrice().ToString("C2") + "\n\n";
        }

        public string getName()
        {
            if (serviceItem != null)
                return this.serviceItem.getName();
            return "GIFT CARD";
        }

        public int getQuantity()
        {
            return this.quantity;
        }

        public decimal getPrice()
        {
            if (serviceItem != null)
                return serviceItem.getPrice() * this.quantity;
            return giftCardSale.getPrice();
        }
    }
}
