using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TurisGo.Calificaciones;
using TurisGo.Destinos;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Users;
using TurisGo.Usuarios;
using System;
using System.Linq;
using System.Linq.Expressions;
using TurisGo.Experiencias;


namespace TurisGo.EntityFrameworkCore;

/*[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
*/

public class TurisGoDbContext : AbpDbContext<TurisGoDbContext>,
    IIdentityDbContext
{

    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Destino> Destinos { get; set; }
    public DbSet<Calificacion> Calificaciones { get; set; }

    public DbSet<Experiencia> Experiencias { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext 
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext .
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    #endregion

    private readonly ICurrentUser _currentUser;

    public TurisGoDbContext(DbContextOptions<TurisGoDbContext> options, ICurrentUser currentUser) 
        : base(options)
    {
        _currentUser = currentUser;
    }

    public TurisGoDbContext(DbContextOptions<TurisGoDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();


        builder.Entity<Destino>(b =>
        {
            b.ToTable(TurisGoConsts.DbTablePrefix + "Destinos", TurisGoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
            b.Property(x => x.Pais).IsRequired().HasMaxLength(60);
            b.Property(x => x.Poblacion).IsRequired();
            b.Property(x => x.Imagen).IsRequired().HasMaxLength(500);

            b.OwnsOne(x => x.Coordenada, cb =>
            {
                cb.Property(c => c.Latitud)
                .HasColumnName("Latitud")
                .IsRequired();

                cb.Property(c => c.Longitud)
                .HasColumnName("Longitud")
                .IsRequired();
            });
        });

        builder.Entity<Calificacion>(b =>
        {
            b.ToTable(TurisGoConsts.DbTablePrefix + "Calificaciones", TurisGoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Puntuacion).IsRequired();
            b.Property(x => x.Comentario).HasMaxLength(1000);
            b.Property(x => x.DestinoId).IsRequired();
            b.Property(x => x.UserId).IsRequired();

            b.HasIndex(x => new { x.DestinoId, x.UserId }).IsUnique();

            b.HasQueryFilter(e =>
                !_currentUser.IsAuthenticated ||
                (_currentUser.Id.HasValue && e.UserId == _currentUser.Id.Value));

        });

        builder.Entity<Experiencia>(b =>
        {
            b.ToTable(TurisGoConsts.DbTablePrefix + "Experiencias", TurisGoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.DestinoId).IsRequired();
            b.Property(x => x.Titulo).IsRequired().HasMaxLength(100);
            b.Property(x => x.Descripcion).IsRequired().HasMaxLength(500);

        });

    }

    protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType)
    {
        if (typeof(IUserOwned).IsAssignableFrom(typeof(TEntity)))
        {
            return true;
        }
        return base.ShouldFilterEntity<TEntity>(entityType);
    }

}



