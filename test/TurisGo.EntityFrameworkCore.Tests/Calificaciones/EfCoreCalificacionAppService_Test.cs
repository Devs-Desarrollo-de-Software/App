using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.EntityFrameworkCore;
using Xunit;

namespace TurisGo.Calificaciones
{
    [Collection(TurisGoTestConsts.CollectionDefinitionName)]
    public class EfCoreCalificacionAppService_Test : CalificacionAppService_IntegrationTest<TurisGoEntityFrameworkCoreTestModule>
    {
    }
}
