import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
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
}
