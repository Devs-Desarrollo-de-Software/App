using System.Threading.Tasks;

namespace TurisGo.Data;

public interface ITurisGoDbSchemaMigrator
{
    Task MigrateAsync();
}
