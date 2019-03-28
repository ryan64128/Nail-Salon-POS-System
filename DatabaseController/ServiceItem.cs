using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    public class ServiceItem : IServiceItem
    {
        private int id;
        private string serviceName;
        private decimal price;
        private string category;

        public ServiceItem(int id, string serviceName, decimal price, string category)
        {
            this.id = id;
            this.serviceName = serviceName;
            this.price = price;
            this.category = category;
        }

        public string getIdAsString()
        {
            return this.id.ToString();
        }

        public string getName()
        {
            return serviceName;
        }

        public decimal getPrice()
        {
            return this.price;
        }

        public string getCategory()
        {
            return this.category;
        }

        public override string ToString()
        {
            return this.serviceName.PadRight(30) + this.id.ToString() + "\t" + this.price.ToString() + "\t" + this.category;
        }
    }
}
