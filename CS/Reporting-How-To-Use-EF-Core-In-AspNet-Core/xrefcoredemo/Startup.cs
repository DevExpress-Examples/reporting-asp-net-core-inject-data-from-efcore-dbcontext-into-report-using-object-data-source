using System;
using System.IO;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Sql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using DevExpress.XtraReports.Web.Extensions;
using xrefcoredemo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using xrefcoredemo.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.XtraReports.Web.ReportDesigner.Services;

namespace xrefcoredemo {
    public class Startup {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) {
            Configuration = configuration;
            Env = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddDevExpressControls();
            services.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var builder = services
                .AddMvc()
                .RemoveDefaultReportingControllers()    // NOTE: make sure the default document viewer controller is not registered
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
#if DEBUG
            if(Env.IsDevelopment()) {
                builder.AddRazorRuntimeCompilation();
            }
#endif

            services.ConfigureReportingServices(configurator => {
                configurator.ConfigureReportDesigner((reportDesignerConfigurator) => {
                    reportDesignerConfigurator.RegisterObjectDataSourceConstructorFilterService<ObjectDataSourceConstructorFilterService>();
                    reportDesignerConfigurator.RegisterObjectDataSourceWizardTypeProvider<ObjectDataSourceWizardTypeProvider>();
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
            });

            services.AddScoped<IWebDocumentViewerAuthorizationService, DocumentViewerAuthorizationService>();
            services.AddScoped<WebDocumentViewerOperationLogger, DocumentViewerAuthorizationService>();

            services.AddSingleton<IScopedDbContextProvider<SchoolContext>, ScopedDbContextProvider<SchoolContext>>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWebDocumentViewerReportResolver, WebDocumentViewerReportResolver>();
            services.AddScoped<IObjectDataSourceInjector, ObjectDataSourceInjector>();
            services.AddTransient<ReportStorageWebExtension, EFCoreReportStorageWebExtension>();
            services.AddTransient<CourseListReportRepository>();
            services.AddTransient<MyEnrollmentsReportRepository>();
            services.AddScoped<PreviewReportCustomizationService, CustomPreviewReportCustomizationService>();
            //// Alternative way to register the repository
            //services.AddTransient<MyEnrollmentsReportRepository>(sp => {
            //    return new MyEnrollmentsReportRepository(
            //        sp.GetRequiredService<IScopedDbContextProvider<SchoolContext>>(),
            //        sp.GetRequiredService<IUserService>().GetCurrentUserId());
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {
            app.UseDevExpressControls();
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
