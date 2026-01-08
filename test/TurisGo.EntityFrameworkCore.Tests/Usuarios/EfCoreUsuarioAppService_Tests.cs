using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.EntityFrameworkCore;
using TurisGo.Experiencias;
using Xunit;

namespace TurisGo.Usuarios
{
    [Collection(TurisGoTestConsts.CollectionDefinitionName)]
    public class EfCoreUsuarioAppService_Tests: UsuarioAppService_Tests<TurisGoEntityFrameworkCoreTestModule>
    {
    }
}
