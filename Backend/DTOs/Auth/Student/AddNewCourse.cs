using TeacherModel;

namespace AddNewCourse
{
    public class AddCourseDto
{
    public required string CourseCode { get; set; } // Required
    public required string CourseName { get; set; } // Required
    public required string TeacherName {get; set;}
}
    
}

