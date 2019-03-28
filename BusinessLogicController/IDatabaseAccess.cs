using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public interface IDatabaseAccess
    {
        IInvoice createInvoice(IEmployee emp, ICashDrawer drawer);
        //void closeCurrentInvoice();
        void updateInvoiceTotal();
        void updateCurrentInvoiceTax();
        string currentInvoiceToString();
        string addEmployee(string firstName, string lastName, string PIN);
        IEmployee getEmployeeById(string id);
        //AbstractLineItem addLineItem(string serviceItemId, string quantity);
        void addLinePayment(IInvoice invoice, string paymentType, string amount);
        decimal getBalance();
        decimal getAmountPaidCash();
        decimal getCurrentInvoiceTotal();
        decimal getInvoiceGrandTotal();
        string getTax();
        string getLineItems();
        string getLinePayments();
        string getLastInsertId(string tableName);
        void voidInvoice(string invoiceId);
        void setcurrentInvoicePaid();
        void voidLineItem(ILineItem item, IInvoice currentInvoice);
        void removeLineItem(string lineItemId);
        void removeLinePayment(string linePaymentId);
        bool currentInvoiceIsPaid();
        ITotal getCurrentInvoiceTotals(IInvoice invoice);
        //string getLineItem(int id);
        IServiceItem getServiceItemById(string id);
        void insert(string tableName, string[] attributeNames, string[] attributeValues);

        IDailySalesReport getDailySalesReport(DateTime day);
        ICustomSalesReport getCustomSalesReport(DateTime startDay, DateTime endDay);
        ICashDrawer getCurrentCashDrawer();

        ICashDrawer createCashDrawer(IEmployee emp, string bank);
        List<IEmployee> loadEmployeeList();
        ICashReport getDrawerReport(ICashDrawer drawer);

        List<IServiceItem> getServiceItemList();
        IServiceItem insertServiceItem(string name, decimal price, string category);
        IServiceItem updateServiceItem(int id, string name, decimal price, string category);
        void deactivateServiceItem(IServiceItem item);
        IHourlySalesReport getHourlySalesReport(DateTime day);
        List<IInvoice> getInvoiceListByDate(DateTime day);
    }
}
