using Microsoft.EntityFrameworkCore;

namespace SmartDoctor.Medical.Models
{
    public partial class SmartDoctor_MedicalContext : DbContext
    {
        public SmartDoctor_MedicalContext()
        {
        }

        public SmartDoctor_MedicalContext(DbContextOptions<SmartDoctor_MedicalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Diseases> Diseases { get; set; }
        public virtual DbSet<OutpatientCards> OutpatientCards { get; set; }

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Diseases>(entity =>
            {
                entity.HasKey(e => e.DiseaseId);

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<OutpatientCards>(entity =>
            {
                entity.HasKey(e => e.OutpatientCardId);

                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.Description).HasMaxLength(50);
            });
        }
    }
}
