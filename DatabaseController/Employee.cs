using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicController;

namespace DatabaseController
{
    public class Employee : IEmployee
    {
        string fName;
        string lName;
        string PIN;
        int id;

        public Employee(int id, string fName, string lName, string PIN)
        {
            this.id = id;
            this.fName = fName;
            this.lName = lName;
            this.PIN = PIN;
        }

        public string getName()
        {
            return (fName + " " + lName).Replace("\n", "");
        }

        public string getFirstName()
        {
            return fName.Replace("\n", "");
        }
        public string getIdAsString()
        {
            return this.id.ToString();
        }

        public override string ToString()
        {
            return id + "  " + lName + ", " + fName;
        }

        public string getPIN()
        {
            return PIN;
        }
    }
}
