import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth/auth';
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
  isError: boolean = false;
  showPassword: boolean = false;
;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {

    if(this.authService.isAuthenticated())
      this.router.navigate(['/home']);

    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

    togglePasswordVisibility(): void {
      this.showPassword = !this.showPassword;
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isError = false;
      this.loading = true;
      const { username, password } = this.loginForm.value;
      this.authService.login(username, password).subscribe({
        next: (res: any) => {
          this.loading = false;
          this.router.navigate(['/home']); // Or appropriate role-based route
        },
        error: () => {
          this.loading = false;
          this.isError = true;
          this.loginForm.reset();
        },
      });
    }
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }
}