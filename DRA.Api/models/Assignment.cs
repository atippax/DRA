using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("assignment")]
public class AssignmentModel
{

    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int id { get; set; }

    [Column("resources")]
    public Dictionary<string, int> resources { get; set; }

    [Column("area_id")]
    public int areaId { get; set; }

    [Column("truck_id")]
    public int truckId { get; set; }
}
