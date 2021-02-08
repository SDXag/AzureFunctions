using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SDX.FunctionsDemo.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // in-memory Verarbeitung
            services.AddSingleton<ImageProcessing.IImageProcessor, ImageProcessing.ImageProcessor>();
            services.AddSingleton<ImageProcessing.IImageFileService, Services.InMemoryImageFileService>();

            #region function
            var hasFunctionApp = this.Configuration["FunctionApp:Url"] != null;
            if (hasFunctionApp)
            {
                // Delegation an die FunctionApp
                services.Configure<Services.FunctionAppOptions>(Configuration.GetSection("FunctionApp"));
                services.AddSingleton<ImageProcessing.IImageFileService, Services.FunctionAppImageFileService>();
            }
            else
            {
                // in-memory Verarbeitung
                services.AddSingleton<ImageProcessing.IImageProcessor, ImageProcessing.ImageProcessor>();
                services.AddSingleton<ImageProcessing.IImageFileService, Services.InMemoryImageFileService>();
            }
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
