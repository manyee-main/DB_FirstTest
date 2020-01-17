using Autofac;
using Autofac.Extras.DynamicProxy;
using BusinessObjects.Aspect;
using BusinessObjects.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace DB_FirstTest
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
            services.AddDbContext<AppDbContext>(
                options =>
                    options.UseSqlServer(Configuration.GetConnectionString("University")),
                ServiceLifetime.Scoped
            ) ;
            services.AddControllersWithViews().AddControllersAsServices();
            services.AddControllersWithViews();
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.Register(l => new Logger(System.Console.Out))//ע�������� ����ע��
            //    .Named<IInterceptor>("_logger");
            builder.Register(l => new Logger(System.Console.Out));//ע�������� ����ע��

            //Type baseType = typeof(IDependency);
            //Type basetype = typeof(IDependency);  //��Ч �����ֶ�ʹ��Autofac��ȡ
            //builder.RegisterAssemblyTypes(System.Reflection.Assembly.GetExecutingAssembly())
            //    .Where(t => baseType.IsAssignableFrom(t) && t.IsClass)
            //    .AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(BusinessObjects.Dao.StudentDao).Assembly)
                   .Where(t => t.Name.EndsWith("Dao"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope()
                   .EnableInterfaceInterceptors();
            builder.RegisterAssemblyTypes(typeof(BusinessObjects.Services.StudentServices).Assembly)
                   .Where(t => t.Name.EndsWith("Services"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope()
                   .EnableInterfaceInterceptors();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
