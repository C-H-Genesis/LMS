import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { RegisterCourseDto } from '../DTOs/RegisterCourseDto';

@Injectable({
  providedIn: 'root',
})
export class StudentService {
  private apiUrl = 'http://localhost:5183/api/Student'; // Replace with actual API base URL

  constructor(private http: HttpClient, private authService: AuthService) {}

  getProfile(): Observable<any> {
    const token = this.authService.getToken();
    if (!token) {
      console.error('Token is missing. User must log in.');
    throw new Error('Token is missing. User must log in.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get(`${this.apiUrl}/profile`, { headers }).pipe(
      tap((data) => console.log('Fetched profile:')),
      catchError((error) => {
        console.error('Error fetching profile:', error);
        return throwError(error);
      })
    );
  }

 updateProfile(profile: any): Observable<any> {
  const token = localStorage.getItem('authToken'); // Adjust based on how you store tokens
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  return this.http.put(`${this.apiUrl}/profile`, profile, { headers });
}

  registerCourse(courseData: RegisterCourseDto): Observable<any> {
    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.apiUrl}/registerCourse`, courseData, { headers });
  }

  getRegisteredCourses(): Observable<any> {
    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get(`${this.apiUrl}/registered-courses`, { headers });
  }


  uploadAssignmentFile(file: File, courseId: string, userId: string): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('courseId', courseId);
    formData.append('studentId', userId);

    return this.http.post(`http://localhost:5183/api/uploadAssignmentFile`, formData);
  }

  submitWrittenAssignment(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/submitWrittenAssignment`, data);
  }

}
