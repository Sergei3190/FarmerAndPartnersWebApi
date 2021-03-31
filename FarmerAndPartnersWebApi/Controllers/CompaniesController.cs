using AutoMapper;
using FarmerAndPartnersEF.Repository;
using FarmerAndPartnersModels;
using FarmerAndPartnersWebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmerAndPartnersWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FormatFilter]
    public class CompaniesController : ControllerBase
    {
        private IAsyncRepository _repository;
        private IMapper _mapper;

        public CompaniesController(IAsyncRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyViewModel>>> GetCompaniesAsync()
        {
            var companies = await _repository.GetCompaniesWithoutUsersAsync();

            if (companies is null)
                return NotFound();

            return Ok(_mapper.Map<List<CompanyViewModel>>(companies));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyViewModel>> GetCompanyAsync(int id)
        {
            var company = await _repository.GetCompanyAsync(id);

            if (company is null)
                return NotFound($"Компания с id = {id} не найдена в базе данных");

            return Ok(_mapper.Map<CompanyViewModel>(company));
        }

        [HttpPost]
        [ActionName(nameof(GetCompanyAsync))]
        public async Task<ActionResult<CompanyViewModel>> CreateCompanyAsync(CompanyViewModel companyViewModel)
        {
            if (companyViewModel is null)
                return BadRequest();

            var isDbContainName = await _repository.FindNameCompanyAsync(companyViewModel.Name);

            if (isDbContainName)
                ModelState.AddModelError(nameof(companyViewModel.Name), "Компания с таким именем уже существует");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = _mapper.Map<Company>(companyViewModel);

            var lastCompanyId = await _repository.GetIdLastCompanyAsync();

            company.Id = ++lastCompanyId;

            var result = await _repository.AddCompanyAsync(company);

            if (result < 0)
                return StatusCode(500);

            company.ContractStatus = await _repository.GetContractStatusAsync(company.ContractStatusId);

            return CreatedAtAction(nameof(GetCompanyAsync), new { id = company.Id }, _mapper.Map<CompanyViewModel>(company));
        }

        [HttpPut]
        public async Task<ActionResult<CompanyViewModel>> UpdateCompanyAsync(CompanyViewModel companyViewModel)
        {
            if (companyViewModel is null)
                return BadRequest();

            var id = await _repository.CheckNameCompanyAsync(companyViewModel.Name);

            if (companyViewModel.Id != id && id > 0)
                ModelState.AddModelError(nameof(companyViewModel.Name), "Компания с таким именем уже существует");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = _mapper.Map<Company>(companyViewModel);

            var result = await _repository.UpdateCompanyAsync(company);

            if (result < 0)
                return Conflict();

            company.ContractStatus = await _repository.GetContractStatusAsync(company.ContractStatusId);

            return Ok(_mapper.Map<CompanyViewModel>(company));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CompanyViewModel>> DeleteCompanyAsync(int id)
        {
            var company = await _repository.GetCompanyAsync(id);

            if (company is null)
                return NotFound($"Компания с id = {id} не найдена в базе данных");

            var result = await _repository.DeleteCompanyAsync(company);

            if (result < 0)
                return Conflict();

            return Ok(_mapper.Map<CompanyViewModel>(company));
        }
    }
}
