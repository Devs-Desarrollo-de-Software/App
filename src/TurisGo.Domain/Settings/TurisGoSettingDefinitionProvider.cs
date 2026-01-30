using Volo.Abp.Settings;

namespace TurisGo.Settings;

public class TurisGoSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(TurisGoSettings.MySetting1));
    }
}
