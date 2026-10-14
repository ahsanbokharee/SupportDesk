import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Ticket, TicketService } from './services/ticket';
import { AuthService } from './services/auth';
import { Router } from '@angular/router';


@Component({
  selector: 'app-root',
  standalone: false,
  styleUrl: './app.css',
  templateUrl: './app.html',
})

export class App {
  auth = inject(AuthService);
  private router = inject(Router);

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  protected readonly title = signal('supportdesk.client');
}
