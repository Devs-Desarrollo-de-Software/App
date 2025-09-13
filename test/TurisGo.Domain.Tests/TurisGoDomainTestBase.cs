using Volo.Abp.Modularity;

namespace TurisGo;

/* Inherit from this class for your domain layer tests. */
public abstract class TurisGoDomainTestBase<TStartupModule> : TurisGoTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
