using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Services;

public class SubscriptionService
{
    private readonly List<Subscription> _subscriptions = new();
    private int _nextId = 1;

    public Subscription AddSubscription(string url)
    {
        var subscription = new Subscription
        {
            Id = _nextId++,
            Url = url,
            AddedAt = DateTime.UtcNow
        };

        _subscriptions.Add(subscription);
        return subscription;
    }

    public List<Subscription> GetSubscriptions()
    {
        return _subscriptions.ToList();
    }
}
