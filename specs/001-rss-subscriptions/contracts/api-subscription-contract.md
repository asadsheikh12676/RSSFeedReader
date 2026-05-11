# API Contract: Subscription Management

**Feature**: RSS Feed Subscription Management (MVP)  
**Service**: RSSFeedReader.Api (ASP.NET Core Web API backend)  
**Version**: 1.0.0  
**Status**: For Implementation

## Overview

The Subscription Management API provides endpoints for adding feed subscriptions and retrieving the list of all subscriptions. The API is designed to be consumed by the Blazor WebAssembly frontend running on a separate port.

## Base Configuration

| Property | Value |
|----------|-------|
| **Base URL** | `http://localhost:5151/api/` (local development) |
| **Protocol** | HTTP (local) / HTTPS (production) |
| **Format** | JSON |
| **Port** | 5151 (backend) |
| **CORS Policy** | Allow frontend origin: `http://localhost:5213` and `https://localhost:7025` |

---

## Endpoints

### 1. Add Subscription

**Endpoint**: `POST /api/subscriptions`

**Purpose**: Create a new subscription by providing a feed URL.

#### Request

```http
POST /api/subscriptions HTTP/1.1
Host: localhost:5151
Content-Type: application/json

{
  "url": "https://devblogs.microsoft.com/dotnet/feed/"
}
```

**Request Body**:

| Field | Type | Required | Validation | Notes |
|-------|------|----------|------------|-------|
| `url` | string | Yes | Non-empty | No format validation in MVP; user responsible for valid URL |

#### Response

**Success (201 Created)**:

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "url": "https://devblogs.microsoft.com/dotnet/feed/",
  "addedAt": "2026-05-08T10:30:00Z"
}
```

| Field | Type | Notes |
|-------|------|-------|
| `id` | string (UUID) | Unique subscription identifier; reserved for future use (removal, updates) |
| `url` | string | Feed URL as provided by user |
| `addedAt` | string (ISO 8601) | Server timestamp; reserved for future use (sorting, filtering) |

**Status Codes**:

| Code | Reason | Body |
|------|--------|------|
| 201 | Created | Subscription object (see above) |
| 400 | Bad Request | URL field is missing or empty |
| 500 | Internal Server Error | Unexpected error (rare for MVP) |

#### Example Client Usage

**C# (HttpClient)**:
```csharp
var client = new HttpClient { BaseAddress = new Uri("http://localhost:5151/api/") };
var request = new { url = "https://devblogs.microsoft.com/dotnet/feed/" };
var response = await client.PostAsJsonAsync("subscriptions", request);
var subscription = await response.Content.ReadAsAsync<Subscription>();
```

**JavaScript (Fetch)**:
```javascript
const response = await fetch('http://localhost:5151/api/subscriptions', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ url: 'https://devblogs.microsoft.com/dotnet/feed/' })
});
const subscription = await response.json();
```

---

### 2. Get Subscriptions

**Endpoint**: `GET /api/subscriptions`

**Purpose**: Retrieve all subscriptions for the current session.

#### Request

```http
GET /api/subscriptions HTTP/1.1
Host: localhost:5151
Accept: application/json
```

**Query Parameters**: None for MVP

#### Response

**Success (200 OK)**:

```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "url": "https://devblogs.microsoft.com/dotnet/feed/",
    "addedAt": "2026-05-08T10:30:00Z"
  },
  {
    "id": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
    "url": "https://example.com/feed.xml",
    "addedAt": "2026-05-08T10:31:15Z"
  }
]
```

**Status Codes**:

| Code | Reason | Body |
|------|--------|------|
| 200 | OK | Array of Subscription objects (empty array if no subscriptions) |
| 500 | Internal Server Error | Unexpected error (rare for MVP) |

#### Example Client Usage

**C# (HttpClient)**:
```csharp
var client = new HttpClient { BaseAddress = new Uri("http://localhost:5151/api/") };
var response = await client.GetAsync("subscriptions");
var subscriptions = await response.Content.ReadAsAsync<List<Subscription>>();
```

**JavaScript (Fetch)**:
```javascript
const response = await fetch('http://localhost:5151/api/subscriptions');
const subscriptions = await response.json();
```

---

## Data Models

### Subscription

Represents a feed subscription in the API response.

```csharp
public class Subscription
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public DateTime AddedAt { get; set; }
}
```

---

## Error Handling

### MVP Error Handling

**No detailed error messages for MVP** — application assumes valid input.

| Scenario | Response | Body |
|----------|----------|------|
| Empty URL | 400 Bad Request | `{ "error": "URL is required" }` |
| Malformed JSON | 400 Bad Request | ASP.NET Core default error response |
| Server crash | 500 Internal Server Error | Generic error message |

### Future Error Handling (Extended-MVP+)

- Invalid URL format → 400 with specific message
- Feed not accessible → 502 with retry hint
- Rate limiting → 429 with Retry-After header
- Authentication failures → 401 or 403 with realm info

---

## CORS Configuration

**Requirement**: Backend MUST explicitly allow frontend origin to avoid browser blocking requests.

**Configuration** (in `backend/RSSFeedReader.Api/Program.cs`):

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder
            .WithOrigins(
                "http://localhost:5213",      // Development (HTTP)
                "https://localhost:7025"      // Development (HTTPS, if applicable)
            )
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

app.UseCors("AllowFrontend");
```

