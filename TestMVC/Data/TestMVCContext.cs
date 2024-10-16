using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestMVC.Models;

namespace TestMVC.Data
{
    public class TestMVCContext : DbContext
    {
        public TestMVCContext (DbContextOptions<TestMVCContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; } = default!;
    }
}
