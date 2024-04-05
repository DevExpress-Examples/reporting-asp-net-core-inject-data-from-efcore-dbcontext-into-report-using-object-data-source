using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace xrefcoredemo.Services {
    public class CustomWebDocumentViewerOperationLogger: WebDocumentViewerOperationLogger {
        private readonly IObjectDataSourceInjector objectDataSourceInjector;

        public CustomWebDocumentViewerOperationLogger(IObjectDataSourceInjector objectDataSourceInjector) {
            this.objectDataSourceInjector = objectDataSourceInjector;
        }

        public override void ReportLoadedFromLayout(string reportId, XtraReport report, out CachedReportSourceWeb cachedReportSourceWeb) {
            objectDataSourceInjector.Process(report);
            cachedReportSourceWeb = new CachedReportSourceWeb(report);
        }
    }
}
