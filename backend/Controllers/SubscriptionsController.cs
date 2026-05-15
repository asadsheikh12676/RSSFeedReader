using RSSFeedReader.Api.Models;
using RSSFeedReader.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace RSSFeedReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionsController(SubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost]
    public ActionResult<Subscription> AddSubscription([FromBody] AddSubscriptionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("URL is required");
        }

        var subscription = _subscriptionService.AddSubscription(request.Url);
        return CreatedAtAction(nameof(GetSubscriptions), new { id = subscription.Id }, subscription);
    }

    [HttpGet]
    public ActionResult<List<Subscription>> GetSubscriptions()
    {
        var subscriptions = _subscriptionService.GetSubscriptions();
        return Ok(subscriptions);
    }
}

public class AddSubscriptionRequest
{
    public string Url { get; set; } = string.Empty;
}
