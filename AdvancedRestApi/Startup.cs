using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using AdvancedRestApi.Data;
using AdvancedRestApi.Interfaces;
using AdvancedRestApi.Profiles;
using AdvancedRestApi.Services;
using AutoWrapper;
using AspNetCoreRateLimit;

namespace AdvancedRestApi
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
            services.AddControllers().AddOData(option => option.Select().Filter().OrderBy());
            services.AddAutoMapper(typeof(AutoMapperConfig));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AdvancedRestApi", Version = "v1" });
            });

            //services.AddDbContext<UserDbContext>(option => option.UseSqlServer(@"Server=.;Database=AdvancedApi;MultipleActiveResultSets=true;Trusted_Connection=True"));
            services.AddScoped<IUser, UserService>();

            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>((options) =>
                    {
                        options.GeneralRules = new List<RateLimitRule>()
                                                   {
                                                       new RateLimitRule() { Endpoint = "*", Limit = 100, Period = "1m" }
                                                   };
                    });

            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIpRateLimiting();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdvancedRestApi v1"));
            }

            app.UseHttpsRedirection();
            app.UseApiResponseAndExceptionWrapper();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}