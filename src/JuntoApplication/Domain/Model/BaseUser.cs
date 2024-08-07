﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JuntoApplication.Model
{
    public class BaseUser
    {
        [Key]
        [Column ("Id")]
        public long Id { get; set; }
        [Required]
        [StringLength (250)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }               

        public BaseUser(long id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;            
        }
    }
}
