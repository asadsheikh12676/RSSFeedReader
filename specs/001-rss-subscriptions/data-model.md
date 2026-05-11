# Data Model: RSS Feed Subscription Management

**Feature**: RSS Feed Subscription Management (MVP)  
**Version**: 1.0.0  
**Scope**: MVP — Subscription management only, no feed operations

## Overview

The data model for the MVP consists of a single entity (`Subscription`) representing a user's feed subscription. All subscriptions are stored in an in-memory collection during the application session.

## Entities

### Subscription

Represents a single RSS/Atom feed subscription.

#### Fields

| Field | Type | Required | Constraints | Notes |
|-------|------|----------|-------------|-------|
| `Id` | `Guid` | Yes | Unique identifier | Generated on creation; used for future removal/updates |
| `Url` | `string` | Yes | Non-empty string | Feed URL as entered by user; no validation applied in MVP |
| `AddedAt` | `DateTime` | No (MVP) | ISO 8601 format | Timestamp of subscription creation; reserved for future use (sorting, filtering) |

#### Creation Logic

```csharp
public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
```

#### Validation Rules (MVP)

- **Url field**: 
  - Must be non-empty string (enforce at API layer)
  - No format validation (URL validity checked in Extended-MVP)
  - Duplicates allowed (no deduplication in MVP)

#### Relationships

**None for MVP**: Subscriptions are independent entities with no foreign keys or relationships.

**Future Extensions** (Extended-MVP and beyond):
- User (when multi-user support added)
- Category/Folder (when organization features added)
- FeedItems (when item display added)
- ReadStatus (when read/unread tracking added)

---

## Collections

### SubscriptionList

In-memory collection of all user subscriptions.

#### Responsibilities

| Operation | Input | Output | Behavior |
|-----------|-------|--------|----------|
| `AddSubscription(url)` | URL string | `Subscription` object | Create new subscription, add to collection, return created entity |
| `GetSubscriptions()` | None | `List<Subscription>` | Return all subscriptions in insertion order |

#### Storage Implementation

**MVP**: In-memory `List<Subscription>` in a singleton service

```csharp
public class SubscriptionService
{
    private readonly List<Subscription> _subscriptions = new();

    public Subscription AddSubscription(string url)
    {
        var subscription = new Subscription { Url = url };
        _subscriptions.Add(subscription);
        return subscription;
    }

    public List<Subscription> GetSubscriptions()
    {
        return _subscriptions.ToList(); // Return copy for safety
    }
}
```

#### Scope & Lifetime

- **Scope**: Application-scoped singleton service (shared across all requests during session)
- **Lifetime**: Persists for duration of application uptime
- **Loss**: All subscriptions lost when application stops or is restarted
- **Concurrency**: Single-threaded for MVP; thread safety deferred to Extended-MVP if concurrent API calls needed

---

## State Transitions

### Subscription Lifecycle (MVP)

```
[Not Exists] 
    ↓ (User adds URL)
[Added to List]
    ↓ (Displayed in UI)
[Visible to User]
    ↓ (Application stops)
[Destroyed]
```

**Deletion**: Not supported in MVP. All subscriptions deleted when application stops.

### Future State Transitions (Extended-MVP)

```
[Added to List] 
    ↓ (User clicks Refresh)
[Fetching]
    ↓ (Feed fetch succeeds)
[Active - Contains Items]
    ↓ (User deletes subscription)
[Deleted]
```

---

## Query Patterns

### MVP Queries

1. **Get all subscriptions** (used by frontend to display list)
   ```
   Query: GetSubscriptions()
   Result: List of all Subscription objects in insertion order
   ```

2. **Add new subscription** (triggered by user form submission)
   ```
   Query: AddSubscription(url: string)
   Result: Newly created Subscription object
   ```

### Future Query Patterns (Extended-MVP+)

- Get subscriptions by category (requires Category field)
- Get subscriptions with unread count (requires FeedItem relationship)
- Get subscriptions modified since (requires LastFetchedAt field)
- Search subscriptions by URL pattern (requires indexing)

---

## Data Integrity & Constraints

### MVP Constraints

- **No NULL values**: All Subscription Url fields must have non-empty string value
- **No duplicates at application level**: Duplicates are silently allowed (deduplication post-MVP if needed)
- **No orphaned data**: No relationships to manage in MVP

### Validation Responsibility

- **API Layer**: Validate URL field is non-empty before adding
- **Data Layer**: No additional validation (no persistence layer to enforce constraints)
- **Client Layer**: Display validation messages to user (optional for MVP)

---

## Forward Compatibility

This data model is designed to support future extensions without breaking API changes:

**Phase 1 (MVP)**: `Subscription` with `Url` field only
```json
{ "id": "guid", "url": "https://..." }
```

**Phase 2 (Extended-MVP)**: Add timestamps and status
```json
{ 
  "id": "guid", 
  "url": "https://...",
  "addedAt": "2026-05-08T10:30:00Z",
  "lastFetchedAt": "2026-05-08T10:31:00Z",
  "status": "active"
}
```

**Phase 3 (Production)**: Add categories, read tracking, items
```json
{ 
  "id": "guid", 
  "url": "https://...",
  "addedAt": "2026-05-08T10:30:00Z",
  "lastFetchedAt": "2026-05-08T10:31:00Z",
  "status": "active",
  "category": "tech-blogs",
  "unreadItemCount": 5,
  "totalItemCount": 42
}
```

All future additions can be made as optional fields without breaking existing clients that don't expect them.

---

## Glossary

| Term | Definition |
|------|-----------|
| **Subscription** | A single RSS/Atom feed URL that a user has added to the reader |
| **Feed URL** | The web address of an RSS or Atom feed (e.g., `https://example.com/feed.xml`) |
| **Collection** | In-memory list storing all subscriptions for the current session |
| **Session** | One execution cycle of the application from startup to shutdown |

---

**Version**: 1.0.0 | **Created**: 2026-05-08 | **Status**: Finalized for Phase 2
