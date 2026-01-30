using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using TurisGo.Destinos;
using System.Net.Http;
using Volo.Abp.BackgroundWorkers;
using TurisGo.Notificaciones;
using TurisGo.Favoritos;
using System.Threading.Tasks;
using Volo.Abp;

namespace TurisGo;

[DependsOn(
    typeof(TurisGoDomainModule),
    typeof(TurisGoApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class TurisGoApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<TurisGoApplicationModule>();
        });

       // context.Services.AddTransient<IDestinoAppService, DestinoAppService>();
       // Agregge esto!
        context.Services.AddHttpClient<ICitySearchService, GeoDbCitySearchService>();
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        // Registrar Background Workers
        await context.ServiceProvider
            .GetRequiredService<IBackgroundWorkerManager>()
            .AddAsync(
                context.ServiceProvider
                    .GetRequiredService<EnviarNotificacionesEmailWorker>()
            );

        await context.ServiceProvider
            .GetRequiredService<IBackgroundWorkerManager>()
            .AddAsync(
                context.ServiceProvider
                    .GetRequiredService<EnviarResumenSemanalWorker>()
            );

        await context.ServiceProvider
            .GetRequiredService<IBackgroundWorkerManager>()
            .AddAsync(
                context.ServiceProvider
                    .GetRequiredService<VerificarCambiosDestinosWorker>()
            );
    }
}
