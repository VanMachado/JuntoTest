using JuntoApplication.Dto;
using JuntoApplication.Infrastructure.Repository.IRepository;
using JuntoApplication.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JuntoApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseUsersController : ControllerBase
    {
        private readonly IUserBaseRepository _repository;

        public BaseUsersController(IUserBaseRepository repository)
        {
            _repository = repository;
        }        

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            if (userDto == null)
                return BadRequest();
            
            var user = await _repository.CreateAsync(userDto);
            
            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await _repository.FindAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FindById(long id, string role)
        {
            var user = await _repository.FindByIdAsync(id, role);

            if (user == null)
                return BadRequest();

            return Ok(user);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePassword(long id, string role, string password)
        {
            var user = await _repository.UpdateAsync(id, role, password);

            if (user == null)
                return BadRequest();

            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteById(long id, string role)
        {
            var status = await _repository.DeleteAsync(id, role);

            if (!status)
                return BadRequest();

            return Ok();            
        }
    }
}
