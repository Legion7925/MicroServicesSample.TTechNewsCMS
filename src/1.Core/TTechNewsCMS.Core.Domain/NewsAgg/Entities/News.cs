using TTechNewsCMS.Core.Domain.NewsAgg.Events;
using Zamin.Core.Domain.Entities;
using Zamin.Core.Domain.Toolkits.ValueObjects;
using Zamin.Core.Domain.ValueObjects;

namespace TTechNewsCMS.Core.Domain.NewsAgg.Entities;

public class News : AggregateRoot
{
    public Title Title { get; set; }

    public Description Description { get; set; }

    public string Body { get; set; }

    private readonly List<NewsKeyword> _newsKeywords = new List<NewsKeyword>();

    public IReadOnlyList<NewsKeyword> NewsKeywords => _newsKeywords;

    public News()
    {

    }

    public News(BusinessId businessId, Title title, Description description, string body , List<NewsKeyword> newsKeywords)
    {
        BusinessId = businessId;
        Title = title;
        Description = description;
        Body = body;
        _newsKeywords.AddRange(newsKeywords);

        AddEvent(new NewsCreatedEvent(businessId.Value.ToString() , title.Value, description.Value , body ,
            string.Join(";" , _newsKeywords.Select(k=> k.BusinessId).ToList())));
    }
}
