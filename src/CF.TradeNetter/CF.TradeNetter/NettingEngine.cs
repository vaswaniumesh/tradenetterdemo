using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.TradeNetter
{
    public class NettingEngine
    {
        private readonly INettingStrategy _nettingStrategy;

        public NettingEngine(INettingStrategy nettingStrategy)
        {
            if (nettingStrategy == null)
                throw new ArgumentNullException(nameof(nettingStrategy));

            this._nettingStrategy = nettingStrategy;
        }

        public decimal CalculatePnL(IEnumerable<Trade> tradeList)
        {
            return this._nettingStrategy.CalculatePnL(tradeList);
        }
    }
}
