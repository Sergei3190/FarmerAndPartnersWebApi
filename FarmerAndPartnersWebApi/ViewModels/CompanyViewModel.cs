using System.ComponentModel.DataAnnotations;

namespace FarmerAndPartnersWebApi.ViewModels
{
    public class CompanyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя компании, не более 50 символов")]
        [StringLength(50)]
        public string Name { get; set; }
        public byte[] TimeStamp { get; set; }

        [RegularExpression("^[1-3]$", ErrorMessage = "Выберите статус контракта для указанной компании")]
        public int ContractStatusId { get; set; }
        public ContractStatusViewModel ContractStatus { get; set; }
    }
}