Trade Netter Demo 
Instructions for download
Option 1
1. Download the code or clone the repository
2. Open the soltion file located src\CF.TradeNetter\CF.TradeNetter.sln
3. Build the code (dependant nuget package needs to be downloaded)
Option 2
1. Download the tradeNetterExe.zip executable

Instructions to Run
Prerequisites - .NET Frameowrk 4.5 runtime needs to be present in the machine

1. Open the command prompt and go to the path where CF.TradeNetter.exe is located
2. You can pass the path of test trades csv file path as an optional argument (Sample Trades.csv file is included in the code and also in the zip folder)
Eg. CF.TradeNetter.exe Trades.csv
Note: Format of the test csv file
Direction,Quantity,Price,Underlying
3. You can run the application CF.TradeNetter.exe in the interactive mode without passing any argument as below

CF.TradeNetter.exe
Enter trade (Press Y to proceed, any other key to exit):
y
Enter Trade Direction (1 for Buy, 2 for Sell)
1
Enter Quantity (should be greater than 0):
1
Enter Price (should be greater than 0:
100
Enter Underlying (case insensitive):
Oil
Enter trade (Press Y to proceed, any other key to exit):
y
Enter Trade Direction (1 for Buy, 2 for Sell)
2
Enter Quantity (should be greater than 0):
1
Enter Price (should be greater than 0:
110
Enter Underlying (case insensitive):
Oil
Enter trade (Press Y to proceed, any other key to exit):
n
Input Trades
Direction:Buy Quantity:1 Price:100 Underlying:Oil
Direction:Sell Quantity:1 Price:110 Underlying:Oil
Calculating PnL...
Netted Pnl:10.0

