using CVBuilder.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Data
{
    public class CVBuilderDbContext : DbContext
    {
        public CVBuilderDbContext(DbContextOptions<CVBuilderDbContext> options)
          : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<FileCV> FileCVs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileCV>().OwnsMany(f => f.WorkExperiences);
            modelBuilder.Entity<FileCV>().OwnsMany(f => f.Educations);
            modelBuilder.Entity<FileCV>().OwnsMany(f => f.Languages);
        }
    }
}
