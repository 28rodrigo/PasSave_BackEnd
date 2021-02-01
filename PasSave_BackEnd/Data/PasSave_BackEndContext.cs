using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PasSave_BackEnd.Models;

namespace PasSave_BackEnd.Data
{
    public class PasSave_BackEndContext : DbContext
    {
        public PasSave_BackEndContext (DbContextOptions<PasSave_BackEndContext> options)
            : base(options)
        {
        }

        public DbSet<PasSave_BackEnd.Models.Password> Password { get; set; }
    }
}
