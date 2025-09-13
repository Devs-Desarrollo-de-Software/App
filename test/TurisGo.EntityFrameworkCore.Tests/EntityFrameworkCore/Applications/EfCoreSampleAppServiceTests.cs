using TurisGo.Samples;
using Xunit;

namespace TurisGo.EntityFrameworkCore.Applications;

[Collection(TurisGoTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<TurisGoEntityFrameworkCoreTestModule>
{

}
