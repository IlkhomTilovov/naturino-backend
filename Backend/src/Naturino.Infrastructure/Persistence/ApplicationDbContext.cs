using Microsoft.EntityFrameworkCore;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<MediaFolder> MediaFolders => Set<MediaFolder>();
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<PageSection> PageSections => Set<PageSection>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<BlogCategory> BlogCategories => Set<BlogCategory>();
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<SeoMetadata> SeoMetadata => Set<SeoMetadata>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Theme> Themes => Set<Theme>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}