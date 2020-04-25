using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xrefcoredemo.Data {
    public enum Grade {
        A, B, C, D, F
    }

    public class Enrollment {
        public int EnrollmentID { get; set; }
        public Grade? Grade { get; set; }

        public Course Course { get; set; }
        public Student Student { get; set; }
    }
}
