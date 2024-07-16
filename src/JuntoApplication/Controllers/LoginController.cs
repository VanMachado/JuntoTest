using JuntoApplication.Infrastructure.Repository.IRepository;
using JuntoApplication.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JuntoApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {        
        private readonly ITokenRepository _repository;

        public LoginController(ITokenRepository repository)
        {            
            _repository = repository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            var token = await _repository.Authenticate(request.Username, request.Password);

            if (token == null)
                return Unauthorized();

            return Ok(new { Token = token });
        }        
    }
}