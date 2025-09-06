// src/app/core/services/auth.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs';
import { loginDto, loginResponse, registerDto } from '../../models/auth.model';
import { BASE_URL } from '../common-exports';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenKey = 'access_token';
  private roleKey = 'role';
  private usernameKey = 'username';

  constructor(private http: HttpClient,
    private router: Router
  ) { }

  login(username: string, password: string) {
    const data: loginDto = { email: username, password: password };
    return this.http.post<any>(`${BASE_URL}/auth/login`, data)
      .pipe(
        tap(res => {
          localStorage.setItem('threadId', res.threadId);   
          localStorage.setItem(this.tokenKey, res.token);
          localStorage.setItem(this.roleKey, res.role);
          localStorage.setItem(this.usernameKey, res.username);
        })
      );
  }

  register(register: registerDto) {
    return this.http.post<any>(`${BASE_URL}/auth/register`, register);
  }

  get token() {
    return localStorage.getItem(this.tokenKey);
  }

  get role() {
    return localStorage.getItem(this.roleKey) || undefined;
  }

  get userName() {
    return localStorage.getItem(this.usernameKey);
  }

  isAuthenticated(): boolean {
    return !!this.token;
  }
}
