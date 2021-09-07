using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using TrunkRings;
using TrunkRings.Settings;
using TrunkRings.WebAPI.Services;
using TrunkRings.WebAPI.Settings;

namespace TrunkRings.WebAPI
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
            services.AddHttpClient("tgClient").AddTypedClient<ITgBotClientEx>(httpClient => new TgBotClientEx(Secrets.TgBotToken, httpClient));
            services.AddSingleton<ISecretaryBot, SecretaryBot>(serviceProvider => new SecretaryBot(
                serviceProvider.GetService<ITgBotClientEx>(),
                serviceProvider.GetService<ILogger<SecretaryBot>>()));
            services.AddHostedService<InitializeBotService>();
            services.AddHostedService<ConfigureWebhookService>();
            services.AddScoped<HandleUpdateService>();
            services.AddScoped<HandleDistributeMessagesService>();
            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SecretaryWebAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecretaryWebAPI v1"));
            }

            app.UseHttpsRedirection();
            //app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                var botToken = WebhookSettings.BotToken;
                endpoints.MapControllerRoute(name: "tgWebhook",
                    pattern: $"bot/{botToken}",
                    new { controller = "Webhook", action = "Post" });

                var managingToken = WebhookSettings.DistributeManagingToken;
                endpoints.MapControllerRoute(name: "distributeMessages",
                    pattern: $"api/{managingToken}/{{controller=Distribute}}/{{action=Messages}}");

                endpoints.MapControllers();
            });
        }
    }
}
