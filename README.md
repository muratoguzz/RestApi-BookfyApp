# ASP.NET Core REST API

This project is a comprehensive REST API built with ASP.NET Core, designed to showcase a variety of essential features for modern web applications.

## Features

- **Logging**: Centralized logging for all API requests and responses.
- **Content Negotiation**: Supports JSON and XML formats.
- **Validation**: Ensures incoming data meets the required standards.
- **Action Filters**: Custom logic executed before or after an action method.
- **Pagination, Filtering, Searching, Sorting**: Efficient data management and retrieval mechanisms.
- **Data Shaping**: Return only the fields requested by the client.
- **Support for HEAD and OPTIONS Verbs**: Provides additional HTTP verb functionality.
- **Media Type Support**: Handles both JSON and XML responses.
- **Versioning**: Supports multiple API versions.
- **Caching**: Improves performance by reducing the need to fetch the same data multiple times.
- **Rate Limiting**: Protects the API from being overwhelmed by requests.
- **Security**: Implements JWT for secure authentication.

## Documentation

Detailed documentation for each feature can be found through the following Postman links:

- [Books API](https://documenter.getpostman.com/view/36013169/2sA3dxEsAo#8c40889e-d9db-4309-acfb-f3afd35488e4)
- [Authentication API](https://documenter.getpostman.com/view/36013169/2sA3dxEsAp)
- [Books V2 API](https://documenter.getpostman.com/view/36013169/2sA3dxEsF6)
- [Categories API](https://documenter.getpostman.com/view/36013169/2sA3dxEsF7)
- [Root API](https://documenter.getpostman.com/view/36013169/2sA3dxEsF8)

## NuGet Packages

This project uses the following NuGet packages:

- **AspNetCoreRateLimit**: Implements rate limiting to prevent abuse of the API.
- **AutoMapper.Extensions.Microsoft.DependencyInjection**: Simplifies object-object mapping.
- **Marvin.Cache.Headers**: Adds caching headers to improve performance.
- **Microsoft.AspNetCore.Authentication.JwtBearer**: Implements JWT Bearer token authentication.
- **Microsoft.AspNetCore.EntityFrameworkCore**: Essential for working with Entity Framework Core in ASP.NET Core.
- **Microsoft.AspNetCore.JsonPatch**: Supports JSON Patch operations.
- **Microsoft.AspNetCore.Mvc.Core**: Core functionalities for ASP.NET Core MVC.
- **Microsoft.AspNetCore.Mvc.NewtonsoftJson**: Adds support for JSON serialization using Newtonsoft.Json.
- **Microsoft.AspNetCore.Mvc.Versioning**: Provides API versioning support.
- **Microsoft.EntityFrameworkCore**: Core package for Entity Framework Core.
- **Microsoft.EntityFrameworkCore.Design**: Includes tools required for design-time development.
- **Microsoft.EntityFrameworkCore.SqlServer**: Enables SQL Server as the database provider.
- **Microsoft.EntityFrameworkCore.Tools**: Includes tools required for Entity Framework Core.
- **Microsoft.IdentityModel.Tokens**: Provides key and token validation support for JWT.
- **Nlog.Extensions.Logging**: Integrates NLog with ASP.NET Core for logging.
- **System.IdentityModel.Tokens.Jwt**: Provides support for JWT token creation and validation.
- **System.Linq.Dynamic.Core**: Adds dynamic LINQ capabilities to your project.

## Getting Started

### Prerequisites

- .NET SDK 6.0 or later

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/your-repo-name.git
   cd your-repo-name
