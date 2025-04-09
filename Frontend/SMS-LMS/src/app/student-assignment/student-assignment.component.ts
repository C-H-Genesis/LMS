import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { StudentService } from '../services/student.service';

@Component({
  selector: 'app-student-assignment',
  templateUrl: './student-assignment.component.html',
  styleUrl: './student-assignment.component.css'
})
export class StudentAssignmentComponent {
  assignmentForm: FormGroup;
  isFileUpload = false;
  selectedFile: File | null = null;

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
      this.assignmentService.submitWrittenAssignment(this.assignmentForm.value).subscribe(response => {
        console.log('Written assignment submitted successfully', response);
      });
    }
  }

}
