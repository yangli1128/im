using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace webServer
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
            services.AddCors(options => options.AddPolicy("all",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithMethods("OPTIONS"))
                );
            services.AddControllers().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "IM API", Version = "v1" });
                //为 Swagger JSON and UI设置xml文档注释路径
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                var files = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
                foreach (var xmlPath in files)
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var virtualPath = Configuration["virtualPath"];
            if (env.IsDevelopment())
            {
                virtualPath = "";
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("all");
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(virtualPath + "/swagger/v1/swagger.json", "IM API V1");
            });
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
