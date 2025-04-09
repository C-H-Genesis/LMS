import { Injectable } from '@angular/core';
import { catchError, map, Observable, throwError } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TeacherService {
private apiUrl = `${environment.apiUrl}/api`;
  errorMessage: string | null = null;
  courses: any[] = [];

  constructor(private http: HttpClient) { }

  loadCourses(): Observable<any> {
   
    const token = localStorage.getItem('authToken'); // Assuming the JWT token is saved in local storage

    if (!token) {
      console.error("No auth token found!");
      return new Observable(observer => observer.error("No auth token found"));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get(`${this.apiUrl}/Teachers/GetCourses`, { headers }); 
  }

  getStudentsByCourseCode(courseCode: string): Observable<any[]> {
    if (!courseCode.trim()) {
      return throwError(() => new Error('Please enter a valid course code.'));
    }

    const token = localStorage.getItem('authToken');
    if (!token) {
      return throwError(() => new Error('Authentication token not found.'));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/teachers/students/${courseCode.trim()}`, { headers })
      .pipe(
        catchError(error => {
          console.error('Error fetching students:', error);
          return throwError(() => new Error('Failed to load students. Please check the course code and try again.'));
        })
      );
  }

  uploadFile(file: File): Observable<string> {
    const formData = new FormData();
    formData.append('file', file);

    const token = localStorage.getItem('authToken'); // Assuming the JWT token is saved in local storage

    if (!token) {
      console.error("No auth token found!");
      return new Observable(observer => observer.error("No auth token found"));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  
    return this.http.post<{ fileUrl: string }>(
      `${this.apiUrl}/Teachers/uploadAssignmentFile`,
      formData,{ headers }
    ).pipe(map(response => response.fileUrl)); // 🔥 Extract fileUrl
  }
  

  createAssignment(assignmentData: any): Observable<any> {
    const token = localStorage.getItem('authToken'); // Assuming the JWT token is saved in local storage

    if (!token) {
      console.error("No auth token found!");
      return new Observable(observer => observer.error("No auth token found"));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const formData = new FormData();
    const formattedDueDate = new Date(assignmentData.dueDate).toISOString().split('T')[0];
  
    // Append assignment details
    formData.append('title', assignmentData.title);
    formData.append('description', assignmentData.description);
    formData.append('WrittenAssignment', assignmentData.WrittenAssignment);
    formData.append('dueDate', formattedDueDate);
    formData.append('fileUrl', assignmentData.fileUrl);

    return this.http.post(`${this.apiUrl}/teachers/createNewAssignment`, assignmentData,{ headers });
  }
  
}
