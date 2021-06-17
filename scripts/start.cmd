start /d ..\Valuator\ dotnet run --urls "http://localhost:5001"
start /d ..\Valuator\ dotnet run --urls "http://localhost:5002"

start "nginx" /d ..\nginx\ nginx.exe

start "RankCalculator1" /d ..\RankCalculator\ dotnet run 
start "RankCalculator2" /d ..\RankCalculator\ dotnet run 

start "EventsLogger1" /d ..\EventsLogger\ dotnet run 
start "EventsLogger2" /d ..\EventsLogger\ dotnet run 