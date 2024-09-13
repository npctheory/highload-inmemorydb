### О проекте
Домашнее задание по in-memory базам данных.  
Проект состоит из следующих компонентов:  
* Приложение .NET WebApi в папке ./server, которое собирается в образ server:local и контейнер server.  
* Dockerfile и сид базы данных citus-координатора в папке ./db, который собирается в образ db:local (контейнер pg_master). Библиотекой Faker сгенерированы пользователи, френды, посты, сообщения.
* Dockerfile для citus-воркеров в папке ./citus-worker, который собирается в образ citus-worker:local (контейнеры pg_worker1,pg_worker2,pg_worker3).
* В папке tests находятся запросы для расширения VSCode REST Client и экспорты коллекций и окружений Postman.
### Начало работы
Склонировать проект, сделать cd в корень репозитория и запустить Docker Compose.  
Дождаться статуса healthy на контейнерах pg_master, pg_worker1, pg_worker2, pg_worker3 - контейнеры станут healthy когда будет загружен сид(может занять некоторое время).  
```bash
https://github.com/npctheory/highload-queries.git
cd highload-sharding
docker compose up --build -d
```
### Redis в качества Primary-БД для сервиса диалогов  
