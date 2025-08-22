// src/app/core/services/auth.service.ts
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUser: any = null;

  constructor() {}

  login(username: string, password: string): boolean {
    // Mock: if username contains "staff" → staff, else patient
    if (username && password) {
      this.currentUser = {
        id: username.includes('staff') ? 'S001' : 'P001',
        name: username,
        role: username.includes('staff') ? 'staff' : 'patient'
      };
      return true;
    }
    return false;
  }

  register(username: string, password: string): boolean {
    // Mock register (always succeeds)
    return true;
  }

  getUser() {
    return this.currentUser;
  }

  logout() {
    this.currentUser = null;
  }

  isLoggedIn(): boolean {
    return this.currentUser !== null;
  }
}
