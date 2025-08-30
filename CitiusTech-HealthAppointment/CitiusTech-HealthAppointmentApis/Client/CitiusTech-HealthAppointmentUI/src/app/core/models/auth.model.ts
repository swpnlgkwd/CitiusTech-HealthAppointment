export interface loginResponse {
    token: string;
    refreshToken: string;
    expiresAt: Date;
}

export interface registerDto {
    username: string;
    email: string;
    role: string;
    password: string;
}