using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnovationOne.Models
{
    public class Context : DbContext
    {
        
        public DbSet<Categoria> Categoria { set; get; }
        public DbSet<Produto> Produto { set; get; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-9T20DTA;Database=Cursomvc;Integrated Security=True");
        }
    }
}
