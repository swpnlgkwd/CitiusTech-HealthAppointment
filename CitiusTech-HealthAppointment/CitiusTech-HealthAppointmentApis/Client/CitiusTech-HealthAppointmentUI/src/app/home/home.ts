// src/app/home/home.component.ts
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../core/services/auth/auth';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.html',
  imports: [ CommonModule, FormsModule ],
  styleUrls: ['./home.css']
})
export class HomeComponent implements OnInit {
  user: any;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.user = this.authService.isAuthenticated();
  }

  ViewCalendar(): void {
    // Navigate to the calendar view
    this.router.navigate(['/calendar']);
  }
}
