using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerAndPartnersModels.BaseModel
{
    public class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [Timestamp]
        public byte[] TimeStamp { get; set; }
        public override string ToString() => $" {nameof(Id)}: {Id}" +
                                             $" {nameof(Name)}: {Name}";
    }
}
