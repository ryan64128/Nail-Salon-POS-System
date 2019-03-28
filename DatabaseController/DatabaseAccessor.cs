using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BusinessLogicController;

namespace DatabaseController
{
    public class DatabaseAccessor : IDatabaseAccess
    {
        private DatabaseClass dc;
        string CURRENT_TAX_RATE = ".0635";

        //------------------ Publicly Accesible Constants ------------
        public const string CASH = "CASH";
        public const string CREDIT = "CREDIT";
        public const string DISCOUNT = "DISCOUNT";
        enum PAYMENT_TYPE { CASH, CREDIT, DISCOUNT };

        //------------------ Messages --------------------------------
        private const string CONNECTION_OK = "Connection Opened Successfully";
        private const string CONNECTION_OPEN_ERROR = "Error Opening Connection";
        private const string QUERY_OK = "Query Performed Successfully";
        private const string QUERY_ERROR = "Error Performing Query";
        private const string CONNECTION_CLOSE_OK = "Connection Closed Successfully";
        private const string CONNECTION_CLOSE_ERROR = "Error Closing Connection";
        private const string INSERT_OK = "Insert Command Performed Successfully";
        private const string INSERT_ERROR = "Error Performing Insert Command";

        //------------------ State Variables -------------------------

        public const string INVOICE_TABLE_STRING = "Invoice";
        public DatabaseAccessor()
        {
            dc = new DatabaseClass();
        }

        //--------------------- Employee Methods ---------------------------------------------
        public string addEmployee(string firstName, string lastName, string PIN)
        {
            //string insertString = "INSERT INTO Employee (First_Name, Last_Name, PIN) VALUES ('" + firstName + "', '" + lastName + "', " + PIN + ")";
            string[] attributeNames = { "First_Name", "Last_Name", "PIN" };
            string[] attributeValues = { firstName, lastName, PIN };
            dc.insert("Employee", attributeNames, attributeValues);
            return dc.getLastInsertId(INVOICE_TABLE_STRING);

        }
        public IEmployee getEmployeeById(string id)
        {
            string queryFName = "SELECT First_Name " +
                               "FROM Employee " +
                               "WHERE Employee_Id = " + id.ToString();
            string queryLName = "SELECT Last_Name " +
                                    "FROM Employee " +
                                    "WHERE Employee_Id = " + id.ToString();
            string queryPIN = "SELECT PIN " +
                                    "FROM Employee " +
                                    "WHERE Employee_Id = " + id.ToString();
            return new Employee(Convert.ToInt32(id), dc.query(queryFName), dc.query(queryLName), dc.query(queryPIN));
        }

        public List<IEmployee> loadEmployeeList()
        {
            string queryString = "SELECT Employee_Id, First_Name, Last_Name, PIN " +
                                 "FROM Employee";
            string result = dc.query(queryString);
            string[] resultArray = result.Split(' ');
            for (int i = 0; i < resultArray.Length - 1; i++)
            {
                resultArray[i] = resultArray[i].Trim('\n');
                Console.WriteLine("Employee fill result = " + resultArray[i]);
            }
            List<IEmployee> empList = new List<IEmployee>();
            for (int i = 0; i < resultArray.Length - 1; i += 4)
            {
                empList.Add(new Employee(Convert.ToInt32(resultArray[i]), resultArray[i + 1], resultArray[i + 2], resultArray[i + 3]));
            }
            return empList;
        }

        //--------------------- Line Item Methods --------------------------------------------

        private string getLineItemQuantity(string id)
        {
            string queryString = "SELECT Quantity " +
                                 "FROM Line_Item " +
                                 "WHERE Line_Item_Id = " + id;
            return dc.query(queryString);
        }


        public void voidLineItem(ILineItem item, IInvoice currentInvoice)
        {
            System.Console.WriteLine("************************** about to void **********************");
            if (item != null)
            {
                System.Console.WriteLine("@@@@*********************** now to void **********************");
                string updateString = "UPDATE Line_Item " +
                                      "SET Is_Void = 'true' " +
                                      "WHERE Line_Item_Id = " + item.getId();
                dc.update(updateString);
                updateInvoiceTotal();
                currentInvoice.removeLineItemFromList(item);
                currentInvoice.getTotal().updateTotal(this.getCurrentInvoiceTotal(), Convert.ToDecimal(this.getTax()), Convert.ToDecimal(this.getInvoiceGrandTotal()));

            }
        }
        public void removeLineItem(string lineItemId)
        {
            string deleteString = "DELETE FROM Line_Item " +
                                  "WHERE Line_Item_Id = " + lineItemId;
            dc.delete(deleteString);
        }

        public string getLineItems()
        {
            string queryString = "SELECT Service_Name, Quantity, Price " +
                                 "FROM Invoice i JOIN Line_Item l ON i.Invoice_Id = l.Invoice_Id " +
                                 "JOIN Service_Item s ON s.Service_Item_Id = l.Service_Item_Id " +
                                 "WHERE i.Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING);
            return dc.query(queryString);
        }

