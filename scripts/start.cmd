start /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5001"
start /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5002"

start /d ..\nginx\ nginx.exe
start /d ..\nats\ nats-server.exe

start "Rank calculator 1" /d ..\RankCalculator\ dotnet run 
start "Rank calculator 2" /d ..\RankCalculator\ dotnet run 