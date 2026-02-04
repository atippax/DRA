using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("area")]
public class AreaModel
{

    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int id { get; set; }

    [Column("required_resources")]
    public Dictionary<string, int> requiredResources { get; set; }

    [Column("time_constraint")]
    public int timeConstraint { get; set; }

    [Column("urgency_level")]
    public int urgencyLevel { get; set; }

    [Column("has_delivered")]
    public bool hasDelivered { get; set; }

}
