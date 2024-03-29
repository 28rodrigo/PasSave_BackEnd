﻿using System;
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

        public string Url { get; set; }

        public string Nome { get; set; }
 
        public string Username { get; set; }

        public string Pass { get; set; }

        public DateTime DateCreated { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }


    }
}
