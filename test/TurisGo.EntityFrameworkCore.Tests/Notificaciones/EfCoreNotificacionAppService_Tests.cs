using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.EntityFrameworkCore;
using Xunit;

namespace TurisGo.Notificaciones
{
    [Collection(TurisGoTestConsts.CollectionDefinitionName)]
    public class EfCoreNotificacionAppService_Tests: NotificacionAppService_Tests<TurisGoEntityFrameworkCoreTestModule>
    {
    }
}
