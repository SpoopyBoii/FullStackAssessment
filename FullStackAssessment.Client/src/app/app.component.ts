import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { NotificationService } from './services/notification.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive], 
  templateUrl: './app.component.html' 
})
export class AppComponent {
  title = 'FullStackAssessment';
  notificationService = inject(NotificationService);
}