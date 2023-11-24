import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {StorageService} from 'src/app/services/storage.service';
import * as jwt_decode from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, private storageService: StorageService) { }

  public async CreateAccount(user: User) {
    await this.http.post<UserModel>('https://localhost:7001/Login', user)
    .subscribe(e => {
      console.log(e.accessToken);
      this.storageService.set('AccessToken', e.accessToken);
      this.storageService.get('AccessToken').then(x => {
        console.log(x);
      });
    });
  }

  async Logout() : Promise<void> {
    await this.storageService.clear();
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
  dateOfBirth: string;
}
