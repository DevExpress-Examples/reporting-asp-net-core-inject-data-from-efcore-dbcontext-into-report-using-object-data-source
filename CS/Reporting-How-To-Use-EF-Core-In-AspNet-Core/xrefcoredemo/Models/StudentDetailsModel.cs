using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xrefcoredemo.Models {
    public class StudentDetailsModel {
        public int StudentID { get; set; }
        public string FirstMidName { get; set; }
        public string LastName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
