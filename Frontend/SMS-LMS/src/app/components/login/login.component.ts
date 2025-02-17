import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginRequest } from '../../DTOs/LoginRequest';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.loginForm.valid) {
      const loginData: LoginRequest = this.loginForm.value;
      this.authService.login(loginData).subscribe(
        (response: any) => {
          // Save the token
          this.authService.saveToken(response.token);
  
          // Get the role from the token
          const role = this.authService.getRole();
  
          // Navigate based on role
          switch (role) {
            case 'Admin':
              this.router.navigate(['/admin-dashboard']);
              break;
            case 'Teacher':
              this.router.navigate(['/teacher-dashboard']);
              break;
            case 'Student':
              this.router.navigate(['/student-dashboard']);
              break;
            case 'Finance':
              this.router.navigate(['/finance-dashboard']);
              break;
            default:
              this.router.navigate(['/login']);
             alert('Role not recognized. Navigation failed.');
          }
        },
        (error: any) => {
          console.error('Login error:', error);
          this.errorMessage = 'Invalid username or password.';
        }
      );
    }
  }
  
}


