import { Component } from '@angular/core';
import { TeacherService } from '../../services/teacher.service';

@Component({
    selector: 'app-teacher-assignments',
    templateUrl: './teacher-assignments.component.html',
    styleUrl: './teacher-assignments.component.css',
    standalone: false
})
export class TeacherAssignmentsComponent {

  courses: any[] = [];
  selectedCourseId: string = '';
  title: string = '';
  description: string = '';
  WrittenAssignment: string ='';
  dueDate: string = '';
  selectedFile: File | undefined;

  constructor(private assignmentService: TeacherService) {}

  ngOnInit() {
    this.loadTeacherCourses();
  }

  // Fetch courses assigned to the teacher
  loadTeacherCourses() {
    this.assignmentService.loadCourses().subscribe({
      next: (data: any) => {
        this.courses = data;
      },
      error: (err) => {
        console.error("Error fetching courses:", err);
      }
    });
  }

  // Handle file selection
  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  // Submit assignment
  submitAssignment() {
    if (!this.selectedCourseId) {
      alert('Please select a course');
      return;
    }

    if (this.selectedFile) {
      // First upload the file
      this.assignmentService.uploadFile(this.selectedFile).subscribe({
        next: (fileUrl: string) => {
          // Once the file is uploaded, create the assignment
          const assignmentData = {
            title: this.title,
            description: this.description,
            WrittenAssignment: this.WrittenAssignment,
            dueDate: this.dueDate,
            courseId: this.selectedCourseId,
            fileUrl: fileUrl // Pass the file URL received after upload
          };

          this.assignmentService.createAssignment(assignmentData).subscribe({
            next: (response) => {
              alert('Assignment posted successfully!');
            },
            error: (err) => {
              console.error('Error creating assignment:', err);
              alert('Error posting assignment');
            }
          });
        },
        error: (err) => {
          console.error('Error uploading file:', err);
          alert('Error uploading file');
        }
      });
    } else {
      // Create assignment without file if no file is selected
      const assignmentData = {
        title: this.title,
        description: this.description,
        WrittenAssignment: this.WrittenAssignment,
        dueDate: this.dueDate,
        courseId: this.selectedCourseId
      };

      this.assignmentService.createAssignment(assignmentData).subscribe({
        next: (response) => {
          alert('Assignment posted successfully!');
          // ✅ Clear form fields
        this.title = '';
        this.description = '';
        this.dueDate = '';
        this.selectedCourseId = ''; // Reset dropdown selection

        // ✅ Clear file input
        const fileInput = document.getElementById("fileInput") as HTMLInputElement;
        if (fileInput) {
          fileInput.value = ""; // Reset file input field
        }
          },
        error: (err) => {
          console.error('Error creating assignment:', err);
          alert('Error posting assignment');
        }
      });
    }
  }
}
