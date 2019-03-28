using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    public class Total : ITotal
    {
        private decimal subTotal;
        private decimal tax;
        private decimal total;
        private List<ILinePayment> paymentsList;

        public Total(decimal s, decimal tx, decimal t)
        {
            subTotal = s;
            tax = tx;
            total = t;
            paymentsList = new List<ILinePayment>();
        }

        public Total(decimal s, decimal tx, decimal t, List<ILinePayment> payments)
        {
            subTotal = s;
            tax = tx;
            total = t;
            paymentsList = payments;
        }

        public string ToString(IDatabaseAccess db)
        {
            string result = "Subtotal\t" + subTotal.ToString("C2") + "\n" +
                   "Tax\t\t" + tax.ToString("C2") + "\n" +
                   "Total\t\t" + total.ToString("C2") + "\n" +
                   "Payments\t" + this.printPaymentList() + "\n";

            if (Convert.ToDecimal(db.getBalance()) > 0)
                result += "Amount Due:\t" + Convert.ToDecimal(db.getBalance()).ToString("C2");
            else
                result += "Change Due:\t" + Convert.ToDecimal(db.getBalance()).ToString("C2");
            return result;
        }

        public string printPaymentList()
        {
            string result = "";
            for (int i = 0; i < paymentsList.Count; i++)
            {
                result += paymentsList.ElementAt<ILinePayment>(i).ToString();
            }
            return result;
        }

        public decimal getSubtotal()
        {
            return this.subTotal;
        }

        public decimal getTax()
        {
            return this.tax;
        }

        public decimal getTotal()
        {
            return this.total;
        }

        public List<ILinePayment> getLinePaymentList()
        {
            return this.paymentsList;
        }

        public void addLinePayment(ILinePayment linePayment)
        {
            paymentsList.Add(linePayment);
        }

        public void updateTotal(decimal subtotal, decimal tax, decimal total)
        {
            this.subTotal = subtotal;
            this.tax = tax;
            this.total = total;
        }
    }
}
