using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker
{
    internal class Income : Item
    {
        public bool Recurring { get; set; }

        public Income(string name, string description, DateTime date, decimal price) : base(name, description, date, price)
        {
            Recurring = false;
        }

        public Income(string name, string description, DateTime date, decimal price, bool recurring) : base(name, description, date, price)
        {
            Recurring = recurring;
        }
    }
}
