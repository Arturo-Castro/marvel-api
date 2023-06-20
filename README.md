# marvel-api

### Tech Stack
- .Net Core 6
- Entity framework core
- Code first approach
- Log4net for logging
- XUnit for unit testing
- Moq for mocking functionality
- Fluent Assertions for better asserts readability
- Automapper for mapping objects
- SQLServer
- DotNetEnv
- Iron PDF for pdf creation
- Refit for easier interaction with the marvel rest api service

### Entity relationship diagram

![ERD](https://github.com/Arturo-Castro/marvel-api/assets/69820855/ecadd8cc-21cd-4665-89da-a26284e1b1ae)

Code coverage: 89.2%

### How to run this project
- Clone project
- Rename .env-copy to .env
- Replace values from .env with your keys from marvel api docs, and change path for logs
- Replace "DefaultConnection" value in "ConnectionStrings" inside appsettings.Development.json
- Run migrations from package manager console `Update-Database`
