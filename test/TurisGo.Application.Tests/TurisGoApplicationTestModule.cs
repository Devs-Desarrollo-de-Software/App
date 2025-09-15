using Volo.Abp.Modularity;

namespace TurisGo;

[DependsOn(
    typeof(TurisGoApplicationModule),
    typeof(TurisGoDomainTestModule)
)]
public class TurisGoApplicationTestModule : AbpModule
{

}
