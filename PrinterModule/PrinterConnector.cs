using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Printing;
using BusinessLogicController;
using DatabaseController;


namespace PrinterModule
{
    public class PrinterConnector : IPrinterModule
    {
        private IDatabaseAccess db;
        private string initPrinter = "\x1b@";
        private string smallText = "\x1b!\x01";
        private string normalText = "\x1b!\x00";
        private string centerAlign = "\x1b\x61\x01";
        private string leftAlign = "\x1b\x61\x00";
        private string rightAlign = "\x1b\x61\x02";
        private string underline = "\x1b!\x80";
        private string fontA = "\x1bm\x00";
        private string fontB = "\x1bm\x01";
        private string lineFeed = "\x0a";
        private string leftMargin = "\x1d\x4c\x30\x00";
        private string boldText = "\x1b!\x08";
        private string boldOff = "\x1b!\x00";
        private string largeText = "\x1d!\x11";
        private string cancelLargeText = "\x1b!\x00";
        private string skipThree = "\x1b\x64\x03";
        private string skipTwo = "\x1b\x64\x02";
        private string openDrawer = "\x1b\x70";
        private string tab = "\x9";
        private string moveForward = "\x1B\x5C\x0C\x00";
        private string _currencyTabPoint = "\x16";
        private string currencyFormat = @"$#,##0.00;-$#,##0.00";