**Critical**: Do NOT use wildcard (`*`) for production deployments.

---

## Testing Scenarios

### Happy Path

1. **Add first subscription**
   - POST `/api/subscriptions` with valid URL
   - Verify 201 response with subscription ID
   - Verify `id` is unique UUID
   - Verify `addedAt` is current UTC time

2. **Add multiple subscriptions**
   - POST `/api/subscriptions` three times with different URLs
   - GET `/api/subscriptions`
   - Verify all three subscriptions returned in insertion order

3. **Get subscriptions (empty state)**
   - GET `/api/subscriptions` without any POST calls first
   - Verify 200 response with empty array `[]`

### Edge Cases

1. **Empty URL**
   - POST `/api/subscriptions` with `{ "url": "" }`
   - Verify 400 Bad Request

2. **Missing URL field**
   - POST `/api/subscriptions` with `{}`
   - Verify 400 Bad Request

3. **Duplicate URL**
   - POST `/api/subscriptions` twice with same URL
   - GET `/api/subscriptions`
   - Verify both subscriptions returned (no deduplication in MVP)

---

## Deployment Notes

### Local Development

1. **Start Backend**: `dotnet run --project backend/RSSFeedReader.Api`
2. **Verify Port**: Backend accessible at `http://localhost:5151`
3. **Verify CORS**: Request from frontend at `http://localhost:5213` succeeds

### Production Deployment

- Use HTTPS (`https://` not `http://`)
- Update CORS policy with production frontend origin
- Use environment variables for configuration (not hardcoded)
- Store API base URL in secure configuration, not frontend code

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2026-05-08 | Initial MVP API with add/get subscriptions endpoints |

---

## Future Extensions (Extended-MVP+)

### Additional Endpoints

- `DELETE /api/subscriptions/{id}` — Remove subscription
- `GET /api/subscriptions/{id}/items` — Get feed items (Extended-MVP)
- `POST /api/subscriptions/{id}/refresh` — Manual feed refresh (Extended-MVP)
- `PUT /api/subscriptions/{id}` — Update subscription metadata (post-MVP)

### Field Additions

- `lastFetchedAt`: Timestamp of last successful feed fetch
- `status`: "active", "error", "fetching" (for Extended-MVP)
- `category`: Folder/category name (future)
- `unreadCount`: Number of unread items (future)

---

**Status**: Ready for Implementation | **Contract Version**: 1.0.0 | **Created**: 2026-05-08
