using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MVCTest.Models;

namespace MVCTest.Data
{
    public class MVCTestContext : DbContext
    {
        public MVCTestContext (DbContextOptions<MVCTestContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; }
        public DbSet<Campaign> Campaign { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<Message> Message { get; set; }
    }
}
