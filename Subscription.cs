using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker
{
    internal class Subscription : Expense
    {
        public Subscription(string name, string description, DateTime date, decimal price) : base(name, description, date, price)
        {
        }

        public override decimal substractPrice()
        {
            if (Date.Date == DateTime.Today && !Payed)
            {
                Payed = true;
                return Price * -1;
            }
            return 0.00M;
        }
    }
}
