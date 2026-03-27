using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker
{
    internal class Salary : Income
    {
        public Salary(string name, string description, DateTime date, decimal price) : base(name, description, date, price)
        {
        }

    }
}
