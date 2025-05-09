import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StudentService } from '../services/student.service';
import { AuthService } from '../auth/auth.service';

@Component({
    selector: 'app-student-assignment',
    templateUrl: './student-assignment.component.html',
    styleUrl: './student-assignment.component.css',
    standalone: false
})
export class StudentAssignmentComponent {
  assignments: any[] = [];
  loadingAssignments = false;
  errorMessage = '';
  submitError = '';
  submitSuccess = '';
  selectedAssignment: any = null;
  showModal = true;
  assignmentForm!: FormGroup;
  isFileUpload = false;
  selectedFile: File | null = null;

  constructor(
    private fb: FormBuilder,
    private submissionService: StudentService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadAssignments();

    this.assignmentForm = this.fb.group({
      assignmentId: [''],
      courseId: [''],
      course: ['', Validators.required],
      userId: [''], // populated from token
      fileUrl: [''],
      writtenAssignment: [''],
      submittedAt: [new Date().toISOString().substring(0, 16), Validators.required],
    });
  }

  loadAssignments() {
    this.loadingAssignments = true;
    this.submissionService.getAssignmentsByCourse().subscribe({
      next: (data) => {
        this.assignments = data;
        this.loadingAssignments = false;
      },
      error: (err) => {
        this.errorMessage = err.error || 'Failed to load assignments.';
        this.loadingAssignments = false;
      }
    });
  }

  selectAssignment(assignment: any) {
    this.selectedAssignment = assignment;
    this.assignmentForm.patchValue({
      assignmentId: assignment.id,
      courseId: assignment.courseId,
      course: assignment.courseName,
      userId: this.getUserIdFromToken(),
      submittedAt: new Date().toISOString().substring(0, 16)
    });

    this.submitSuccess = '';
    this.submitError = '';
    this.selectedFile = null;
    this.isFileUpload = false;
  }

  cancelSelection() {
    this.selectedAssignment = null;
    this.assignmentForm.reset();
    this.selectedFile = null;
    this.isFileUpload = false;
  }

  toggleUploadMode() {
    this.isFileUpload = !this.isFileUpload;
    if (this.isFileUpload) {
      this.assignmentForm.get('writtenSubmission')?.setValue('');
    } else {
      this.selectedFile = null;
    }
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  submitAssignment() {
    const formData = this.assignmentForm.value;

    // Validate presence of content
    if (this.isFileUpload && !this.selectedFile) {
      this.submitError = 'Please select a file to upload.';
      return;
    }

    if (!this.isFileUpload && !formData.writtenSubmission?.trim()) {
      this.submitError = 'Please provide a written submission.';
      return;
    }

    // Handle file upload
    if (this.isFileUpload && this.selectedFile) {
      this.submissionService.uploadAssignmentFile(
        this.selectedFile,
        formData.assignmentId,
        formData.courseId
      ).subscribe({
        next: (response) => {
          formData.fileUrl = response.fileUrl;
          this.submitFinal(formData);
        },
        error: (err) => {
          this.submitError = err.error || 'File upload failed.';
        }
      });
    } else {
      this.submitFinal(formData);
    }
  }

  submitFinal(formData: any) {
    this.submissionService.postSubmission(formData).subscribe({
      next: () => {
        this.submitSuccess = 'Assignment submitted successfully!';
        this.submitError = '';
        this.cancelSelection();
      },
      error: (err) => {
        this.submitError = err.error || 'Submission failed.';
        this.submitSuccess = '';
      }
    });
  }

  getUserIdFromToken() {
    return this.authService.getToken();
  }

  closeModal() {
    this.showModal = false;
  }
  }

  
  

 
