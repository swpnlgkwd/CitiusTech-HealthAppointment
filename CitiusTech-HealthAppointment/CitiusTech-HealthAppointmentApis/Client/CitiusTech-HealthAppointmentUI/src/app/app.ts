import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router'; // <-- Add this import
import { ChatWidgetComponent } from "./chat/chat-widget/chat-widget";
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services/auth/auth';
import { AgentService } from './core/services/agent';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
  imports: [RouterModule, ChatWidgetComponent, CommonModule] // <-- Add RouterModule here
})
export class AppComponent {
  constructor(private router: Router, private authService: AuthService, private agentService: AgentService) { }
  isLoading = false;
  get isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }

  logout() {
    this.isLoading = true;
    this.isLoading = false;
      localStorage.clear();
      this.router.navigate(['/login']);
  }
}
