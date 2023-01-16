using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Establish a relationship between Compensation objects
            // and Employee objects.
            modelBuilder.Entity<Compensation>()
                //.HasNoKey()
                .HasOne(comp => comp.Employee);
        }

        public DbSet<Employee> Employees { get; set; }

        // Add a set to store employee compensations
        public DbSet<Compensation> Compensations { get; set; }
    }
}
