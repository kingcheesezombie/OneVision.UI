using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;

namespace OneVision.UI;

public static class OneVisionUiServiceCollectionExtensions
{
    public static IServiceCollection AddOneVisionUI(this IServiceCollection services)
    {
        services.AddMudServices();
        services.AddScoped<OVNotificationService>();
        return services;
    }
}
