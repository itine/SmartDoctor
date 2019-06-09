using Microsoft.EntityFrameworkCore;

namespace SmartDoctor.Data.ContextModels
{
    public partial class SmartDoctor_UsersContext : DbContext
    {
        public SmartDoctor_UsersContext()
        {
        }

        public SmartDoctor_UsersContext(DbContextOptions<SmartDoctor_UsersContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Patients> Patients { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patients>(entity =>
            {
                entity.HasKey(e => e.PatientId);

                entity.Property(e => e.DateBirth).HasColumnType("date");

                entity.Property(e => e.Fio)
                    .IsRequired()
                    .HasColumnName("FIO")
                    .HasMaxLength(100);

                entity.Property(e => e.SpecificNumber).HasMaxLength(50);

                entity.Property(e => e.WorkPlace).HasMaxLength(50);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.Fio)
                   .IsRequired()
                   .HasColumnName("fio")
                   .HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(20);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });
        }
    }
}
