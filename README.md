Movie Seat Booking –  Backend System

This project implements a backend system responsible for managing movie show seat availability and booking behavior, focusing on correctness, concurrency safety, and reliability. It is built using ASP.NET Core with Entity Framework Core (Code First) and SQL Server.

## Problem Description
The core challenge is to manage a limited number of seats for a movie show where multiple users may attempt to book tickets simultaneously. The system must ensure accurate seat availability and prevent overbooking (no seat sold more than once) .

## Technical Scope
*   **Backend-only:** No UI, payment processing, user authentication, movie listings, or theatre management are implemented.
*   **Technology Stack:** ASP.NET Core (.NET 8), Entity Framework Core (Code First), SQL Server.
*   **Architecture:** Clean Architecture (API / Application / Infrastructure / Domain).

## Quick Start (Local Setup)

### Prerequisites
*   .NET 8 SDK
*   SQL Server (LocalDB, SQL Server Express, or a full instance)

### Setup Steps
1.  **Clone the repository:**
    ```bash
    git clone <YOUR_GITHUB_REPO_LINK>
    cd BookMyShow/BookMyShow
    ```
2.  **Configure Database Connection String:**
    *   Open `BookMyShow.API/appsettings.Development.json` (or `appsettings.json` for production).
    *   Update the `DefaultConnection` string to point to your SQL Server instance. Example for LocalDB:
        ```json
        "ConnectionStrings": {
            "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BookMyShowDb;Trusted_Connection=True;MultipleActiveResultSets=true"
        }
        ```
