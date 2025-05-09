import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { RegisterRequest } from '../../DTOs/RegisterRequest';
import { AuthService } from '../../auth/auth.service';
import { Console } from 'node:console';



@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrl: './register.component.css',
    standalone: false
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  successMessage: string = ''; 
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      role: ['', Validators.required],
      fullName: ['', Validators.required],
      email: ['', Validators.required]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.registerForm.valid) {
      const registerData: RegisterRequest = this.registerForm.value;
      this.authService.register(registerData).subscribe(
        () => {
          this.router.navigate(['/login']);
        },
        () => {
         console.log(this.errorMessage = 'Registration failed. Please try again.');
        }
      );
    }
  }
}
