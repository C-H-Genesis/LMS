import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../auth/auth.service';  // Service to get role and token

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  userRole: string | null = null;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.userRole = this.authService.getRole();  // Get user role from the token
  }

  logout(): void {
    this.authService.logout();  // Log out logic
  }
}
