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
- RedisConnectionString строка подключения к редис

- countFakeUser количество тестовых значений для вставки в таблицу User.
- countFakePost количество тестовых значений для вставки в таблицу Post.
- 
## docker-compose сервисы

- *dbmanager*:
	- Создаст базу и таблиц. 
	- Заполнит таблицу User рандомными даннымии создаст тестового пользователя. 
- *userapi*:
	- Коллекция вызовов для Postman лежит в папке Postman
	- Для авторизации можно использовать тестового пользователя логин *example* пароль *Qwerty123*
- *postgres*:
	- база данных
- *redis*:
	- кеш

# Postman
В папке Postman хранится коллекция запросов для ручного тестирования.

# Кеширование
- Добавлены новые методы для управлления друзьями friend/add/{id} friend/delete/{id}
- Добавлены новые методы для управлления постами post/create post/update post/get/{id} post/feed 

Метод post/feed использует кеширование. Для Кеша используется редис. Кеш хранится 20 минут потом обновляется. Можно принудительно очистить кеш командой redis-cli -h redis -p 6379 FLUSHALL.


# Новый метод user/search/
## Пример запроса поиска пользователей 

```sql
Select "Id","FirstName","LastName","BirthDay","Sex","Interests","City" from "User"
where "FirstName" like 'D%' and "LastName" like 'L%'
order by "Id"
```

explain без индекса

```
Gather Merge  (cost=22453.80..22801.50 rows=2980 width=57)
  Workers Planned: 2
  ->  Sort  (cost=21453.78..21457.51 rows=1490 width=57)
        Sort Key: "Id"
        ->  Parallel Seq Scan on "User"  (cost=0.00..21375.25 rows=1490 width=57)
              Filter: ((("FirstName")::text ~~ 'D%'::text) AND (("LastName")::text ~~ 'L%'::text))
```

## Добавленный индекс

```sql
CREATE INDEX firstname_lastname_btree_idx ON public."User" using btree ("FirstName" varchar_pattern_ops,"LastName" varchar_pattern_ops) ;
```
Добавленный индекс используется для оптимизации запросов в базе данных PostgreSQL, которые выполняют поиск с использованием операторов LIKE или для других операций с шаблонами на текстовых полях FirstName и LastName.
Использование varchar_pattern_ops в индексе помогает эффективно обрабатывать такие запросы, поскольку этот оператор лучше подходит для поиска по шаблону, чем стандартные индексы B-tree.

explain после индекса

```
Sort  (cost=10137.15..10146.09 rows=3577 width=57)
  Sort Key: "Id"
  ->  Bitmap Heap Scan on "User"  (cost=1697.49..9926.03 rows=3577 width=57)
        Filter: ((("FirstName")::text ~~ 'D%'::text) AND (("LastName")::text ~~ 'L%'::text))
        ->  Bitmap Index Scan on firstname_lastname_btree_idx  (cost=0.00..1696.60 rows=3437 width=0)
              Index Cond: ((("FirstName")::text ~>=~ 'D'::text) AND (("FirstName")::text ~<~ 'E'::text) AND (("LastName")::text ~>=~ 'L'::text) AND (("LastName")::text ~<~ 'M'::text))
```
## Результат тестирования метода  до и после добавления индекса

|Label                 |# Samples|Average|Median|90% Line|95% Line|99% Line|Min|Max|Throughput|Received KB/sec|Sent KB/sec|
|----------------------|---------|-------|------|--------|--------|--------|---|---|----------|---------------|-----------|
|user/search        |10       |94     |87    |106     |106     |119     |83 |119|10.61571  |6020.17        |1.59       |
|user/search       |100      |90     |88    |101     |104     |113     |82 |130|11.07420  |6280.17        |1.65       |
|user/search      |1000     |90     |88    |101     |104     |112     |82 |199|11.01188  |6244.83        |1.65       |
|user/search_index  |10       |50     |42    |83      |83      |86      |39 |86 |19.80198  |11229.70       |2.96       |
|user/search_index |100      |43     |40    |48      |55      |97      |38 |125|23.03617  |13063.80       |3.44       |
|user/search_index|1000     |43     |40    |45      |60      |86      |38 |317|22.74692  |12899.77       |3.40       |

![Средния скорость ответа api](https://github.com/ROLOBOOK/HighloadArchitectHomeOne/blob/main/Test_apiresult_graf.png?raw=true)
