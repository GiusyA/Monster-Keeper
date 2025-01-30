
using Postgrest.Models;
using Postgrest.Attributes;

[Table("characters")]
public class SB_CharacterModel : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; } = 0;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("level")]
    public int Level { get; set; } = 1;

    [Column("experience")]
    public int Experience { get; set; } = 0;

    [Column("money")]
    public float Money { get; set; } = 0.0f;

    [Column("players.id")]
    public string PlayerID { get; set; } = string.Empty;
}
