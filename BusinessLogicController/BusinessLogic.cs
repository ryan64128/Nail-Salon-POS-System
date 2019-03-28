using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public class BusinessLogic
    {
        IDatabaseAccess _db;
        IInvoice _currentInvoice;
        IPrinterModule pm;
        ICashDrawer currentCashDrawer;
        bool _isOrderStarted;

        public BusinessLogic(IDatabaseAccess db, IPrinterModule pm)
        {
            this._db = db;
            this._currentInvoice = null;
            this.pm = pm;
            this.currentCashDrawer = null;
            this._isOrderStarted = false;
        }

        public void logOnCashDrawer(string empId, string bank)
        {
            currentCashDrawer = _db.createCashDrawer(_db.getEmployeeById(empId), bank);
        }

        public IInvoice processCreateInvoice(string empID)
        {
            _currentInvoice = _db.createInvoice(_db.getEmployeeById(empID), _db.getCurrentCashDrawer());
            _isOrderStarted = true;
            return _currentInvoice;
        }

        public ILineItem processAddLineItem(string serviceItemId, string quantity)
        {
            if (_currentInvoice != null)
            {
                return _currentInvoice.addLineItem(_db.getServiceItemById(serviceItemId), quantity, _db);
            }
            return null;
        }

        public ILineItem processAddLineItemGiftCard(string price)
        {
            if (_currentInvoice != null)
            {
                return _currentInvoice.addLineItemGiftCard(price, _db);
            }
            return null;
        }

        public void processMakePayment(string paymentType, string amount)
        {
            _db.addLinePayment(_currentInvoice, paymentType, amount);
            if (_db.getBalance() <= 0)
            {
                _db.setcurrentInvoicePaid();
                _currentInvoice.setIsPaid();
            }
        }

        public bool isCurrentInvoiceClosed()
        {
            return _currentInvoice.getIsPaid();
        }

        public void processVoidLineItem(ILineItem lineItem)
        {
            _db.voidLineItem(lineItem, _currentInvoice);
        }

        public string getCurrentInvoiceTotals()
        {
            return _db.getCurrentInvoiceTotals(_currentInvoice).ToString(_db);
        }

        public bool currentInvoiceIsPaid()
        {
            return _db.currentInvoiceIsPaid();
        }

        public void printReceipt()
        {
            Console.WriteLine("********************** *********\n****************\nAbout to print receipt\n\n");
            pm.printOrder(_currentInvoice);
        }

        public void printReceipt(IInvoice invoice)
        {
            pm.printOrder(invoice);
        }

        public void printDrawerCashReport()
        {
            Console.WriteLine("About to print cash report receipt");
            pm.printDrawerCashReport(_db.getDrawerReport(currentCashDrawer));
        }

        public void setCurrentInvoice(IInvoice invoice)
        {
            _currentInvoice = invoice;
        }

        public IInvoice getCurrentInvoice()
        {
            return _currentInvoice;
        }

        public IDailySalesReport processGetDailySalesReport(DateTime day)
        {
            return _db.getDailySalesReport(day);
        }

        public IHourlySalesReport processGetHourlySalesReport(DateTime day)
        {
            return _db.getHourlySalesReport(day);
        }

        public ICustomSalesReport processGetCustomSalesReport(DateTime startDay, DateTime endDay)
        {
            return _db.getCustomSalesReport(startDay, endDay);
        }

        public List<IEmployee> loadEmployeeList()
        {
            return _db.loadEmployeeList();
        }

        public string getBalance()
        {
            return _db.getBalance().ToString();
        }

        public List<IServiceItem> getServiceItemList()
        {
            List<IServiceItem> resultList = _db.getServiceItemList();
            for (int i = 0; i < resultList.Count; i++)
            {
                Console.WriteLine("BL: " + resultList.ElementAt(i).ToString());
            }
            return resultList;
        }

        public IServiceItem processAddServiceItem(string name, decimal price, string category)
        {
            return _db.insertServiceItem(name, price, category);
        }

        public IServiceItem processUpdateServiceItem(int id, string name, decimal price, string category)
        {
            return _db.updateServiceItem(id, name, price, category);
        }

        public void setItemInactive(IServiceItem item)
        {
            _db.deactivateServiceItem(item);
        }

        public List<IInvoice> processGetInvoiceList(DateTime day)
        {
            return _db.getInvoiceListByDate(day);
        }

        public void voidOrder(IInvoice invoice)
        {
            _db.voidInvoice(invoice.getIdAsString());
        }
    }
}
