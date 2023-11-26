import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {StorageService} from 'src/app/services/storage.service';
import * as jwt_decode from 'jwt-decode';
import { BehaviorSubject, Observable, from, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  public tokenRefreshInProgress: BehaviorSubject<boolean> = new BehaviorSubject(false);
  constructor(private http: HttpClient, private storageService: StorageService) { }

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type':  'application/json'
    })
  };

  public async CreateAccount(user: User) {
    await this.http.post<UserModel>('https://localhost:7001/Login', user)
    .subscribe(e => {
      console.log("HALLOW");
      console.log(e.refreshToken);
      this.storageService.set('AccessToken', e.accessToken);
      this.storageService.set('RefreshToken', e.refreshToken);
      this.storageService.get('AccessToken').then(x => {
        console.log(x);
      });
    });
  }

  async Logout() : Promise<void> {
    await this.storageService.clear();
  }

  async RefreshAccessToken() : Promise<void>{
    console.log("Hej");
    await this.storageService.get('RefreshToken').then(e => {
      console.log("med");
      console.log(this.tokenRefreshInProgress.value);
      this.tokenRefreshInProgress.next(true);
      console.log(e);
      return this.http.post<UserModel>('https://localhost:7001/RefreshAccessToken', JSON.stringify(e), this.httpOptions)
        .subscribe(x => {
          console.log("KOMMER VI HERIND?");
          this.storageService.set('AccessToken', x.accessToken).finally(() => {
            console.log("KOMMER VI HERIND?");
            this.tokenRefreshInProgress.next(false);
            console.log("dig");
            console.log(this.tokenRefreshInProgress.value);
            console.log(this.tokenRefreshInProgress)
          });
        });
    });
  }

  async isTokenExpired(): Promise<boolean> {
    return this.storageService.get('AccessToken').then(e => {
      console.log(e);
      if (!e) return true;
  
      const decoded: any = jwt_decode.jwtDecode(e);
      console.log(decoded);
      if (!decoded.exp) return true;
  
      const now = Math.floor(Date.now() / 1000);
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
