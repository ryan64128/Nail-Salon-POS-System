﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicController
{
    public interface IEmployee
    {
        string getIdAsString();
        string getPIN();
        string getName();
        string getFirstName();
    }
}
