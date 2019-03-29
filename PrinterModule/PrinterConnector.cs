/**
 * PrinterConnector.cs
 * 
 * @Description A class that facilitates the use of a POS terminal printer by encapsulating underlying details and providing an application specific API 
 * @Author Ryan Roberts
 * @Dependencies Needs Business Logic class from BusinessLogicController namespace and DatabaseAccessor from DatabaseController namespace
 */

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
        // reference to a database object
        private IDatabaseAccess db;
        
        /**
         * @Description collection string variables to simplify printer codes
         */
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
        
        // string variable to hold info on how to display currency
        private string currencyFormat = @"$#,##0.00;-$#,##0.00";

        // variable used by Windows API to access printers
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
        /**
         * @Description the following DLLImports define Windows API functions from winspool.Drv driver to be called later when using printOrder and printCashReport functions
         */
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

        /**
         * @desc this function is called by the associated BusinessLogic class when it requests the printing of a specific order
         * @param IInvoice object called invoice which holds all the needed information in order to print an order
         * @returns void
         */
        public void printOrder(IInvoice invoice)
        {
            DOCINFOA docInfo = new DOCINFOA();
            // first parameter is the windows recognized name of the printer to be used
            if (OpenPrinter("POS-58", out hPrinter, IntPtr.Zero)) // if WINAPI function OpenPrinter is successful..
            {
                if (StartDocPrinter(hPrinter, 1, docInfo))        // if WINAPI function StartDocPrinter is successful
                {
                    if (StartPagePrinter(hPrinter))               // if WINAPI function StartPagePrinter is successful
                    {
                        // printBody function encapsulates a series of functions called to facilitate the printing of an order
                        printBody(invoice);
                    }
                }
                ClosePrinter(hPrinter);                           // WINAPI function to close handle to printer
            }
        }

        /**
         * @desc this function is called by the associated BusinessLogic class when it requests the printing of a cash drawer
         * @param ICashReport object called cashReport which holds all the needed information in order to print a cash drawer report
         * @returns void
         */
        public void printDrawerCashReport(ICashReport cashReport)
        {
            DOCINFOA docInfo = new DOCINFOA();
            // first parameter is the windows recognized name of the printer to be used
            if (OpenPrinter("POS-58", out hPrinter, IntPtr.Zero))   // if WINAPI function OpenPrinter is successful..
            {
                if (StartDocPrinter(hPrinter, 1, docInfo))          // if WINAPI function StartDocPrinter is successful..
                {
                    if (StartPagePrinter(hPrinter))                 // if WINAPI function StartPagePrinter is successful..
                    {
                        // printCashReportBody function encapsulates a series of functions called to facilitate the printing of a Cash Drawer Report
                        printCashReportBody(cashReport);
                    }
                }
                ClosePrinter(hPrinter);                             // WINAPI function to close handle to printer
            }
        }

        /**
         * @desc function that encapsulates several function calls needed to print an order receipt
         * @param IInvoice invoice object that holds all the required information needed to print
         * @return void
         */
        public void printBody(IInvoice invoice)
        {
            // get total object reference that invoice object holds
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

        /**
         * @desc function that encapsulates several function calls needed to print a cash report
         * @param ICashReport report object that holds all the required information needed to print
         * @return void
         */
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

        /**
         * @desc function that prints the control characters to printer to get it ready for a new print job
         */
        public void initializePrinter()
        {
            print(initPrinter);
        }

        /**
         * @desc prints the title of the print job in large text then prints info about the store and the employee who created the order and the order number
         * @params string title is the title text, string employeeName is the name of the employee on the order, string orderNumber is the order number
         * @returns void
         */
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

        /**
         * @desc limits length of strings to 14 characters so they fit better on printed receipts
         * @params string str is the string to be truncated
         * @returns a string that has been limited to 14 characters in length
         */
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

        /**
         * @desc function that encapsulates some of the low level funcionality needed to actually print text on the device
         * @params string msg which is the message or printer codes to be printed
         * @return void
         */
        public void print(string msg)
        {
            Int32 dwCount = 4;
            Int32 dwWritten = 0;
            IntPtr bPtr = new IntPtr(0);
            dwCount = msg.ToCharArray().Length;     // how many characters in string
            bPtr = Marshal.StringToCoTaskMemAnsi(msg);
            bool result = WritePrinter(hPrinter, bPtr, dwCount, out dwWritten);
            Marshal.FreeCoTaskMem(bPtr);
        }
    }
}
