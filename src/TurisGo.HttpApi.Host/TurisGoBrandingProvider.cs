using Microsoft.Extensions.Localization;
using TurisGo.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace TurisGo;

[Dependency(ReplaceServices = true)]
public class TurisGoBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<TurisGoResource> _localizer;

    public TurisGoBrandingProvider(IStringLocalizer<TurisGoResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
