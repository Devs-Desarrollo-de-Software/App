using Volo.Abp.Modularity;

namespace TurisGo;

public abstract class TurisGoApplicationTestBase<TStartupModule> : TurisGoTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
