# Influunt Developer Guide

## Project Overview
Influunt is a .NET 8 + Vue.js RSS aggregator with MongoDB storage and Redis caching.

## Key Technologies
- Backend: .NET 8, ASP.NET Core
- Frontend: Vue.js 2.x with Vue CLI 3
- Database: MongoDB
- Caching: Redis
- Authentication: Google OAuth2 and guest login

## Project Structure
```
src/
├── Influunt.Host/          # Main .NET backend with Vue frontend
│   ├── ClientApp/          # Vue.js frontend
│   └── appsettings.json    # Configuration
└── Influunt.Feed/          # Feed processing logic
└── Influunt.Storage/       # Data access layer
```

## Development Commands
### Backend (.NET)
- Build: `dotnet build`
- Run: `dotnet run` (from src/Influunt.Host/)
- Test: `dotnet test` (from tests/ directory)

### Frontend (Vue.js)
- Install dependencies: `npm install` (in src/Influunt.Host/ClientApp/)
- Development server: `npm run serve`
- Production build: `npm run build`
- Lint: `npm run lint`

## Important Notes
1. **Environment Setup**: 
   - Requires .NET 8 SDK
   - Requires Node.js 16+ (due to legacy provider flag in package.json)
   - Requires MongoDB and Redis instances

2. **Configuration**:
   - All settings in `appsettings.json` or environment variables
   - Google Auth requires ClientId/ClientSecret
   - Connection strings for MongoDB and Redis

3. **Build Process**:
   - .NET builds automatically restore npm packages for the frontend
   - Vue build requires `NODE_OPTIONS=--openssl-legacy-provider` environment variable

4. **Testing**:
   - Tests are in the `tests/` directory
   - Run with `dotnet test` from the test project directory

5. **Docker Deployment**:
   - Use provided docker-compose or docker run commands in README
   - Requires MongoDB and Redis connection strings
   - Google Auth credentials for authentication

6. **Architecture**:
   - Uses VueCliMiddleware for frontend integration
   - Separated concerns: Host (web), Feed (processing), Storage (data)
   - Background feed crawler enabled by default