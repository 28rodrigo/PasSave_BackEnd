using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasSave_BackEnd.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; }

        [Required]      
        public string Email { get; set; }

        [Required]
        public string Pass { get; set; }

        public List<Password> Passwords { get; set; }
    }
}
