using Zamin.Core.Domain.Events;

namespace TTechNewsCMS.Core.Domain.NewsAgg.Events;

public class NewsCreatedEvent : IDomainEvent
{
    public string BussinesId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Body { get; set; }
    public string Keywords { get; set; }

    public NewsCreatedEvent(string bussinesId , string title , string description , string body , string keywords)
    {
        BussinesId = bussinesId;
        Title = title;
        Description = description;
        Body = body;
    }
}
