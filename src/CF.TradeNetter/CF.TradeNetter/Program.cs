using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CF.TradeNetter
{
    class Program
    {
        static void Main(string[] args)
        {
            #region GetInputData
            List<Trade> tradesList;
            if (args.Length > 0) //Get the trades list from csv file
                tradesList = GetTradeListFromCsv(args[0]);
            else
            {
                //Inputs from console
                tradesList = GetInputFromConsole();
                //Below logging is only for demo purpose in console mode
                Console.WriteLine("\nInput Trades");
                tradesList.ForEach(Console.WriteLine);
            }

            #endregion

            Console.WriteLine("Calculating PnL...");
            decimal netPnl;
            try
            {
                INettingStrategy nettingStrategy = new FIFOTradeNetting();

                //Inject the relevant trade netting/balancing algorithn in netting engine
                NettingEngine nettingEngine = new NettingEngine(nettingStrategy);
                netPnl = nettingEngine.CalculatePnL(tradesList);
                
                Console.WriteLine($"Netted Pnl:{netPnl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while calculating pnl : {ex.Message}");
            }
            
            Console.ReadKey();
            
        }

        static List<Trade> GetInputFromConsole()
        {
            ConsoleKeyInfo ckey;
            int direction;
            int quantity;
            decimal price;
            string underlying;
            var tradesList = new List<Trade>();

            Console.WriteLine("Enter trade (Press Y to proceed, any other key to exit):");
            ckey = Console.ReadKey();
            while (ckey.Key == ConsoleKey.Y)
            {
                Console.WriteLine("\nEnter Trade Direction (1 for Buy, 2 for Sell)");
                Int32.TryParse(Console.ReadLine(), out direction);

                TradeType tradeType = TradeType.Invalid;
                if (direction == 1)
                    tradeType = TradeType.Buy;
                else if (direction == 2)
                    tradeType = TradeType.Sell;

                //Client validation rules can be refined based on requirement 
                //whether to accept the trade or handle while processing
                Console.WriteLine($"Enter Quantity (should be greater than 0):");
                Int32.TryParse(Console.ReadLine(), out quantity);
                Console.WriteLine("Enter Price (should be greater than 0:");
                Decimal.TryParse(Console.ReadLine(), out price);
                Console.WriteLine("Enter Underlying (case insensitive):");
                underlying = Console.ReadLine();

                var trade = new Trade(tradeType, quantity, price, underlying);
                tradesList.Add(trade);

                Console.WriteLine("Enter trade (Press Y to proceed, any other key to exit):");
                ckey = Console.ReadKey();
            }

            return tradesList;
        }

        static List<Trade> GetTradeListFromCsv(string csvFileName)
        {
            var tradesList = new List<Trade>();
            try
            {
                var csvLines = File.ReadAllLines(csvFileName);

             
                TradeType direction;
                int quantity;
                decimal price;
                string underlying;

                foreach (var line in csvLines)
                {
                    string[] values = line.Replace(" ", "").Split(',');

                    bool valid = Enum.TryParse(values[0], true, out direction);
                    if (!valid)
                        direction = TradeType.Invalid;

                    Int32.TryParse(values[1], out quantity);
                    Decimal.TryParse(values[2], out price);
                    underlying = values[3];

                    var trade = new Trade(direction, quantity, price, underlying);
                    tradesList.Add(trade);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading trade csv file: {ex.Message}");
            }

            return tradesList;
        }
    }
}
