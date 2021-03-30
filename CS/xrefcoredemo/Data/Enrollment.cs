namespace xrefcoredemo.Data {
    public enum Grade {
        A, B, C, D, F
    }

    public class Enrollment {
        public int EnrollmentID { get; set; }
        public Grade? Grade { get; set; }

        public string Course { get; set; }
        public Student Student { get; set; }
    }
}
