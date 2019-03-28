using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    public class GiftCardSale : IGiftCardSale
    {
        private int id;
        private decimal price;

        public GiftCardSale(int id, decimal price)
        {
            this.id = id;
            this.price = price;
        }

        public string getIdAsString()
        {
            return this.id.ToString();
        }

        public decimal getPrice()
        {
            return this.price;
        }

        public override string ToString()
        {
            return ("GIFT CARD SOLD").PadRight(30) + this.id.ToString() + "\t" + this.price.ToString() + "\t";
        }
    }
}
