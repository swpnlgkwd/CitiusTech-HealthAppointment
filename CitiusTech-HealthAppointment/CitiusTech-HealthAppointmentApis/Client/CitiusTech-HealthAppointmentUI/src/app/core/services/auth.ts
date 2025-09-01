// src/app/core/services/auth.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs';
import { loginDto, loginResponse, registerDto } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUser: any = null;
  private baseUrl = 'http://localhost:5053/api/Auth'; // Replace with actual API URL
  constructor(private http: HttpClient) { }

  login(username: string, password: string) {
    const data : loginDto = { email: username, password: password };
    return this.http.post<loginResponse>(`${this.baseUrl}/login`, data);
  }

  register(register: registerDto) {
    return this.http.post<any>(`${this.baseUrl}/register`, register);
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
