using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
        private static int idCounter = 0;
        public int Id { get; set; }


        public Item(string name, string description, DateTime date, decimal price)
        {
            idCounter++;
            Id = idCounter;
            this.Name = name;
            this.Description = description;
            this.Date = date;
            this.Price = price;
        }

    }
}
