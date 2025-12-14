# Product Order API

A .NET-based API for managing products, categories, carts, and orders, built with **ASP.NET Core**, **Entity Framework Core**, and **SQL Server**. This project supports user authentication, cart management, and order processing.

---

## Table of Contents

- [Overview](#overview)  
- [Features](#features)  
- [Tech Stack](#tech-stack)  
- [Setup Instructions](#setup-instructions)  
- [Assumptions](#assumptions)
- [Endpoints](#endpoints)
- [Notes](#notes) 
 
## Overview

The Product Order API allows users to:  

- Browse products and categories  
- Add products to their cart  
- Checkout and create orders  
- View, add, and delete orders  
- Delete items from their cart  

It leverages a **Unit of Work** pattern for clean separation of concerns and supports **JWT-based authentication**.

---

## Features

- JWT-based authentication and authorization  
- CRUD operations for orders and cart  
- Stock validation during order creation  
- Transaction handling for checkout  
- Auto-mapping between entities and DTOs for API responses  

---

## Tech Stack

| Layer | Technology / Library |
|-------|-------------------|
| Backend | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Authentication | JWT (JSON Web Tokens) |
| Dependency Injection | Built-in ASP.NET Core DI |
| DTO Mapping | Manual mapping (can be extended with AutoMapper) |

---

## Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
- Optional: [Postman](https://www.postman.com/) or Swagger for testing API endpoints

### Steps

 1. **Clone the repository**
bash
git clone https://github.com/olufasco/ProductOrderApi.git
cd ProductOrderApi

 2. Configure the database connection string**  
   Update the `appsettings.json` file with your SQL Server connection string:
json
"ConnectionStrings": {
  "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_username;Password=your_password;"
  }

 3. Run database migrations and update the database**  
   Open a terminal in the project directory and run:
   bash
   dotnet ef database update
   
 4. **Run the application**
	bash
   dotnet run

5. Test the API endpoints using Postman or Swagger.
   

### Assumptions

- Users must be authenticated to manage their cart or orders
- Each product has a unique SKU
- Stock is validated during order creation
- Orders cannot be created if stock is insufficient
- Users can only view their own orders
- Deleted cart items will not be recoverable
- Deleting a cart item or order only affects the logged-in user

### Endpoints

| Method         | Endpoint                  | Description                     |
| -------------- | ------------------------- | ------------------------------- |
|   **Auth**     |                           |                                 |
| POST           | `/api/auth/register`      | Register user                   |
| POST           | `/api/auth/login`         | Login user                      |
|  **Products**  |                           |                                 |
| GET            | `/api/products`           | Get all products                |
| GET            | `/api/products/{id}`      | Get product by ID               |
| POST           | `/api/products`           | Create a new product            |
| PUT            | `/api/products/{id}`      | Update an existing product      |
| DELETE         | `/api/products/{id}`      | Delete a product                |
| **Categories** |                           |                                 |
| GET            | `/api/categories`         | Get all categories              |
| GET            | `/api/categories/{id}`    | Get category by ID              |
| POST           | `/api/categories`         | Create a new category           |
| PUT            | `/api/categories/{id}`    | Update an existing category     |
| DELETE         | `/api/categories/{id}`    | Delete a category               |
| **Cart**       |                           |                                 |
| GET            | `/api/Cart`               | Get current user’s cart         |
| POST           | `/api/Cart/add`           | Add a product to cart           |
| DELETE         | `/api/Cart/item/{sku}`    | Remove a product from cart      |
| POST           | `/api/Cart/checkout`      | Checkout cart and create order  |
| DELETE         | `/api/Cart/clear`         | Clear all items from cart       |
| **Orders**     |                           |                                 |
| GET            | `/api/orders`             | Get all orders for current user |
| POST           | `/api/orders`             | Add a new order                 |
| DELETE         | `/api/orders/{orderId}`   | Delete an order                 |

### Notes

- API responses are wrapped in ApiResponse<T> for consistency
- Uses Unit of Work pattern to manage repositories and transactions
- Designed for extensibility: more repositories and DTOs can be added easily
- Circular references are handled to prevent JSON serialization errors
- Manual mapping is used, but can be replaced with AutoMapper for   scalability
