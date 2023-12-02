import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {StorageService} from 'src/app/services/storage.service';
import * as jwt_decode from 'jwt-decode';
import { BehaviorSubject, Observable, from, map } from 'rxjs';
import { ToastService } from './toast.service';
import { environment } from '../../environments/environment';
import { Router, NavigationExtras } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  public tokenRefreshInProgress: BehaviorSubject<boolean> = new BehaviorSubject(false);
  private apiBaseUrl = environment.firebase.apiBaseUrl;
  constructor(private http: HttpClient, private storageService: StorageService, private toastService: ToastService, private router: Router) { }
  private isLoggedIn = new BehaviorSubject<boolean>(false);
  public isLoggedIn$ = this.isLoggedIn.asObservable();

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type':  'application/json'
    })
  };

  initializeAuth(): Promise<any> {
    return new Promise((resolve, reject) => {
      this.isTokenExpired().then(e => {
        if(e){
          this.isLoggedIn.next(true);
        }
        if(!e){
          this.isLoggedIn.next(false);
        }
      })
      resolve(true);
    });
  }

  public async CreateAccount(user: User) {
    await this.http.post<UserModel>(`${this.apiBaseUrl}/Login`, user)
    .subscribe(e => {
      this.storageService.set('AccessToken', e.accessToken).then(() => {
        this.router.navigate(['tabs/tab2']); // Redirect to home or another page
      });
      this.router.navigate(['tabs/tab2']); // Redirect to home or another page
      this.storageService.set('RefreshToken', e.refreshToken);
    });
  }

  async Logout() : Promise<void> {
    await this.storageService.get('RefreshToken').then(e => {
      return this.http.post(`${this.apiBaseUrl}/Logout`, JSON.stringify(e), this.httpOptions).subscribe(() => {
        this.isLoggedIn.next(false);
        this.router.navigate(['tabs/tab1']);
        this.storageService.clear();
      })
    });
  }

  async RefreshAccessToken() : Promise<void>{
    await this.storageService.get('RefreshToken').then(e => {
      this.tokenRefreshInProgress.next(true);
      return this.http.post<UserModel>(`${this.apiBaseUrl}/RefreshAccessToken`, JSON.stringify(e), this.httpOptions)
        .subscribe(x => {
          this.storageService.set('AccessToken', x.accessToken).finally(() => {
            this.tokenRefreshInProgress.next(false);
            this.isLoggedIn.next(true);
          }).catch(() => {
            this.toastService.showToast("Error trying to refresh session");
            this.isLoggedIn.next(false);
          });
        },
        error => {
          this.toastService.showToast("Error trying to refresh session");
          this.isLoggedIn.next(false);
        }
        );
    });
  }

  async isTokenExpired(): Promise<boolean> {
    return this.storageService.get('AccessToken').then(e => {
      if (!e){ 
        this.isLoggedIn.next(false);
        return true
      };

      const decoded: any = jwt_decode.jwtDecode(e);
      if (!decoded.exp) {
        this.isLoggedIn.next(false);
        return true
      };
  
      const now = Math.floor(Date.now() / 1000);
      this.isLoggedIn.next(true);
      return decoded.exp < now;
    });
  }

  shouldRefreshToken(): Observable<boolean> {
    return from(this.storageService.get('AccessToken')).pipe(
      map(e => {
        if (!e) return true;
  
        const decoded: any = jwt_decode.jwtDecode(e);
        if (!decoded.exp) return true;
  
        const now = Math.floor(Date.now() / 1000);
        // Check if the expiration time is less than or equal to 60 seconds from now
        return decoded.exp - now <= 60;
      })
    );
  }
}

export interface User {
  GoogleAccessToken: string;
  FulLName: string;
  ImageUrl: string;
}

export interface UserModel {
  email: string;
  iulLName: string;
  imageUrl: string;
  accessToken: string;
  refreshToken: string;
  dateOfBirth: string;
}
