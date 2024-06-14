# ME.Commerce Application

This is an example e-commerce application showcasing the vertical slice architecture. The application is designed to demonstrate a modular and scalable approach to building software systems.

## Key features
- Pact (Contract tests)
- ASP.NET Core .NET 8
- Slice Architecture
- Contract Test
- TDD

## Product Listing feature
- InsertAsync
- GetProductAsync

## Shopping Cart feature
- AddToCart
- GetCartItems
- RemoveFromCart

## Project Structure

The project follows a structured folder organization:

- src
  - ME.Commerce.Core
    - Features
      - ProductListing
        - Contracts
        - Services
      - ShoppingCart
        - Contracts
        - Services
    - Shared
      - Logging
  - ME.Commerce.Infrastructure
    - Data
      - Repositories
    - ExternalServices
  - ME.Commerce.Web
    - Controllers
    - Views



## Vertical Slice Architecture

The application follows a vertical slice architecture pattern, where each feature module is organized as a self-contained unit. This architecture promotes modularity, encapsulation, and easier maintenance.

Key characteristics of the vertical slice architecture:

- **Feature Modules**: Each feature, such as product listing or shopping cart, has its own folder containing contracts and services specific to that feature. This modular structure facilitates independent development and testing of individual features.
- **Encapsulation**: Business logic and related components are encapsulated within their respective feature modules, ensuring clear separation and isolation of functionality.
- **Scalability**: The vertical slice architecture allows for easy scalability as new features can be added without impacting existing modules, enabling parallel development and deployment.

