
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnrollmentModel;
using UsersModel;

namespace StudentModel
{
    public class Student : User
{
    [Column("RegNumber")]
    public string? RegNumber { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
   

}

}