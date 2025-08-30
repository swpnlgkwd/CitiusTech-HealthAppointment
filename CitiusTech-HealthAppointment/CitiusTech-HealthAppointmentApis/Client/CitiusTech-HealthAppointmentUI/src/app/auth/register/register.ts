// src/app/auth/register/register.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { registerDto } from '../../core/models/auth.model';

@Component({
  selector: 'app-register',
  templateUrl: './register.html',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  standalone: true
})
export class RegisterComponent {
  registerForm: FormGroup;
  username = '';
  password = '';
  loading: boolean = false;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      role: ['Patient', Validators.required],
      password: ['', [Validators.required, this.passwordStrengthValidator]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  register() {
     if (this.registerForm.valid) {
      this.loading = true;
      const data: registerDto = {
        username: this.registerForm.value.username,
        email: this.registerForm.value.email,
        role: this.registerForm.value.role,
        password: this.registerForm.value.password
      };
      this.authService.register(data).subscribe({
        next: (res: any) => {
          this.loading = false;
          this.router.navigate(['/login']);
        },
        error: () => {
          this.loading = false;
          this.registerForm.reset();
        },
      });
    }
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }

  //custom validator to check if password and confirm password match
  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }

  //custom validator to check password strength
  passwordStrengthValidator(control: any) {
    const value = control.value;
    if (!value) {
      return null;
    }
    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumeric = /[0-9]/.test(value);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(value);
    const isValidLength = value.length >= 8;
    const passwordValid = hasUpperCase && hasLowerCase && hasNumeric && hasSpecialChar && isValidLength;
    return !passwordValid ? { weakPassword: true } : null;
  }
}
