import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BASE_URL } from './common-exports';
import { AppointmentInfo } from '../../calendar/calendar';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {

  constructor(private http: HttpClient) { }

  private appointments = [
    { id: 1, patientId: 'P001', title: 'General Checkup', start: new Date(), riskFlag: 'low', patientName: 'John Doe' },
    { id: 2, patientId: 'P002', title: 'Heart Specialist', start: new Date(Date.now() + 86400000), riskFlag: 'high', patientName: 'Alice Smith' }
  ];

  getAppointmentsByPatient(patientId: string): Observable<any[]> {
    return of(this.appointments.filter(a => a.patientId === patientId));
  }

  getAllAppointments(): Observable<any[]> {
    return of(this.appointments);
  }

  // for dashboard only
  fetchAppintments():  Observable<AppointmentInfo[]> {
    return this.http.get<AppointmentInfo[]>(`${BASE_URL}/appointment`)
  }
}
