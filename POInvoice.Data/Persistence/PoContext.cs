using POInvoice.Data.Domain;
using System.Data.Entity;

namespace POInvoice.Data.Persistence
{
    public class PoInvoice : DbContext
    {
        public static bool? Test { get; set; }

        public DbSet<PoForm> PoForms { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasRequired<PoForm>(t => t.PoForm)
                .WithMany(p => p.Documents)
                .HasForeignKey(t => t.PoFormId);

            modelBuilder.Entity<LineItem>()
                .HasRequired<PoForm>(l => l.PoForm)
                .WithMany(p => p.LineItems)
                .HasForeignKey(l => l.PoFormId);

            modelBuilder.Entity<PoForm>()
                .HasRequired<Vendor>(p => p.Vendor)
                .WithMany(v => v.PoForms)
                .HasForeignKey(p => p.VendorId);
        }

        public PoInvoice()
            :base($"name={(Test == null ? "POInvoiceMigration" : (Test.Value ? "POInvoiceTest" : "POInvoice"))}")
        {
            
        }
    }
}
