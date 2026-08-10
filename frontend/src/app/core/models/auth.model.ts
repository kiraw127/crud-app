export interface Session {
  token: string;
  name: string;
  role: 'Admin' | 'Customer';
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest extends LoginRequest {
  name: string;
}
