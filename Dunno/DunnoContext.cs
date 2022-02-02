using System;
using Dunno.Models;
using Microsoft.EntityFrameworkCore;

namespace Dunno
{
    public class DunnoContext : DbContext
    {
        public DunnoContext(DbContextOptions<DunnoContext> options) : base(options)
        {
        }

        public DbSet<News> News { get; set; }
    }
}
