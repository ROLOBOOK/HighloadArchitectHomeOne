# Инструкция по запуску

Для запуска проекта нужно запустить *docker-compose.yml*.

в *example.env* хранятся настройки:

- DbSettings__Server имя сервера с БД
- DbSettings__Port порт для подключния
- DbSettings__BdName - имя базы для приложения
- DbSettings__User имя пользователя
- DbSettings__Password пароль пользователя DbSettings__User

- Salt хранит соль для генерации паролей (не менять чтобы корретно работал тестовый пользователь).
- AuthOptions__ISSUER, AuthOptions__AUDIENCE, AuthOptions__Key данные для генерация jwt токена.

- countFakeData число строк для заполнения тестовыми данными. Если не указать то данные заполняться не будут.

## docker-compose сервисы

- *dbmanager*:
	- Создаст базу и таблиц. 
	- Заполнит таблицу User рандомными даннымии создаст тестового пользователя. 
- *userapi*:
	- Коллекция вызовов для Postman лежит в папке Postman
	- Для авторизации можно использовать тестового пользователя логин *example* пароль *Qwerty123*
- *postgres*:
	- база данных