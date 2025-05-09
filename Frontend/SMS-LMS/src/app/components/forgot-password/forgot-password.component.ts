import { Component } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  standalone: false,
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {

  email = '';
  message: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    this.authService.forgotPassword(this.email).subscribe({
      next: () => {
        this.message = 'Reset link was sent to your email.';
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.message = 'Failed to send reset link. Please try again.';
        console.error(err);
      }
    });
  }

}
