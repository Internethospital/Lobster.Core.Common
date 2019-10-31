using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Core.Common.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.JwtAuthorize;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using Core.Common.CoreFrame;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Core.Common
{
    public class ServiceStartup
    {
        /// <summary>
        /// Api版本提者信息
        /// </summary>
        private IApiVersionDescriptionProvider provider;

        public ServiceStartup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigHelper.t = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            //注入APIHelper
            services.AddTransient<IApiHelper, ApiHelper>();
            services.AddSession();
            //注入JWT 登录
            services.AddTokenJwtAuthorize();
            //注入JWT 验证
            services.AddApiJwtAuthorize((context) =>
            {
                return ValidatePermission(context);
            });
            //API版本管理
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;//当设置为 true 时, API 将返回响应标头中支持的版本信息
                options.AssumeDefaultVersionWhenUnspecified = true; //此选项将用于不提供版本的请求。默认情况下, 假定的 API 版本为1.0
                options.DefaultApiVersion = new ApiVersion(1, 0);
            }).AddVersionedApiExplorer(option =>
            {
                option.GroupNameFormat = "'v'VVV";
                option.AssumeDefaultVersionWhenUnspecified = true;
            });
            this.provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            //Swagger API管理
            services.AddSwaggerGen(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName,
                        new Info()
                        {
                            Title = Configuration["Swagger:Title"] + description.ApiVersion,
                            Version = description.ApiVersion.ToString(),
                            Contact = new Contact { Email = Configuration["Swagger:Email"], Name = Configuration["Swagger:Name"], Url = Configuration["Swagger:Url"] },
                            Description = "切换版本请点右上角版本切换"
                        }
                    );
                }
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, Configuration["Swagger:XmlCommentPath"]);
                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "请输入带有Bearer的Token", Name = "Authorization", Type = "apiKey" });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    {
                        "Bearer",
                        Enumerable.Empty<string>()
                    }
                });
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.IgnoreObsoleteActions();
                //排除第三方controller
                //options.DocInclusionPredicate((docName, apiDesc) =>
                //{
                //    var assemblyName = ((ControllerActionDescriptor)apiDesc.ActionDescriptor).ControllerTypeInfo.Assembly.GetName().Name;
                //    var currentAssemblyName = GetType().Assembly.GetName().Name;
                //    return currentAssemblyName == assemblyName;
                //});
                options.DocumentFilter<HiddenApiFilter>();
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //支持跨域
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSameDomain", builder =>
                {
                    builder.AllowAnyOrigin() //允许任何来源的主机访问          
                    //.WithOrigins("http://localhost:8080")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                    //.AllowCredentials();//指定处理cookie
                });
            });

            services.AddMvc(options =>
            {
                options.Filters.Add<ActionApiController>();
                options.Filters.Add<ExceptionApiController>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifeTime, ILoggerFactory loggerFactory)
        {
            //使用NLog作为日志记录工具
            loggerFactory.AddNLog();
            //引入Nlog配置文件
            env.ConfigureNLog("nlog.config");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }
            //异常处理
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An Error Occurred for Lobster.");
                });
            });

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSession();
            app.UseMvcWithDefaultRoute();
            app.UseCors("AllowSameDomain"); //没作用
            app.UseCors(builder =>          //没作用
            {
                //builder.AllowAnyOrigin();
                //builder.AllowAnyHeader();
                //builder.AllowAnyMethod();
                //builder.AllowCredentials();
                builder.WithExposedHeaders(new string[] { "SysLoginRight" });
            });

            app.UseSwagger().UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }

        /// <summary>
        /// JWT验证身份
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public virtual bool ValidatePermission(HttpContext httpContext)
        {
            return true;
        }
    }
}
