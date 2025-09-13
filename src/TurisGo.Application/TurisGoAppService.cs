using TurisGo.Localization;
using Volo.Abp.Application.Services;

namespace TurisGo;

/* Inherit your application services from this class.
 */
public abstract class TurisGoAppService : ApplicationService
{
    protected TurisGoAppService()
    {
        LocalizationResource = typeof(TurisGoResource);
    }
}
