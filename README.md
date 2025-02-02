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




## Запрос для проверки

```sql
explain 
Select "Id","FirstName","LastName","BirthDay","Sex","Interests","City" from "User"
where "FirstName" like 'D%' and "LastName" like 'L%'
order by "Id"
```

Без индекса

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
используется для оптимизации запросов в базе данных PostgreSQL, которые выполняют поиск с использованием операторов LIKE или для других операций с шаблонами на текстовых полях FirstName и LastName.
Использование varchar_pattern_ops в индексе помогает эффективно обрабатывать такие запросы, поскольку этот оператор лучше подходит для поиска по шаблону, чем стандартные индексы B-tree.
После индекса

```
Sort  (cost=10137.15..10146.09 rows=3577 width=57)
  Sort Key: "Id"
  ->  Bitmap Heap Scan on "User"  (cost=1697.49..9926.03 rows=3577 width=57)
        Filter: ((("FirstName")::text ~~ 'D%'::text) AND (("LastName")::text ~~ 'L%'::text))
        ->  Bitmap Index Scan on firstname_lastname_btree_idx  (cost=0.00..1696.60 rows=3437 width=0)
              Index Cond: ((("FirstName")::text ~>=~ 'D'::text) AND (("FirstName")::text ~<~ 'E'::text) AND (("LastName")::text ~>=~ 'L'::text) AND (("LastName")::text ~<~ 'M'::text))
```
## Результат

|Label                 |# Samples|Average|Median|90% Line|95% Line|99% Line|Min|Max|Throughput|Received KB/sec|Sent KB/sec|
|----------------------|---------|-------|------|--------|--------|--------|---|---|----------|---------------|-----------|
|user/search_10        |10       |94     |87    |106     |106     |119     |83 |119|10.61571  |6020.17        |1.59       |
|user/search_100       |100      |90     |88    |101     |104     |113     |82 |130|11.07420  |6280.17        |1.65       |
|user/search_1000      |1000     |90     |88    |101     |104     |112     |82 |199|11.01188  |6244.83        |1.65       |
|user/search_index_10  |10       |50     |42    |83      |83      |86      |39 |86 |19.80198  |11229.70       |2.96       |
|user/search_index_100 |100      |43     |40    |48      |55      |97      |38 |125|23.03617  |13063.80       |3.44       |
|user/search_index_1000|1000     |43     |40    |45      |60      |86      |38 |317|22.74692  |12899.77       |3.40       |

!["Средния скорость ответа api"]( )