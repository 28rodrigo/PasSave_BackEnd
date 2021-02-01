using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasSave_BackEnd.Models
{
    public class Password
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Username { get; set; }

    }
}
