import { Injectable, signal } from '@angular/core';

export interface ToastMessage {
  message: string;
  type: 'success' | 'danger';
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  toast = signal<ToastMessage | null>(null);

  showSuccess(message: string) {
    this.show(message, 'success');
  }

  showError(message: string) {
    this.show(message, 'danger');
  }

  private show(message: string, type: 'success' | 'danger') {
    this.toast.set({ message, type });
    setTimeout(() => {
      this.toast.set(null);
    }, 3000);
  }
}