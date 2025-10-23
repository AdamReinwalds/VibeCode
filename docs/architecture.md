# E-Commerce MVP Architecture

## Overview

This is a minimum viable product (MVP) for an e-commerce application featuring login, basket, and checkout functionality. The backend is built with .NET and MongoDB, while the frontend uses Vue 3 with TypeScript.

## System Components

### Backend (.NET Web API + MongoDB)

- **Framework**: ASP.NET Core Web API
- **Database**: MongoDB (with MongoDB.Driver)
- **Authentication**: JWT-based authentication
- **Architecture**: Clean Architecture with layers (Presentation, Application, Domain, Infrastructure)

### Frontend (Vue 3 + TypeScript)

- **Framework**: Vue 3 with Composition API
- **Language**: TypeScript
- **State Management**: Pinia (for reactive state)
- **Routing**: Vue Router
- **HTTP Client**: Axios for API calls

## Data Flow

1. User interacts with Vue frontend
2. Frontend sends HTTP requests to .NET API
3. API validates requests and interacts with MongoDB
4. API returns JSON responses to frontend
5. Frontend updates UI based on responses

## Database Schema (MongoDB Collections)

- **Users**: User accounts with authentication info
- **Products**: Product catalog (for basket/checkout items)
- **Baskets**: User shopping baskets
- **Orders**: Completed purchases

## API Endpoints

- **Auth**: POST /api/auth/login, POST /api/auth/register
- **Basket**: GET /api/basket, POST /api/basket/add, DELETE /api/basket/remove
- **Checkout**: POST /api/checkout (creates order)

## Testing Strategy

- **Backend**: xUnit for unit tests and integration tests (80% coverage target)
- **Frontend**: Vitest for component function tests
- **E2E**: Playwright for UI testing

## Deployment

- Backend: Docker container with .NET runtime
- Frontend: Static hosting (e.g., Vercel, Netlify)
- Database: MongoDB Atlas or local MongoDB instance
