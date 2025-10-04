using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TurisGo.Data;
using Volo.Abp.DependencyInjection;

namespace TurisGo.EntityFrameworkCore;

public class EntityFrameworkCoreTurisGoDbSchemaMigrator
    : ITurisGoDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreTurisGoDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the TurisGoDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<TurisGoDbContext>()
            .Database
            .MigrateAsync();
    }
}
