import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';

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

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses(): void {
    this.loadingCourses = true;
    const token = localStorage.getItem('authToken'); // Assuming the JWT token is saved in local storage
    if (!token) {
      this.errorMessage = 'Authentication token not found.';
      this.loadingCourses = false;
      return;
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.get<any[]>(`${this.apiUrl}/teachers/courses`, { headers })
      .subscribe(
        data => {
          this.courses = data;
          this.loadingCourses = false;
        },
        error => {
          this.errorMessage = 'Failed to load courses. Please try again later.';
          console.log(this.errorMessage);
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
    const token = localStorage.getItem('authToken');

    if (!token) {
      this.errorMessage = 'Authentication token not found.';
      this.loadingStudents = false;
      return;
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.http.get<any[]>(

        `${this.apiUrl}/teachers/students/${this.enteredCourseCode.trim()}`,{ headers })

      .subscribe(
        (data) => {
          this.students = data.map(student => ({
            regNumber: student.regNumber || 'N/A',
            fullName: student.fullName || 'Unknown'
          }));
          this.loadingStudents = false;
        },
        (error) => {
          console.error('Error fetching students:', error);
          this.errorMessage = 'Failed to load students. Please check the course code and try again.';
          this.loadingStudents = false;
          alert(this.errorMessage);
        }
      );
  }
}