using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using xrefcoredemo.Data;
using xrefcoredemo.Services;

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
                .AddNewtonsoftJson();
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
            services.AddTransient<MyEnrollmentsReportRepository>();
            services.AddScoped<PreviewReportCustomizationService, CustomPreviewReportCustomizationService>();
            services.AddScoped<WebDocumentViewerOperationLogger, CustomWebDocumentViewerOperationLogger>();

            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(MyEnrollmentsReportRepository));
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
