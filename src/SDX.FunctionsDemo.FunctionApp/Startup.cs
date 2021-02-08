
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SDX.FunctionsDemo.FunctionApp.Utils;
using SDX.FunctionsDemo.ImageProcessing;

[assembly: FunctionsStartup(typeof(SDX.FunctionsDemo.FunctionApp.Startup))]

namespace SDX.FunctionsDemo.FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IImageProcessor, ImageProcessor>();
            builder.Services.AddSingleton<IImageStorage, ImageStorage>();
        }
    }
}
