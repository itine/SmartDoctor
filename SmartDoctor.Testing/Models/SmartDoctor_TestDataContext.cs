using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SmartDoctor.Testing.Models
{
    public partial class SmartDoctor_TestDataContext : DbContext
    {
        public SmartDoctor_TestDataContext()
        {
        }

        public SmartDoctor_TestDataContext(DbContextOptions<SmartDoctor_TestDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Answers> Answers { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answers>(entity =>
            {
                entity.HasKey(e => e.AnswerId);

                entity.Property(e => e.AnswerData).HasMaxLength(50);

                entity.Property(e => e.AnswerDate).HasColumnType("date");
            });

            modelBuilder.Entity<Questions>(entity =>
            {
                entity.HasKey(e => e.QuestionId);

                entity.Property(e => e.Name).HasMaxLength(20);

                entity.Property(e => e.Text).HasMaxLength(50);
            });
        }
    }
}
