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
    public class UsersController : ControllerBase
    {
        private IAsyncRepository _repository;
        private IMapper _mapper;

        public UsersController(IAsyncRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsersAsync()
        {
            var users = await _repository.GetUsersAsync();

            if (users is null)
                return NotFound();

            return Ok(_mapper.Map<List<UserViewModel>>(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> GetUserAsync(int id)
        {
            var user = await _repository.GetUserAsync(id);

            if (user is null)
                return NotFound($"Пользователь с id = {id} не найден в базе данных");

            return Ok(_mapper.Map<UserViewModel>(user));
        }

        [HttpPost]
        [ActionName(nameof(GetUserAsync))]
        public async Task<ActionResult<UserViewModel>> CreateUserAsync(UserViewModel userViewModel)
        {
            if (userViewModel is null)
                return BadRequest();

            var isDbConteinLogin = await _repository.FindLoginUserAsync(userViewModel.Login);

            if (isDbConteinLogin)
                ModelState.AddModelError(nameof(userViewModel.Login), "Пользователь с таким логином уже существует");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<User>(userViewModel);

            var lastUserId = await _repository.GetIdLastUserAsync();

            user.Id = ++lastUserId;

            var result = await _repository.AddUserAsync(user);

            if (result < 0)
                return StatusCode(500);

            user.Company = await _repository.GetCompanyAsync(user.CompanyId);

            return CreatedAtAction(nameof(GetUserAsync), new { id = user.Id }, _mapper.Map<UserViewModel>(user));
        }

        [HttpPut]
        public async Task<ActionResult<UserViewModel>> UpdateUserAsync(UserViewModel userViewModel)
        {
            if (userViewModel is null)
                return BadRequest();

            var id = await _repository.CheckLoginUserAsync(userViewModel.Login);

            if (userViewModel.Id != id && id > 0)
                ModelState.AddModelError(nameof(userViewModel.Login), "Пользователь с таким логином уже существует");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<User>(userViewModel);

            var result = await _repository.UpdateUserAsync(user);

            if (result < 0)
                return Conflict();

            user.Company = await _repository.GetCompanyAsync(user.CompanyId);

            return Ok(_mapper.Map<UserViewModel>(user));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<UserViewModel>> DeleteUserAsync(int id)
        {
            var user = await _repository.GetUserAsync(id);

            if (user is null)
                return NotFound($"Пользователь с id = {id} не найден в базе данных");

            var result = await _repository.DeleteUserAsync(user);

            if (result < 0)
                return Conflict();

            return Ok(_mapper.Map<UserViewModel>(user));
        }
    }
}
