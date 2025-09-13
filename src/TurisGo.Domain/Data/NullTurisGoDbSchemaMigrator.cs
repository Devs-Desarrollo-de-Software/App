using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace TurisGo.Data;

/* This is used if database provider does't define
 * ITurisGoDbSchemaMigrator implementation.
 */
public class NullTurisGoDbSchemaMigrator : ITurisGoDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
