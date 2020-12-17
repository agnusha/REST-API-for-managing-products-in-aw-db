using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace FunctionDoc.Models
{
    [ExcludeFromCodeCoverage]
    public partial class AdventureWorksFuncContext : DbContext
    {
        public AdventureWorksFuncContext()
        {
        }

        public AdventureWorksFuncContext(DbContextOptions<AdventureWorksFuncContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Document> Documents { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
