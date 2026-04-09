using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker
{
    internal class Income : Item
    {
        public Income(string name, string description, DateTime date, decimal price) : base(name, description, date, price)
        {
        }
    }
}
