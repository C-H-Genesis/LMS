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
          console.log('Token saved:', response.token);
  
          // Get the role from the token
          const role = this.authService.getRole();
          console.log('Extracted role:', role);
  
          // Navigate based on role
          switch (role) {
            case 'Admin':
              this.router.navigate(['/admin-dashboard']);
              console.log('Navigated to Admin dashboard');
              break;
            case 'Teacher':
              this.router.navigate(['/teacher-dashboard']);
              console.log('Navigated to Teacher dashboard');
              break;
            case 'Student':
              this.router.navigate(['/student-dashboard']);
              console.log('Navigated to Student dashboard');
              break;
            case 'Finance':
              this.router.navigate(['/finance-dashboard']);
              console.log('Navigated to Finance dashboard');
              break;
            default:
              this.router.navigate(['/login']);
              console.log('Role not recognized. Navigation failed.');
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


