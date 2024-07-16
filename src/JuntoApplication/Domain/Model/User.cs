using System.ComponentModel.DataAnnotations;

namespace JuntoApplication.Model
{
    public class User : BaseUser
    {
        [Required]
        public string Cpf { get; set; }

        public User(long id, string name, string email, 
            string password, string cpf) : base(id, name, email, password)
        {            
            Cpf = cpf;
        }
    }
}
