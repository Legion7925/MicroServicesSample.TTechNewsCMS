using Zamin.Core.Domain.Entities;
using Zamin.Core.Domain.ValueObjects;

namespace TTechNewsCMS.Core.Domain.NewsAgg.Entities;

public class NewsKeyword : Entity
{
    public BusinessId KeywordBusinessId { get; set; }
}
