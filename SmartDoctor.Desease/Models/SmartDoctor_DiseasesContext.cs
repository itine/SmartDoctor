using Microsoft.EntityFrameworkCore;

namespace SmartDoctor.Testing.Models
{
    public partial class SmartDoctor_DiseasesContext : DbContext
    {
        public SmartDoctor_DiseasesContext()
        {
        }

        public SmartDoctor_DiseasesContext(DbContextOptions<SmartDoctor_DiseasesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Diseases> Diseases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Diseases>(entity =>
            {
                entity.HasKey(e => e.DiseaseId);

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });
        }
    }
}
