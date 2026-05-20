# MUEats

In large infrastructure hubs such as university campuses, corporate headquarters, or private business centers, local canteens and cafes often operate independently. 
Due to high commission fees, bureaucratic hurdles, or technical limitations, these local vendors cannot integrate with mainstream food delivery aggregators (like Foodpanda or Keeta).

This fragmentation leads to critical inefficiencies:

- Peak-Hour Bottlenecks: Massive physical queues during lunch breaks (e.g., 11:00 - 16:00), leading to long waiting times and lost revenue for vendors.
- No Pre-ordering Support: Customers cannot order ahead for a quick pick-up; they must wait for food creation on-site.
- Lack of Internal Digitization: Most local organization's canteens lack dedicated systems to process digital pre-orders.

MUEats bridges this gap by providing a lightweight, low-overhead, high-performance solution tailored for localized pick-up ecosystems, allowing users to pre-order food and bypass the queues entirely.

# Business product roadmap

Note: This is an educational and portfolio project designed to deeply explore complex distributed system patterns manually, rather than relying on heavy high-level frameworks.
While currently a fully functioning prototype with real-world deployment potential for localized hubs, there is a clear strategic pipeline to upgrade MUEats into a production-grade enterprise system.

1. Core Business Features
- Payment Flow Integration: Design and subsequently integrate a dedicated payment service handling secure transactions and refunds as well as holding canteen's and platform's revenue.
- Notifications & Alerts: Implement an notification system to dispatch order status updates, promotions and etc. through vairous channels like SMS, WeChat, Email, etc.
- ID verification service: Implement a service that provide discounts based on specific rules, for instance, if a customer is staff or student.  
- Discount & Promotions service: Design and develop a service that will allow creating different kinds of promotions & hot deals & loyalty programmes.

2. Operations & Tech Support
- Internal Ticketing System: Introduce a support ticket module allowing users to report wrong items, payment failures, or canteen delays, complete with automated SLA tracking.

# Technical roadmap

1. Deep Observability & Monitoring
- Metrics Collection: Integrate Prometheus to track system performance, API latency, and database connection pools.
- Visualization: Build operational dashboards in Grafana to monitor Kafka throughput and order processing metrics.
- Distributed Tracing: Implement OpenTelemetry (Jaeger) to trace the lifecycle of a single order as it passes through the Saga state machine and Kafka topics.

2. Cloud Native Infrastructure
- Container Orchestration: Migrate localized Docker Compose setups into a scalable Kubernetes (K8s) cluster setup with horizontal pod autoscaling (HPA) to handle dynamic lunch-hour spikes effortlessly.

3. Identity
- Redesign public/private keys management, in order to make service automatically download public key.

4. Database replication & backups

# Tech stack

- ASP.NET Core (.NET 10), EFCore
- PostgreSQL 16 (database)
- Apache Kafka (messaging)
- Docker (containerization)
- Prometheus & Grafana (Observability)

# Prerequisites

To start, please ensure you have following installed on your computer:
1) .NET 10 SDK 
2) Git
3) PostgreSQL 16
4) Docker

# Setup guide

!!! SOON WILL BE MOVED TO A SINGLE DOCKER-COMPOSE !!!

1) clone repository by using `git clone https://github.com/Grigoriyy0/MUEats.git`
2) in `src/core/MUEats.Api/appsetting.json`  in `"Postgres": "Host=localhost;Port=5432;Username=postgres;Password=123123;Database=mue.master"` section input your connection credentials to postgres
3) in `src/restaurants/MUEats.Restaurants/appsettings.json` in `"Postgres": "Host=localhost;Port=5432;Username=postgres;Password=123123;Database=mue.restaurants"` section input your connection credentials to postgres
4) generate RSA key, put the private key in `src/core/MUEats.Api/`, put the public key in 
5) from `solution items` folder run `docker compose up -d` to start infrastructure
6) from `src/core` folder run `dotnet run` to start API
7) from `src/restaurants` folder run `dotnet run` to start Restaurants service
8) from `src/gateway` folder run `dotnet run` to start Gateway
