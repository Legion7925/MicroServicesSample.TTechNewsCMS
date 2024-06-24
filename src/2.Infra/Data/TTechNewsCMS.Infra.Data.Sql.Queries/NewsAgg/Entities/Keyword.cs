using System.ComponentModel.DataAnnotations;

namespace NewCms.Infra.Data.Sql.Queries.NewsAgg.Entities;

public class Keyword
{
    [Key]
    public Guid KeywordBusinessId { get; set; }

    public string KeywordTitle { get; set; }
}