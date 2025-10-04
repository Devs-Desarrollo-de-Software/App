using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TurisGo.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class TurisGoDbContextFactory : IDesignTimeDbContextFactory<TurisGoDbContext>
{
    public TurisGoDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        TurisGoEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<TurisGoDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new TurisGoDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TurisGo.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
