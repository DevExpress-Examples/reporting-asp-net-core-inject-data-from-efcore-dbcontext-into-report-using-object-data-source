using System;
using System.ComponentModel.Design;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using Microsoft.Extensions.DependencyInjection;

namespace xrefcoredemo.Services {
    public interface IObjectDataSourceInjector {
        public void Process(XtraReport report);
    }

    class ObjectDataSourceInjector : IObjectDataSourceInjector {
        IServiceProvider ServiceProvider { get; }

        public ObjectDataSourceInjector(IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Process(XtraReport report) {
            var dse = new UniqueDataSourceEnumerator();
            ((IServiceContainer)report).ReplaceService(typeof(IReportProvider), ServiceProvider.GetRequiredService<IReportProvider>());
            foreach(var dataSource in dse.EnumerateDataSources(report, true)) {
                if(dataSource is ObjectDataSource ods && ods.DataSource is Type dataSourceType) {
                    ods.DataSource = ServiceProvider.GetRequiredService(dataSourceType);
                }
            }
        }
    }
}
