# Entity Framework Core Learning Reference
This application was created while completing a Dometrain course as a reference for myself.

Course link: [From Zero to Hero: Entity Framework Core in .NET](https://dometrain.com/course/from-zero-to-hero-entity-framework-core-in-dotnet/)

### Prerequisites
1. Docker or Sql Server (Save yourself and use Docker...)
2. If Docker, run this command to get the container running: `docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=P@ssw0rd123" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest`
3. Create the database by connecting to the db on localhost, port 1433 by running: `CREATE DATABASE MoviesDB;`