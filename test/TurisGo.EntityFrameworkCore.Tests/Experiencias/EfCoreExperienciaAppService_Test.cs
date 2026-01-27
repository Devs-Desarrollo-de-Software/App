using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.EntityFrameworkCore;
using Xunit;

namespace TurisGo.Experiencias
{
    [Collection(TurisGoTestConsts.CollectionDefinitionName)]
    public class EfCoreExperienciaAppService_Test : ExperienciaAppService_Tests<TurisGoEntityFrameworkCoreTestModule>
    {
    }
}
