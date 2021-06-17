@echo off

setx DB_RUS "localhost:6000"
setx DB_EU "localhost:6001"
setx DB_OTHER "localhost:6002"


rem Path to your redis
pushd "../redis"

start "DefaultRedisServer" redis-server
start "ShardRus" redis-server --port 6000
start "ShardEu" redis-server --port 6001
start "ShardOther" redis-server --port 6002

popd