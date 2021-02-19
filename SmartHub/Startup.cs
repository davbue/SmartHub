using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartHub.Models;
using SmartHub.Clients;
using Microsoft.AspNetCore.Http;
using SmartHub.Extensions;
using SmartHub.Services;
using SmartHub.Settings;

namespace SmartHub
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            MapConfiguration();
            //MqttHubClient mqttHubClient = new MqttHubClient();
            //Task task = mqttHubClient.ConnectAsync();
            //task.Wait();
            //mqttHubClient.ConfigureListening();
            
            //task = mqttHubClient.Subscribe("Esp32-48676d12cfa4/state/data");
            //task.Wait();
        }

        public IConfiguration Configuration { get; }

        private void MapConfiguration()
        {
            MapBrokerHostSettings();
            MapClientSettings();
        }

        private void MapBrokerHostSettings()
        {
            BrokerHostSettings brokerHostSettings = new BrokerHostSettings();
            Configuration.GetSection(nameof(BrokerHostSettings)).Bind(brokerHostSettings);
            AppSettingsProvider.BrokerHostSettings = brokerHostSettings;
        }

        private void MapClientSettings()
        {
            ClientSettings clientSettings = new ClientSettings();
            Configuration.GetSection(nameof(ClientSettings)).Bind(clientSettings);
            AppSettingsProvider.ClientSettings = clientSettings;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SmartHomeContext>();
            services.AddControllers();
            services.AddMqttClientHostedService();
            services.AddSingleton<ExtarnalService>();
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://192.168.1.109", "http://192.168.1.109:4200", "http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
                    });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
