using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseController
{
    class GiftCardReportQuery
    {
        private string giftCardReportQuery;

        public GiftCardReportQuery(DateTime day)
        {
            giftCardReportQuery = "SELECT Employee.Employee_Id, Invoice.Invoice_Id,  Cash_Drawer_Id, Invoice_Create_Time, " +
                                                        "Line_Item_Id, price, Gift_Card_Sale.Gift_Card_Id, Gift_Card_Sale.Gift_Card_Id, price, price, " +
                                                        "Line_Payment_Id, Payment_Type, Amount,  Invoice_Total, Tax, Invoice.Is_Void " +
                                                    "FROM Invoice JOIN Line_Item ON Invoice.Invoice_Id = Line_Item.Invoice_Id " +
                                                        "JOIN Line_Payment ON Line_Payment.Invoice_Id = Invoice.Invoice_Id " +
                                                        "JOIN Gift_Card_Sale ON Line_Item.Gift_Card_Id = Gift_Card_Sale.Gift_Card_Id " +
                                                        "JOIN Employee ON Employee.Employee_Id = Invoice.Employee_Id " +
                                                    "WHERE DATEPART(yy, Invoice_Create_Time) = " + day.Year + " " +
                                                        "AND DATEPART(MM, Invoice_Create_Time) = " + day.Month + " " +
                                                        "AND DATEPART(dd, Invoice_Create_Time) = " + day.Day + " " +
                                                        "AND Line_Item.Is_Void = 0";
        }

        public string getQuery()
        {
            return this.giftCardReportQuery;
        }
    }
}
