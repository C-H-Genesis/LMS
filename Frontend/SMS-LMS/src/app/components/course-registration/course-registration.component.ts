import { Component, OnInit } from '@angular/core';
import { StudentService } from '../../services/student.service';
import { RegisterCourseDto } from '../../DTOs/RegisterCourseDto';


@Component({
  selector: 'app-course-registration',
  templateUrl: './course-registration.component.html',
  styleUrl: './course-registration.component.css'
})
export class CourseRegistrationComponent  implements OnInit {
  registerCourseDto! : RegisterCourseDto;
  registeredCourses: any[] = [];
  showModal = false;

  constructor(private studentService: StudentService) {}

  ngOnInit(): void {
    this.registerCourseDto = {
      CourseCode: '',
      Status: true  || false// Default to "First Attempt"
    };
    this.fetchRegisteredCourses();
  }
  
  openModal() : void {
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  registerCourse(): void { 

    this.studentService.registerCourse(this.registerCourseDto).subscribe(
      (response) => {

        this.fetchRegisteredCourses(); // Refresh the list of registered courses
        this.registerCourseDto.CourseCode = ''; // Reset the form
        alert('Course Registration Successfull');
      },
      (error) => {
        console.error('Error registering course:', error);
        console.log('Error: ' + JSON.stringify(error.error));
        alert('Error registering course');
      }
    );
  }

  fetchRegisteredCourses(): void {
    this.studentService.getRegisteredCourses().subscribe(
      (courses) => {
        this.registeredCourses = courses;
      },
      (error) => {
        console.error('Error fetching registered courses:', error);
        this.registeredCourses = [];
      }
    );
  }

 
}
