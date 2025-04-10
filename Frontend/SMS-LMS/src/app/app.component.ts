import { Component } from '@angular/core';
import { Router } from "@angular/router";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    standalone: false
})
export class AppComponent {
  title = 'SMS-LMS';
  showNavbar: boolean = true;

  constructor(private router: Router) {
    router.events.subscribe(() => {
      this.showNavbar = !['/login', '/register'].includes(router.url);
    });
  }
}
