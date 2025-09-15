using Xunit;

namespace TurisGo.EntityFrameworkCore;

[CollectionDefinition(TurisGoTestConsts.CollectionDefinitionName)]
public class TurisGoEntityFrameworkCoreCollection : ICollectionFixture<TurisGoEntityFrameworkCoreFixture>
{

}
