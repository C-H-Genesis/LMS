import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../services/admin.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-manage-courses',
    templateUrl: './manage-courses.component.html',
    styleUrl: './manage-courses.component.css',
    standalone: false
})
export class ManageCoursesComponent implements OnInit {
  courses: any[] = [];
  newCourse = { Id: '',courseCode: '', courseName: '', teacherName: '' };
  errorMessage = '';
  showModal = false;

  constructor(private courseService: AdminService, private router: Router) {}

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses(): void {
    this.courseService.getCourses().subscribe(
      (courses) => {
        this.courses = courses;
      },
      (error) => {
        console.error('Error loading courses', error);
      }
    );
  }

  openModal() {
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  addCourse(): void {
    if (!this.newCourse.courseCode || !this.newCourse.courseName || !this.newCourse.teacherName) {
      this.errorMessage = 'All fields are required.';
      return;
    }
      this.courseService.addCourse(this.newCourse).subscribe(
        (response) => {
          this.errorMessage = '';
          alert('Course added');
          this.closeModal();
          this.loadCourses();  // Reload courses after adding a new one
          this.newCourse = { Id: '',courseCode: '', courseName: '', teacherName: '' };  // Clear form
        },
        (error) => {
          this.errorMessage = error.error || 'Error adding course';
          console.log('Error adding course');
        }
      );
    }
  
    deleteCourse(courseId: string): void {
      if (confirm('Are you sure you want to delete this course?')) {
        if (!courseId) {  // Ensure courseId exists
          console.error("Invalid course ID:");
          console.log(courseId);
          return;
        }
        this.courseService.deleteCourse(courseId).subscribe(
          (response) => {
            this.loadCourses();  // Reload courses after deletion
          },
          (error) => {
            this.errorMessage = error.error || 'Error deleting course';
            console.log(this.errorMessage);
          }
        );
      }
    }
  }
  