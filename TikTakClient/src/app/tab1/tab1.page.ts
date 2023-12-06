import { Component } from '@angular/core';
import { GoogleAuth } from '@codetrix-studio/capacitor-google-auth';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { LocationService } from '../services/location.service';

@Component({
  selector: 'app-tab1',
  templateUrl: 'tab1.page.html',
  styleUrls: ['tab1.page.scss']
})
export class Tab1Page {

  private lat: number = 0;
  private long: number = 0;
  constructor(private router: Router, private authService: AuthService, private locationService: LocationService) { }

  ionViewDidEnter() {
    this.locationService.printCurrentPosition().then(x => {
      this.lat = x.coords.latitude;
      this.long = x.coords.longitude;
    });
    GoogleAuth.initialize({
      clientId: '69308154140-5lk0usbipr9j6es0v9pg4tvkj84nn7sf.apps.googleusercontent.com',
      scopes: ['email', 'profile'],
      grantOfflineAccess: true,
    });
  }

  async doLogin() {
    let user = await GoogleAuth.signIn();
    const newUser: User = {
      GoogleAccessToken: user.authentication.accessToken,
      FulLName: user.name,
      ImageUrl: user.imageUrl,
      Latitude: this.lat,
      Longitude: this.long
    };
    await this.authService.CreateAccount(newUser).then(() => {
    });
  }

  async checkLoggedIn() {
    const loggedOut = await this.authService.isTokenExpired();
    if (loggedOut) {
      this.doLogin()
    }
  }
}

export interface User {
  GoogleAccessToken: string;
  FulLName: string;
  ImageUrl: string;
  Latitude: number;
  Longitude: number;
}
