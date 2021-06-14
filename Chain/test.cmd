dotnet build
start "Node 1" dotnet run 7000 localhost 7001 --no-build
start "Node 2" dotnet run 7001 localhost 7002 --no-build
start "Node 3" dotnet run 7002 localhost 7003 true --no-build
start "Node 4" dotnet run 7003 localhost 7004 --no-build
start "Node 5" dotnet run 7004 localhost 7000 --no-build