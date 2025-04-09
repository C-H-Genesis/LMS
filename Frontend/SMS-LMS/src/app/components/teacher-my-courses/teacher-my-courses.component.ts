import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';
import { TeacherService } from '../../services/teacher.service';

@Component({
  selector: 'app-teacher-my-courses',
  templateUrl: './teacher-my-courses.component.html',
  styleUrl: './teacher-my-courses.component.css'
})
export class TeacherMyCoursesComponent implements OnInit {

  private apiUrl = `${environment.apiUrl}/api`;

  courses: any[] = [];
  students: any[] = [];
  enteredCourseCode: string = '';
  selectedCourseCode: string | null = null;
  selectedCourseName: string | null = null;
  loadingCourses = false;
  loadingStudents = false;
  errorMessage: string | null = null;

  constructor(private teacherService : TeacherService,private http: HttpClient) {}

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses(): void {
    this.loadingCourses = true;
    
    this.teacherService.loadCourses().subscribe(
      data => {
        this.courses = data;
        this.loadingCourses = false;
      },
      error => {
        this.errorMessage = error.message;
        this.loadingCourses = false;
      }
    );
  }

  selectCourse(courseCode: string): void {
    this.selectedCourseCode = courseCode;
    this.selectedCourseName = this.courses.find(c => c.courseCode === courseCode)?.courseName;
    
  }

  fetchStudents(): void {
    if (!this.enteredCourseCode.trim()) {
      this.errorMessage = 'Please enter a valid course code.';
      return;
    }

    this.errorMessage = null;
    this.loadingStudents = true;

    this.teacherService.getStudentsByCourseCode(this.enteredCourseCode).subscribe(
      (data: any[]) => { 
        this.students = data.map(student => ({
          regNumber: student.regNumber || 'N/A',
          fullName: student.fullName || 'Unknown'
        }));
        this.loadingStudents = false;
      },
      (error: any) => { 
        this.errorMessage = error.message;
        this.loadingStudents = false;
        console.log(this.errorMessage);
      }
    );
  }
}