        //--------------------- Service Item Methods ------------------------------------------
        private string getServiceName(string serviceItemId)
        {
            string queryString = "SELECT Service_Name " +
                                 "FROM Service_Item " +
                                 "WHERE Service_Item_Id = " + serviceItemId;
            return dc.query(queryString);
        }
        public IServiceItem getServiceItemById(string id)
        {
            string queryName = "SELECT Service_Name " +
                               "FROM Service_Item " +
                               "WHERE Service_Item_Id = " + id.ToString();
            string queryPrice = "SELECT Price " +
                                "FROM Service_Item " +
                                "WHERE Service_Item_Id = " + id.ToString();
            string queryCategory = "SELECT Category " +
                              "FROM Service_Item " +
                              "WHERE Service_Item_Id = " + id.ToString();
            return new ServiceItem(Convert.ToInt32(id), dc.query(queryName).Trim('\n', ' '), Convert.ToDecimal(dc.query(queryPrice)), dc.query(queryCategory));
        }
        private string getPrice(string serviceItemId)
        {
            string queryString = "SELECT Price " +
                                 "FROM Service_Item " +
                                 "WHERE Service_Item_Id = " + serviceItemId;
            return dc.query(queryString);
        }

        public IServiceItem insertServiceItem(string name, decimal price, string category)
        {
            string insertString = "INSERT INTO Service_Item (Service_Name, Price, Category) VALUES (" + dc.makeSQLString(name) + ", " + price.ToString() + ", " + dc.makeSQLString(category) + ")";
            dc.insert(insertString);
            return new ServiceItem(Convert.ToInt32(dc.getLastInsertId("Service_Item")), name, price, category);
        }

        public IServiceItem updateServiceItem(int id, string name, decimal price, string category)
        {
            string updateNameString = "UPDATE Service_Item " +
                                  "SET Service_Name = " + dc.makeSQLString(name) + " " +
                                  "WHERE Service_Item_Id = " + id.ToString();
            dc.update(updateNameString);
            string updatePriceString = "UPDATE Service_Item " +
                                  "SET Price = " + price.ToString() + " " +
                                  "WHERE Service_Item_Id = " + id.ToString();
            dc.update(updatePriceString);
            string updateCategoryString = "UPDATE Service_Item " +
                                  "SET Category = " + dc.makeSQLString(category) + " " +
                                  "WHERE Service_Item_Id = " + id.ToString();
            dc.update(updateCategoryString);
            return new ServiceItem(id, name, price, category);
        }

        public void deactivateServiceItem(IServiceItem item)
        {
            string updateString = "UPDATE Service_Item " +
                                  "SET Is_Active = 'false' " +
                                  "WHERE Service_Item_Id = " + item.getIdAsString();
            dc.update(updateString);
        }

        //--------------------- Invoice Methods -----------------------------------------------
        public IInvoice createInvoice(IEmployee emp, ICashDrawer drawer)
        {
            return DatabaseInvoiceQuery.createInvoice(emp, drawer, dc);
        }

        public ICashDrawer createCashDrawer(IEmployee emp, string bank)
        {
            return DatabaseInvoiceQuery.createCashDrawer(emp, bank, dc);
        }

        public ITotal getCurrentInvoiceTotals(IInvoice currentInvoice)
        {
            return DatabaseInvoiceQuery.getCurrentInvoiceTotals(currentInvoice);
        }

        public void voidInvoice(string invoiceId)
        {
            DatabaseInvoiceQuery.voidInvoice(invoiceId, dc);
        }

        public decimal getCurrentInvoiceTotal()
        {
            string queryString = "SELECT Invoice_Total " +
                                "FROM Invoice " +
                                "WHERE Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING);
            return Convert.ToDecimal(dc.query(queryString));
        }

        public decimal getTotalByInvoiceId(int invoiceId)
        {
            string queryString = "SELECT Invoice_Total " +
                                "FROM Invoice " +
                                "WHERE Invoice_Id = " + invoiceId;
            return Convert.ToDecimal(dc.query(queryString));
        }

        public void updateCurrentInvoiceTax()
        {
            string updateString = "UPDATE Invoice " +
                                  "SET Tax = (ISNULL((SELECT Invoice_Total " +
                                             "FROM Invoice " +
                                             "WHERE Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING) + " AND Is_Void = 'false'), 0) - ISNULL((select sum(price) from Line_Item l join Gift_Card_Sale g on l.Gift_Card_Id = g.Gift_Card_Id where l.Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING) + " AND l.Is_Void = 'false'), 0)) * " + CURRENT_TAX_RATE + " " +
                                  "WHERE Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING);
            dc.update(updateString);
        }
        public string currentInvoiceToString()
        {
            string printString = "************* RECEIPT **********\n";
            printString += "Services: " + getLineItems() + "\n";
            printString += "Subtotal: $" + getCurrentInvoiceTotal() + "\n";
            printString += "Tax: $" + getTax() + "\n";
            printString += "Amount Paid: $" + getLinePayments() + "\n";
            if (Convert.ToDouble(getBalance()) <= 0)
                printString += "Amount Due: $0.00\nChange: $" + getBalance();
            else
                printString += "Amount Due: $" + getBalance();
            Console.WriteLine("PRINT_STRING = " + printString);
            return printString;
        }
        public decimal getInvoiceGrandTotal()
        {
            return getCurrentInvoiceTotal() + Convert.ToDecimal(getTax());
        }

