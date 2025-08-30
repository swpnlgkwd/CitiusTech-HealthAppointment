import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
  imports: [ReactiveFormsModule, FormsModule, CommonModule],
  standalone: true
})
export class LoginComponent {
  loginForm: FormGroup;
  error: string = '';
  loading: boolean = false;
  errorMessage: string = "";

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    this.loginForm.valueChanges.subscribe(() => {
      this.errorMessage = '';
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.loading = true;
      const { username, password } = this.loginForm.value;
      this.authService.login(username, password).subscribe({
        next: (res: any) => {
          this.loading = false;
          this.router.navigate(['/home']); // Or appropriate role-based route
          localStorage.setItem('token', res.token);
          localStorage.setItem('refreshToken', res.refreshToken);
          localStorage.setItem('expiresAt', res.expiresAt);
        },
        error: () => {
          this.loading = false;
          this.errorMessage = 'Login failed. please check your credentials.';
          this.loginForm.reset();
        },
      });
    }
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }
}