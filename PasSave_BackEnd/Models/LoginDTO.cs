using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasSave_BackEnd.Models
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Pass { get; set; }
    }
}
