using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    public class LinePayment : ILinePayment
    {
        private int _id;
        private string _paymentType;
        private decimal _amount;
        public LinePayment(int id, string paymentType, decimal amount)
        {
            this._id = id;
            this._paymentType = paymentType;
            this._amount = amount;
        }

        public override string ToString()
        {
            return _paymentType + "\t" + _amount.ToString("C2") + "\n";
        }

        public string getPaymentType()
        {
            return _paymentType;
        }

        public decimal getAmountPaid()
        {
            return _amount;
        }
    }
}
