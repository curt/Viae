# Viae

A modern, geo-location focused blog platform with ActivityPub integration for the decentralized social web.

## Features

- **Geographic Focus**: All posts can be associated with locations using PostGIS
- **ActivityPub Integration**: Federate with Mastodon, Pleroma, and other ActivityPub services
- **Modern Stack**: Vue.js frontend, .NET 10 backend, PostgreSQL with PostGIS
- **AWS Cognito Auth**: Secure authentication using AWS Cognito
- **Docker Ready**: Full containerization with docker-compose
- **Modular Architecture**: Clean separation of concerns with layered architecture

## Quick Start

### Prerequisites

- Docker and Docker Compose (for containerized deployment)
- .NET 10 SDK (for backend development)
- Node.js 20+ (for frontend development)
- PostgreSQL 13+ with PostGIS extension
- AWS Cognito User Pool (for authentication)

## Architecture

### Backend (.NET 10)

- **Domain**: Core business models and interfaces with NetTopologySuite for spatial data
- **ActivityPub**: ActivityPub protocol models and serialization
- **Persistence**: Entity Framework Core with PostgreSQL/PostGIS
  - NetTopologySuite.Geometries.Point for geographic coordinates
  - Convenience Latitude/Longitude properties (not mapped)
  - PostGIS geography columns with spatial indexes
- **Presentation.Mvc**: HTML/AS2 stream rendering with Fluid templates
- **Presentation.Api**: REST API for frontend
- **Presentation.ActivityPub**: ActivityPub inbox/outbox/well-known endpoints
- **HttpSignatures**: HTTP signature validation and generation (MIT License)
- **Postgres**: Base58 ID generation and type mapping (MIT License)
- **Host**: ASP.NET Core host application

### Frontend (Vue.js)

- Vue 3 with TypeScript
- Pinia for state management
- Tailwind CSS for styling
- Leaflet.js for map visualization
- AWS Cognito integration

### Database

- PostgreSQL with PostGIS extension
- Base58 ID custom type
- Code-first migrations

## Authentication

Viae uses AWS Cognito for secure authentication with HTTP-only cookies:

### Security Features

- **HTTP-only cookies**: JWT tokens stored in HTTP-only cookies to prevent XSS attacks
- **Secure flag**: Cookies only sent over HTTPS in production
- **SameSite=Strict**: Prevents CSRF attacks
- **Backend validation**: All requests validated by custom authentication handler
- **Automatic session restoration**: Frontend checks authentication on app load

## Identifiers

All public-facing identifiers use Base58 encoding with a flake-like generation strategy for distributed uniqueness.