3.  **Apply Entity Framework Core Migrations:** The project uses a Code First approach. The database schema and initial seed data are managed via EF Core Migrations.
    *   Open a terminal in the solution root (`BookMyShow/BookMyShow`).
    *   Add a new migration (if any changes were made after setup or to generate the initial schema):
        ```bash
        dotnet ef migrations add InitialCreate -p BookMyShow.Infrastructure -s BookMyShow.API
        ```
        *(Note: If you've already generated migrations and only need to update the database, skip this step.)*
    *   Apply migrations to create or update the database schema and seed initial data:
        ```bash
        dotnet ef database update -p BookMyShow.Infrastructure -s BookMyShow.API
        ```
4.  **Run the API:**
    *   Navigate to the solution root (`BookMyShow/BookMyShow`).
    *   Run the API project:
        ```bash
        dotnet run --project BookMyShow.API
        ```
    *   The API will typically run on `https://localhost:7068` (or a similar port). Swagger UI will be available at `/swagger` (e.g., `https://localhost:7068/swagger`).

## Scope of the System
### Included
*   Seat availability tracking
*   Temporary seat holds with configurable expiry
*   Seat booking confirmation
*   Concurrency handling to prevent overbooking
*   Automatic release of expired unconfirmed seat holds
*   Idempotent API operations for confirmations and cancellations
*   Consistent error handling

### Out of Scope
*   Payments processing
*   User authentication / authorization
*   User Interface (UI) / Frontend
*   Complex seat selection logic (e.g., specific algorithms for picking contiguous seats)
*   Theatre or movie management beyond basic show details
*   Advanced distributed system components (e.g., Redis, message queues, distributed transactions)

## High-Level Design (Clean Architecture)
The project is structured into four layers, emphasizing separation of concerns, testability, and independence from frameworks and databases.

### 1. `BookMyShow.Domain` (Core Layer)
*   **Purpose:** Contains core business entities, value objects, and pure domain rules. It has no dependencies on other layers or external frameworks.
*   **Contents:**
    *   `MovieShow`: Represents a movie showing (`Id`, `MovieTitle`, `ShowTime`, `ScreenNumber`, `TotalSeats` - all `Guid` based IDs).
    *   `Seat`: Represents an individual seat (`Id`, `MovieShowId`, `SeatNumber`, `Status`, `BookingId`, `ExpiryTime`, `RowVersion`).
    *   `Booking`: Represents a user's booking attempt (`Id`, `CorrelationId`, `MovieShowId`, `BookingStatus`, `BookingTime`, `ExpiryTime`).
    *   `Enums`: `SeatStatus` (`Available`, `Held`, `Booked`), `BookingStatus` (`Pending`, `Confirmed`, `Cancelled`, `Expired`).

### 2. `BookMyShow.Application` (Application Layer)
*   **Purpose:** Defines the application's use cases, data transfer objects (DTOs), and interfaces for application-level services. It depends only on the `BookMyShow.Domain` layer.
*   **Contents:**
    *   `Interfaces/IBookingService`: Defines application-level booking workflows (e.g., `HoldSeatsAsync`, `ConfirmBookingAsync`, `CancelBookingAsync`, `GetMovieShowAvailabilityAsync`, `GetAllMovieShowsAsync`, `ReleaseExpiredHeldSeatsAsync`).
    *   `DTOs`: Data Transfer Objects for request/response payloads and data summaries (`BookSeatsRequest`, `BookSeatsResponse`, `MovieShowAvailabilityDto`, `MovieShowSummaryDto`).

### 3. `BookMyShow.Infrastructure` (Infrastructure Layer)
*   **Purpose:** Provides concrete implementations of interfaces defined in the `Application` layer. Handles external concerns like database access (EF Core), background services, and specific SQL Server features.
*   **Contents:**
    *   `Data/ApplicationDbContext`: EF Core `DbContext` for SQL Server. Configures entities, `RowVersion` for optimistic concurrency, and enum-to-string conversions. Includes seed data for `MovieShow` and `Seat` entities.
    *   `Services/BookingService`: Concrete implementation of `IBookingService`. Contains core business logic, including pessimistic locking and secure SQL parameterization.
    *   `Services/ExpiredSeatsReleaseService`: An `IHostedService` that runs periodically to automatically release expired held seats.

### 4. `BookMyShow.API` (Presentation / API Layer)
*   **Purpose:** The entry point for clients, exposing RESTful API endpoints. It depends only on the `BookMyShow.Application` layer.
*   **Contents:**
    *   `Controllers/MovieShowsController`: Exposes API endpoints for `GET /api/MovieShows` (all shows), `GET /api/movieshows/{id}/availability`, `POST /api/movieshows/{id}/hold-seats`, `POST /api/bookings/{bookingId}/confirm`, and `POST /api/bookings/{bookingId}/cancel`.
    *   `Program.cs`: Configures dependency injection for `IBookingService` and `ExpiredSeatsReleaseService`, EF Core, and Swagger.

## Seat Booking Flow & API Endpoints
All IDs (MovieShowId, BookingId, CorrelationId) are `Guid`s.

### 1. Get All Movie Shows
**Endpoint:** `GET /api/MovieShows`
**Description:** Retrieves a list of all available movie shows with summary details.
**Response Example:**
```json
[
  {
    "id": "67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16",
    "movieTitle": "Spider-Man: No Way Home",
    "showTime": "2026-01-24T16:56:45Z",
    "screenNumber": "Screen 2",
    "totalSeats": 75
  }
]
```

### 2. Check Seat Availability for a Specific Show
**Endpoint:** `GET /api/movieshows/{id}/availability`
**Description:** Returns the total, available, held, and booked seat counts for a given movie show.
**Response Example:**
```json
{
  "movieShowId": "67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16",
  "totalSeats": 75,
  "availableSeats": 60,
  "heldSeats": 5,
  "bookedSeats": 10
}
```

### 3. Hold Seats
**Endpoint:** `POST /api/movieshows/{id}/hold-seats`
**Request Body Example:**
```json
{
  "movieShowId": "67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16",
  "seatNumbers": ["A1", "A2", "B1"],
  "correlationId": "a1b2c3d4-e5f6-7890-1234-567890abcdef"
}
```
**Behavior:**
*   Validates `MovieShowId` and requested `seatNumbers`.
*   Atomically marks specified `Available` seats as `HELD` within a database transaction.
*   Creates a `PENDING` `Booking` record with an `ExpiryTime`.
*   Employs pessimistic locking to prevent concurrent access to the same seats.
**Response Example (Success):**
```json
{
  "bookingId": "some-new-guid",
  "success": true,
  "message": "Seats held successfully."
}
```
**Response Example (Failure - 400 Bad Request):**
```json
{
  "bookingId": "00000000-0000-0000-0000-000000000000",
  "success": false,
  "message": "Some selected seats are no longer available or are held by another user."
}
```

### 4. Confirm Booking
**Endpoint:** `POST /api/bookings/{bookingId}/confirm`
**Description:** Transitions a `PENDING` booking (and its associated `HELD` seats) to `CONFIRMED`.
**Behavior:**
*   Validates the `bookingId` and checks if the booking is `PENDING` and **not expired**.
*   Atomically transitions seats from `HELD` to `BOOKED` and sets `BookingStatus` to `CONFIRMED`.
*   The operation is idempotent: attempting to confirm an already `CONFIRMED` booking will return success.
**Response Example (Success - 200 OK):**
```
Booking confirmed successfully.
```

### 5. Cancel Booking
**Endpoint:** `POST /api/bookings/{bookingId}/cancel`
**Description:** Reverts a `PENDING` booking (and its associated `HELD` seats) to `CANCELLED` and makes seats `AVAILABLE`.
**Behavior:**
*   Validates the `bookingId` and checks if the booking is `PENDING`.
*   **Important Limitation:** Only `PENDING` bookings can be cancelled. Confirmed bookings require separate (out-of-scope) payment reversal logic.
*   Atomically transitions seats from `HELD` to `AVAILABLE` and sets `BookingStatus` to `CANCELLED`.
*   The operation is idempotent.
**Response Example (Success - 200 OK):**
```
Booking cancelled successfully.
```

## Concurrency Handling
The system is designed to prevent overbooking under high concurrency by utilizing:
*   **Database Transactions:** All multi-step state changes (holding, confirming, cancelling) are wrapped in atomic database transactions, ensuring data consistency.
*   **Pessimistic Locking (SQL Server `UPDLOCK, ROWLOCK, READPAST`):** During the `HoldSeatsAsync` operation, exclusive row-level locks are acquired on selected `Seat` records. `READPAST` allows concurrent requests targeting already locked seats to be immediately skipped and rejected, preventing deadlocks and reducing contention.
*   **Optimistic Concurrency (`RowVersion`):** A `RowVersion` (SQL `timestamp`) column on the `Seat` entity acts as a safety net, detecting and preventing 'lost updates' if a record is modified by another transaction unexpectedly.
*   **Seat State Validation:** Explicit checks ensure seats are in the `Available` state before holding, or `Held` before confirming/cancelling.

This combination guarantees that seats are never oversold, and concurrent requests are handled safely, with conflicting requests failing gracefully.

## Seat Hold Expiry & Automatic Release
*   When seats are held, an `ExpiryTime` is set (e.g., 1 minute for testing, configurable via `appsettings.json`'s `SeatHold:ExpiryMinutes`).
*   The `ExpiredSeatsReleaseService` (`IHostedService`) runs periodically in the background.
*   It automatically identifies and processes `HELD` seats (and their `PENDING` bookings) that have passed their `ExpiryTime`, reverting the seats to `AVAILABLE` and marking the booking `EXPIRED`.
*   Crucially, `ConfirmBookingAsync` also checks `ExpiryTime`, so even if the background service hasn't run yet, an expired hold cannot be confirmed.
*   The system survives restarts, as all state is persisted in the database, and the hosted service will resume cleanup.

## Error Handling
API errors (e.g., show not found, insufficient seats, expired hold) are returned with appropriate HTTP status codes and descriptive messages in the response body.

## Idempotency
*   **`CorrelationId`:** Clients should provide a unique `CorrelationId` with each logical `HoldSeats` request. This ID is stored with the `Booking` entity.
*   **Confirm/Cancel Idempotency:** The `ConfirmBookingAsync` and `CancelBookingAsync` methods explicitly check the current `BookingStatus`. If an operation is attempted on a booking that is already in the target state (e.g., confirming an already confirmed booking), the system will simply return success without re-processing, ensuring idempotency.
    *   *(Note: For `HoldSeatsAsync`, advanced idempotency (returning existing `BookingId` for duplicate holds) could be implemented by checking for an existing `PENDING` booking with the same `CorrelationId` and requested seats, but is omitted for simplicity in this task.)*

## Edge Cases Considered
*   Invalid `MovieShowId`
*   Requesting zero or negative seats
*   Insufficient available seats for a hold request
*   Concurrent attempts to hold/confirm/cancel the same seats/booking
*   Attempting to confirm an expired hold
*   Attempting to cancel a non-pending (e.g., confirmed) booking
*   System restarts during critical operations (handled by transactions and background service)

## Architecture Notes
*   **Clean Architecture:** Clear separation of concerns (Domain, Application, Infrastructure, API layers).
*   **Business Logic:** Primarily resides within the `BookingService` in the `Infrastructure` layer (as the concrete implementation of `IBookingService`), orchestrating domain entities and persistence.
*   **Data Access:** Isolated in the `Infrastructure` layer using EF Core, ensuring the `Domain` and `Application` layers remain persistence-ignorant.
*   **Controllers:** Remain thin, primarily responsible for routing HTTP requests to application services and formatting responses.
*   **Over-engineering:** Intentionally avoided, focusing on a lean and effective solution for the given problem constraints.

## Summary
This system prioritizes correctness, security (via parameterized SQL), and reliability under concurrent usage, while maintaining a clear, simple, and maintainable Clean Architecture design.
