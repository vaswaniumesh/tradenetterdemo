using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.TradeNetter
{
    /// <summary>
    /// Trade Model 
    /// </summary>
    public class Trade
    {
        public Trade(TradeType direction, int quantity, decimal price, string underlying)
        {
            this.Direction = direction;
            this.Quantity = quantity;
            this.Price = price;
            this.Underlying = underlying;
        }

        public TradeType Direction { get; }

        //Based on spec, qty is defined as int, so max qty allowed is approx 2bn, can be declared long for supporting larger qty
        public int Quantity { get; set; }
        public decimal Price { get; }
        public string Underlying { get; }

        public override string ToString()
        {
            return $"Direction:{Direction} Quantity:{Quantity} Price:{Price} Underlying:{Underlying}";
        }
    }
}

