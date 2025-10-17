using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.EntityFrameworkCore;
using Xunit;

namespace TurisGo.Destinos
{
    [Collection(TurisGoTestConsts.CollectionDefinitionName)]
    public class EfCoreCityAppService_Test : CityAppService_Integration_Tests<TurisGoEntityFrameworkCoreTestModule>
    {
    }
}
