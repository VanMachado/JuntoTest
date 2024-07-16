using JuntoApplication.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace JuntoApplication.Model
{
    public class Admin : BaseUser
    {
        [Required]
        public Role Role { get; set; }

        public Admin(long id, string name, string email,
            string password, Role role) : base(id, name, email, password)
        {
            Role = role;
        }
    }
}
