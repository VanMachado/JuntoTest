using JuntoApplication.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace JuntoApplication.Model
{
    public class Admin : BaseUser
    {
        [Required]
        public Role Role { get; set; }

        public Admin(long id, string name, 
            string email, Role role) : base(id, name, email)
        {
            Role = role;
        }
    }
}
