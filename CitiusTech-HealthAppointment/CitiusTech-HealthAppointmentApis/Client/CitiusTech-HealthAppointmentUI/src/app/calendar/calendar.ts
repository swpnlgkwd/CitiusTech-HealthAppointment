// src/app/calendar/calendar.component.ts
import { Component, OnInit } from '@angular/core';
import { AppointmentService } from '../core/services/appointment';
import { CommonModule } from '@angular/common';
import { Appointment } from '../core/models/appointment.model'; // Add this import, adjust path if needed
import { Router } from '@angular/router';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.html',
  styleUrls: ['./calendar.css'],
  imports: [ CommonModule],
  standalone: true
})
export class CalendarComponent implements OnInit {

  events: Appointment[] = [];
  
  // Mock user role → swap this to 'staff' or 'patient' to test
  userRole: 'patient' | 'staff' = 'patient';
  loggedInUserId: string = 'P001'; // simulate John Doe logged in

  constructor(private appointmentService: AppointmentService, private router: Router) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments() {
    this.appointmentService.getAllAppointments()
      .subscribe(events => {
        this.events = events;
      });
  }

  backToHome() {
    this.router.navigate(['/home']);
  }
}
