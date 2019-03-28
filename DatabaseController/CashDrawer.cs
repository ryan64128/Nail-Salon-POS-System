using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    public class CashDrawer : ICashDrawer
    {
        private int drawerId;
        private int empId;
        private decimal bank;
        public CashDrawer(int drawerId, int empId, decimal bank)
        {
            this.drawerId = drawerId;
            this.empId = empId;
            this.bank = bank;
        }

        public int getId()
        {
            return drawerId;
        }

        public decimal getBank()
        {
            return this.bank;
        }
    }
}
