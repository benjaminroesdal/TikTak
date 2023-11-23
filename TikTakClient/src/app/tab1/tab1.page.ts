import { Component } from '@angular/core';
import { GoogleAuth } from '@codetrix-studio/capacitor-google-auth';
import { Router, NavigationExtras } from '@angular/router';

@Component({
  selector: 'app-tab1',
  templateUrl: 'tab1.page.html',
  styleUrls: ['tab1.page.scss']
})
export class Tab1Page {

  constructor(private router: Router) { }

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
    console.log(user);
    if (user) { this.goToHome(user); }
  }

  checkLoggedIn() {
    GoogleAuth.refresh().then((data) => {
      console.log(data);
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

}
