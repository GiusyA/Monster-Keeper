
using Postgrest.Models;
using Postgrest.Attributes;

[Table("players")]
public class SB_PlayerModel : BaseModel
{
    [PrimaryKey("id", true)]
    public string Id { get; set; } = string.Empty;
}