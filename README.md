# MUEats.API

Modern backend API for Restaurants, ShoppingCarts, Users, Orders management. 
System is designed as a food-service that allow make orders for pick-up



# Tech stack

- ASP.NET Core (.NET 10), EFCore
- PostgreSQL 16 (database)
- Apache Kafka (messaging)
- Docker (containerization)

# Prerequisites

To start, please insure you have following installed on your computer:
1) .NET 10 SDK 
2) Git
3) PostgreSQL 16
4) Docker

# Setup guide
1) clone repository by using `git clone https://github.com/Grigoriyy0/MUEats.git`
2) in `src/MUEats.Api/appsetting.json`  in `"Postgres": "Host=localhost;Port=5432;Username=postgres;Password=123123;Database=mue.master"` section input your connection credentials to postgres
3) from `solution items` folder run `docker compose up -d` to start kafka
4) from `src` folder run `dotnet run` to start API

# Swagger

visit `http://localhost:5138/swagger/index.html` URL to use Swagger