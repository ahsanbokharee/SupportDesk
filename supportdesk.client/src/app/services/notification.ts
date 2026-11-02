import { inject, Service } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Service()
export class NotificationService {
  private snackBar = inject(MatSnackBar);

  private readonly defaults: MatSnackBarConfig = {
    horizontalPosition: 'end',
    verticalPosition: 'top',
  };

  success(message: string) {
    this.snackBar.open(message, 'OK', {
      ...this.defaults,
      duration: 4000,
      panelClass: ['snack', 'snack-success'],
    });
  }

  info(message: string) {
    this.snackBar.open(message, 'OK', {
      ...this.defaults,
      duration: 6000,
      panelClass: ['snack', 'snack-info'],
    });
  }
}