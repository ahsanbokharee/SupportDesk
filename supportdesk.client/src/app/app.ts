import { Component, inject, OnInit, signal } from '@angular/core';
import { AuthService } from './services/auth';
import { Router } from '@angular/router';
import { NotificationService } from './services/notification';


@Component({
  selector: 'app-root',
  standalone: false,
  styleUrl: './app.css',
  templateUrl: './app.html',
})

export class App {
  auth = inject(AuthService);
  private router = inject(Router);
  private notify = inject(NotificationService);

  logout() {
    this.auth.logout();
    this.notify.success('You have been logged out');  
    this.router.navigate(['/login']);
  }

  protected readonly title = signal('supportdesk.client');
}
