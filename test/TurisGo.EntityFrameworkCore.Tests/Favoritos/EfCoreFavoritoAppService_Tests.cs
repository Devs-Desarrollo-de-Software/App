using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.EntityFrameworkCore;
using Xunit;

namespace TurisGo.Favoritos
{
    [Collection(TurisGoTestConsts.CollectionDefinitionName)]
    public class EfCoreFavoritoAppService_Tests : FavoritoAppService_Tests<TurisGoEntityFrameworkCoreTestModule>
    {
    }
}
