using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public interface IServiceItem
    {
        string getIdAsString();

        string getName();
        decimal getPrice();
        string getCategory();
    }
}
