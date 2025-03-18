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
    public class CVBuilderDbContext: DbContext
    {
        public CVBuilderDbContext(DbContextOptions<CVBuilderDbContext> options)
        : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<FileCV> FileCVs { get; set; }
        public DbSet<Education> Educations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileCV>()
                .HasMany(f => f.Educations)
                .WithOne()  // אם אין קשר ישיר ל-Education (למשל לא הגדרת ForeignKey)
                .HasForeignKey("FileCVId");  // יש להגדיר את שם ה-ForeignKey אם יש צורך
        }
    }
}
