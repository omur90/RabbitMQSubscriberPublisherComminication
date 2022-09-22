
using Microsoft.EntityFrameworkCore;


namespace FileCreateWorkerService.Models
{
    public partial class WorkOrderDbContext : DbContext
    {
        public WorkOrderDbContext()
        {
        }

        public WorkOrderDbContext(DbContextOptions<WorkOrderDbContext> options) : base(options)
        {
        }

        public virtual DbSet<TblWorkOrder> TblWorkOrders { get; set; } = null!;

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblWorkOrder>(entity =>
            {
                entity.ToTable("tblWorkOrders", "workOrder");

                entity.Property(e => e.Id).HasDefaultValueSql("('00be01a1-5cd2-49fc-8bb1-c2f967ae8c85')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
