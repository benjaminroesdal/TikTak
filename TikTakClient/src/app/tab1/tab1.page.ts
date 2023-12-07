import { Component } from '@angular/core';
import { GoogleAuth } from '@codetrix-studio/capacitor-google-auth';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-tab1',
  templateUrl: 'tab1.page.html',
  styleUrls: ['tab1.page.scss']
})
export class Tab1Page {
  constructor(private authService: AuthService) { }

  ionViewDidEnter() {
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
      ImageUrl: user.imageUrl
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
}
