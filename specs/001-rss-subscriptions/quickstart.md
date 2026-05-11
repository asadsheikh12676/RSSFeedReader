# Quick Start Guide: RSS Feed Subscription Management (MVP)

**Feature**: RSS Feed Subscription Management (MVP)  
**Date**: 2026-05-08  
**Target**: Local development on Windows, macOS, or Linux

## Prerequisites

### Required Tools

- **.NET 8 SDK**: Download from https://dotnet.microsoft.com/download/dotnet/8.0
  - Verify: `dotnet --version` (should show 8.0.x)
  
- **Code Editor**: Visual Studio Code or Visual Studio 2022+
  - VS Code: Install C# extension (powered by OmniSharp or Roslyn)
  - VS 2022: Install ".NET desktop development" workload during setup

- **Git**: For version control (optional for MVP, required for team development)

- **Browser**: Chrome, Firefox, Safari, or Edge (latest version preferred)

### Optional Tools

- **PowerShell 7+**: For cross-platform terminal commands (useful for macOS/Linux)
- **Postman or cURL**: For testing API endpoints manually

---

## Project Setup

### 1. Create Projects

Use .NET templates to scaffold the MVP application:

#### Backend (ASP.NET Core Web API)

```powershell
# Create backend project
dotnet new webapi -n RSSFeedReader.Api -o backend/RSSFeedReader.Api

# Navigate to backend directory
cd backend/RSSFeedReader.Api
```

**Verify**: `RSSFeedReader.Api.csproj` file should exist and contain:
```xml
<TargetFramework>net8.0</TargetFramework>
```

#### Frontend (Blazor WebAssembly)

```powershell
# From repository root
dotnet new blazorwasm -n RSSFeedReader.UI -o frontend/RSSFeedReader.UI --skip-restore

# Navigate to frontend directory
cd frontend/RSSFeedReader.UI
```

**Verify**: `RSSFeedReader.UI.csproj` file should exist and contain:
```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```

#### Solution File

```powershell
# From repository root
dotnet new sln -n RSSFeedReader

# Add projects to solution
dotnet sln RSSFeedReader.sln add backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj
dotnet sln RSSFeedReader.sln add frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj
```

---

### 2. Backend Configuration

#### Remove Template Demo Pages

Blazor templates include demo pages that conflict with MVP routing. **CRITICAL**: This must be done before running the application.

```powershell
# Navigate to frontend Pages directory
cd frontend/RSSFeedReader.UI/Pages

# List current pages
Get-ChildItem *.razor | Select-Object Name

# Delete demo pages
Remove-Item Home.razor
Remove-Item Counter.razor
Remove-Item Weather.razor

# Verify deletion
Get-ChildItem *.razor | Select-Object Name
# Expected: only NotFound.razor and _Host.cshtml remain
```

**Why**: Blazor templates have a `Home.razor` with route `@page "/"`. You'll create your own `Subscriptions.razor` with the same route. Without removing the template, you'll get an "ambiguous route" error at runtime.

#### Create Subscription Models

**File**: `backend/RSSFeedReader.Api/Models/Subscription.cs`

```csharp
namespace RSSFeedReader.Api.Models;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

public class AddSubscriptionRequest
{
    public string Url { get; set; } = string.Empty;
}
```

#### Create Subscription Service

**File**: `backend/RSSFeedReader.Api/Services/SubscriptionService.cs`

```csharp
using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Services;

public interface ISubscriptionService
{
    Subscription AddSubscription(string url);
    List<Subscription> GetSubscriptions();
}

public class SubscriptionService : ISubscriptionService
{
    private readonly List<Subscription> _subscriptions = new();

    public Subscription AddSubscription(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be empty", nameof(url));
        }

        var subscription = new Subscription { Url = url };
        _subscriptions.Add(subscription);
        return subscription;
    }

    public List<Subscription> GetSubscriptions()
    {
        return _subscriptions.ToList();
    }
}
```

#### Create Subscriptions Controller

**File**: `backend/RSSFeedReader.Api/Controllers/SubscriptionsController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using RSSFeedReader.Api.Models;
using RSSFeedReader.Api.Services;

namespace RSSFeedReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost]
    public ActionResult<Subscription> AddSubscription([FromBody] AddSubscriptionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest(new { error = "URL is required" });
        }

        try
        {
            var subscription = _subscriptionService.AddSubscription(request.Url);
            return CreatedAtAction(nameof(GetSubscriptions), new { id = subscription.Id }, subscription);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public ActionResult<List<Subscription>> GetSubscriptions()
    {
        return Ok(_subscriptionService.GetSubscriptions());
    }
}
```

