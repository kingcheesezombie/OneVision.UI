using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace OneVision.UI;

public static class OneVisionUiServiceCollectionExtensions
{
    public static IServiceCollection AddOneVisionUI(this IServiceCollection services)
    {
        services.AddMudServices();
        return services;
    }
}
