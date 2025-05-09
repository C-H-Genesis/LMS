import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "../environments/environment";
import { RegisterRequest } from '../DTOs/RegisterRequest';
import { LoginRequest } from "../DTOs/LoginRequest";
import { jwtDecode } from "jwt-decode";
import { Router } from '@angular/router';



@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/api/auth`;

  constructor(private http: HttpClient, private router: Router) {}

  // Register method
  register(userData: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData);
  }

  // Login method
  login(credentials: LoginRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, credentials);
  }

  // Save token to localStorage
  saveToken(token: string): void {
    localStorage.setItem('authToken', token);
  }

  // Retrieve token from localStorage
 getToken(): string | null {
  if (typeof window !== 'undefined' && localStorage) {
    return localStorage.getItem('authToken');
  }
  return null;
}


   // Decode token to get role
   getRole(): string | null {
    const token = this.getToken();
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        return decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null; // Assuming the token has a 'role' field
      } catch (err) {
        console.error('Error decoding token', err);
        return null;
      }
    }
    return null;
  }


  // Logout method
  logout(): void {
    localStorage.removeItem('authToken');
    this.router.navigate(['/login']);
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`, { email })
  }

  resetPassword(data: { email: string; token: string; newPassword: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, data);
  }
}