import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { APIResponse } from '../models/response-api.model';
import { lauching } from '../../../lauching.conf';
import { LoginModel } from '../models/auth.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${lauching.apiUrl}/api/auths`;
  private isAuthenticated = new BehaviorSubject<boolean>(false);
  isAuthenticated$ = this.isAuthenticated.asObservable();
  constructor(private http: HttpClient) {
    const token = localStorage.getItem('token');
    this.isAuthenticated.next(!!token);
  }
  getAuth(): Observable<APIResponse<any>> {
    return this.http.get<APIResponse<any>>(`${this.apiUrl}/all`);
  }
  login(loginModel: LoginModel): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, loginModel).pipe(
      tap((response: any) => {
        if (response.success && response.data) {
          localStorage.setItem('token', response.data.token);
          if (response.data.unit) {
            localStorage.setItem('unit', response.data.unit);
            lauching.unit = response.data.unit;
          }
          this.isAuthenticated.next(true);
        }
      })
    );
  }
}
