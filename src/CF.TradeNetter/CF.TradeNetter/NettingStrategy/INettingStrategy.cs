using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.TradeNetter
{
    public interface INettingStrategy
    {
        decimal CalculatePnL(IEnumerable<Trade> tradeList);
    }
}
