using TurisGo.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace TurisGo.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class TurisGoController : AbpControllerBase
{
    protected TurisGoController()
    {
        LocalizationResource = typeof(TurisGoResource);
    }
}
