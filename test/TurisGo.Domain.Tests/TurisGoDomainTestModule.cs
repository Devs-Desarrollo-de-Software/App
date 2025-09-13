using Volo.Abp.Modularity;

namespace TurisGo;

[DependsOn(
    typeof(TurisGoDomainModule),
    typeof(TurisGoTestBaseModule)
)]
public class TurisGoDomainTestModule : AbpModule
{

}
