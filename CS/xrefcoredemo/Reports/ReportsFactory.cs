using System;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;

namespace xrefcoredemo.Reports {
    public class ReportsFactory {
        public Dictionary<string, Func<XtraReport>> Reports {
            get {
                return new Dictionary<string, Func<XtraReport>>() {
                    ["Enrollments"] = () => new MyEnrollmentsReport()
                };
            }
        }
    }
}
