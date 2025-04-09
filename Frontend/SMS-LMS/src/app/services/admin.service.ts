import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { catchError, Observable, of, tap } from 'rxjs';
import { Course } from '../DTOs/CourseModel';
@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private baseUrl = 'http://localhost:5183/api/admin';
  
  

  constructor(private http: HttpClient, private authService: AuthService) {}

  getAllUsers() {
    const token = this.authService.getToken(); // Retrieve token from localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get(`${this.baseUrl}/users`, { headers });
  }

  getAllUsersById(userId: string) {
    const token = localStorage.getItem('authToken'); // Retrieve token from localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get(`${this.baseUrl}/${userId}`, { headers });
  }

  getAllUsersByRole(role: string){
    const token = this.authService.getToken(); // Retrieve token from localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get(`${this.baseUrl}/user/${role}`, { headers }).pipe(
      tap(users => console.log(`Fetched ${role} users:`)),
      catchError(error => {
        console.error(`Error fetching ${role} users:`, error);
        return of([]); // Return an empty array if there's an error
      })
    );
  }


  deleteUser(userId: string) {
    const token = this.authService.getToken(); // Retrieve token from localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.baseUrl}/delete-user/${userId}`, { headers });
  }

  createUser(userData: any) {
    const token = this.authService.getToken(); // Retrieve token from localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.baseUrl}/create-user`, userData, { headers });
  }

  updateUserInfo(userId : any){
    const token = this.authService.getToken();
    const headers = new HttpHeaders().set('Authorization',`Bearer ${token}`);
    return this.http.put(`${this.baseUrl}/profile/${userId}`,{ headers });
  }

  //             Course Operations                       //

  getCourses(): Observable<Course[]> {
    const token = this.authService.getToken(); // Retrieve token from localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<Course[]>(`${this.baseUrl}/courses`, { headers });
  }

  addCourse(course: any): Observable<any> {
    const token = this.authService.getToken(); // Retrieve token from localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.baseUrl}/Add-New-Course`, course, { headers });
  }

  deleteCourse(courseId: string): Observable<any> {
    const token = this.authService.getToken(); // Retrieve token from localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.baseUrl}/delete-course/${courseId}`, { headers });
  }
}
