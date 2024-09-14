### О проекте
Домашнее задание по in-memory базам данных.  
Проект состоит из следующих компонентов:  
* Приложение .NET WebApi в папке ./server, которое собирается в образ server:local и контейнер server.  
* Dockerfile и сид базы данных Postgres ./db, который собирается в образ db:local (контейнер pg_master). Библиотекой Faker сгенерированы пользователи, френды, посты.
* В Docker Compose подключаются контейнеры Redis, RabbitMQ, Redis Insight, PGAdmin.  
* В папке tests находятся запросы для расширения VSCode REST Client, экспорты коллекций и окружений Postman, план теста Dialogs.jmx для Jmeter.  
### Начало работы  
Склонировать проект, сделать cd в корень репозитория и запустить один из файлов Docker Compose.  
Дождаться статуса healthy на контейнере pg_master - контейнер станет healthy когда будет загружен сид(может занять некоторое время).  
```bash
https://github.com/npctheory/highload-inmemorydb.git
cd highload-inmemorydb
```
Есть два файла Docker Compose. Разница только в переменной окружения DialogRepositorySettings__Type, которая указывает какой репозиторий будет инжектится в хэндлеры диалогов в приложении в контейнере server.   
Запустить проект с сервисом диалогов на Redis  
```bash
docker-compose -f redis.yml up --build -d
```

Запустить проект с сервисом диалогов на Postgres  
```bash
docker-compose -f postgres.yml up --build -d
```
### Redis в качества Primary-БД для сервиса диалогов  
За работу с хранилищем диалогов отвечает интерфейс [IDialogRepository](https://github.com/npctheory/highload-inmemorydb/blob/main/server/Core.Domain/Interfaces/IDialogRepository.cs), который имплементирует один из двух классов, в зависимости от значения переменной окружения DialogRepositorySettings__Type:  
[PostgresDialogRepository](https://github.com/npctheory/highload-inmemorydb/blob/main/server/Core.Infrastructure/Repositories/PostgresDialogRepository.cs) если "Postgres"  
[RedisDialogRepository](https://github.com/npctheory/highload-inmemorydb/blob/main/server/Core.Infrastructure/Repositories/RedisDialogRepository.cs) если "Redis"    
### Lua-скрипты  
Класс RedisDialogRepository для запросов к Redis использует lua-скрипты, хранящиеся как константы в статическом классе [RedisDialogRepositoryScripts](https://github.com/npctheory/highload-inmemorydb/blob/main/server/Core.Infrastructure/Repositories/RedisDialogRepositoryScripts.cs) .  
Всего три скрипта:  
**Отправка сообщения**  
```lua
local key = ARGV[1]
local senderId = ARGV[2]
local receiverId = ARGV[3]
local text = ARGV[4]
local isRead = ARGV[5]
local timestamp = ARGV[6]
local id = ARGV[7]

redis.call('HMSET', key, 'Id', id, 'SenderId', senderId, 'ReceiverId', receiverId, 'Text', text, 'IsRead', isRead, 'Timestamp', timestamp)
redis.call('SADD', senderId .. ':' .. receiverId, key)
return 'OK'
```

**Получение списка диалогов**  
```lua
local user = ARGV[1]
local keysWithPrefix = redis.call('KEYS', user .. ':*')
local keysWithPostfix = redis.call('KEYS', '*:' .. user)
local combinedKeys = {}

for _, key in ipairs(keysWithPrefix) do
    table.insert(combinedKeys, key)
end

for _, key in ipairs(keysWithPostfix) do
    table.insert(combinedKeys, key)
end

local dialogs = {}

for _, key in ipairs(combinedKeys) do
    local otherUser

    if string.sub(key, 1, #user + 1) == user .. ':' then
        otherUser = string.sub(key, #user + 2)
    else
        otherUser = string.sub(key, 1, string.find(key, ':') - 1)
    end

    dialogs[otherUser] = true  -- Ensure unique dialogs
end

local result = {}
for otherUser, _ in pairs(dialogs) do
    table.insert(result, otherUser)
end

return result
```

**Получение списка сообщений**
```lua
local userId = ARGV[1]
local agentId = ARGV[2]
local set1 = redis.call('SMEMBERS', userId .. ':' .. agentId)
local set2 = redis.call('SMEMBERS', agentId .. ':' .. userId)
local combinedKeys = {}

for _, key in ipairs(set1) do
    table.insert(combinedKeys, key)
end

for _, key in ipairs(set2) do
    table.insert(combinedKeys, key)
end

local messages = {}

for _, key in ipairs(combinedKeys) do
    local message = redis.call('HGETALL', key)
    table.insert(messages, message)
end

return messages
```

### Нагрузочное тестирование  
Нагрузка на запись в Postgres  


Нагрузка на запись в Redis  
