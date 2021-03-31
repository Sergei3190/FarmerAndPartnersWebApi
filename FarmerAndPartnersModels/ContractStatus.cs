using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerAndPartnersModels
{
    public class ContractStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string Definition { get; set; }
        public List<Company> Companies { get; set; } = new List<Company>();
        public override string ToString() => $" {nameof(Id)}: {Id}" +
                                             $" {nameof(Definition)}: {Definition}";
    }
}
