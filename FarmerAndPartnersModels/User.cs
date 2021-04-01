using FarmerAndPartnersModels.BaseModel;
using System.ComponentModel.DataAnnotations;

namespace FarmerAndPartnersModels
{
    public class User : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Login { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public override string ToString() => $" {base.ToString()}," +
                                             $" {nameof(Login)}: {Login}," +
                                             $" {nameof(Password)}: {Password}" +
                                             $" {nameof(CompanyId)}: {Company?.Id}" +
                                             $" {nameof(Company)}: {Company?.Name}";
    }
}
