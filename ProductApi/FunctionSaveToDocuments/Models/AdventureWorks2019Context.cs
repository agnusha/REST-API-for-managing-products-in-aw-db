using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace FunctionSaveToDocuments.Models
{
    [ExcludeFromCodeCoverage]
    public partial class AdventureWorks2019Context : DbContext
    {
        public AdventureWorks2019Context()
        {
        }

        public AdventureWorks2019Context(DbContextOptions<AdventureWorks2019Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.DocumentNode)
                    .HasName("PK_Document_DocumentNode");

                entity.ToTable("Document", "Production");

                entity.HasComment("Product maintenance documents.");

                entity.HasIndex(e => new { e.DocumentLevel, e.DocumentNode }, "AK_Document_DocumentLevel_DocumentNode")
                    .IsUnique();

                entity.HasIndex(e => e.Rowguid, "AK_Document_rowguid")
                    .IsUnique();

                entity.HasIndex(e => new { e.FileName, e.Revision }, "IX_Document_FileName_Revision");

                entity.HasIndex(e => e.Rowguid, "UQ__Document__F73921F7A5124F78")
                    .IsUnique();

                entity.Property(e => e.DocumentNode).HasComment("Primary key for Document records.");

                entity.Property(e => e.ChangeNumber).HasComment("Engineering change approval number.");

                entity.Property(e => e.Document1)
                    .HasColumnName("Document")
                    .HasComment("Complete document.");

                entity.Property(e => e.DocumentLevel)
                    .HasComputedColumnSql("([DocumentNode].[GetLevel]())", false)
                    .HasComment("Depth in the document hierarchy.");

                entity.Property(e => e.DocumentSummary).HasComment("Document abstract.");

                entity.Property(e => e.FileExtension)
                    .IsRequired()
                    .HasMaxLength(8)
                    .HasComment("File extension indicating the document type. For example, .doc or .txt.");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(400)
                    .HasComment("File name of the document");

                entity.Property(e => e.FolderFlag).HasComment("0 = This is a folder, 1 = This is a document.");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Date and time the record was last updated.");

                entity.Property(e => e.Owner).HasComment("Employee who controls the document.  Foreign key to Employee.BusinessEntityID");

                entity.Property(e => e.Revision)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsFixedLength(true)
                    .HasComment("Revision number of the document. ");

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid")
                    .HasDefaultValueSql("(newid())")
                    .HasComment("ROWGUIDCOL number uniquely identifying the record. Required for FileStream.");

                entity.Property(e => e.Status).HasComment("1 = Pending approval, 2 = Approved, 3 = Obsolete");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Title of the document.");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document", "Production");

                entity.HasKey(e => e.DocumentNode)
                    .HasName("PK_Document_DocumentNode");

                entity.HasComment("Product maintenance documents.");

                entity.HasIndex(e => new { e.DocumentLevel, e.DocumentNode }, "AK_Document_DocumentLevel_DocumentNode")
                    .IsUnique();

                entity.HasIndex(e => e.Rowguid, "AK_Document_rowguid")
                    .IsUnique();

                entity.HasIndex(e => new { e.FileName, e.Revision }, "IX_Document_FileName_Revision");

                entity.HasIndex(e => e.Rowguid, "UQ__Document__F73921F7A5124F78")
                    .IsUnique();

                entity.Property(e => e.DocumentNode).HasComment("Primary key for Document records.");

                entity.Property(e => e.ChangeNumber).HasComment("Engineering change approval number.");

                entity.Property(e => e.Document1)
                    .HasColumnName("Document")
                    .HasComment("Complete document.");

                entity.Property(e => e.DocumentLevel)
                    .HasComputedColumnSql("([DocumentNode].[GetLevel]())", false)
                    .HasComment("Depth in the document hierarchy.");

                entity.Property(e => e.DocumentSummary).HasComment("Document abstract.");

                entity.Property(e => e.FileExtension)
                    .IsRequired()
                    .HasMaxLength(8)
                    .HasComment("File extension indicating the document type. For example, .doc or .txt.");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(400)
                    .HasComment("File name of the document");

                entity.Property(e => e.FolderFlag).HasComment("0 = This is a folder, 1 = This is a document.");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Date and time the record was last updated.");

                entity.Property(e => e.Owner).HasComment("Employee who controls the document.  Foreign key to Employee.BusinessEntityID");

                entity.Property(e => e.Revision)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsFixedLength(true)
                    .HasComment("Revision number of the document. ");

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid")
                    .HasDefaultValueSql("(newid())")
                    .HasComment("ROWGUIDCOL number uniquely identifying the record. Required for FileStream.");

                entity.Property(e => e.Status).HasComment("1 = Pending approval, 2 = Approved, 3 = Obsolete");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Title of the document.");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
