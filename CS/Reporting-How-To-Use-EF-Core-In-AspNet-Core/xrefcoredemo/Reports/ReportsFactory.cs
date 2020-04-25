using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace xrefcoredemo.Reports {
    public class ReportsFactory {               
        public Dictionary<string, Func<XtraReport>> Reports {
            get {
                return new Dictionary<string, Func<XtraReport>>() {
                    ["Enrollments"] = () => new MyEnrollmentsReport(),
                    ["CourseList"] = () => new CourseListReport(),
                };
            }
        } 
    }
}
