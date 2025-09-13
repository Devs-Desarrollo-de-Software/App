using TurisGo.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace TurisGo.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(TurisGoEntityFrameworkCoreModule),
    typeof(TurisGoApplicationContractsModule)
)]
public class TurisGoDbMigratorModule : AbpModule
{
}
