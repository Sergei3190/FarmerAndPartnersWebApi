using FarmerAndPartnersModels.BaseModel;
using System.Collections.Generic;

namespace FarmerAndPartnersModels
{
    public class Company : BaseEntity
    {
        public int ContractStatusId { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public override string ToString() => $" {base.ToString()}" +
                                             $" {nameof(ContractStatusId)}: {ContractStatus.Id}" +
                                             $" {nameof(ContractStatus)}: {ContractStatus.Definition}" +
                                             $" {nameof(Users)}: {Users?.Count}";
    }
}