#### Configure Dependency Injection & CORS

**File**: `backend/RSSFeedReader.Api/Program.cs`

Replace the entire file with:

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5213",
                "https://localhost:7025"
            )
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
```

#### Update Launch Settings

**File**: `backend/RSSFeedReader.Api/Properties/launchSettings.json`

Ensure the API runs on port 5151:

```json
{
  "profiles": {
    "RSSFeedReader.Api": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5151",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

### 3. Frontend Configuration

#### Create Frontend Models

**File**: `frontend/RSSFeedReader.UI/Models/Subscription.cs`

```csharp
namespace RSSFeedReader.UI.Models;

public class Subscription
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}
```

#### Create API Client Service

**File**: `frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs`

```csharp
using RSSFeedReader.UI.Models;

namespace RSSFeedReader.UI.Services;

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
        return await response.Content.ReadAsAsync<Subscription>() 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<List<Subscription>> GetSubscriptionsAsync()
    {
        var response = await _httpClient.GetAsync("subscriptions");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<List<Subscription>>() ?? new List<Subscription>();
    }
}
```

#### Configure Startup

**File**: `frontend/RSSFeedReader.UI/Program.cs`

```csharp
using RSSFeedReader.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client with API base URL
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5151/api/";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<SubscriptionApiClient>();

await builder.Build().RunAsync();
```

#### Configure API Base URL

**File**: `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`

Create new file with:

```json
{
  "ApiBaseUrl": "http://localhost:5151/api/"
}
```

#### Create Subscriptions Page

**File**: `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`

```razor
@page "/"
@using RSSFeedReader.UI.Models
@using RSSFeedReader.UI.Services
@inject SubscriptionApiClient ApiClient

<div class="subscription-manager">
    <h1>Subscription Manager</h1>
    
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

    @if (!string.IsNullOrEmpty(error))
    {
        <div class="error-message">@error</div>
    }

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
                    <li class="subscription-item">
                        <span class="subscription-url">@subscription.Url</span>
                        <span class="subscription-added">added @subscription.AddedAt.ToLocalTime().ToString("g")</span>
                    </li>
                }
            </ul>
        }
    </div>
</div>

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

#### Update Navigation Menu

**File**: `frontend/RSSFeedReader.UI/Layout/NavMenu.razor`

Simplify to show only MVP navigation:

```razor
<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">RSS Feed Reader</a>
    </div>
</div>

<input type="checkbox" title="Navigation menu" class="navbar-toggler" />

<div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
    <nav class="flex-column">
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                <span class="oi oi-home" aria-hidden="true"></span> Subscriptions
            </NavLink>
        </div>
    </nav>
</div>
```

#### Update App Styling

**File**: `frontend/RSSFeedReader.UI/wwwroot/css/app.css`

Add or update:

```css
.subscription-manager {
    max-width: 800px;
    margin: 20px auto;
    padding: 20px;
    font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
}

.add-subscription-form {
    display: flex;
    gap: 10px;
    margin-bottom: 30px;
    flex-wrap: wrap;
}

.add-subscription-form label {
    width: 100%;
    margin-bottom: 8px;
    font-weight: 500;
}

.add-subscription-form input {
    flex: 1;
    min-width: 300px;
    padding: 8px 12px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 14px;
}

.add-subscription-form button {
    padding: 8px 16px;
    background-color: #0066cc;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 14px;
    white-space: nowrap;
}

.add-subscription-form button:hover:not(:disabled) {
    background-color: #0052a3;
}

.add-subscription-form button:disabled {
    background-color: #ccc;
    cursor: not-allowed;
}

.error-message {
    background-color: #fee;
    color: #c33;
    padding: 12px;
    border-radius: 4px;
    margin-bottom: 20px;
    border-left: 4px solid #c33;
}

.subscription-list h2 {
    margin-top: 0;
    font-size: 18px;
    color: #333;
}

.subscription-list ul {
    list-style: none;
    padding: 0;
    margin: 0;
    border: 1px solid #ddd;
    border-radius: 4px;
    overflow: hidden;
}

.subscription-item {
    padding: 12px;
    border-bottom: 1px solid #e0e0e0;
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 20px;
}

.subscription-item:last-child {
    border-bottom: none;
}

.subscription-url {
    flex: 1;
    word-break: break-all;
    font-family: monospace;
    color: #0066cc;
    font-size: 14px;
}

.subscription-added {
    color: #666;
    font-size: 0.85em;
    white-space: nowrap;
}

.empty-state {
    padding: 40px 20px;
    text-align: center;
    color: #999;
}

@media (max-width: 768px) {
    .subscription-manager {
        padding: 10px;
    }

    .add-subscription-form input {
        min-width: 200px;
    }

    .subscription-item {
        flex-direction: column;
        align-items: flex-start;
        gap: 4px;
    }

    .subscription-added {
        width: 100%;
    }
}
```

---

## Running the Application

### Step 1: Build Projects

```powershell
# From repository root
dotnet build RSSFeedReader.sln
```

**Expected**: Both backend and frontend compile without errors.

### Step 2: Start Backend API

**Terminal 1**:

```powershell
dotnet run --project backend/RSSFeedReader.Api
```

**Expected output**:
```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5151
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to exit.
```

**Verify**: Open https://localhost:5151/api/subscriptions in browser → Should see empty JSON array `[]`

### Step 3: Start Frontend (New Terminal)

**Terminal 2**:

```powershell
dotnet run --project frontend/RSSFeedReader.UI
```

**Expected output**:
```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5213
```

**Open Browser**: Navigate to `http://localhost:5213`

---

## Testing the MVP

### Test Case 1: Add Subscription

1. Open `http://localhost:5213` in browser
2. Enter URL: `https://devblogs.microsoft.com/dotnet/feed/`
3. Click "Add Subscription"
4. **Expected**: 
   - Subscription appears in list immediately
   - Input field clears
   - No page refresh needed

### Test Case 2: Display Multiple Subscriptions

1. Add 3 different URLs in sequence:
   - `https://example.com/feed.xml`
   - `https://another-blog.com/rss`
   - `https://tech-news.com/feed`
2. **Expected**: All 3 subscriptions visible in list in order of addition

### Test Case 3: API Direct Testing

Using cURL or Postman:

```powershell
# Get all subscriptions
curl http://localhost:5151/api/subscriptions

# Add a subscription
curl -X POST http://localhost:5151/api/subscriptions `
  -H "Content-Type: application/json" `
  -d '{"url":"https://example.com/feed.xml"}'
```

---

## Troubleshooting

### Issue: "Ambiguous route" error

**Symptom**: Browser console shows routing error after starting frontend

**Solution**: Delete template demo pages (Home.razor, Counter.razor, Weather.razor) from `frontend/RSSFeedReader.UI/Pages/`

### Issue: CORS error in browser console

**Symptom**: Network tab shows 401 or OPTIONS request failing

**Solution**: 
1. Verify backend is running on port 5151
2. Verify `appsettings.json` has correct API base URL
3. Verify CORS policy in backend Program.cs includes frontend origin

### Issue: API returns 500 error

**Symptom**: Backend console shows exception

**Solution**: 
1. Check backend terminal output for exception details
2. Verify all models and services created correctly
3. Restart backend after code changes

### Issue: Blank page loads but nothing appears

**Symptom**: Page renders but no content visible

**Solution**:
1. Open browser DevTools (F12) → Console tab
2. Look for JavaScript errors
3. Check network tab to verify API calls succeed
4. Ensure Blazor script is loading from `wwwroot/` directory

---

## Next Steps

### For Extended-MVP

Once MVP is working:

1. **Add feed fetching**: Install `System.ServiceModel.Syndication` NuGet package
2. **Implement refresh button**: Add `GET /api/subscriptions/{id}/items` endpoint
3. **Display items**: Create `ItemsList.razor` component to show feed content
4. **Add persistence**: Install `Microsoft.EntityFrameworkCore` and create SQLite database

### For Post-MVP

1. **Database**: Replace in-memory storage with EF Core + SQLite
2. **Background polling**: Add `BackgroundService` for automatic feed refresh
3. **Better UI**: Enhance styling with CSS framework (Bootstrap, Tailwind)
4. **Testing**: Add xUnit tests and Selenium integration tests

---

## Additional Resources

- **ASP.NET Core Docs**: https://learn.microsoft.com/aspnet/core/
- **Blazor Docs**: https://learn.microsoft.com/aspnet/core/blazor/
- **.NET 8 Download**: https://dotnet.microsoft.com/download/dotnet/8.0
- **RSS Standard**: https://www.rssboard.org/rss-specification
- **Atom Specification**: https://www.ietf.org/rfc/rfc4287

---

**Status**: Ready for Local Development | **Version**: 1.0.0 | **Created**: 2026-05-08
