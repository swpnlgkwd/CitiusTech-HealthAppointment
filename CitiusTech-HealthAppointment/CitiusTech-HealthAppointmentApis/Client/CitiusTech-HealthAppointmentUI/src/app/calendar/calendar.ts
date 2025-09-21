// src/app/calendar/calendar.component.ts
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AppointmentService } from '../core/services/appointment';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';


export interface AppointmentInfo {
  id: number;
  doctor: string;
  type: string;
  date: string;   // ISO date-time
  status: string;
}

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.html',
  styleUrls: ['./calendar.css'],
  imports: [CommonModule],
  standalone: true
})

export class CalendarComponent implements OnInit {
  constructor(private appointmentService: AppointmentService, private router: Router, private cdRef: ChangeDetectorRef) { }
  appointments: AppointmentInfo[] = [];
  isLoading: boolean = false;

  // UI state
  currentDate: Date = new Date();
  weekDates: Date[] = [];
  selectedDateKey: string = ''; // YYYY-MM-DD
  appointmentsByDate: Record<string, AppointmentInfo[]> = {};
  filteredAppointments: AppointmentInfo[] = [];

  // aggregated stats
  stats = {
    upcoming: 0,
    completed: 0,
    cancelled: 0,
    noshow: 0
  };

  refresh() {
    this.isLoading = true;
    this.appointmentService.fetchAppintments().subscribe(data => {
      this.appointments = data;
      this.isLoading = false;

      this.appointments = this.appointments.map(a => {
        // if time has no seconds, add ':00'
        if (/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}$/.test(a.date)) {
          a.date = `${a.date}:00`;
        }
        return a;
      });
      this.buildAppointmentsMap();
      this.computeStats();
      // select today's date by default (YYYY-MM-DD)
      const todayKey = this.dateKey(this.currentDate);
      // if today not in the map but exists in week, still select it
      this.selectDate(todayKey);
      this.cdRef.detectChanges();
    });
  }

  ngOnInit(): void {



    this.generateWeek(this.currentDate);
    this.refresh();



  }

  // Build weekDates array (Monday start)
  generateWeek(baseDate: Date) {
    const base = new Date(baseDate);
    // Monday as start: compute offset (JS getDay: Sunday=0 ... Saturday=6)
    const day = base.getDay();
    const offsetToMonday = (day === 0) ? -6 : (1 - day);
    const startOfWeek = new Date(base);
    startOfWeek.setDate(base.getDate() + offsetToMonday);

    this.weekDates = Array.from({ length: 7 }, (_, i) => {
      const d = new Date(startOfWeek);
      d.setDate(startOfWeek.getDate() + i);
      return d;
    });
  }

  // Build map of appointments keyed by YYYY-MM-DD
  buildAppointmentsMap() {
    this.appointmentsByDate = {};
    for (const a of this.appointments) {
      const key = a.date.split('T')[0];
      if (!this.appointmentsByDate[key]) this.appointmentsByDate[key] = [];
      this.appointmentsByDate[key].push(a);
    }
  }

  // Compute simple stats (Upcoming = future & booked/rescheduled)
  computeStats() {
    const now = new Date();
    let upcoming = 0, completed = 0, cancelled = 0, noshow = 0;
    for (const a of this.appointments) {
      const apptDate = new Date(a.date);
      const s = (a.status || '').toLowerCase();
      if (s === 'cancelled') cancelled++;
      else if (s === 'completed') completed++;
      else if (s === 'noshow') noshow++;
      else {
        // treat future and present as upcoming
        if (apptDate >= now) upcoming++;
        else completed++; // past not-cancelled considered completed for this simple logic
      }
    }
    this.stats.upcoming = upcoming;
    this.stats.completed = completed;
    this.stats.cancelled = cancelled;
    this.stats.noshow = noshow;
  }

  // Helpers
  dateKey(d: Date | string): string {
    if (typeof d === 'string') return d;
    //convert d to ISO format
    var c = new Date(d.getTime() - (d.getTimezoneOffset() * 60000));
    return c.toISOString().split('T')[0];
  }

  formatTime(iso: string): string {
    const dt = new Date(iso);
    return dt.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  formatDateLong(iso: string): string {
    const dt = new Date(iso);
    return dt.toLocaleString([], { dateStyle: 'medium', timeStyle: 'short' });
  }

  // Navigation
  prevWeek() {
    this.currentDate.setDate(this.currentDate.getDate() - 7);
    this.generateWeek(this.currentDate);
    // auto-select day: keep same weekday if possible, else today
    const todayKey = this.dateKey(new Date());
    this.selectDate(todayKey);
  }
  nextWeek() {
    this.currentDate.setDate(this.currentDate.getDate() + 7);
    this.generateWeek(this.currentDate);
    const todayKey = this.dateKey(new Date());
    this.selectDate(todayKey);
  }

  // When a day is clicked, set selected date key and update details
  selectDate(dateKey: string) {
    this.selectedDateKey = dateKey;
    this.filteredAppointments = this.appointmentsByDate[dateKey] ? [...this.appointmentsByDate[dateKey]] : [];
    // sort by time
    this.filteredAppointments.sort((a, b) => a.date.localeCompare(b.date));
  }

  // quick reset to show all upcoming in right pane (if desired)
  resetFilter() {
    this.selectedDateKey = '';
    this.filteredAppointments = [];
  }

  // For right pane upcoming list (we show upcoming appointments sorted ascending)
  getUpcomingList(): AppointmentInfo[] {
    const now = new Date();
    return this.appointments
      .filter(a => new Date(a.date) >= now && (a.status.toLowerCase() !== 'cancelled'))
      .sort((a, b) => a.date.localeCompare(b.date));
  }

  // badge class for status
  getStatusClass(status: string) {
    const s = (status || '').toLowerCase();
    if (s === 'booked') return 'badge-success';
    if (s === 'rescheduled') return 'badge-warning';
    if (s === 'cancelled') return 'badge-danger';
    if (s === 'noshow') return 'badge-secondary';
    if (s === 'completed') return 'badge-completed';
    return 'badge-light';
  }
}
