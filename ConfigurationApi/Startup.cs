using ConfigurationApi.Common;
using ConfigurationApi.Consumers;
using ConfigurationApi.Interfaces;
using ConfigurationApi.Middlewares;
using ConfigurationApi.Repositories;
using ConfigurationApi.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using StackExchange.Redis;
using System;

namespace ConfigurationApi
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
            MongoDbPersistence.Configure();

            var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");

            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));

            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

            services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));

            services.AddScoped<IDatabase>(sp =>
            {
                var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                return multiplexer.GetDatabase();
            });

            services.AddScoped<ICacheService, RedisCacheService>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<ConfigurationRecordUpdatedConsumer>();
                x.AddConsumer<ConfigurationRecordDeletedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING"));

                    cfg.UseMessageRetry(configurator =>
                    {
                        configurator.Incremental(5, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(1000));
                        configurator.Ignore<ApplicationException>();
                    });

                    cfg.ReceiveEndpoint("config_updated", e =>
                    {
                        e.ConfigureConsumer<ConfigurationRecordUpdatedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("config_deleted", e =>
                    {
                        e.ConfigureConsumer<ConfigurationRecordDeletedConsumer>(context);
                    });
                });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ConfigurationApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConfigurationApi v1"));
            }
            else
            {
                app.UseHttpsRedirection();
            }


            app.UseMiddleware<CustomExceptionHandlerMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