        IntPtr hPrinter = new IntPtr(0);
        public PrinterConnector(IDatabaseAccess db)
        {
            this.db = db;
        }
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        public void printOrder(IInvoice invoice)
        {
            DOCINFOA docInfo = new DOCINFOA();

            if (OpenPrinter("POS-58", out hPrinter, IntPtr.Zero))
            {
                if (StartDocPrinter(hPrinter, 1, docInfo))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        printBody(invoice);
                        //printTabTest();
                    }
                }
                ClosePrinter(hPrinter);
            }
        }

        public void printDrawerCashReport(ICashReport cashReport)
        {
            DOCINFOA docInfo = new DOCINFOA();

            if (OpenPrinter("POS-58", out hPrinter, IntPtr.Zero))
            {
                if (StartDocPrinter(hPrinter, 1, docInfo))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        printCashReportBody(cashReport);
                        //printTabTest();
                    }
                }
                ClosePrinter(hPrinter);
            }
        }

        public void printBody(IInvoice invoice)
        {
            ITotal total = invoice.getTotal();
            List<ILineItem> lineItemList = invoice.getLineItemList();
            List<ILinePayment> paymentsList = total.getLinePaymentList();

            initializePrinter();
            if (invoice.getIsVoid().Equals("True"))
            {
                printReturnHeader("Nail Salon", invoice.getEmployee().getFirstName(), invoice.ToString());
            }
            else
            {
                printHeader("Nail Salon", invoice.getEmployee().getFirstName(), invoice.ToString());
            }
            printLineItems(lineItemList);
            printSeparator();
            printTotals(total);
            printPayments(paymentsList);
            printChangeDue();
            openCashDrawer();
        }

        public void printCashReportBody(ICashReport report)
        {
            initializePrinter();
            print(largeText + centerAlign + "Cash Report" + skipTwo + cancelLargeText);
            setTabs(new string[] { "\x10", _currencyTabPoint });
            print(leftAlign + "Count" + tab + tab);
            print(report.NumberOfOrders.ToString() + lineFeed);
            print("Cash Sales" + tab + tab);
            alignDecimalsAndPrintWithLineFeed(report.Cash);
            print("Credit" + tab + tab);
            alignDecimalsAndPrintWithLineFeed(report.Credit);
            print("Gift" + tab + tab);
            print(report.Gift.ToString(currencyFormat) + lineFeed);
            print("Net Sales" + tab + tab);
            print(report.NetSales.ToString(currencyFormat) + lineFeed);
            print("Sales Tax" + tab + tab);
            print(report.Tax.ToString(currencyFormat) + lineFeed);
            print("Gross Sales" + tab + tab);
            print(report.GrossSales.ToString(currencyFormat) + lineFeed);
            print("Cash to Count" + tab + tab);
            print(report.CashToCount.ToString(currencyFormat) + lineFeed);
            print(skipThree);
        }

        public void initializePrinter()
        {
            print(initPrinter);
        }

        public void printHeader(string title, string employeeName, string orderNumber)
        {
            print(largeText + centerAlign + title + lineFeed + cancelLargeText);
            print(centerAlign + fontA + DateTime.Now.ToString() + lineFeed + "(860)555-5555" + lineFeed + "68 East Pearl Street" + lineFeed + "Torrington, CT 06790" + skipTwo + leftAlign + "CASHIER: " + employeeName + lineFeed + leftAlign + orderNumber + skipTwo + leftAlign + fontB);
        }

        public void printReturnHeader(string title, string employeeName, string orderNumber)
        {
            print(largeText + centerAlign + title + lineFeed + cancelLargeText);
            print(centerAlign + fontA + DateTime.Now.ToString() + lineFeed + "(860)555-5555" + lineFeed + "68 East Pearl Street" + lineFeed + "Torrington, CT 06790" + skipTwo + largeText + "** RETURN **" + cancelLargeText + skipTwo + leftAlign + "CASHIER: " + employeeName + lineFeed + leftAlign + orderNumber + skipTwo + leftAlign + fontB);
        }

        public void printLineItems(List<ILineItem> list)
        {
            setTabs(new string[] { "\x10", _currencyTabPoint });
            for (int i = 0; i < list.Count; i++)
            {
                print(truncate(list.ElementAt<ILineItem>(i).getName()) + tab);
                alignQuantityAndPrintWithTab(list.ElementAt<ILineItem>(i).getQuantity());
                alignDecimalsAndPrintWithLineFeed(list.ElementAt<ILineItem>(i).getPrice());
            }
            cancelTabs();
        }

        public string truncate(string str)
        {
            if (str.Length > 14)
                return str.Substring(0, 14);
            return str;
        }

        public void alignQuantityAndPrintWithTab(int quant)
        {
            if (quant < 10)
                print(moveForward);
            print(quant.ToString() + tab);
        }

        public void printSeparator()
        {
            print(lineFeed + "********************************" + lineFeed);
        }

        public void printTotals(ITotal total)
        {
            setTabs(new string[] { _currencyTabPoint });
            print("Subtotal" + tab);
            alignDecimalsAndPrintWithLineFeed(total.getSubtotal());
            print("Tax" + tab);
            alignDecimalsAndPrintWithLineFeed(total.getTax());
            print("Total" + tab);
            alignDecimalsAndPrintWithLineFeed(total.getTotal());
            cancelTabs();
        }

        public void printPayments(List<ILinePayment> paymentsList)
        {
            print(lineFeed);
            setTabs(new string[] { "\x03", "\x08", _currencyTabPoint });
            for (int i = 0; i < paymentsList.Count; i++)
            {
                print(tab + "PAID " + paymentsList.ElementAt<ILinePayment>(i).getPaymentType() + tab);
                alignDecimalsAndPrintWithLineFeed(paymentsList.ElementAt<ILinePayment>(i).getAmountPaid());
            }
            cancelTabs();
        }

        public void printChangeDue()
        {
            setTabs(new string[] { "\x03", "\x08", _currencyTabPoint });
            print(tab + "CHANGE DUE  " + tab);
            alignDecimalsAndPrintWithLineFeed(db.getBalance());
            cancelTabs();
        }

        public void openCashDrawer()
        {
            print(skipThree + initPrinter + openDrawer + "\x00" + ".}");
        }

        public void alignDecimalsAndPrintWithLineFeed(decimal value)
        {
            if (value < 0)
            {
                print(moveForward);
            }
            else if (value < 10)
            {
                print(moveForward);
                print(moveForward);
            }
            else if (value < 100)
            {
                print(moveForward);
            }
            print(value.ToString(@"$#,##0.00;-$#,##0.00") + lineFeed);
        }

        public void printTabTest()
        {
            print(initPrinter);
            print("01234567890123456789012345678901" + lineFeed);
            setTabs(new string[] { "\x06", "\x0C" });
            print("test" + tab + "test" + tab + "test" + "b" + lineFeed);
            print(skipThree);
        }

        //--------------- tabValues string array expects strings in form of "\x0f" hex chars ------------
        public void setTabs(string[] tabValues)
        {

            string result = "\x1B\x44";
            for (int i = 0; i < tabValues.Length; i++)
            {
                result += tabValues[i];
            }
            result += "\x00";

            print(result);
        }

        public void cancelTabs()
        {
            print("\x1B\x44\x00");
        }

        public void print(string msg)
        {
            Int32 dwCount = 4;
            Int32 dwWritten = 0;
            IntPtr bPtr = new IntPtr(0);
            dwCount = msg.ToCharArray().Length;
            bPtr = Marshal.StringToCoTaskMemAnsi(msg);
            bool result = WritePrinter(hPrinter, bPtr, dwCount, out dwWritten);
            Marshal.FreeCoTaskMem(bPtr);
        }
    }
}
