import { Component, inject, signal } from '@angular/core';
import { User, UserService } from '../../services/user';

@Component({
  selector: 'app-user-list',
  standalone: false,
  styleUrl: './user-list.css',
  templateUrl: './user-list.html',
})
export class UserList {
  private userService = inject(UserService);

  users = signal<User[] | null>(null);
  errorMessage = signal<string | null>(null);

  ngOnInit() {
    this.userService.getUsers().subscribe(
      {
        next: users => this.users.set(users),
        error: err => {
          this.errorMessage.set('Could not load users.');
          console.log(err);
        }
      }
    );
  }
}
