using JuntoApplication.Dto.EnumsDto;
using System.Text.Json.Serialization;

namespace JuntoApplication.Dto
{
    public class UserDto
    {
        public long Id { get; set; }        
        public string Name { get; set; }        
        public string Email { get; set; }        
        public string Password { get; set; }
        public string? UserType { get; set; }
        public RoleDto? Role { get; set; }
        public string? CPF { get; set; }
    }
}
