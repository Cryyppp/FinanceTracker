using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker
{
    internal class Subscription : Expense
    {
        public bool Payed { get; set; }
        public Subscription(string name, string description, DateTime date, decimal price) : base(name, description, date, price)
        {
            Payed = false;
        }
        public Subscription(string name, string description, DateTime date, decimal price, bool payed) : base(name, description, date, price)
        {
            Payed = payed;
        }

    }
}
