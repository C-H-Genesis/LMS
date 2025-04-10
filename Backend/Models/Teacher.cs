
using System.ComponentModel.DataAnnotations.Schema;
using CourseModel;
using UsersModel;

namespace TeacherModel
{
    public class Teacher : User
{
    [Column("Department")]
    public string? Department { get; set; }
     
}

}