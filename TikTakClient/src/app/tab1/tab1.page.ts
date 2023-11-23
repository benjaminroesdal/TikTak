import { Component } from '@angular/core';
import { GoogleAuth } from '@codetrix-studio/capacitor-google-auth';
import { Router, NavigationExtras } from '@angular/router';
import { Preferences } from '@capacitor/preferences';
import {StorageService} from 'src/app/services/storage.service';
import {AuthService} from 'src/app/services/auth.service';

@Component({
  selector: 'app-tab1',
  templateUrl: 'tab1.page.html',
  styleUrls: ['tab1.page.scss']
})
export class Tab1Page {

  constructor(private router: Router, private authService: AuthService) { }

  ionViewDidEnter() {
    GoogleAuth.initialize({
      clientId: '69308154140-5lk0usbipr9j6es0v9pg4tvkj84nn7sf.apps.googleusercontent.com',
      scopes: ['email', 'profile'],
      grantOfflineAccess: true,
    });
  }

  goToHome(user : any) {
      let navigationExtras: NavigationExtras = { state: { user: user } };
      this.router.navigate(['tabs/tab2'], navigationExtras);
    }

  async doLogin() {
    let user = await GoogleAuth.signIn();
    const newUser: User = {
      GoogleAccessToken: user.authentication.accessToken,
      FulLName: user.name,
      ImageUrl: user.imageUrl
    };
    await this.authService.CreateAccount(newUser);
    if (user) { this.goToHome(user); }
  }

  async checkLoggedIn() {
    const loggedOut = await this.authService.isTokenExpired();
    console.log(loggedOut);
    if(loggedOut == false){
      GoogleAuth.refresh().then((data) => {
        console.log(data + 'Hello');
        if (data.accessToken) {
          let navigationExtras: NavigationExtras = {
            state: {
              user: { type: 'existing', accessToken: data.accessToken, idToken: data.idToken }
            }
          };
          this.router.navigate(['tabs/tab2'], navigationExtras);
        }
      }).catch(e => {
        if (e.type === 'userLoggedOut') {
          this.doLogin();
        }
      });
    }
    if(loggedOut == true){
      this.doLogin();
    }
  }
}

export interface User {
  GoogleAccessToken: string;
  FulLName: string;
  ImageUrl: string;
}