        //-------------------------------- Business Logic ---------------------------------
        public bool currentInvoiceIsPaid()
        {
            if (this.getBalance() <= 0 && this.getCurrentInvoiceTotal() > 0)
                return true;
            else
                return false;
        }

        public void setcurrentInvoicePaid()
        {
            string updateString = "UPDATE Invoice " +
                                 "SET Is_Paid = 'true' " +
                                 "WHERE Invoice_Id = " + dc.getLastInsertId("Invoice");
            dc.update(updateString);
        }
        public string getTax()
        {
            string queryString = "SELECT Tax " +
                                 "FROM Invoice " +
                                 "WHERE Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING);
            return dc.query(queryString);
        }

        public string getTaxByInvoiceId(int invoiceId)
        {
            string queryString = "SELECT Tax " +
                                 "FROM Invoice " +
                                 "WHERE Invoice_Id = " + invoiceId;
            return dc.query(queryString);
        }

        public decimal getBalance()
        {
            return getInvoiceGrandTotal() - getAmountPaidCash();
        }
        public decimal getAmountPaidCash()
        {
            string queryString = "SELECT SUM(Amount) " +
                                 "FROM Line_Payment " +
                                 "WHERE Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING);
            string result = dc.query(queryString);
            try
            {
                return Convert.ToDecimal(result);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public void updateInvoiceTotal()
        {
            // old updateString before gift cards were added
            /*string updateString = "UPDATE Invoice " +
                                  "SET Invoice_Total = ISNULL((SELECT SUM(Price * Quantity) " +
                                                       "FROM Line_Item l JOIN Service_Item s " +
                                                       "ON l.Service_Item_Id = s.Service_Item_Id " +
                                                       "WHERE Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING) + " AND Is_Void = 'false'), 0) " +
                                  "WHERE Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING);*/


            string updateString = "UPDATE Invoice " +
                                  "SET Invoice_Total = ISNULL((SELECT SUM(totalPrice) FROM (SELECT SUM(Price * Quantity) AS totalPrice " +
                                                                                    "FROM Line_Item l JOIN Service_Item s " +
                                                                                    "ON l.Service_Item_Id = s.Service_Item_Id " +
                                                                                    "WHERE Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING) + " AND l.Is_Void = 'false' " +
                                                                                    "UNION ALL " +
                                                                                    "SELECT SUM(Price) " +
                                                                                    "FROM Line_Item l JOIN Gift_Card_Sale g " +
                                                                                    "ON l.Gift_Card_Id = g.Gift_Card_Id " +
                                                                                    "WHERE Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING) + " AND l.Is_Void = 'false') " +
                                                        "AS priceTotal), 0) " +
                                  "WHERE Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING);
            dc.update(updateString);
            updateCurrentInvoiceTax();

        }

        //-------------------------------- Line Payment Methods ---------------------------
        public void addLinePayment(IInvoice invoice, string paymentType, string amount)
        {
            string[] attributeNames = { "Invoice_Id", "Payment_Type", "Amount" };
            string[] attributeValues = { invoice.getIdAsString(), dc.makeSQLString(paymentType), amount };
            dc.insert("Line_Payment", attributeNames, attributeValues);
            invoice.addLinePayment(new LinePayment(Convert.ToInt32(dc.getLastInsertId("Line_Payment")), paymentType, Convert.ToDecimal(amount)), this);
        }
        public string getLinePayments()
        {
            string queryString = "SELECT Payment_Type, Amount " +
                                 "FROM Invoice i JOIN Line_Payment l ON i.Invoice_Id = l.Invoice_Id " +
                                 "WHERE i.Invoice_Id = " + dc.getLastInsertId(INVOICE_TABLE_STRING);
            return dc.query(queryString);
        }
        public void removeLinePayment(string linePaymentId)
        {
            string deleteString = "DELETE FROM Line_Payment " +
                                  "WHERE Line_Payment_Id = " + linePaymentId;
            dc.delete(deleteString);
        }

        public string getLastInsertId(string tableName)
        {
            return dc.getLastInsertId(tableName);
        }

        public void insert(string tableName, string[] attributeNames, string[] attributeValues)
        {
            dc.insert(tableName, attributeNames, attributeValues);
        }

        public IDailySalesReport getDailySalesReport(DateTime day)
        {
            string queryNetSales = "SELECT SUM(Invoice_Total) " +
                                   "FROM Invoice " +
                                   "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day.ToString("yyyy-MM-dd")) +
                                   " AND Invoice_Create_Time <= " + dc.makeSQLString(day.AddDays(1).ToString("yyyy-MM-dd")) + " " +
                                   "  AND Is_Void = 0";
            string queryCreditSales = "SELECT SUM(Amount) " +
                                      "FROM Invoice i JOIN Line_Payment l ON i.Invoice_Id = l.Invoice_Id " +
                                      "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day.ToString("yyyy-MM-dd")) +
                                      " AND Invoice_Create_Time <= " + dc.makeSQLString(day.AddDays(1).ToString("yyyy-MM-dd")) +
                                      " AND Payment_Type = 'CREDIT' AND Is_Void = 0";
            string queryGiftSales = "SELECT SUM(Amount) " +
                                      "FROM Invoice i JOIN Line_Payment l ON i.Invoice_Id = l.Invoice_Id " +
                                      "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day.ToString("yyyy-MM-dd")) +
                                      " AND Invoice_Create_Time <= " + dc.makeSQLString(day.AddDays(1).ToString("yyyy-MM-dd")) +
                                      " AND Payment_Type = 'GIFT' AND Is_Void = 0";
            string queryCashSales = "SELECT SUM(Amount) " +
                                      "FROM Invoice i JOIN Line_Payment l ON i.Invoice_Id = l.Invoice_Id " +
                                      "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day.ToString("yyyy-MM-dd")) +
                                      " AND Invoice_Create_Time <= " + dc.makeSQLString(day.AddDays(1).ToString("yyyy-MM-dd")) +
                                      " AND Payment_Type = 'CASH' AND Is_Void = 0";
            string queryTaxSales = "SELECT SUM(Tax) " +
                                    "FROM Invoice " +
                                    "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day.ToString("yyyy-MM-dd")) +
                                    " AND Invoice_Create_Time <= " + dc.makeSQLString(day.AddDays(1).ToString("yyyy-MM-dd")) + " " +
                                    "  AND Is_Void = 0";
            string queryTotalSales = "SELECT SUM(Invoice_Total) + SUM(Tax) " +
                                     "FROM Invoice " +
                                     "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day.ToString("yyyy-MM-dd")) +
                                     " AND Invoice_Create_Time <= " + dc.makeSQLString(day.AddDays(1).ToString("yyyy-MM-dd")) + " " +
                                     "  AND Is_Void = 0";
            string netResult = dc.query(queryNetSales).Trim('\n', ' ');
            decimal netSales = 0;
            if (netResult != null && netResult != "")
                netSales = Convert.ToDecimal(netResult);

            string creditResult = dc.query(queryCreditSales).Trim('\n', ' ');
            decimal creditSales = 0;
            if (creditResult != null && creditResult != "")
                creditSales = Convert.ToDecimal(creditResult);
            string giftResult = dc.query(queryGiftSales).Trim('\n', ' ');
            decimal giftSales = 0;
            if (giftResult != null && giftResult != "")
                giftSales = Convert.ToDecimal(giftResult);
            string taxResult = dc.query(queryTaxSales).Trim('\n', ' ');
            decimal taxSales = 0;
            if (taxResult != null && taxResult != "")
                taxSales = Convert.ToDecimal(taxResult);
            string totalResult = dc.query(queryTotalSales).Trim('\n', ' ');
            decimal totalSales = 0;
            if (totalResult != null && totalResult != "")
                totalSales = Convert.ToDecimal(totalResult);


            decimal cashSales = totalSales - creditSales - giftSales;

            return new DailySalesReport(day, cashSales, creditSales, giftSales, netSales, taxSales, totalSales);
        }

        public ICustomSalesReport getCustomSalesReport(DateTime day1, DateTime day2)
        {
            string queryNetSales = "SELECT SUM(Invoice_Total) " +
                                   "FROM Invoice " +
                                   "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day1.ToString("yyyy-MM-dd")) +
                                   " AND Invoice_Create_Time <= " + dc.makeSQLString(day2.AddDays(1).ToString("yyyy-MM-dd")) + " " +
                                   "  AND Is_Void = 0";
            string queryCreditSales = "SELECT SUM(Amount) " +
                                      "FROM Invoice i JOIN Line_Payment l ON i.Invoice_Id = l.Invoice_Id " +
                                      "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day1.ToString("yyyy-MM-dd")) +
                                      " AND Invoice_Create_Time <= " + dc.makeSQLString(day2.AddDays(1).ToString("yyyy-MM-dd")) +
                                      " AND Payment_Type = 'CREDIT' AND Is_Void = 0";
            string queryGiftSales = "SELECT SUM(Amount) " +
                                      "FROM Invoice i JOIN Line_Payment l ON i.Invoice_Id = l.Invoice_Id " +
                                      "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day1.ToString("yyyy-MM-dd")) +
                                      " AND Invoice_Create_Time <= " + dc.makeSQLString(day2.AddDays(1).ToString("yyyy-MM-dd")) +
                                      " AND Payment_Type = 'GIFT' AND Is_Void = 0";
            string queryCashSales = "SELECT SUM(Amount) " +
                                      "FROM Invoice i JOIN Line_Payment l ON i.Invoice_Id = l.Invoice_Id " +
                                      "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day1.ToString("yyyy-MM-dd")) +
                                      " AND Invoice_Create_Time <= " + dc.makeSQLString(day2.AddDays(1).ToString("yyyy-MM-dd")) +
                                      " AND Payment_Type = 'CASH' AND Is_Void = 0";
            string queryTaxSales = "SELECT SUM(Tax) " +
                                    "FROM Invoice " +
                                    "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day1.ToString("yyyy-MM-dd")) +
                                    " AND Invoice_Create_Time <= " + dc.makeSQLString(day2.AddDays(1).ToString("yyyy-MM-dd")) + " " +
                                    "  AND Is_Void = 0";
            string queryTotalSales = "SELECT SUM(Invoice_Total) + SUM(Tax) " +
                                     "FROM Invoice " +
                                     "WHERE Invoice_Create_Time >= " + dc.makeSQLString(day1.ToString("yyyy-MM-dd")) +
                                     " AND Invoice_Create_Time <= " + dc.makeSQLString(day2.AddDays(1).ToString("yyyy-MM-dd")) + " " +
                                     "  AND Is_Void = 0";
            string netResult = dc.query(queryNetSales).Trim('\n', ' ');
            decimal netSales = 0;
            if (netResult != null && netResult != "")
                netSales = Convert.ToDecimal(netResult);

            string creditResult = dc.query(queryCreditSales).Trim('\n', ' ');
            decimal creditSales = 0;
            if (creditResult != null && creditResult != "")
                creditSales = Convert.ToDecimal(creditResult);
            string giftResult = dc.query(queryGiftSales).Trim('\n', ' ');
            decimal giftSales = 0;
            if (giftResult != null && giftResult != "")
                giftSales = Convert.ToDecimal(giftResult);
            string taxResult = dc.query(queryTaxSales).Trim('\n', ' ');
            decimal taxSales = 0;
            if (taxResult != null && taxResult != "")
                taxSales = Convert.ToDecimal(taxResult);
            string totalResult = dc.query(queryTotalSales).Trim('\n', ' ');
            decimal totalSales = 0;
            if (totalResult != null && totalResult != "")
                totalSales = Convert.ToDecimal(totalResult);


            decimal cashSales = totalSales - creditSales - giftSales;

            return new CustomSalesReport(day1, day2, cashSales, creditSales, giftSales, netSales, taxSales, totalSales);
        }

        public IHourlySalesReport getHourlySalesReport(DateTime day)
        {
            List<decimal> hoursList = new List<decimal>();
            string queryBase = "SELECT ISNULL(SUM(Invoice_Total + Tax), 0) " +
                              "FROM Invoice " +
                              "WHERE Is_Void = 0 " +
                              "  AND DATEPART(yy, Invoice_Create_Time) = " + day.Year + " " +
                              "  AND DATEPART(MM, Invoice_Create_Time) = " + day.Month + " " +
                              "  AND DATEPART(dd, Invoice_Create_Time) = " + day.Day + " " +
                              "  AND DATEPART(hh, Invoice_Create_Time) = ";

            for (int i = 7; i < 22; i++)
            {
                hoursList.Add(Convert.ToDecimal(dc.query(queryBase + i.ToString())));
            }
            return new HourlySalesReport(hoursList, day);
        }

        public ICashDrawer getCurrentCashDrawer()
        {
            string queryEmpId = "SELECT Employee_Id " +
                                 "FROM Cash_Drawer " +
                                 "WHERE Cash_Drawer_Id = " + dc.getLastInsertId("Cash_Drawer");
            string queryBank = "SELECT Bank " +
                               "FROM Cash_Drawer " +
                               "WHERE Cash_Drawer_Id = " + dc.getLastInsertId("Cash_Drawer");
            return new CashDrawer(Convert.ToInt32(dc.getLastInsertId("Cash_Drawer")), Convert.ToInt32(dc.query(queryEmpId)), Convert.ToDecimal(dc.query(queryBank)));
        }

        public ICashReport getDrawerReport(ICashDrawer drawer)
        {
            if (drawer != null)
            {
                string queryNumberOfOrders = "SELECT COUNT(*) " +
                                             "FROM Invoice " +
                                             "WHERE Cash_Drawer_Id = " + drawer.getId().ToString() + " " +
                                             "  AND Is_Void = 0";
                string queryCredit = "SELECT SUM(Amount) " +
                                     "FROM Invoice i JOIN Cash_Drawer c ON i.Cash_Drawer_Id = c.Cash_Drawer_Id " +
                                     "JOIN Line_Payment l ON i.Invoice_Id = l.Invoice_Id " +
                                     "WHERE c.Cash_Drawer_Id = " + drawer.getId().ToString() + " AND " +
                                     "Payment_Type = 'CREDIT' AND Is_Void = 0";
                string queryGift = "SELECT SUM(Amount) " +
                                     "FROM Invoice i JOIN Cash_Drawer c ON i.Cash_Drawer_Id = c.Cash_Drawer_Id " +
                                     "JOIN Line_Payment l ON i.Invoice_Id = l.Invoice_Id " +
                                     "WHERE c.Cash_Drawer_Id = " + drawer.getId().ToString() + " AND " +
                                     "Payment_Type = 'GIFT' AND Is_Void = 0";
                string queryNet = "SELECT SUM(Invoice_Total) " +
                                     "FROM Invoice i JOIN Cash_Drawer c ON i.Cash_Drawer_Id = c.Cash_Drawer_Id " +
                                     "WHERE c.Cash_Drawer_Id = " + drawer.getId().ToString() + " " +
                                     "  AND Is_Void = 0";
                string queryTax = "SELECT SUM(Tax) " +
                                     "FROM Invoice i JOIN Cash_Drawer c ON i.Cash_Drawer_Id = c.Cash_Drawer_Id " +
                                     "WHERE c.Cash_Drawer_Id = " + drawer.getId().ToString() + " " +
                                     "  AND Is_Void = 0";
                string queryGross = "SELECT SUM(Invoice_Total) + SUM(Tax) " +
                                     "FROM Invoice i JOIN Cash_Drawer c ON i.Cash_Drawer_Id = c.Cash_Drawer_Id " +
                                     "WHERE c.Cash_Drawer_Id = " + drawer.getId().ToString() + " " +
                                     "  AND Is_Void = 0";

                string numResult = dc.query(queryNumberOfOrders).Trim('\n', ' ');
                int numSales = 0;
                if (numResult != null && numResult != "")
                    numSales = Convert.ToInt32(numResult);

                string netResult = dc.query(queryNet).Trim('\n', ' ');
                decimal netSales = 0;
                if (netResult != null && netResult != "")
                    netSales = Convert.ToDecimal(netResult);

                string creditResult = dc.query(queryCredit).Trim('\n', ' ');
                decimal creditSales = 0;
                if (creditResult != null && creditResult != "")
                    creditSales = Convert.ToDecimal(creditResult);
                string giftResult = dc.query(queryGift).Trim('\n', ' ');
                decimal giftSales = 0;
                if (giftResult != null && giftResult != "")
                    giftSales = Convert.ToDecimal(giftResult);
                string taxResult = dc.query(queryTax).Trim('\n', ' ');
                decimal taxSales = 0;
                if (taxResult != null && taxResult != "")
                    taxSales = Convert.ToDecimal(taxResult);
                string totalResult = dc.query(queryGross).Trim('\n', ' ');
                decimal totalSales = 0;
                if (totalResult != null && totalResult != "")
                    totalSales = Convert.ToDecimal(totalResult);


                decimal cashSales = totalSales - creditSales - giftSales;
                decimal cashToCount = totalSales - creditSales - giftSales;

                return new CashReport(numSales, cashSales, creditSales, giftSales, netSales, taxSales, totalSales, cashToCount);
            }
            return null;
        }

        public List<IServiceItem> getServiceItemList()
        {
            List<IServiceItem> resultList = new List<IServiceItem>();
            string queryString = "SELECT Service_Item_Id, Service_Name, Price, Category " +
                                 "FROM Service_Item " +
                                 "WHERE Is_Active = 'true'";
            string[] result = dc.query(queryString).Split('\n', ' ');
            for (int i = 0; i < result.Length; i++)
            {
                Console.WriteLine(result[i]);
            }
            for (int i = 0; i < result.Length - 1; i += 5)
            {
                resultList.Add(new ServiceItem(Convert.ToInt32(result[i]), result[i + 1], Convert.ToDecimal(result[i + 2]), result[i + 3]));
            }
            for (int i = 0; i < resultList.Count; i++)
            {
                Console.WriteLine(resultList.ElementAt(i).ToString());
            }
            return resultList;
        }

        public List<IInvoice> getInvoiceListByDate(DateTime day)
        {
            // We do two separate queries, one for service items the other for gift cards to simplfy things
            string result1 = dc.query(new ServiceItemReportQuery(day).getQuery());
            if (result1 == null || result1 == "")
                return null;

            string result2 = dc.query(new GiftCardReportQuery(day).getQuery());
            if (result2 == null || result2 == "")
                return null;

            string[] parsed = SortRowsByInvoiceId(result1 + result2);
            List<int> invoiceStartIndices = determineWhichRowsBeginANewInvoice(parsed);
            List<string[]> invoiceList = groupRowsByInvoiceId(parsed, invoiceStartIndices);
            List<string[]> paymentList = createPaymentList(invoiceList);
            List<string[]> uniqueLineItems = createListOfUniqueLineItems(invoiceList);
            return createFinalListOfInvoices(invoiceList, paymentList, uniqueLineItems); ;
        }

        private string[] SortRowsByInvoiceId(string rawQuery)
        {
            string[] parsed1 = rawQuery.Split("\n\r".ToArray(), StringSplitOptions.RemoveEmptyEntries);  //split query into rows as string array
            List<string[]> tempParsed = new List<string[]>();
            for (int i = 0; i < parsed1.Length; i++)
            {
                tempParsed.Add(parsed1[i].Split(' '));
            }
            tempParsed = tempParsed.OrderBy(r => r[1]).ToList();
            string[] parsed = new string[tempParsed.Count];
            for (int i = 0; i < tempParsed.Count; i++)
            {
                parsed[i] = "";
                for (int j = 0; j < tempParsed.ElementAt(i).Length; j++)
                {
                    parsed[i] += tempParsed.ElementAt(i)[j] + " ";
                }
                parsed[i] += tempParsed.ElementAt(i)[tempParsed.ElementAt(i).Length - 1];
            }
            return parsed;
        }

        private List<int> determineWhichRowsBeginANewInvoice(string[] parsed)
        {                                                                   // get invoice number of first row
            List<int> invoiceStartIndices = new List<int>();                // will hold the row indices of parsed that begin a new invoice id
            int prevIndex = Convert.ToInt32(parsed[0].Split(' ')[1]);
            for (int i = 0; i < parsed.Length - 1; i++)                     // determine which row indices begin a new invoice id
            {
                int currentIndex = Convert.ToInt32(parsed[i].Split(' ')[1]);
                if (currentIndex != prevIndex)
                    invoiceStartIndices.Add(i);
                prevIndex = currentIndex;
            }
            return invoiceStartIndices;
        }

        private List<string[]> groupRowsByInvoiceId(string[] parsed, List<int> invoiceStartIndices)
        {
            List<string[]> invoiceList = new List<string[]>();
            string currentInvoiceString = "";
            int currentStartIndex = 0;
            for (int i = 0; i < parsed.Length; i++)
            {
                if (currentStartIndex < invoiceStartIndices.Count)
                {
                    if (i == invoiceStartIndices[currentStartIndex])
                    {
                        currentStartIndex++;


                        invoiceList.Add(currentInvoiceString.Split("\n\r".ToArray(), StringSplitOptions.RemoveEmptyEntries));
                        currentInvoiceString = "";
                    }
                }
                currentInvoiceString += parsed[i] + "\n";
                if (i + 1 == parsed.Length)
                    invoiceList.Add(currentInvoiceString.Split("\n\r".ToArray(), StringSplitOptions.RemoveEmptyEntries));
            }
            return invoiceList;
        }

        private List<string[]> createPaymentList(List<string[]> invoiceList)
        {
            List<string[]> paymentList = new List<string[]>();

            for (int i = 0; i < invoiceList.Count; i++)                                             // for each invoice
            {
                int prev = Convert.ToInt32(invoiceList.ElementAt(i)[0].Split(' ')[12]);         // 

                for (int j = 0; j < invoiceList.ElementAt(i).Length; j++)                           // for each row in each invoice
                {
                    int next = Convert.ToInt32(invoiceList.ElementAt(i)[j].Split(' ')[12]);
                    if (prev != next)
                    {

                        string[] temp = invoiceList.ElementAt(i)[j - 1].Split(' ');
                        if (!paymentList.Any(new[] { temp[1], temp[12], temp[13], temp[14] }.SequenceEqual))   // eliminate duplicates
                        {
                            paymentList.Add(new[] { temp[1], temp[12], temp[13], temp[14] });       // add current row's invoice id paymeny id type and amount to payments list
                        }
                    }
                    if (j == invoiceList.ElementAt(i).Length - 1)                                 // in case of last row in invoice...
                    {
                        string[] temp = invoiceList.ElementAt(i)[j].Split(' ');
                        if (!paymentList.Any(new[] { temp[1], temp[12], temp[13], temp[14] }.SequenceEqual))   // eliminates duplicates
                        {
                            paymentList.Add(new[] { temp[1], temp[12], temp[13], temp[14] });
                        }
                    }

                    prev = next;
                }
            }
            return paymentList;
        }

        private List<string[]> createListOfUniqueLineItems(List<string[]> invoiceList)
        {
            List<string[]> uniqueLineItems = new List<string[]>();
            for (int i = 0; i < invoiceList.Count; i++)
            {
                //uniqueLineItems = new List<string[]>();
                for (int j = 0; j < invoiceList.ElementAt(i).Length; j++)
                {
                    string[] temp = invoiceList.ElementAt(i)[j].Split(' ');
                    Console.WriteLine(">>" + temp[1] + ", " + temp[6] + ", " + temp[7] + ", " + temp[8] + ", " + temp[9] + ", " + temp[10] + ", " + temp[11] + ">>");
                    if (!uniqueLineItems.Any(new[] { temp[1], temp[6], temp[7], temp[8], temp[9], temp[10], temp[11] }.SequenceEqual))   // determine in line item already exists in uniqueLineItema
                    {
                        Console.WriteLine("Not unique count: " + (i + j));
                        uniqueLineItems.Add(new[] { temp[1], temp[6], temp[7], temp[8], temp[9], temp[10], temp[11] });  // if line item isn't in it then add it
                    }
                }
            }
            return uniqueLineItems;
        }

        private List<IInvoice> createFinalListOfInvoices(List<string[]> invoiceList, List<string[]> paymentList, List<string[]> uniqueLineItems)
        {
            List<IInvoice> invoiceListResult = new List<IInvoice>();
            for (int i = 0; i < invoiceList.Count; i++)
            {
                int invoiceId = Convert.ToInt32(invoiceList.ElementAt(i)[0].Split(' ')[1]);
                List<ILinePayment> invoicePayments = createListOfLinePaymentObjects(invoiceId, paymentList);      // stores payments that belong to current invoice
                List<ILineItem> invoiceItems = createListOfLineItemObjects(invoiceId, uniqueLineItems);
                //1 257 78 5/7/2018 8:34:48 PM 529 1 26 test_for_space 18.00 MANICURE 196 CASH 130.00
                string[] tempInv = invoiceList.ElementAt(i)[0].Split(' ');
                invoiceListResult.Add(new Invoice(this.getEmployeeById(tempInv[0]), Convert.ToInt32(tempInv[1]), Convert.ToInt32(tempInv[2]), DateTime.Parse(tempInv[3] + " " + tempInv[4]), invoiceItems, new Total(getTotalByInvoiceId(Convert.ToInt32(tempInv[1])), Convert.ToDecimal(getTaxByInvoiceId(Convert.ToInt32(tempInv[1]))), getTotalByInvoiceId(Convert.ToInt32(tempInv[1])) + Convert.ToDecimal(getTaxByInvoiceId(Convert.ToInt32(tempInv[1]))), invoicePayments), tempInv[17]));
            }
            return invoiceListResult;
        }

        private List<ILinePayment> createListOfLinePaymentObjects(int invoiceId, List<string[]> paymentList)
        {
            List<ILinePayment> invoicePayments = new List<ILinePayment>();
            for (int j = 0; j < paymentList.Count; j++)                 // find out which payments belong to current invoice
            {
                string[] temp = paymentList.ElementAt(j);
                int tempInvoiceId = Convert.ToInt32(temp[0]);
                // index out of range temp length = 1
                int paymentId = Convert.ToInt32(temp[1]);
                string paymentType = temp[2];
                decimal paymentAmount = Convert.ToDecimal(temp[3]);
                if (tempInvoiceId == invoiceId)
                {
                    invoicePayments.Add(new LinePayment(paymentId, paymentType, paymentAmount));
                }
            }
            return invoicePayments;
        }

        private List<ILineItem> createListOfLineItemObjects(int invoiceId, List<String[]> uniqueLineItems)
        {
            List<ILineItem> invoiceItems = new List<ILineItem>();
            for (int j = 0; j < uniqueLineItems.Count; j++)
            {
                string[] temp = uniqueLineItems.ElementAt(j);
                if (!temp[3].Equals(temp[4]))
                {
                    int tempInvoiceId = Convert.ToInt32(temp[0]);
                    int lineId = Convert.ToInt32(temp[1]);
                    int quantity = Convert.ToInt32(temp[2]);
                    int serviceItemtId = Convert.ToInt32(temp[3]);
                    string serviceItemName = temp[4];
                    decimal serviceItemPrice = Convert.ToDecimal(temp[5]);
                    string category = temp[6];
                    if (tempInvoiceId == invoiceId)
                    {
                        invoiceItems.Add(new LineItem(lineId, quantity, new ServiceItem(serviceItemtId, serviceItemName, serviceItemPrice, category)));
                    }
                }
                else
                {
                    int tempInvoiceId = Convert.ToInt32(temp[0]);
                    int lineId = Convert.ToInt32(temp[1]);
                    int giftCardId = Convert.ToInt32(temp[3]);
                    decimal giftCardPrice = Convert.ToDecimal(temp[5]);
                    if (tempInvoiceId == invoiceId)
                    {
                        invoiceItems.Add(new LineItem(lineId, new GiftCardSale(giftCardId, giftCardPrice)));
                    }
                }
            }
            return invoiceItems;
        }
    }
}
