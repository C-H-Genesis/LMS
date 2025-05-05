import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { StudentService } from '../services/student.service';

@Component({
    selector: 'app-student-assignment',
    templateUrl: './student-assignment.component.html',
    styleUrl: './student-assignment.component.css',
    standalone: false
})
export class StudentAssignmentComponent {
  assignmentForm: FormGroup;
  isFileUpload = false;
  selectedFile: File | null = null;
  selectedAssignment: any = null;
  assignments: any[] = [];
  loadingAssignments = false;
  errorMessage: string = '';

  ngOnInit(){
    this.loadAssignments();
  }

  constructor(private fb: FormBuilder, private assignmentService: StudentService) {
    this.assignmentForm = this.fb.group({
      course: [''],
      writtenAssignment: [''],
      dueDate: ['']
    });
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  toggleUploadMode() {
    this.isFileUpload = !this.isFileUpload;
    if (this.isFileUpload) {
      this.assignmentForm.get('writtenAssignment')?.setValue('');
    } else {
      this.selectedFile = null;
    }
  }

  submitAssignment() {
    if (this.isFileUpload && this.selectedFile) {
      this.assignmentService.uploadAssignmentFile(this.selectedFile, this.assignmentForm.value.course, this.assignmentForm.value.dueDate).subscribe(response => {
        console.log('File uploaded successfully', response);
      });
    } else {
      this.assignmentService.postSubmission(this.assignmentForm.value).subscribe(response => {
        console.log('Written assignment submitted successfully', response);
      });
    }
  }

  loadAssignments(): void {
    this.loadingAssignments = true;
    this.assignmentService.getAssignmentsByCourse().subscribe(
      data => {
        this.assignments = data;
        this.loadingAssignments = false;
      },
      error => {
        this.errorMessage = error.message;
        this.loadingAssignments = false;
      }
    );
  }

  viewAssignment(assignment: any): void {
    if (assignment.fileUrl && assignment.fileUrl !== "string" && assignment.fileUrl.trim() !== "") {
      window.open(assignment.fileUrl, "_blank"); // Open file in a new tab
    } 
    else if (assignment.writtenAssignment && assignment.writtenAssignment.trim() !== "") {
      this.selectedAssignment = assignment; // Show written assignment
    }
  }

  
  

  closeAssignment() {
    this.selectedAssignment = null;
  }


}
