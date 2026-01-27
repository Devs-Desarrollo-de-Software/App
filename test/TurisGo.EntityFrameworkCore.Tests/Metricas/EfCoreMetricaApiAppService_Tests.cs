using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.EntityFrameworkCore;
using Xunit;

namespace TurisGo.Metricas
{
    [Collection(TurisGoTestConsts.CollectionDefinitionName)]
    public class EfCoreMetricaApiAppService_Tests : MetricaApiAppServiceTests<TurisGoEntityFrameworkCoreTestModule>
    {
    }
}
