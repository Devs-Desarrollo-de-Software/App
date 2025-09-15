using TurisGo.Samples;
using Xunit;

namespace TurisGo.EntityFrameworkCore.Domains;

[Collection(TurisGoTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<TurisGoEntityFrameworkCoreTestModule>
{

}
