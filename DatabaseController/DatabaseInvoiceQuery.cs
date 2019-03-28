using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    class DatabaseInvoiceQuery
    {
        public const string INVOICE_TABLE_STRING = "Invoice";

        public static IInvoice createInvoice(IEmployee emp, ICashDrawer drawer, DatabaseClass dc)
        {
            //Console.WriteLine("Creating invoice with employee name = " + emp.getName());
            string[] attributeNames = { "Employee_Id", "Invoice_Create_Time", "Cash_Drawer_Id" };
            string[] attributeValues = { emp.getIdAsString(), "CURRENT_TIMESTAMP", drawer.getId().ToString() };
            dc.insert("Invoice", attributeNames, attributeValues);
            return new Invoice(emp, Convert.ToInt16(dc.getLastInsertId(INVOICE_TABLE_STRING)), drawer.getId());
        }

        public static ICashDrawer createCashDrawer(IEmployee emp, string bank, DatabaseClass dc)
        {
            string[] attributeNames = { "Employee_Id", "Bank" };
            string[] attributeValues = { emp.getIdAsString(), bank };
            dc.insert("Cash_Drawer", attributeNames, attributeValues);
            return new CashDrawer(Convert.ToInt32(dc.getLastInsertId("Cash_Drawer")), Convert.ToInt32(emp.getIdAsString()), Convert.ToInt32(bank));
        }

        public static ITotal getCurrentInvoiceTotals(IInvoice currentInvoice)
        {
            return currentInvoice.getTotal();
        }

        public static void voidInvoice(string invoiceId, DatabaseClass dc)
        {
            string updateString = "UPDATE Invoice " +
                                  "SET Is_Void = 'true' " +
                                  "WHERE Invoice_Id = " + invoiceId;
            dc.update(updateString);
        }
    }
}
