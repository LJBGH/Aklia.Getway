using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aklia.GetWay.API.Startups
{
    public static class SwaggerModule
    {

        private static string _title = string.Empty;
        private static string _version = string.Empty;
        private static string _url = string.Empty;
        private static List<string> _apiList;

        public static void AddSwaggerModule(this IServiceCollection services, IConfiguration configuration)
        {

            _title = configuration["Aklia:Swagger:Title"];
            _version = configuration["Aklia:Swagger:Version"];
            _url = configuration["Aklia:Swagger:Url"];
            _apiList = configuration["Aklia:Swagger:ServicesDocNames"].Split(',').ToList();
 

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(_version, new OpenApiInfo
                {
                    Title = _title + " 接口文档",
                    Version = _version,
                    Description = "基于Ocelot的API网关",
                    Contact = new OpenApiContact
                    {
                        Name = _title,
                        Email = "1983810978@qq.com",
                        Url = new System.Uri("https://github.com/LJBGH/Aklia")
                    }
                });

                // 获取xml文件路径
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                // 获取所有xml文件
                var files = Directory.GetFiles(basePath, "*.xml");

                foreach (var item in files)
                {
                    // 添加控制器层注释，true表示显示控制器注释
                    c.IncludeXmlComments(item, true);
                }
                //一定要返回true
                c.DocInclusionPredicate((docName, description) =>
                {
                    return true;
                });
            });
        }

        public static void UseSwaggerConfig(this IApplicationBuilder app) 
        {

            app.UseSwagger();

            //启用中间件服务对swagger-ui,指定swagger json终点   SwaggerEndpoint的地址要和RouteTemplate对应
            app.UseSwaggerUI(x =>
            {

                _apiList.ForEach(a =>
                {
                    var doc = $"/doc/{a}/v1.0/swagger.json";
                    x.SwaggerEndpoint(doc, a);
                });
               
                x.RoutePrefix = string.Empty; //设置根节点访问
            });
        }
    }
}
