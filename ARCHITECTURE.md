# Architecture Overview

## Solution Overview

The solution uses a simple layered architecture:

- **Backend (`QuickToCash.Api`)**
  - REST API for invoice queries, eligibility checks, and early payment requests
  - Business logic in services
  - In-memory repositories and seeded data
  - Consistent API response envelope (`ApiResponse<T>`)

- **Frontend (`quicktocash-ui`)**
  - Angular + Angular Material
  - Feature-driven UI structure
  - Service-based HTTP access (no HTTP calls in components)
  - Dashboard UI with table, summary filters, detail side panel, and request flow

## Folder Structure

### Backend (high level)

- `Controllers/` - API endpoints
- `Services/` + `Services/Interfaces/` - business logic and contracts
- `Repositories/` + `Repositories/Interfaces/` - in-memory data access
- `Models/` - domain entities and enums
- `DTOs/` - API request/response contracts
- `Common/` - shared envelope types
- `Middleware/` - lightweight global exception handling
- `Seed/` - seed data for invoices and requests

### Frontend (high level)

- `src/app/core/` - app-wide services, interceptors, constants, global handlers
- `src/app/shared/` - shared material module and models
- `src/app/features/supplier-dashboard/` - dashboard module, routes, page component

## Key Design Decisions

1. **Keep business logic in backend services**
   - Controllers are thin and map service outcomes to HTTP status codes.
   - Repositories only handle data retrieval/storage.

2. **Use in-memory repositories**
   - Meets assignment requirement and keeps setup friction low.
   - Makes behavior easy to demo without database dependencies.

3. **Single API envelope**
   - All endpoints return `ApiResponse<T>` for consistency and easy UI handling.

4. **Simple frontend state model**
   - Explicit state properties in dashboard component (`allInvoices`, `filteredInvoices`, `summaryCounts`, etc.).
   - Derived state recomputed only when source data/filters change.
   - No heavy client-state library (NgRx/signals store) to keep complexity low.

5. **Side panel request flow**
   - Clicking an invoice opens detail panel and fetches eligibility.
   - Confirmation step before request submission.
   - Duplicate submission prevention at UI level after success.

## Future Improvements

- Add persistent database (e.g., PostgreSQL) and migrations
- Add authentication/authorization and real JWT handling
- Add backend integration tests for endpoint flows and status mappings
- Add frontend unit tests for dashboard filtering and request flow
- Add environment-based frontend API configuration (`environment.ts`)
- Add pagination/sorting for larger invoice datasets
