using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    public class HourlySalesReport : IHourlySalesReport
    {
        private List<decimal> hourlySalesList;
        private DateTime date;

        public HourlySalesReport(List<decimal> hoursList, DateTime date)
        {
            this.hourlySalesList = hoursList;
            this.date = date;
        }

        public override string ToString()
        {
            string str = "Hourly Sales Report\n" + this.date.ToString("MM-dd-yyyy") + "\n";
            str += "----------------------------------------\n";
            for (int i = 0; i < hourlySalesList.Count; i++)
            {
                string hour = (DateTime.Today + TimeSpan.FromHours(i + 7)).ToString("hh:mm tt");
                str += hour.PadRight(8) + "\t\t" + ("$" + hourlySalesList.ElementAt(i)).ToString().PadLeft(10, ' ') + "\n";
            }
            str += "----------------------------------------\nTotal\t\t\t" + ("$" + hourlySalesList.Sum()).ToString().PadLeft(10, ' ');
            return str;
        }
    }
}
