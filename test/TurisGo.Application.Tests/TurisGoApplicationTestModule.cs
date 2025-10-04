using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using TurisGo.EntityFrameworkCore;

namespace TurisGo;

[DependsOn(
    typeof(TurisGoApplicationModule),
    typeof(TurisGoDomainTestModule),
    typeof(TurisGoEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule)
)]
public class TurisGoApplicationTestModule : AbpModule
{

}
