export interface Appointment {
    id: number;
    patientId: string;
    title: string;
    start: Date;
    riskFlag: 'low' | 'medium' | 'high';
    patientName: string;
}
