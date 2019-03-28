using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    public class Invoice : IInvoice
    {
        private int id;
        private IEmployee emp;
        private List<ILineItem> lineItemList;
        private ITotal total;
        private bool isPaid;
        private int cashDrawerId;
        private DateTime createDate;
        private string isVoid;
        public Invoice(IEmployee emp, int id, int drawerId)
        {
            this.id = id;
            this.emp = emp;
            lineItemList = new List<ILineItem>();
            total = new Total(0, 0, 0);
            isPaid = false;
            cashDrawerId = drawerId;
            isVoid = "False";
        }

        public Invoice(IEmployee emp, int id, int drawerId, DateTime invoiceCreateTime, List<ILineItem> itemList, Total total, string isVoid)
        {
            this.id = id;
            this.emp = emp;
            this.lineItemList = itemList;
            this.total = total;
            this.isPaid = false;
            this.cashDrawerId = drawerId;
            this.createDate = invoiceCreateTime;
            this.isVoid = isVoid;
        }

        public override string ToString()
        {
            return "  ORDER  #" + this.id.ToString().PadLeft(8, '0');
        }

        public string toString()
        {
            string result = "ORDER  #" + this.id.ToString().PadLeft(8, '0') + "\nDATE  " + this.createDate.ToString("MM/dd/yyyy   hh:mm:ss") + "\n";
            result += "Cashier: " + this.emp.getName() + "\n\n";
            for (int i = 0; i < lineItemList.Count; i++)
            {
                result += lineItemList.ElementAt(i).ToString() + "\n";
            }
            result += "--------------------------\n";
            result += "Subtotal\t" + this.total.getSubtotal() + "\nTax\t" + this.total.getTax() + "\nTotal\t" + this.total.getTotal() + "\n";
            result += "\n";
            for (int i = 0; i < total.getLinePaymentList().Count; i++)
            {
                result += "Paid\t" + total.getLinePaymentList().ElementAt(i).ToString();
            }
            result += "\n\n";
            if (this.isVoid.Equals("True"))
            {
                result += "  ********************\n";
                result += "  ***     VOID     ***\n";
                result += "  ********************\n";
            }
            return result;
        }

        public string getIsVoid()
        {
            return this.isVoid;
        }

        public void setIsVoid()
        {
            this.isVoid = "True";
        }

        public IEmployee getEmployee()
        {
            return this.emp;
        }

        public ILineItem addLineItem(IServiceItem serviceItem, string quantity, IDatabaseAccess db)
        {
            string[] attributeNames = { "Service_Item_Id", "Invoice_Id", "Quantity" };
            string[] attributeValues = { serviceItem.getIdAsString(), db.getLastInsertId("Invoice"), quantity };
            db.insert("Line_Item", attributeNames, attributeValues);
            db.updateInvoiceTotal();
            total.updateTotal(db.getCurrentInvoiceTotal(), Convert.ToDecimal(db.getTax()), Convert.ToDecimal(db.getInvoiceGrandTotal()));
            LineItem lineItem = new LineItem(Convert.ToInt32(db.getLastInsertId("Line_Item")), Convert.ToInt32(quantity), serviceItem);
            lineItemList.Add(lineItem);
            return lineItem;
        }

        public ILineItem addLineItemGiftCard(string price, IDatabaseAccess db)
        {
            string[] attributeNames = { "Price" };
            string[] attributeValues = { price };
            db.insert("Gift_Card_Sale", attributeNames, attributeValues);
            GiftCardSale gift = new GiftCardSale(Convert.ToInt32(db.getLastInsertId("Gift_Card_Sale")), Convert.ToDecimal(price));

            string[] attributeNames2 = { "Invoice_Id", "Quantity", "Gift_Card_Id" };
            string[] attributeValues2 = { db.getLastInsertId("Invoice"), "1", db.getLastInsertId("Gift_Card_Sale") };
            db.insert("Line_Item", attributeNames2, attributeValues2);
            db.updateInvoiceTotal();
            total.updateTotal(db.getCurrentInvoiceTotal(), Convert.ToDecimal(db.getTax()), Convert.ToDecimal(db.getInvoiceGrandTotal()));
            LineItem lineItem = new LineItem(Convert.ToInt32(db.getLastInsertId("Line_Item")), gift);
            Console.WriteLine("\n\n***** lineItem added... *****" + lineItem.ToString() + "\n******\n\n");
            lineItemList.Add(lineItem);
            return lineItem;
        }

        public string getIdAsString()
        {
            return this.id.ToString();
        }

        public void addLinePayment(ILinePayment linePayment, IDatabaseAccess db)
        {
            total.updateTotal(db.getCurrentInvoiceTotal(), Convert.ToDecimal(db.getTax()), Convert.ToDecimal(db.getInvoiceGrandTotal()));
            total.addLinePayment(linePayment);
        }
        public ITotal getTotal()
        {
            return this.total;
        }

        public List<ILineItem> getLineItemList()
        {
            return lineItemList;
        }

        public string printLineItemList()
        {
            string result = "";
            for (int i = 0; i < lineItemList.Count; i++)
            {
                result += lineItemList.ElementAt<ILineItem>(i).ToString();
            }
            return result;
        }

        public decimal getAmountDue(IDatabaseAccess db)
        {
            return db.getBalance();
        }

        public void removeLineItemFromList(ILineItem item)
        {
            lineItemList.Remove(item);
        }

        public void setIsPaid()
        {
            isPaid = true;
        }
        public string getInvoiceId()
        {
            return id.ToString();
        }
        public bool getIsPaid()
        {
            return isPaid;
        }

        public void setTotal(ITotal total)
        {
            this.total = total;
        }
    }
}
