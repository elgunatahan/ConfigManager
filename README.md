# ConfigManager

# Running 
- install docker
- run `docker compose up` in terminal 

# Future Enhancements
- Outbox Pattern Implementation
- Authentication & Authorization
- Logging
- Pagination
- Integration Tests


# Tests
- UnitTests

# Technical Details
- .NET 5.0
- .NET 6.0
- MongoDB
- Redis
- Docker

# Configuration Api Details
- AuthApi API will start at port 7002
- For swagger `http://localhost:7002/swagger/index.html`

```sh 
Create config
curl --location 'http://localhost:7002/api/v1/configurations' \
--header 'accept: */*' \
--header 'Content-Type: application/json' \
--data '{"environment":"Development","applicationName":"ExampleService","key":"AuthorSurname","value":"Atahan","type":"string"}'

Update config => Only value and type.
curl --location --request PUT 'http://localhost:7002/api/v1/configurations/060ada59-8d30-4bc0-bd7e-a6776317d56c' \
--header 'accept: */*' \
--header 'Content-Type: application/json' \
--data '{"value":"Elişka","type":"string"}'

Delete config => Soft Delete
curl --location --request DELETE 'http://localhost:7002/api/v1/configurations/060ada59-8d30-4bc0-bd7e-a6776317d56c' \
--header 'accept: */*'

Get config => With given Id
curl --location 'http://localhost:7002/api/v1/configurations/060ada59-8d30-4bc0-bd7e-a6776317d56c' \
--header 'accept: */*'

Query configs => With given env and application name
curl --location 'http://localhost:7002/api/v1/configurations?environment=Development&applicationName=ExampleService' \
--header 'accept: text/plain'
```