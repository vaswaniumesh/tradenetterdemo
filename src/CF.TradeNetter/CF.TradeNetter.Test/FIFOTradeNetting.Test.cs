using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CF.TradeNetter;

namespace CF.TradeNetter.Test
{
    public class FIFOTradeNettingTest
    {
        private INettingStrategy nettingStrategy;
        private NettingEngine nettingEngine;

        [SetUp]
        public void SetUp()
        {
            nettingStrategy = new FIFOTradeNetting();
            nettingEngine = new NettingEngine(nettingStrategy);
        }

        [Test]
        public void OnlyBuyTrades()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, 2, 100, "Oil");
            var trade2 = new Trade(TradeType.Buy, 2, 110, "Oil");
            var trade3 = new Trade(TradeType.Buy, 3, 102, "Oil");

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);
            tradesList.Add(trade3);
            
            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(0.0m));
        }

        [Test]
        public void OppositeTradesWithSameQty()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, 2, 100, "Oil");
            var trade2 = new Trade(TradeType.Sell, 2, 110, "Oil");

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);
        
            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(20.0m));
        }

        [Test]
        public void UnderlyingDifferentCase()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, 2, 100, "OIL");
            var trade2 = new Trade(TradeType.Sell, 2, 110, "Oil");

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);

            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(20.0m));
        }

        [Test]
        public void OppositeTradesWithDiffQty()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, 1, 100, "Oil");
            var trade2 = new Trade(TradeType.Sell, 4, 110, "Oil");

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);

            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(10.0m));
        }

        [Test]
        public void MultipleTradesWithDiffQty()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, 1, 100, "Oil");
            var trade2 = new Trade(TradeType.Sell, 4, 110, "Oil");
            var trade3 = new Trade(TradeType.Buy, 4, 120, "Oil");

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);
            tradesList.Add(trade3);

            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(-20.0m));
        }

        [Test]
        public void Trades_MultipleUnderlying_DiffQty()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, 1, 100, "Oil");
            var trade2 = new Trade(TradeType.Sell, 4, 110, "Gas");
            var trade3 = new Trade(TradeType.Buy, 2, 120, "Gas");
            var trade4 = new Trade(TradeType.Sell, 5, 115, "Oil");

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);
            tradesList.Add(trade3);
            tradesList.Add(trade4);

            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(-5.0m));
        }

        [Test]
        public void Trades_MultipleUnderlying_SameQty()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, 4, 100, "Oil");
            var trade2 = new Trade(TradeType.Sell, 2, 110, "Gas");
            var trade3 = new Trade(TradeType.Buy, 2, 120, "Gas");
            var trade4 = new Trade(TradeType.Sell, 4, 115, "Oil");

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);
            tradesList.Add(trade3);
            tradesList.Add(trade4);

            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(40.0m));
        }

        [Test]
        public void NoTrades()
        {
            //Arrange
            var tradesList = new List<Trade>();

            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(0.0m));
        }

        [Test]
        public void NullList()
        {
            //Assert
            Assert.That(() => new NettingEngine(new FIFOTradeNetting()).CalculatePnL(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvalidTradeQty()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, -1, 100, "Oil");
            var trade2 = new Trade(TradeType.Sell, 0, 110, "Oil");
            var trade3 = new Trade(TradeType.Sell, Int32.MinValue, 110, "Oil");

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);
            tradesList.Add(trade3);
            
            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(0.0m));
        }
        [Test]
        public void MaxTradeQty()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, Int32.MaxValue, 100, "Oil");
            var trade2 = new Trade(TradeType.Sell, Int32.MaxValue, 110, "Oil");

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);

            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);
            decimal expectedValue = Int32.MaxValue * 10.0m;
            //Assert
            Assert.That(netPnl, Is.EqualTo(expectedValue));
        }

        [Test]
        public void InvalidTradePrice()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, 1, 0, "Oil");
            var trade2 = new Trade(TradeType.Sell, 4, -110, "Oil");
            
            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);
            
            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(0.0m));
        }

        [Test]
        public void InvalidTradeUnderlying()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, 1, 100, "");
            var trade2 = new Trade(TradeType.Sell, 4, 110, null);

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);

            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(0.0m));
        }

        [Test]
        public void InvalidTradeDirection()
        {
            //Arrange
            TradeType direction;
            
            bool valid = Enum.TryParse("ABC", out direction);

            if (!valid)
                direction = TradeType.Invalid;

            var trade1 = new Trade(direction, 1, 100, "Oil");
            var trade2 = new Trade(TradeType.Sell, 4, 110, "Oil");

            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);
            
            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(0.0m));
        }

        [Test]
        public void InvalidNullTrade()
        {
            //Arrange
            var trade1 = new Trade(TradeType.Buy, 1, 100, "Oil");
            var trade2 = new Trade(TradeType.Sell, 4, 110, "Gas");
            Trade trade3 = null;
            
            var tradesList = new List<Trade>();
            tradesList.Add(trade1);
            tradesList.Add(trade2);
            tradesList.Add(trade3);
            
            //Act
            decimal netPnl = nettingEngine.CalculatePnL(tradesList);

            //Assert
            Assert.That(netPnl, Is.EqualTo(0.0m));
        }
    }
}
