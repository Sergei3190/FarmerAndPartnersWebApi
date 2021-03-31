using AutoMapper;
using FarmerAndPartnersEF.Repository;
using FarmerAndPartnersWebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmerAndPartnersWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FormatFilter]
    public class ContractStatusesController : ControllerBase
    {
        private IAsyncRepository _repository;
        private IMapper _mapper;

        public ContractStatusesController(IAsyncRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractStatusViewModel>>> GetContractStatusesAsync()
        {
            var contractStatuses = await _repository.GetContractStatusesAsync();

            if (contractStatuses is null)
                return NotFound();

            return Ok(_mapper.Map<List<ContractStatusViewModel>>(contractStatuses));
        }
    }
}
