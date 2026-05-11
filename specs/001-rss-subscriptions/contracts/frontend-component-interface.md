# Frontend Component Interface: Subscription Management

**Feature**: RSS Feed Subscription Management (MVP)  
**Framework**: Blazor WebAssembly (C# .NET 8)  
**Version**: 1.0.0  
**Status**: For Implementation

## Overview

The Blazor frontend provides a subscription management interface through reusable components. The main page (`Subscriptions.razor`) handles user input and displays the subscription list, while a reusable component (`SubscriptionItem.razor`) displays individual subscriptions.

---

## Components

### 1. Subscriptions Page

**Location**: `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`

**Purpose**: Main MVP page providing the subscription management interface (add subscription + display list).

#### Component Metadata

```csharp
@page "/"
@inject SubscriptionApiClient ApiClient
@implements IAsyncDisposable
```

#### State

| State Property | Type | Initial Value | Purpose |
|----------------|------|---------------|---------|
| `subscriptions` | `List<Subscription>` | `new List<Subscription>()` | All current subscriptions |
| `newUrl` | `string` | `""` | Input field value for new subscription URL |
| `isLoading` | `bool` | `false` | Show loading indicator while adding subscription |
| `error` | `string?` | `null` | Error message if subscription addition fails |

#### Events & Handlers

| Handler | Trigger | Behavior |
|---------|---------|----------|
| `OnAddSubscription()` | Click "Add Subscription" button | Call API, add subscription, clear input field, refresh list |
| `OnInputChanged(ChangeEventArgs args)` | Type in input field | Update `newUrl` state |
| `OnPageInitialized()` | Component loads | Fetch initial subscription list from API |

#### UI Layout

```
┌─────────────────────────────────────────┐
│ Subscription Manager (MVP)               │
├─────────────────────────────────────────┤
│                                          │
│ Add a new subscription:                 │
│ [Input field: "Enter feed URL"] [Add]  │
│                                          │
│ Your Subscriptions (N total):           │
│ ────────────────────────────────────── │
│ • https://devblogs.microsoft.com/...   │
│ • https://example.com/feed.xml         │
│ • https://another-blog.com/rss         │
│ ────────────────────────────────────── │
│                                          │
└─────────────────────────────────────────┘
```

#### Markup Structure

```razor
<div class="subscription-manager">
    <h1>Subscription Manager</h1>
    
    <!-- Add Subscription Form -->
    <div class="add-subscription-form">
        <label for="url-input">Add a new subscription:</label>
        <input 
            id="url-input"
            type="text" 
            placeholder="Enter feed URL"
            @bind="newUrl"
            @bind:event="oninput"
            @onkeypress="OnKeyPress"
        />
        <button @onclick="OnAddSubscription" disabled="@isLoading">
            @if (isLoading)
            {
                <span>Adding...</span>
            }
            else
            {
                <span>Add Subscription</span>
            }
        </button>
    </div>

    <!-- Error Message -->
    @if (!string.IsNullOrEmpty(error))
    {
        <div class="error-message">@error</div>
    }

    <!-- Subscription List -->
    <div class="subscription-list">
        <h2>Your Subscriptions (@subscriptions.Count total)</h2>
        @if (subscriptions.Count == 0)
        {
            <p class="empty-state">No subscriptions yet. Add one to get started!</p>
        }
        else
        {
            <ul>
                @foreach (var subscription in subscriptions)
                {
                    <SubscriptionItem Subscription="subscription" />
                }
            </ul>
        }
    </div>
</div>
```

#### Code Behind

```csharp
@code {
    private List<Subscription> subscriptions = new();
    private string newUrl = string.Empty;
    private bool isLoading = false;
    private string? error = null;

    protected override async Task OnInitializedAsync()
    {
        await LoadSubscriptions();
    }

    private async Task LoadSubscriptions()
    {
        try
        {
            subscriptions = await ApiClient.GetSubscriptionsAsync();
            error = null;
        }
        catch (Exception ex)
        {
            error = $"Failed to load subscriptions: {ex.Message}";
        }
    }

    private async Task OnAddSubscription()
    {
        if (string.IsNullOrWhiteSpace(newUrl))
        {
            error = "Please enter a URL";
            return;
        }

        isLoading = true;
        error = null;

        try
        {
            var subscription = await ApiClient.AddSubscriptionAsync(newUrl);
            subscriptions.Add(subscription);
            newUrl = string.Empty;
        }
        catch (Exception ex)
        {
            error = $"Failed to add subscription: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task OnKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnAddSubscription();
        }
    }
}
```

#### Accessibility Requirements

- [x] Input field has associated `<label>`
- [x] Button has descriptive text (not just icon)
- [x] Loading state indicated (button text changes)
- [x] Error messages display clearly
- [x] Empty state message when no subscriptions

#### Styling Requirements

- [x] Simple, functional CSS (no advanced styling for MVP)
- [x] Mobile-responsive layout (basic media queries)
- [x] Focus indicators for keyboard navigation
- [x] Sufficient contrast for readability (WCAG AA not required for MVP, but good practice)

---

### 2. SubscriptionItem Component

**Location**: `frontend/RSSFeedReader.UI/Components/SubscriptionItem.razor`

**Purpose**: Reusable component for displaying a single subscription in the list. Supports future extension for refresh button, delete button, etc.

#### Component Metadata

```csharp
@namespace RSSFeedReader.UI.Components
```

#### Parameters

| Parameter | Type | Required | Default | Purpose |
|-----------|------|----------|---------|---------|
| `Subscription` | `Subscription` | Yes | — | The subscription object to display |

#### Markup

```razor
<li class="subscription-item">
    <span class="subscription-url">@Subscription.Url</span>
    <span class="subscription-added">added @Subscription.AddedAt.ToLocalTime()</span>
</li>
```

#### Code Behind

```csharp
@code {
    [Parameter]
    public Subscription Subscription { get; set; } = default!;
}
```

#### Styling

```css
.subscription-item {
    padding: 12px;
    border-bottom: 1px solid #e0e0e0;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.subscription-url {
    flex: 1;
    word-break: break-all;
    font-family: monospace;
    color: #0066cc;
}

.subscription-added {
    color: #666;
    font-size: 0.85em;
    margin-left: 16px;
    white-space: nowrap;
}
```

#### Future Extensions

Future versions can enhance this component:

```razor
<!-- Extended-MVP: Add refresh button -->
<button @onclick="OnRefresh" title="Refresh this feed">🔄</button>

<!-- Post-MVP: Add delete button -->
<button @onclick="OnDelete" title="Remove subscription">✕</button>

<!-- Future: Show feed status and unread count -->
<span class="unread-count">@Subscription.UnreadItemCount</span>
<span class="status-indicator" data-status="@Subscription.Status"></span>
```

---

## Shared Service

### SubscriptionApiClient

**Location**: `frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs`

**Purpose**: HTTP client service for communicating with the backend API.

#### Interface

```csharp
public class SubscriptionApiClient
{
    public async Task<Subscription> AddSubscriptionAsync(string url)
    {
        // POST /api/subscriptions with { "url": url }
        // Return created Subscription object
    }

    public async Task<List<Subscription>> GetSubscriptionsAsync()
    {
        // GET /api/subscriptions
        // Return list of subscriptions
    }
}
```

#### Implementation Details

```csharp
public class SubscriptionApiClient
{
    private readonly HttpClient _httpClient;

    public SubscriptionApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Subscription> AddSubscriptionAsync(string url)
    {
        var request = new { url };
        var response = await _httpClient.PostAsJsonAsync("subscriptions", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<Subscription>();
    }

    public async Task<List<Subscription>> GetSubscriptionsAsync()
    {
        var response = await _httpClient.GetAsync("subscriptions");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<List<Subscription>>() ?? new List<Subscription>();
    }
}
```

#### Configuration (Program.cs)

```csharp
// Read API URL from configuration
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5151/api/";

// Register HttpClient with SubscriptionApiClient
builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) })
    .AddScoped<SubscriptionApiClient>();
```

---

## Data Models

### Subscription (Frontend)

Matches the backend Subscription model for consistency.

```csharp
public class Subscription
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}
```

---

## Interaction Flow

### User Story 1: Add Subscription

```
User Types URL → OnInputChanged updates newUrl state
                ↓
User Clicks "Add" or Presses Enter → OnAddSubscription()
                ↓
Button shows "Adding..." (isLoading = true)
                ↓
ApiClient.AddSubscriptionAsync(newUrl) calls POST /api/subscriptions
                ↓
Backend returns created Subscription
                ↓
Add to local subscriptions list
Clear input field (newUrl = "")
Update view (Blazor re-renders)
                ↓
SubscriptionItem component displays new subscription
Button returns to normal text (isLoading = false)
```

### User Story 2: Display Subscriptions

```
OnInitializedAsync() fires
                ↓
LoadSubscriptions() calls ApiClient.GetSubscriptionsAsync()
                ↓
GET /api/subscriptions returns all subscriptions
                ↓
Store in subscriptions state
                ↓
@foreach loop renders SubscriptionItem for each subscription
                ↓
User sees complete list
```

---

## Error Handling

### MVP Error Handling

Simple error display without detailed diagnostics:

| Scenario | Message | Action |
|----------|---------|--------|
| Empty URL | "Please enter a URL" | Let user retry |
| API call fails | "Failed to add subscription: [error]" | Let user retry |
| Load fails | "Failed to load subscriptions: [error]" | Let user retry |

### Future Error Handling (Extended-MVP+)

- Network timeout → Retry with backoff
- Invalid URL format → Specific guidance (Extended-MVP)
- Duplicate URL detection → Suggest update instead
- Feed unreachable → Show "Feed offline" with retry button

---

## Responsive Design

### Desktop (1024px+)

```
┌─────────────────────────────────────────────┐
│ Subscription Manager (Full width)           │
├─────────────────────────────────────────────┤
│ Add: [              URL input           ] [Add] │
│                                           │
│ Your Subscriptions (N)                  │
│ ─────────────────────────────────────── │
│ URL 1                              added  │
│ URL 2                              added  │
└─────────────────────────────────────────────┘
```

### Mobile (< 768px)

```
┌──────────────────┐
│ Subscription Mgr │
├──────────────────┤
│ Add:             │
│ [   URL input ]  │
│ [Add button]     │
│                  │
│ Your Subs (N)    │
│ ──────────────── │
│ URL 1            │
│ added            │
│ ──────────────── │
│ URL 2            │
│ added            │
└──────────────────┘
```

---

## Testing

### Acceptance Test Scenarios (from spec.md)

#### User Story 1 - Add Subscription

```
Test 1.1: Add single subscription
  Given: User on Subscriptions page
  When: User enters URL and clicks "Add"
  Then: Subscription appears in list immediately, input cleared

Test 1.2: Add multiple subscriptions in sequence
  Given: User has added first subscription
  When: User adds second subscription
  Then: Both appear in list, new one added to list
```

#### User Story 2 - Display Subscription List

```
Test 2.1: Display added subscriptions
  Given: User has added subscriptions
  When: Page loads
  Then: All subscriptions displayed with their URLs

Test 2.2: List updates on new subscription
  Given: List visible on page
  When: User adds new subscription
  Then: List updates immediately, no page refresh needed
```

### Component Testing

```csharp
// Example test using bUnit
[Test]
public void Subscriptions_AddButtonDisabled_WhenLoading()
{
    var comp = RenderComponent<Subscriptions>();
    var button = comp.Find("button");
    
    comp.InvokeAsync(async () => 
    {
        comp.Instance.isLoading = true;
    });
    comp.Render();
    
    Assert.IsTrue(button.Attributes["disabled"] != null);
}
```

---

## Deployment Notes

### Configuration

**Location**: `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`

```json
{
  "ApiBaseUrl": "http://localhost:5151/api/"
}
```

For production, update to production backend URL:
```json
{
  "ApiBaseUrl": "https://api.example.com/api/"
}
```

### Build & Run

```powershell
# Build frontend
dotnet build frontend/RSSFeedReader.UI

# Run frontend on port 5213
dotnet run --project frontend/RSSFeedReader.UI
```

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2026-05-08 | Initial MVP components: Subscriptions page, SubscriptionItem, SubscriptionApiClient |

---

**Status**: Ready for Implementation | **Contract Version**: 1.0.0 | **Created**: 2026-05-08
