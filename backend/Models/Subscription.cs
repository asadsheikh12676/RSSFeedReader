namespace RSSFeedReader.Api.Models;

public class Subscription
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}
