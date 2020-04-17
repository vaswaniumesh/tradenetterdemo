using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.TradeNetter
{
    public class FIFOTradeNetting : INettingStrategy
    {

        private decimal _netPnl;
        private readonly IDictionary<string, Queue<Trade>> _tradesQueue;

        public FIFOTradeNetting()
        {
            this._tradesQueue = new Dictionary<string, Queue<Trade>>();
            this._netPnl = 0.0m;
        }

        /// <summary>
        /// Calculates the netted pnl for given list of trades using FIFO
        /// </summary>
        /// <returns>NettedPnl</returns>
        public decimal CalculatePnL(IEnumerable<Trade> tradeList)
        {
            if (tradeList == null)
                throw new ArgumentNullException(nameof(tradeList));

            //Traverse the list of trades.If trade with opposite direction exists, net the trade, else add it to queue
            foreach (var trade in tradeList)
            {
                if (trade == null) continue;

                //filter the trades which are inavlid, qty, price less than zero
                //log the invalid trade in log file (for demo show in console) 
                if (!IsValid(trade))
                {
                    Console.WriteLine($"InvalidTrade: {trade}");
                    continue;
                }

                string nettingKey = GetNettingKey(trade);

                if (_tradesQueue.ContainsKey(nettingKey))
                    Net(trade, _tradesQueue[nettingKey]);
                else
                    Add(trade);
            }

            return this._netPnl;
        }
        //Adds the trade to the queue for processing
        private void Add(Trade trade)
        {
            string key = GetKey(trade);
            if (_tradesQueue.ContainsKey(key))
            {
                var tradesQueue = _tradesQueue[key];
                tradesQueue.Enqueue(trade);
            }
            else
            {
                var tradesQueue = new Queue<Trade>();
                tradesQueue.Enqueue(trade);
                _tradesQueue.Add(key, tradesQueue);
            }
        }

        //Handles the logic for trade netting and calculating Net PnL
        private void Net(Trade tradeToNet, Queue<Trade> tradeQueue)
        {
            Trade oppositeTrade = null;
            int nettedQty;

            //Not dequeuing straight away as qty of trade to be netted can be less than qty of opposite matched trade
            //In that case matched trade qty needs to be adjusted down after netting
            if (tradeQueue.Count > 0)
                oppositeTrade = tradeQueue.Peek();

            
            while (tradeToNet.Quantity > 0 && oppositeTrade?.Quantity > 0)
            {
                //Example Buy Oil 120 qty, we have first Sell Oil 110 qty, so we fully net Sell trade and remove from the queue.
                if (tradeToNet.Quantity >= oppositeTrade.Quantity)
                {
                    nettedQty = oppositeTrade.Quantity;
                    _netPnl += nettedQty * tradeToNet.Price * Multiplier(tradeToNet) + oppositeTrade.Quantity * oppositeTrade.Price * Multiplier(oppositeTrade);
                    tradeToNet.Quantity -= nettedQty;
                    oppositeTrade.Quantity -= nettedQty;
                    tradeQueue.Dequeue();

                    //Check if any more opposite trades are there 
                    if (tradeQueue.Count > 0)
                        oppositeTrade = tradeQueue.Peek();
                }
                //Example Buy Oil 100 qty, we have Sell Oil 110 qty, so we just adjust qty of Sell trade to 10 
                else
                {
                    nettedQty = tradeToNet.Quantity;
                    _netPnl += tradeToNet.Quantity * tradeToNet.Price * Multiplier(tradeToNet) + nettedQty * oppositeTrade.Price * Multiplier(oppositeTrade);
                    oppositeTrade.Quantity -= nettedQty;
                    tradeToNet.Quantity -= nettedQty;
                }
            }

            //If still qty left for trade being netted, so add the partially netted trade to the queue for future processing
            if (tradeToNet.Quantity > 0)
                Add(tradeToNet);
        }

        
        private bool IsValid(Trade trade)
        {
            return trade.Quantity > 0 && trade.Price > 0 && !String.IsNullOrEmpty(trade.Underlying) && (trade.Direction == TradeType.Buy || trade.Direction == TradeType.Sell);
        }

        private int Multiplier(Trade trade)
        {
            return trade.Direction == TradeType.Buy ? -1 : 1; 
        }

        private string GetKey(Trade trade)
        {
            //Note: Based on volume of trades and some performance testing, we can try to use String interning
            return trade.Direction.ToString() + "~" + trade.Underlying.ToLowerInvariant();
        }

        private string GetNettingKey(Trade trade)
        {
            TradeType nettedTradeType = trade.Direction == TradeType.Buy ? TradeType.Sell : TradeType.Buy;
            return nettedTradeType.ToString() + "~" + trade.Underlying.ToLowerInvariant();
        }
    }
}
