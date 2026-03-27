# QuickToCash Supplier Dashboard

Take-home implementation of a Supplier Dashboard with:
- ASP.NET Core Web API backend (`QuickToCash.Api`)
- Angular frontend (`quicktocash-ui`)
- In-memory data and request flow for early payments

## Prerequisites

- .NET SDK 10
- Node.js 20+
- npm 11+

## Setup

From the repository root:

```bash
# Backend restore
dotnet restore "QuickToCash.Api/QuickToCash.Api.csproj"

# Frontend install
cd quicktocash-ui
npm install
```

## Run the backend

In one terminal:

```bash
cd QuickToCash.Api
dotnet run
```

Default local URLs:
- `http://localhost:5237`
- `https://localhost:7263`

## Run the frontend

In a second terminal:

```bash
cd quicktocash-ui
npm start
```

Frontend runs on:
- `http://localhost:4200`

The frontend API base URL is configured in:
- `quicktocash-ui/src/app/core/constants/api.constants.ts`

## Run tests

### Backend tests

```bash
dotnet test "QuickToCash.Api.Tests/QuickToCash.Api.Tests.csproj"
```

### Frontend tests

```bash
cd quicktocash-ui
npm test
```

## Dashboard behavior

- Loads invoices for a supplier
- Supports client-side status filtering and search
- Displays summary counts (All/Pending/Approved/Funded/Rejected)
- Opens invoice detail side panel
- Fetches early-payment eligibility
- Shows fee, disbursement, and days early
- Uses a confirmation step before submitting request
- Prevents duplicate submissions in UI after successful request

## Assumptions

- Authentication is not implemented yet; mock auth header is injected by HTTP interceptor
- Data is in-memory only (no persistence across backend restarts)
- A default supplier (`SUP-100`) is used for initial dashboard load
- No background jobs/workflow engine; request flow is synchronous and API-driven

