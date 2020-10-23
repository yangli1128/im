using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using webServer.Filters;
using webServer.Models;
using webServer.Services;

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
            services.AddDbContext<imdbContext>(opt => opt.UseMySql(Configuration.GetConnectionString("imdb")));

            services.AddControllers((configure) => {
                configure.Filters.Add<ValidateAuthorizeFilter>(0);
            }).AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "IM API", Version = "v1" });
                //Ϊ Swagger JSON and UI����xml�ĵ�ע��·��
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                var files = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
                foreach (var xmlPath in files)
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            services.AddTransient(typeof(MsgManager));
            services.AddTransient(typeof(UserManager));
            services.AddTransient(typeof(AccountManager));
            
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
            //�����м����������Swagger��ΪJSON�ս��
            app.UseSwagger();
            //�����м�������swagger-ui��ָ��Swagger JSON�ս��
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(virtualPath + "/swagger/v1/swagger.json", "IM API V1");
            });

            ImHelper.Initialization(new ImClientOptions
            {
                Redis = new CSRedis.CSRedisClient(Configuration["ImServerOption:CSRedisClient"]),
                Servers = Configuration["ImServerOption:Servers"].Split(";"),//IMsever�ĵ�ַ��������ַ
                WsType = Configuration["ImServerOption:SslType"]
            });

            ImHelper.Instance.OnSend += (s, e) =>
                Console.WriteLine($"ImClient.SendMessage(server={e.Server},data={JsonConvert.SerializeObject(e.Message)})");

            ImHelper.EventBus(
                t =>
                {
                    Console.WriteLine(t.clientId + "������");
                    var onlineUids = ImHelper.GetClientListByOnline();
                    ImHelper.SendMessage(t.clientId, onlineUids, $"�û�{t.clientId}������");
                },
                t => Console.WriteLine(t.clientId + "������"));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
