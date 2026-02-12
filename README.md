# ASP.NET Shop (MVC + PostgreSQL)

Simple ASP.NET Core MVC shop app with cookie auth, admin panel, shopping cart, and fuzzy search.

## Requirements
- .NET SDK 9.x
- PostgreSQL 14+

## Quick Start (Linux)
1. Configure the connection string in `shop/appsettings.json`:
```
Host=localhost;Database=example;Username=test_user;Password=test_pwd
```
2. Create database and load schema:
```
sudo -iu postgres psql
CREATE USER test_user WITH PASSWORD 'test_pwd';
CREATE DATABASE example OWNER test_user;
\q

psql -h localhost -U test_user -d example -f schema.sql
```

3. Run the app:
```
cd aspnetwebsite/shop
dotnet run
```

## Database Notes
The app does **not** create tables automatically. You must load `schema.sql` before the first run.

On startup, the app seeds initial data:
- roles: `admin`, `user`
- default admin account
- 2 categories
- 10 sample products

## Default Admin Account
- email: `admin@admin.com`
- password: `admin`

## Features
- Cookie-based authentication
- Roles: `admin` and `user`
- Admin panel:
  - add product
  - add category
  - show users
  - show orders
- Shopping cart stored in cookies (per user)
- Search:
  - exact keyword match
  - fuzzy match using Levenshtein edit distance
