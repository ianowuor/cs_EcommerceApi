# Ecommerce API - .NET 10 Backend

A high-performance, secure e-commerce RESTful API built with **.NET 10**, featuring a decoupled architecture and modern security standards.

## Key Features
* **Advanced Security:** Full User Identity system using **JWT (JSON Web Tokens)** for secure, stateless authentication.
* **DTO Pattern:** Complete separation of concerns using **Data Transfer Objects** for request/response mapping.
* **Media Handling:** Local storage implementation for product image uploads via `multipart/form-data`.
* **Database Management:** **PostgreSQL** integration using **Entity Framework Core** with a code-first approach.
* **Modern .NET 10 Stack:** Utilizing the latest performance improvements and C# features.

## Tech Stack
* **Framework:** .NET 10 (ASP.NET Core)
* **Database:** PostgreSQL (running in Docker)
* **ORM:** Entity Framework Core
* **Security:** ASP.NET Core Identity + JWT Bearer Authentication
* **Storage:** Local static file hosting (wwwroot)

## Getting Started
1. **Prerequisites:** Ensure [.NET 10 SDK](https://dotnet.microsoft.com/download) and Docker are installed.
2. **Infrastructure:** Start the database container:
   ```bash
   docker-compose up -d
3. **Database Setup:** Apply migrations:
    ```bash
    dotnet ef database update
4. **Run Project:**
    ```bash 
    dotnet run
