using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Navigation;

namespace FinanceTracker
{
    internal abstract class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public bool Payed { get; set; }


        public Item(string name, string description, DateTime date, decimal price)
        {
            this.Name = name;
            this.Description = description;
            this.Date = date;
            this.Price = price;
        }

        public virtual decimal substractPrice()
        {
            if(Date.Date == DateTime.Today && !Payed)
            {
                Payed = true;
                return Price * -1;
            }
            return 0.00M;
        }
    }
}
