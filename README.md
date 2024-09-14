### О проекте
Домашнее задание по in-memory базам данных.  
Проект состоит из следующих компонентов:  
* Приложение .NET WebApi в папке ./server, которое собирается в образ server:local и контейнер server.  
* Dockerfile и сид базы данных Postgres ./db, который собирается в образ db:local (контейнер pg_master). Библиотекой Faker сгенерированы пользователи, френды, посты.
* В папке tests находятся запросы для расширения VSCode REST Client и экспорты коллекций и окружений Postman.  
### Начало работы  
Склонировать проект, сделать cd в корень репозитория и запустить Docker Compose.  
Дождаться статуса healthy на контейнере pg_master - контейнер станет healthy когда будет загружен сид(может занять некоторое время).  
```bash
https://github.com/npctheory/highload-inmemorydb.git
cd highload-inmemorydb
```
  
Запустить проект с сервисом диалогов в Redis  
```bash
docker-compose -f redis.yml up --build -d
docker-compose -f redis.yml down -v
```

Запустить проект с сервисом диалогов в Postgres  
```bash
docker-compose -f postgres.yml up --build -d
docker-compose -f postgres.yml down -v
```

Пересобрать .net приложение отдельно от остальных контейнеров:  
```bash
docker-compose -f redis.yml down server -v
docker-compose -f redis.yml up server --build -d
docker-compose -f postgres.yml up server --build -d
```
### Redis в качества Primary-БД для сервиса диалогов  
