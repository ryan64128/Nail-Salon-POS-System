using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public interface ICashDrawer
    {
        int getId();
        decimal getBank();
    }
}
