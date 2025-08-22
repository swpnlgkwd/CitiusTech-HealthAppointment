import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router'; // <-- Add this import
import { ChatWidgetComponent } from "./chat/chat-widget/chat-widget";
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services/auth';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  imports: [RouterModule, ChatWidgetComponent, CommonModule] // <-- Add RouterModule here
})
export class AppComponent {
  constructor(private router: Router, private authService: AuthService) {}

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  logout() {
    localStorage.clear();
    this.router.navigate(['/login']);
  }
}
