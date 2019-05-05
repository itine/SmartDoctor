using Microsoft.EntityFrameworkCore;

namespace SmartDoctor.Testing.Models
{
    public partial class SmartDoctor_DeseasesContext : DbContext
    {
        public SmartDoctor_DeseasesContext()
        {
        }

        public SmartDoctor_DeseasesContext(DbContextOptions<SmartDoctor_DeseasesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Deseases> Deseases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Deseases>(entity =>
            {
                entity.HasKey(e => e.DeseaseId);

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });
        }
    }
}
